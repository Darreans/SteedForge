using System.Collections.Generic;
using Bloodstone.API;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using VampireCommandFramework;
using BloodyShop.Server.Core;

namespace SteedForge
{
    public static class Commands
    {
        [Command("upgradehorse", "uh", description: "Upgrade the stats of a tamed vampire horse you are near", adminOnly: false)]
        public static void UpgradeHorse(ChatCommandContext ctx)
        {
            var entityManager = VWorld.Server.EntityManager;

            // Load configurable values from MainConfig
            float maxSpeed = MainConfig.DefaultSpeed.Value;
            float maxAcceleration = MainConfig.DefaultAcceleration.Value;
            float maxRotation = MainConfig.DefaultRotation.Value;
            int currencyCost = MainConfig.CurrencyCost.Value;
            string currencyName = MainConfig.CurrencyName.Value;
            var requiredCurrencyGUID = new PrefabGUID(MainConfig.RequiredCurrencyGUID.Value);

            // Get the player's position
            var playerPosition = entityManager.GetComponentData<LocalToWorld>(ctx.Event.SenderCharacterEntity).Position;

            
            const float hoverRadius = 2f;

            // Get all nearby horses
            var nearbyHorses = FindNearbyHorses(entityManager, playerPosition, hoverRadius);

            if (nearbyHorses.Count == 0)
            {
                ctx.Reply("<color=yellow>[FS] No Vampiric Steed found within range.</color>");
                return;
            }

            if (nearbyHorses.Count > 1)
            {
                ctx.Reply("<color=red>[FS] Too many Vampiric Steeds detected nearby. No stealing! Please move the other horses away.</color>");
                return;
            }

            // Only one horse is nearby, proceed to check and potentially upgrade its stats
            var hoveredHorse = nearbyHorses[0];
            if (entityManager.HasComponent<Mountable>(hoveredHorse))
            {
                var mount = entityManager.GetComponentData<Mountable>(hoveredHorse);

                // Check if the horse already has maximum stats
                if (AreFloatsEqual(mount.MaxSpeed, maxSpeed) &&
                    AreFloatsEqual(mount.Acceleration, maxAcceleration) &&
                    AreFloatsEqual(mount.RotationSpeed, maxRotation * 10f))
                {
                    ctx.Reply("<color=yellow>[FS] The Vampiric Steed already has the maximum stats. No upgrade applied.</color>");
                    return;
                }

                // Check if the player has sufficient currency
                var characterName = ctx.Event.User.CharacterName.ToString();
                if (!InventorySystem.verifyHaveSuficientPrefabsInInventory(characterName, requiredCurrencyGUID, currencyCost))
                {
                    ctx.Reply($"<color=red>[FS] You need {currencyCost} {currencyName} to upgrade your Vampiric Steed.</color>");
                    return;
                }

                // Deduct the currency from the player's inventory
                if (!InventorySystem.getPrefabFromInventory(characterName, requiredCurrencyGUID, currencyCost))
                {
                    ctx.Reply("<color=red>[FS] Failed to deduct the required items from your inventory.</color>");
                    return;
                }

                // Upgrade the horse's stats
                mount.MaxSpeed = maxSpeed;
                mount.Acceleration = maxAcceleration;
                mount.RotationSpeed = maxRotation * 10f; // Multiply by 10 for rotational speed consistency
                entityManager.SetComponentData(hoveredHorse, mount);

                ctx.Reply($"<color=green>[FS] The Vampiric Steed has been upgraded for {currencyCost} {currencyName}!</color>");
            }
            else
            {
                ctx.Reply("<color=red>[FS] Failed to upgrade the stats. The targeted entity does not have the required components.</color>");
            }
        }

        // Helper method: Find all horses within the hover radius
        private static List<Entity> FindNearbyHorses(EntityManager entityManager, float3 playerPosition, float hoverRadius)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabGUID>(), ComponentType.ReadOnly<LocalToWorld>());
            var entities = query.ToEntityArray(Allocator.Temp);

            var nearbyHorses = new List<Entity>();

            foreach (var entity in entities)
            {
                if (!entityManager.HasComponent<PrefabGUID>(entity)) continue;

                var prefabGUID = entityManager.GetComponentData<PrefabGUID>(entity);
                if (prefabGUID.GuidHash != -1502865710) // Check for Vampire Horse GUID
                    continue;

                var horsePosition = entityManager.GetComponentData<LocalToWorld>(entity).Position;
                if (math.distancesq(playerPosition, horsePosition) <= hoverRadius * hoverRadius)
                {
                    nearbyHorses.Add(entity);
                }
            }

            entities.Dispose(); // Dispose of the query results
            return nearbyHorses;
        }

        // Helper method: Compare two floats with an epsilon tolerance
        private static bool AreFloatsEqual(float value1, float value2, float epsilon = 0.0001f)
        {
            return math.abs(value1 - value2) < epsilon;
        }
    }
}
