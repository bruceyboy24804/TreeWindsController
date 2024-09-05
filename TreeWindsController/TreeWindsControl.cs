using Game;
using Game.Rendering;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace TreeWindsController
{
    public partial class TreeWindsControl : GameSystemBase
    {
        public bool disableAllWind;

        public ClampedFloatParameter strength;
        public ClampedFloatParameter strengthVariance;
        public ClampedFloatParameter strengthVariancePeriod;

        private AnimationCurveParameter _strengthVarianceAnimation;
        private ClampedFloatParameter _globalStrength;
        private ClampedFloatParameter _baseStrength;
        private ClampedFloatParameter _gustStrength;
        private ClampedFloatParameter _gustStrengthControl;
        private ClampedFloatParameter _gustStrengthControlPeriod;

        public ClampedFloatParameter direction;
        public ClampedFloatParameter directionVariance;
        public ClampedFloatParameter directionVariancePeriod;

        public static TreeWindsControl Instance;

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;

            disableAllWind = false;

            strength = new ClampedFloatParameter(0.25f, 0f, 1f);
            strengthVariance = new ClampedFloatParameter(0f, 0f, 1f);
            strengthVariancePeriod = new ClampedFloatParameter(25f, 0.01f, 120f);

            _globalStrength = new ClampedFloatParameter(0f, 0f, 1f);
            _baseStrength = new ClampedFloatParameter(0f, 0.25f, 3f);
            _gustStrength = new ClampedFloatParameter(1f, 1f, 10f);
            _gustStrengthControl = new ClampedFloatParameter(2f, 1f, 2.5f);
            _gustStrengthControlPeriod = new ClampedFloatParameter(8f, 2f, 10f);
            _strengthVarianceAnimation = new AnimationCurveParameter(new AnimationCurve());

            // Defaults taken from WindVolumeComponent default values
            direction = new ClampedFloatParameter(65f, 0f, 360f);
            directionVariance = new ClampedFloatParameter(25f, 0f, 180f);
            directionVariancePeriod = new ClampedFloatParameter(15f, 0.01f, 120f);
        }

        protected override void OnUpdate()
        {

        }

        public void updateWindVolumeComponent(WindVolumeComponent w)
        {
            if (disableAllWind)
            {
                w.windGlobalStrengthScale.Override(0);
                w.windGlobalStrengthScale2.Override(0);
                return;
            }

            var minStrength = strength.value - (strengthVariance.value / 2f) * strength.value;
            var maxStrength = strength.value + (strengthVariance.value / 2f) * strength.value;

            // Creating a new AnimationCurve with keyframes
            _strengthVarianceAnimation.value = new AnimationCurve(
                new Keyframe(0f, minStrength),
                new Keyframe(strengthVariancePeriod.value, maxStrength),
                new Keyframe(2 * strengthVariancePeriod.value, minStrength)
            );

            var strengthAnim = _strengthVarianceAnimation.value.Evaluate(
                UnityEngine.Time.time % (2 * strengthVariancePeriod.value)
            );

            _globalStrength.value = clampedValueRatio(_globalStrength, strengthAnim);
            _baseStrength.value = clampedValueRatio(_baseStrength, strengthAnim);
            _gustStrength.value = clampedValueRatio(_gustStrength, strengthAnim);
            _gustStrengthControl.value = clampedValueRatio(_gustStrengthControl, strengthAnim);
            _gustStrengthControlPeriod.value = clampedValueRatio(_gustStrengthControlPeriod, 1f / strengthAnim);

            w.windGlobalStrengthScale.Override(_globalStrength.value);
            w.windGlobalStrengthScale2.Override(_globalStrength.value);
            w.windTreeBaseStrength.Override(_baseStrength.value);
            // Affects the leaves and branches more than trunk
            w.windTreeGustStrength.Override(_gustStrength.value);
            // Adds some more variation to the leaves and branches

            // Setting the keys on the internal AnimationCurve of windTreeGustStrengthControl
            w.windTreeGustStrengthControl.value.keys = new Keyframe[]
            {
                new Keyframe(0f, 0f),
                new Keyframe(_gustStrengthControlPeriod.value, _gustStrengthControl.value)
            };

            w.windDirection.Override(direction.value);
            w.windDirectionVariance.Override(directionVariance.value);
            w.windDirectionVariancePeriod.Override(directionVariancePeriod.value);

            w.windParameterInterpolationDuration.Override(0.0001f);

            return;
        }

        private float clampedValueRatio(ClampedFloatParameter cfp, float ratio)
        {
            return cfp.min + ratio * (cfp.max - cfp.min);
        }
    }
}
