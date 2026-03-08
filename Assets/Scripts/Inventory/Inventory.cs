using System;
using UnityEngine;

namespace TM.Inventory
{
    [Serializable]
    public class Inventory
    {
        [field: SerializeField] public int slotNumber { get; private set; }
        [field: SerializeField] public InventoryItem[] inventoryItems { get; private set; }
        public Inventory(int slotNumber)
        {
            this.slotNumber = slotNumber;
            this.inventoryItems = new InventoryItem[slotNumber];
            for (int i = 0; i < slotNumber; i++)
            {
                this.inventoryItems[i] = new InventoryItem
                {
                    count = 0
                };
            }
        }
        public void ModifySlot(int slotNumber, ItemData itemData, int count)
        {
            if (slotNumber < inventoryItems.Length)
            {
                inventoryItems[slotNumber].SetItemData(itemData).SetCount(count);
            }
            else
            {
                Debug.LogError($"Trying to modify unexisting slot (Number of slots : {this.slotNumber}, tried to modify : {slotNumber})");
            }
        }
        public void IncreaseSlots(int increase)
        {
            InventoryItem[] inventoryItems;
            inventoryItems = new InventoryItem[slotNumber + increase];
            slotNumber += increase;
            for (int i = 0; i < this.inventoryItems.Length; i++)
            {
                inventoryItems[i] = this.inventoryItems[i];
            }
            for (int i = 0; i < increase; i++)
            {
                inventoryItems[i] = new InventoryItem();
            }
            this.inventoryItems = inventoryItems;
        }
    }
}