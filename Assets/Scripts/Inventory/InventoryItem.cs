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
        public Guid id { get; private set; } = Guid.NewGuid();
        public Vector2Int position { get; private set; }
        public bool rotated { get; private set; }

        public InventoryItem SetItemData(ItemData itemData)
        {
            this.itemData = itemData;
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
                    position = this.position,
                    rotated = this.rotated, 
                    id = this.id,
                };
        }
        public void Load(InventoryItemSaveData saveData, ItemData itemData)
        {
            this.position = saveData.position;
            this.itemData = itemData;
            this.rotated = saveData.rotated;
            this.id = saveData.id;
        }
    }
    [Serializable]
    public struct InventoryItemSaveData
    {
        public UUID? itemDataUUID;
        public Vector2Int position;
        public bool rotated;
        public Guid id;
    }
}