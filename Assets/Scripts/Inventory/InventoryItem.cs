using System;
using UnityEngine;

namespace TM.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        [field: SerializeField] public ItemData itemData { get; private set; }
        [field: SerializeField] public int count { set; private get; }

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
    }
}