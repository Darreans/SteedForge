# SteedForge Mod

## Description
**SteedForge** is a mod designed for **V Rising** that allows players to upgrade the stats of their Vampiric Steed (horse).
It provides a command to enhance the speed, acceleration, and rotation of a tamed horse **you are aiming at with your cursor**, using a configurable in-game currency.
Join our servers for more modded and exciting content:
- [VArena Discord](https://discord.gg/varena)
- [VRising Modding Community](https://wiki.vrisingmods.com)

**Prerequisites:** This mod requires **BepInEx for IL2CPP** installed on your server. Ensure you are using at least version [**1.733.2**](https://github.com/decaprime/VRising-Modding/releases/tag/1.733.2) or a compatible newer version.


## Patch Notes (v1.2.0)
- **Changed Targeting:** Upgrading now requires aiming at the horse with your cursor instead of standing within a radius.
- **Removed Dependency:** Removed the dependency on the Bloodstone API mod.
- **Added Reload Command:** Configuration can now be reloaded via the `.reloadsteedforge` command (admin-only).
- **Added Enable/Disable:** Added a configuration option (`EnableMod`) to easily enable or disable the mod's functionality.
- General stability improvements.

## Features
- Upgrade the stats of a Vampiric Steed (horse) you are **aiming at**.
- Customizable maximum stats and currency requirements via config file.
- **Enable/Disable** the entire mod via a config setting.
- Prevents upgrading if the horse already has max stats.
- Ensures upgrades only apply if sufficient currency is available.
- **Reload config** at runtime with `.reloadsteedforge` (Admin only).

## Commands

### Upgrade Horse
- **Command:** `.upgradehorse` or `.uh`
- **Description:** Upgrades the stats of the Vampiric Steed you are currently aiming at. The upgrade costs an in-game currency and only applies if certain conditions are met.
- **Usage:**
  1. Aim your cursor directly at the Vampiric Steed you wish to upgrade (ensure it's reasonably close to your cursor).
  2. Type `.uh` (or `.upgradehorse`) in chat.

### Reload Config
- **Command:** `.reloadsteedforge` or `.rsf`
- **Description:** Forces the plugin to re-read the `SteedForge.cfg` file from disk, updating mod settings immediately without a server restart.
- **Permissions:** Admin-only.

## Installation
1. Ensure BepInEx for IL2CPP is installed correctly on your server.
2. Download the latest `SteedForge.dll`.
3. Place `SteedForge.dll` into your server's `BepInEx\plugins` folder.
4. Launch or restart the server.

## Configuration
You can customize the mod via **`BepInEx\config\SteedForge.cfg`**. This file is generated on the first run.

### Available Configurations
- **EnableMod**: Enable or disable the SteedForge mod functionality (default: `true`).
- **DefaultSpeed**: Max speed for the upgraded Vampiric Steed (default: `10.0`).
- **DefaultAcceleration**: Acceleration for the upgraded Vampiric Steed (default: `7.0`).
- **DefaultRotation**: Rotation speed for the upgraded Vampiric Steed (default: `14.0`).
- **CurrencyName**: Name of the currency required for upgrades (default: `Silver Coin`).
- **RequiredCurrencyGUID**: Prefab GUID (Integer) of the required currency (default: `-949672483` for Silver Coin).
- **CurrencyCost**: Cost of the upgrade in the specified currency (default: `500`).

## Support
For support, bug reports, or feature requests:
- **Discord DM:** `inility#4118` 
- **Discord Servers:**
    - [VArena](https://discord.gg/varena)
    - [VRising Modding Community](https://wiki.vrisingmods.com)	
## License
This mod is **open-source** and is free to use or modify for personal use.

## Additional Notes
- Ensure you are aiming close enough to the horse when using the upgrade command .
- Verify you have sufficient in-game currency before attempting an upgrade.
- Join our servers to experience more “forge system” mods in action!