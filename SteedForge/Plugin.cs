using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using Bloodstone.API;
using Bloodstone.Hooks;  

namespace SteedForge
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]  // we use VCF
    [BepInDependency("gg.deca.Bloodstone", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BasePlugin
    {
        private Harmony _harmony;

        public override void Load()
        {
            Log.LogInfo($"[SteedForge] {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded!");

            // Initialize config
            MainConfig.Init(Config);
            Log.LogInfo("[SteedForge] Configuration initialized.");

            // Harmony patch
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            // Register commands from this assembly
            CommandRegistry.RegisterAll();
            Log.LogInfo("[SteedForge] Commands registered.");

            // Listen for !reload in the chat
            Chat.OnChatMessage += HandleReloadCommand;
        }

        public override bool Unload()
        {
            // Unregister chat hook
            Chat.OnChatMessage -= HandleReloadCommand;

            // Unpatch
            _harmony?.UnpatchSelf();

            // Unregister commands
            CommandRegistry.UnregisterAssembly();

            Log.LogInfo($"[SteedForge] {MyPluginInfo.PLUGIN_NAME} unloaded.");
            return true;
        }

        /// <summary>
        /// Handle the chat messages to see if user typed "!reload" (admin only).
        /// </summary>
        private void HandleReloadCommand(VChatEvent ev)
        {
            // If the message is exactly "!reload" and user is an admin
            if (ev.Message == "!reload" && ev.User.IsAdmin)
            {
                Log.LogInfo("[SteedForge] !reload command received.");
                try
                {
                    // Reload the config from disk
                    MainConfig.ReloadConfig(Config);

                    // Notify the admin
                    ev.User.SendSystemMessage("<color=#00FF00>SteedForge configuration reloaded successfully.</color>");
                    Log.LogInfo("[SteedForge] Config reloaded via !reload command.");
                }
                catch (System.Exception ex)
                {
                    ev.User.SendSystemMessage($"<color=#FF0000>Failed to reload SteedForge config:</color> {ex.Message}");
                    Log.LogError($"[SteedForge] Error reloading config: {ex}");
                }
            }
        }
    }
}
