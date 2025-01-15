using System.IO;
using BepInEx.Configuration;

public static class MainConfig
{
    public static ConfigEntry<float> DefaultSpeed { get; private set; }
    public static ConfigEntry<float> DefaultAcceleration { get; private set; }
    public static ConfigEntry<float> DefaultRotation { get; private set; }
    public static ConfigEntry<string> CurrencyName { get; private set; }
    public static ConfigEntry<int> RequiredCurrencyGUID { get; private set; }
    public static ConfigEntry<int> CurrencyCost { get; private set; }

    public static void Init(ConfigFile config)
    {
        DefaultSpeed = config.Bind("Horse", "DefaultSpeed", 11f, "Default maximum speed for the spawned horse.");
        DefaultAcceleration = config.Bind("Horse", "DefaultAcceleration", 7f, "Default acceleration for the spawned horse.");
        DefaultRotation = config.Bind("Horse", "DefaultRotation", 14f, "Default rotation speed for the spawned horse.");
        CurrencyName = config.Bind("Currency", "CurrencyName", "Silver Coin", "The name of the currency required for upgrades.");
        RequiredCurrencyGUID = config.Bind("Currency", "RequiredCurrencyGUID", -949672483, "The GUID of the required currency.");
        CurrencyCost = config.Bind("Currency", "CurrencyCost", 500, "The cost in currency for the upgrade.");
    }
}
