using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using UnityEngine;

namespace SteedForge
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    public class Plugin : BasePlugin
    {
        private Harmony _harmony;
        internal static BepInEx.Logging.ManualLogSource LogInstance { get; private set; }
        internal static Plugin? Instance { get; private set; }
        internal static BepInEx.Configuration.ConfigFile? Configuration { get; private set; }

        public override void Load()
        {
            Instance = this;
            LogInstance = Log;
            Configuration = Config;
            MainConfig.Init(Config);

            Log.LogInfo($"[SteedForge] {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded!");
            Log.LogInfo("[SteedForge] Configuration initialized.");

            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
            Log.LogInfo("[SteedForge] Harmony Patches applied (if any).");

            CommandRegistry.RegisterAll();
            Log.LogInfo("[SteedForge] Commands registered.");
        }

        public override bool Unload()
        {
            CommandRegistry.UnregisterAssembly();
            _harmony?.UnpatchSelf();
            Log.LogInfo($"[SteedForge] {MyPluginInfo.PLUGIN_NAME} unloaded.");
            Instance = null;
            Configuration = null;
            return true;
        }
    }
}