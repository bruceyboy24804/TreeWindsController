using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Simulation;
using Game.UI;
using HarmonyLib;
using System.IO;

namespace TreeWindsController
{

    public class Mod : IMod
    {
        public static Mod Instance { get; private set; }
        public static ILog log = LogManager.GetLogger($"{nameof(TreeWindsController)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));


            AssetDatabase.global.LoadSettings(nameof(TreeWindsController), m_Setting, new Setting(this));
            updateSystem.UpdateAt<WindControlSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<WindControlSystem>(SystemUpdatePhase.Rendering);
            var harmony = new Harmony("com.treewindscontroller.windpatch");
            harmony.PatchAll();

            Instance = this;
        }
        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            // Ensure the WindUpdateSystem is created and running in the game world
            updateSystem.World.GetOrCreateSystem<WindControlSystem>();
        }
        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
