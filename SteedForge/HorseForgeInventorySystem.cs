using System;
using Bloodstone.API;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace SteedForge
{
    public static class HorseForgeInventorySystem
    {
     
        public static bool HasEnoughOfItem(Entity playerEntity, PrefabGUID itemGuid, int neededAmount)
        {
            var em = VWorld.Server.EntityManager;

            // 1) Get the actual "inventory entity" for this character
            if (!InventoryUtilities.TryGetInventoryEntity(em, playerEntity, out var inventoryEntity))
            {
                return false;
            }

            // 2) Ensure it has an InventoryBuffer
            if (!em.HasComponent<InventoryBuffer>(inventoryEntity))
            {
                return false;
            }

            // 3) Sum how many of itemGuid are present in the buffer
            var buffer = em.GetBuffer<InventoryBuffer>(inventoryEntity);

            int count = 0;
            foreach (var slot in buffer)
            {
                if (slot.ItemType == itemGuid)
                {
                    count += slot.Amount;
                    if (count >= neededAmount)
                    {
                        return true;
                    }
                }
            }

            return (count >= neededAmount);
        }

       
        public static bool TryRemoveItem(Entity playerEntity, PrefabGUID itemGuid, int removeAmount)
        {
            if (removeAmount <= 0)
                return true; // no items to remove

            var em = VWorld.Server.EntityManager;

            // 1) Get the "inventory entity"
            if (!InventoryUtilities.TryGetInventoryEntity(em, playerEntity, out var inventoryEntity))
            {
                return false;
            }

            if (!em.HasComponent<InventoryBuffer>(inventoryEntity))
            {
                return false;
            }

            // 2) Loop across each slot, remove partial amounts from stacks if necessary
            var buffer = em.GetBuffer<InventoryBuffer>(inventoryEntity);
            int neededToRemove = removeAmount;

            for (int i = 0; i < buffer.Length && neededToRemove > 0; i++)
            {
                var slot = buffer[i];
                if (slot.ItemType == itemGuid && slot.Amount > 0)
                {
                    int toRemove = math.min(slot.Amount, neededToRemove);

                    // Remove from this slot using built-in ECS method:
                    InventoryUtilitiesServer.TryRemoveItemAtIndex(
                        em,
                        playerEntity,    // The character entity that has this inventory
                        slot.ItemType,   // The item type we are removing
                        toRemove,        // How many to remove
                        i,               // Index of the slot
                        false            // 'removeAllIfNotEnough' -> false means remove EXACTLY toRemove
                    );

                    neededToRemove -= toRemove;
                }
            }

            // If neededToRemove is still > 0, we couldn't remove enough
            return (neededToRemove <= 0);
        }
    }
}
