using System;
using ProjectM; 
using Stunlock.Core;
using Unity.Entities; 
using Unity.Mathematics; 
using Unity.Collections;

namespace SteedForge
{
    public static class SteedForgeInventorySystem
    {
        public static bool HasEnoughOfItem(Entity playerEntity, PrefabGUID itemGuid, int neededAmount)
        {
            var em = VWorldUtils.Server.EntityManager; 

            if (!InventoryUtilities.TryGetInventoryEntity(em, playerEntity, out var inventoryEntity))
            {
                return false;
            }
            if (!em.HasBuffer<InventoryBuffer>(inventoryEntity))
            {
                return false;
            }
            var buffer = em.GetBuffer<InventoryBuffer>(inventoryEntity);
            int count = 0;
            foreach (var slot in buffer)
            {
                if (slot.ItemType == itemGuid)
                {
                    count += slot.Amount;
                }
            }
            return (count >= neededAmount);
        }

        public static bool TryRemoveItem(Entity playerEntity, PrefabGUID itemGuid, int removeAmount)
        {
            if (removeAmount <= 0)
                return true; 

            var em = VWorldUtils.Server.EntityManager; 

            bool success = InventoryUtilitiesServer.TryRemoveItem(em, playerEntity, itemGuid, removeAmount);


            return success;
        }
    }
}