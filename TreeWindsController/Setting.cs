using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using UnityEngine.Rendering;
using static Game.Rendering.Debug.RenderPrefabRenderer;

namespace TreeWindsController
{
    [FileLocation(nameof(TreeWindsController))]
    [SettingsUITabOrder(WindTab)]
    [SettingsUIGroupOrder(DisableWinds, WindSlidersGroup)]
    [SettingsUIShowGroupName(DisableWinds, WindSlidersGroup)]
    public class Setting : ModSetting
    {
        private readonly TreeWindsControl _treeWindsControl;

        public const string WindTab = "Wind";
        public const string WindSlidersGroup = "WindSlidersGroup";
        public const string DisableWinds = "DisableWinds";




        public Setting(IMod mod) : base(mod)
        {
            _treeWindsControl = TreeWindsControl.Instance;

            if (_treeWindsControl == null)
            {
                // Handle the case where TreeWindsControl.Instance is not yet available
                Mod.log.Warn("TreeWindsControl.Instance is not initialized yet.");
            }
        }

        private TreeWindsControl Get_treeWindsControl()
        {
            return _treeWindsControl;
        }

        private void InitializeTreeWindsControl(TreeWindsControl _treeWindsControl)
        {
            if (TreeWindsControl.Instance == null)
            {
                // Log a warning or handle initialization failure
                Mod.log.Warn("TreeWindsControl.Instance is not initialized yet.");
            }
            else
            {
                _treeWindsControl = TreeWindsControl.Instance;
            }
        }
        [SettingsUISection(WindTab, DisableWinds)]
         public bool DisableWind
        {
            get
            {
                if (_treeWindsControl == null)
                {
                    Mod.log.Error("Attempted to access TreeWindsControl before initialization.");
                    return false;
                }

                return _treeWindsControl.disableAllWind;
            }
            set
            {
                if (_treeWindsControl == null)
                {
                    Mod.log.Error("Attempted to set TreeWindsControl before initialization.");
                    return;
                }

                _treeWindsControl.disableAllWind = value;
                _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
            }
        }
        [SettingsUISection(WindTab, WindSlidersGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1, unit = Unit.kPercentage)]
        public float WindStrength
        {
            get => percentageClamped(_treeWindsControl?.strength);
            set
            {
                if (_treeWindsControl != null)
                {
                    _treeWindsControl.strength.value = clampedValuePercent(_treeWindsControl.strength, value);
                    _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
                }
            }
        }
        [SettingsUISection(WindTab, WindSlidersGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1, unit = Unit.kPercentage)]
        public float WindStrengthVariance
        {
            get => percentageClamped(_treeWindsControl?.strengthVariance);
            set
            {
                if (_treeWindsControl != null)
                {
                    _treeWindsControl.strengthVariance.value = clampedValuePercent(_treeWindsControl.strengthVariance, value);
                    _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
                }
            }
        }
        [SettingsUISection(WindTab, WindSlidersGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1, unit = Unit.kPercentage)]
        public float WindStrengthVariancePeriod
        {
            get => percentageClamped(_treeWindsControl?.strengthVariancePeriod);
            set
            {
                if (_treeWindsControl != null)
                {
                    _treeWindsControl.strengthVariancePeriod.value = clampedValuePercent(_treeWindsControl.strengthVariancePeriod, value);
                    _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
                }
            }
        }

        [SettingsUISection(WindTab, WindSlidersGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1, unit = Unit.kPercentage)]
        public float WindDirection
        {
            get => percentageClamped(_treeWindsControl?.direction);
            set
            {
                if (_treeWindsControl != null)
                {
                    _treeWindsControl.direction.value = clampedValuePercent(_treeWindsControl.direction, value);
                    _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
                }
            }
        }
        [SettingsUISection(WindTab, WindSlidersGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1, unit = Unit.kPercentage)]
        public float WindDirectionVariance
        {
            get => percentageClamped(_treeWindsControl?.directionVariance);
            set
            {
                if (_treeWindsControl != null)
                {
                    _treeWindsControl.directionVariance.value = clampedValuePercent(_treeWindsControl.directionVariance, value);
                    _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
                }
            }
        }
        [SettingsUISection(WindTab, WindSlidersGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1, unit = Unit.kPercentage)]
        public float WindDirectionVariancePeriod
        {
            get => percentageClamped(_treeWindsControl?.directionVariancePeriod);
            set
            {
                if (_treeWindsControl != null)
                {
                    _treeWindsControl.directionVariancePeriod.value = clampedValuePercent(_treeWindsControl.directionVariancePeriod, value);
                    _treeWindsControl.updateWindVolumeComponent(null);  // Assuming you have a wind volume component to pass
                }
            }
        }










        public override void SetDefaults()
        {
            
        }

       

        private float percentageClamped(ClampedFloatParameter cfp)
        {
            return 100f * (cfp.value - cfp.min) / (cfp.max - cfp.min);
        }

        private float clampedValuePercent(ClampedFloatParameter cfp, float percent)
        {
            return cfp.min + percent / 100f * (cfp.max - cfp.min);
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "TreeWindsController" },
                { m_Setting.GetOptionTabLocaleID(Setting.WindTab), "Wind" },

                { m_Setting.GetOptionGroupLocaleID(Setting.DisableWinds), "DisableWinds" },
                { m_Setting.GetOptionGroupLocaleID(Setting.WindSlidersGroup), "WindSliders" },
  
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableWind)), "Disable wind" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableWind)) , "Toggle to disable wind" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WindStrength)), "Wind strength" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WindStrength)), "Set wind strength" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WindStrengthVariance)), "Wind strength variance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WindStrengthVariance)), "Set wind strength variance" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WindStrengthVariancePeriod)), "Wind strength variance period" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WindStrengthVariancePeriod)), "Set wind strength variance period" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WindDirection)), "Wind direction" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WindDirection)), "Set wind direction" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WindDirectionVariance)), "Wind direction variance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WindDirectionVariance)), "Set wind direction variance" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WindDirectionVariancePeriod)), "Wind direction variance period" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WindDirectionVariancePeriod)), "Set wind direction variance period" },

               

            };
        }

        public void Unload()
        {

        }
    }
}
