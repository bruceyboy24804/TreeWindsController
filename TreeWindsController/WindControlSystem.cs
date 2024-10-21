using Game;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace TreeWindsController
{
    public partial class WindControlSystem : GameSystemBase
    {
        public bool windEnabled;
        private static WindControlSystem _instance;
        public bool isInitialized = false; // Add the isInitialized flag
        public static WindControlSystem Instance => _instance ??= new WindControlSystem();

        // Global Wind Settings
        public class GlobalWindSettings
        {
            public ClampedFloatParameter globalStrengthScale = new ClampedFloatParameter(1f, 0f, 3f);
            public ClampedFloatParameter globalStrengthScale2 = new ClampedFloatParameter(1f, 0f, 3f);
            public ClampedFloatParameter windDirection = new ClampedFloatParameter(65f, 0f, 360f);
            public ClampedFloatParameter windDirectionVariance = new ClampedFloatParameter(25f, 0f, 90f);
            public ClampedFloatParameter windDirectionVariancePeriod = new ClampedFloatParameter(15f, 0.01f, 120f);
            public ClampedFloatParameter interpolationDuration = new ClampedFloatParameter(0.5f, 0.0001f, 5f);
        }

       

        public GlobalWindSettings globalSettings = new GlobalWindSettings();
        

        private AnimationCurveParameter strengthVarianceAnimation;
        private ClampedFloatParameter strength;
        private ClampedFloatParameter strengthVariance;
        private ClampedFloatParameter strengthVariancePeriod;

        public WindControlSystem()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (isInitialized) return; // Ensure initialization only happens once

            // Perform initialization logic
            strength = new ClampedFloatParameter(0.25f, 0f, 1f);
            strengthVariance = new ClampedFloatParameter(0f, 0f, 1f);
            strengthVariancePeriod = new ClampedFloatParameter(25f, 0.01f, 120f);
            strengthVarianceAnimation = new AnimationCurveParameter(new AnimationCurve());

            Debug.Log("WindControlSystem Initialized.");
            isInitialized = true; // Mark the system as initialized
        }

        public void ApplyWindSettings()
        {
            // Ensure the system is initialized before applying settings
            if (!isInitialized)
            {
                Initialize();
            }

            if (!windEnabled)
            {
                DisableAllWind();
                return;
            }

            var windVolumeComponent = VolumeManager.instance.stack.GetComponent<WindVolumeComponent>();
            if (windVolumeComponent != null)
            {
                // Calculate wind strength with variance using animation curve
                float minStrength = strength.value - (strengthVariance.value / 2f) * strength.value;
                float maxStrength = strength.value + (strengthVariance.value / 2f) * strength.value;

                strengthVarianceAnimation.value = new AnimationCurve(
                    new Keyframe(0f, minStrength),
                    new Keyframe(strengthVariancePeriod.value, maxStrength),
                    new Keyframe(2 * strengthVariancePeriod.value, minStrength)
                );

                float time = (float)SystemAPI.Time.DeltaTime;
                float strengthAnim = strengthVarianceAnimation.value.Evaluate(time % (2 * strengthVariancePeriod.value));

                globalSettings.globalStrengthScale.value = clampedValueRatio(globalSettings.globalStrengthScale, strengthAnim);

                // Apply wind settings to volume component
                windVolumeComponent.windGlobalStrengthScale.Override(globalSettings.globalStrengthScale.value);
                windVolumeComponent.windGlobalStrengthScale2.Override(globalSettings.globalStrengthScale2.value);
                windVolumeComponent.windDirection.Override(globalSettings.windDirection.value);
                windVolumeComponent.windDirectionVariance.Override(globalSettings.windDirectionVariance.value);
                windVolumeComponent.windDirectionVariancePeriod.Override(globalSettings.windDirectionVariancePeriod.value);
                windVolumeComponent.windParameterInterpolationDuration.Override(globalSettings.interpolationDuration.value);

               
            }
            else
            {
                Debug.LogError("WindVolumeComponent not found.");
            }
        }

        public void DisableAllWind()
        {
            var windVolumeComponent = VolumeManager.instance.stack.GetComponent<WindVolumeComponent>();
            if (windVolumeComponent != null)
            {
                // Disable all wind settings
                windVolumeComponent.windGlobalStrengthScale.Override(0f);
                windVolumeComponent.windGlobalStrengthScale2.Override(0f);
                windVolumeComponent.windBaseStrength.Override(0f);
                windVolumeComponent.windGustStrength.Override(0f);
                windVolumeComponent.windTreeBaseStrength.Override(0f);
                windVolumeComponent.windTreeGustStrength.Override(0f);
                windVolumeComponent.windTreeFlutterStrength.Override(0f);
                Debug.Log("All wind settings disabled.");
            }
        }

        private float clampedValueRatio(ClampedFloatParameter cfp, float ratio)
        {
            return cfp.min + ratio * (cfp.max - cfp.min);
        }

        protected override void OnUpdate()
        {
            // Update wind every frame, if needed
           
        }
    }
}
