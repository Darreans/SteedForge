using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;

namespace SteedForge
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private Harmony _harmony;

        public override void Load()
        {
            // Log plugin initialization
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");

            // Initialize configuration
            MainConfig.Init(Config);
            Log.LogInfo("Configuration initialized successfully!");

            // Apply Harmony patches
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            // Register commands
            CommandRegistry.RegisterAll();
            Log.LogInfo("Commands registered successfully!");
        }

        public override bool Unload()
        {
            // Unpatch Harmony and unregister commands
            CommandRegistry.UnregisterAssembly();
            _harmony?.UnpatchSelf();
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is unloaded.");
            return true;
        }
    }
}
