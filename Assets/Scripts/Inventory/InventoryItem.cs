using System;
using TM.Saving;
using TM.Items;

namespace TM.Inventory
{
    public class InventoryItem
    {
        public ItemData itemData { get; private set; }
        public int count { set; private get; }

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
        public InventoryItemSaveData Save()
        {
            if (this.itemData)
            {
                return new InventoryItemSaveData
                {
                    itemDataUUID = this.itemData.UUID,
                    count = this.count,
                };
            }
            return new InventoryItemSaveData
            {
                itemDataUUID = null,
                count = this.count,
            };
        }
        public void Load(InventoryItemSaveData saveData, ItemData itemData)
        {
            this.count = saveData.count;
            this.itemData = itemData;
        }
    }
    [Serializable]
    public struct InventoryItemSaveData
    {
        public UUID? itemDataUUID;
        public int count;
    }
}