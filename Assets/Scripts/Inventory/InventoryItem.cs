using System;
using TM.Saving;
using TM.Items;
using System.Numerics;
using UnityEngine;

namespace TM.Inventory
{
    public class InventoryItem
    {
        public ItemData itemData { get; private set; }
        public int count { get; private set; }
        public Vector2Int position { get; private set; }
        public bool rotated { get; private set; }

        public InventoryItem SetItemData(ItemData itemData)
        {
            this.itemData = itemData;
            return this;
        }

        public InventoryItem SetCount(int count)
        {
            this.count = count;
            return this;
        }
        public InventoryItem SetPosition(Vector2Int pos)
        {
            this.position = pos;
            return this;
        }
        public InventoryItem Rotate()
        {
            this.rotated = !this.rotated;
            return this;
        }
        public InventoryItemSaveData Save()
        {
            return new InventoryItemSaveData
                {
                    itemDataUUID = this.itemData ? this.itemData.UUID : null,
                    count = this.count,
                    position = this.position,
                    rotated = this.rotated, 
                };
        }
        public void Load(InventoryItemSaveData saveData, ItemData itemData)
        {
            this.position = saveData.position;
            this.count = saveData.count;
            this.itemData = itemData;
            this.rotated = rotated;
        }
    }
    [Serializable]
    public struct InventoryItemSaveData
    {
        public UUID? itemDataUUID;
        public int count;
        public Vector2Int position;
        public bool rotated;
    }
}