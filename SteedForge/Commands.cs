using System.Collections.Generic;
using Bloodstone.API;
using ProjectM;
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

        // Example ECS query that finds “mountable horses”
        private static readonly EntityQueryDesc _vampireHorseQueryDesc = new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Mountable>(),
                ComponentType.ReadOnly<PrefabGUID>(),
                ComponentType.ReadOnly<LocalToWorld>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<Dead>(),
                ComponentType.ReadOnly<DestroyTag>()
            }
        };

        [Command("upgradehorse", "uh", description: "Upgrade the stats of a tamed vampire horse you are near", adminOnly: false)]
        public static void UpgradeHorse(ChatCommandContext ctx)
        {
            var entityManager = VWorld.Server.EntityManager;

            // Config values
            float maxSpeed = MainConfig.DefaultSpeed.Value;
            float maxAcceleration = MainConfig.DefaultAcceleration.Value;
            float maxRotation = MainConfig.DefaultRotation.Value;
            int currencyCost = MainConfig.CurrencyCost.Value;
            string currencyName = MainConfig.CurrencyName.Value;
            var requiredCurrencyGUID = new PrefabGUID(MainConfig.RequiredCurrencyGUID.Value);

            // The player's position
            var playerPosition = entityManager.GetComponentData<LocalToWorld>(ctx.Event.SenderCharacterEntity).Position;
            const float hoverRadius = 2f;

            // Query for all living mountable entities with a PrefabGUID & LocalToWorld
            // Then filter for the "vampire horse" GUID and distance
            var nearbyHorses = FindNearbyHorses(entityManager, playerPosition, hoverRadius);

            if (nearbyHorses.Count == 0)
            {
                ctx.Reply("<color=yellow>[FS] No Vampiric Steed found within range.</color>");
                return;
            }
            if (nearbyHorses.Count > 1)
            {
                ctx.Reply("<color=red>[FS] Too many Vampiric Steeds detected nearby. Please move the extras away.</color>");
                return;
            }

            var hoveredHorse = nearbyHorses[0];
            if (entityManager.HasComponent<Mountable>(hoveredHorse))
            {
                var mount = entityManager.GetComponentData<Mountable>(hoveredHorse);

                // If the horse already has maximum stats
                if (AreFloatsEqual(mount.MaxSpeed, maxSpeed) &&
                    AreFloatsEqual(mount.Acceleration, maxAcceleration) &&
                    AreFloatsEqual(mount.RotationSpeed, maxRotation * 10f))
                {
                    ctx.Reply("<color=yellow>[FS] The Vampiric Steed already has max stats. No upgrade applied.</color>");
                    return;
                }

                // **Here** we pass the player's *Entity* (SenderCharacterEntity) to our new system
                var playerCharacter = ctx.Event.SenderCharacterEntity;

                // 1) Check if they have enough
                if (!HorseForgeInventorySystem.HasEnoughOfItem(playerCharacter, requiredCurrencyGUID, currencyCost))
                {
                    ctx.Reply($"<color=red>[FS] You need {currencyCost} {currencyName} to upgrade your Vampiric Steed.</color>");
                    return;
                }

                // 2) Remove items
                if (!HorseForgeInventorySystem.TryRemoveItem(playerCharacter, requiredCurrencyGUID, currencyCost))
                {
                    ctx.Reply("<color=red>[FS] Failed to remove the required items from your inventory.</color>");
                    return;
                }

                // 3) Actually do the upgrade
                mount.MaxSpeed      = maxSpeed;
                mount.Acceleration  = maxAcceleration;
                mount.RotationSpeed = maxRotation * 10f;
                entityManager.SetComponentData(hoveredHorse, mount);

                ctx.Reply($"<color=green>[FS] The Vampiric Steed has been upgraded for {currencyCost} {currencyName}!</color>");
            }
            else
            {
                ctx.Reply("<color=red>[FS] This entity does not have Mountable. Cannot upgrade.</color>");
            }
        }

        private static List<Entity> FindNearbyHorses(EntityManager entityManager, float3 playerPosition, float hoverRadius)
        {
            var query = entityManager.CreateEntityQuery(_vampireHorseQueryDesc);
            var allCandidates = query.ToEntityArray(Allocator.Temp);

            var nearbyHorses = new List<Entity>();
            try
            {
                foreach (var entity in allCandidates)
                {
                    var prefab = entityManager.GetComponentData<PrefabGUID>(entity);
                    if (prefab.GuidHash != VampireHorseGUID)
                        continue;

                    var horsePosition = entityManager.GetComponentData<LocalToWorld>(entity).Position;
                    if (math.distancesq(playerPosition, horsePosition) <= hoverRadius * hoverRadius)
                    {
                        nearbyHorses.Add(entity);
                    }
                }
            }
            finally
            {
                allCandidates.Dispose();
            }

            return nearbyHorses;
        }

        private static bool AreFloatsEqual(float value1, float value2, float epsilon = 0.0001f)
        {
            return math.abs(value1 - value2) < epsilon;
        }
    }
}
