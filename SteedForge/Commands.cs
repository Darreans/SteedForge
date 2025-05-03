using System.Collections.Generic;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using VampireCommandFramework;

namespace SteedForge
{
    public static class Commands
    {
        private const int VampireHorseGUID = -1502865710;

        [Command("upgradehorse", "uh", description: "Upgrade the stats of a tamed vampire horse you are aiming at", adminOnly: false)]
        public static void UpgradeHorse(ChatCommandContext ctx)
        {
            if (!MainConfig.ModEnabled.Value)
            {
                ctx.Reply("<color=orange>This mod is currently disabled.</color>");
                return;
            }

            var entityManager = VWorldUtils.Server.EntityManager;

            float maxSpeed = MainConfig.DefaultSpeed.Value;
            float maxAcceleration = MainConfig.DefaultAcceleration.Value;
            float maxRotation = MainConfig.DefaultRotation.Value;
            int currencyCost = MainConfig.CurrencyCost.Value;
            string currencyName = MainConfig.CurrencyName.Value;
            var requiredCurrencyGUID = new PrefabGUID(MainConfig.RequiredCurrencyGUID.Value);

            if (!entityManager.TryGetComponentData<EntityAimData>(ctx.Event.SenderCharacterEntity, out var aimData))
            {
                ctx.Reply("<color=red> Error: Could not read your aim/cursor data.</color>");
                return;
            }
            float3 cursorPosition = aimData.AimPosition;
            const float maxCursorDistance = 2.5f;

            var hoveredHorse = FindClosestHorseNearCursor(entityManager, cursorPosition, maxCursorDistance);

            if (hoveredHorse == Entity.Null)
            {
                ctx.Reply("<color=yellow> No Vampiric Steed found near your cursor.</color>");
                return;
            }

            if (entityManager.TryGetComponentData<Mountable>(hoveredHorse, out var mount))
            {
                if (AreFloatsEqual(mount.MaxSpeed, maxSpeed) &&
                    AreFloatsEqual(mount.Acceleration, maxAcceleration) &&
                    AreFloatsEqual(mount.RotationSpeed, maxRotation * 10f))
                {
                    ctx.Reply("<color=yellow> The Vampiric Steed already has max stats. No upgrade applied.</color>");
                    return;
                }

                var playerCharacter = ctx.Event.SenderCharacterEntity;

                bool hasEnough = SteedForgeInventorySystem.HasEnoughOfItem(playerCharacter, requiredCurrencyGUID, currencyCost);
                if (!hasEnough)
                {
                    ctx.Reply($"<color=red> You need {currencyCost} {currencyName} to upgrade your Vampiric Steed.</color>");
                    return;
                }

                bool removedSuccessfully = SteedForgeInventorySystem.TryRemoveItem(playerCharacter, requiredCurrencyGUID, currencyCost);
                if (!removedSuccessfully)
                {
                    ctx.Reply("<color=red> Failed to remove the required items from your inventory.</color>");
                    return;
                }

                mount.MaxSpeed = maxSpeed;
                mount.Acceleration = maxAcceleration;
                mount.RotationSpeed = maxRotation * 10f;
                entityManager.SetComponentData(hoveredHorse, mount);

                ctx.Reply($"<color=green> The Vampiric Steed has been upgraded for {currencyCost} {currencyName}!</color>");
            }
            else
            {
                ctx.Reply("<color=red> Internal error: Target entity unexpectedly missing Mountable component.</color>");
            }
        }

        [Command("reloadsteedforge", "rsf", description: "Reloads the SteedForge configuration file.", adminOnly: true)]
        public static void ReloadSteedForgeConfig(ChatCommandContext ctx)
        {
            if (Plugin.Instance == null || Plugin.Configuration == null)
            {
                ctx.Reply("<color=red>Error: Plugin instance or configuration not found. Cannot reload config.</color>");
                Plugin.LogInstance?.LogError("Attempted to reload config, but Plugin.Instance or Plugin.Configuration was null.");
                return;
            }

            try
            {
                // Pass the stored ConfigFile instance to ReloadConfig
                MainConfig.ReloadConfig(Plugin.Configuration);
                Plugin.LogInstance?.LogInfo($"SteedForge configuration reloaded by admin {ctx.User.CharacterName}. Mod Enabled: {MainConfig.ModEnabled.Value}");
                ctx.Reply("<color=green>SteedForge configuration reloaded successfully.</color>");
            }
            catch (System.Exception ex)
            {
                Plugin.LogInstance?.LogError($"Error reloading SteedForge config: {ex}");
                ctx.Reply($"<color=red>Error reloading SteedForge config: {ex.Message}</color>");
            }
        }

        private static Entity FindClosestHorseNearCursor(EntityManager entityManager, float3 cursorPosition, float maxDistance)
        {
            ComponentType[] allComponents = new ComponentType[]
            {
                 ComponentType.ReadOnly<Mountable>(),
                 ComponentType.ReadOnly<PrefabGUID>(),
                 ComponentType.ReadOnly<LocalToWorld>()
            };

            var query = entityManager.CreateEntityQuery(allComponents);
            var candidates = query.ToEntityArray(Allocator.Temp);

            Entity closestHorse = Entity.Null;
            float minDistSq = maxDistance * maxDistance;

            try
            {
                foreach (var entity in candidates)
                {
                    if (entityManager.HasComponent<Dead>(entity) || entityManager.HasComponent<DestroyTag>(entity))
                    {
                        continue;
                    }

                    var prefab = entityManager.GetComponentData<PrefabGUID>(entity);
                    if (prefab.GuidHash != VampireHorseGUID)
                    {
                        continue;
                    }

                    var horsePosition = entityManager.GetComponentData<LocalToWorld>(entity).Position;
                    float dsq = math.distancesq(cursorPosition, horsePosition);

                    if (dsq < minDistSq)
                    {
                        minDistSq = dsq;
                        closestHorse = entity;
                    }
                }
            }
            finally
            {
                candidates.Dispose();
            }

            return closestHorse;
        }

        private static bool AreFloatsEqual(float value1, float value2, float epsilon = 0.0001f)
        {
            return math.abs(value1 - value2) < epsilon;
        }
    }
}