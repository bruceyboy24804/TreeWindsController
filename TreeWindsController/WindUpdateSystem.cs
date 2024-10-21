using Game.Rendering;
using Unity.Entities;
using UnityEngine.Rendering;

namespace TreeWindsController
{
    public partial class WindUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // Fetch the wind control system instance
            var windControlSystem = WindControlSystem.Instance;

            // Apply wind settings in real-time
            if (windControlSystem.windEnabled)
            {
                // Apply the current wind settings from WindControlSystem to the game's volume component
                windControlSystem.ApplyWindSettings();
            }
            else
            {
                // If wind is disabled, call DisableAllWind
                windControlSystem.DisableAllWind();
            }

            // Optionally, evaluate wind gusts using the time
            var windVolumeComponent = VolumeManager.instance.stack.GetComponent<WindVolumeComponent>();
            if (windVolumeComponent != null)
            {
                float time = (float)SystemAPI.Time.ElapsedTime;
                windVolumeComponent.windGustStrengthControl.value.Evaluate(time);
                windVolumeComponent.windTreeGustStrengthControl.value.Evaluate(time);
            }
        }
    }
}
