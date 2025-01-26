# SteedForge Mod

## Description
**SteedForge** is a mod designed for **V Rising** that allows players to upgrade the stats of their Vampiric Steed (horse).  
It provides a command to enhance a nearby tamed horse's speed, acceleration, and rotation, using an in-game currency.  
Join our server for more modded and exciting content: [Sanguine Reign Discord](https://discord.gg/sanguineReign) 

## Patch Notes (1.1.0)
- **Removed** dependency on BloodyShop and BloodyCore.
- **Added** reloadable config via `!reload` command (admin-only). No server restart required to change configs.
- General stability improvements, should no longer "bounce" when using the upgrade horse command.

## Features
- Upgrade the stats of a Vampiric Steed (horse).
- Customizable maximum stats and currency requirements.
- Prevents multiple horses from being upgraded at once.
- Ensures upgrades only apply if sufficient currency is available.
- **Reload config** at runtime with `!reload`.

## Commands

### Upgrade Horse
- **Command:** `.upgradehorse` or `.uh`
- **Description:** Upgrades the stats of a nearby Vampiric Steed.  
  The upgrade costs an in-game currency and only applies if certain conditions are met.  
- **Usage:**
  1. Stand near your Vampiric Steed within a small radius.
  2. Type `.uh` (or `.upgradehorse`) in chat.

### Reload Config
- **Command:** `!reload`
- **Description:** Forces the plugin to re-read the config file from disk, updating your mod settings immediately.  
  - **Permissions:** Admin-only (server owners).

## Installation
1. Download or extract **SteedForge** into `BepInEx\plugins`.
2. Launch or restart the game/server.

## Configuration
You can customize the mod via **`BepInEx\config\SteedForge.cfg`**.

### Available Configurations
- **DefaultSpeed**: Default max speed for the Vampiric Steed (default: `11.0`).
- **DefaultAcceleration**: Default acceleration for the Vampiric Steed (default: `7.0`).
- **DefaultRotation**: Default rotation speed for the Vampiric Steed (default: `14.0`).
- **CurrencyName**: Name of the currency required for upgrades (default: `Silver Coin`).
- **RequiredCurrencyGUID**: GUID of the required currency (default: `-949672483`).
- **CurrencyCost**: Cost of the upgrade in the specified currency (default: `500`).

## Support
For support, bug reports, or feature requests:
- **Discord DM:** `inility#4118`
- **Discord Server:** [Sanguine Reign](https://discord.gg/sanguineReign)

## License
This mod is **open-source** and is free to use or modify for personal use. 

## Additional Notes
- Ensure no other horses are within the hover radius when using the upgrade command.
- Verify you have sufficient in-game currency before attempting an upgrade.
- Join our server to experience more “forge system” mods in action:  
  [Sanguine Reign Discord](https://discord.gg/sanguineReign)
