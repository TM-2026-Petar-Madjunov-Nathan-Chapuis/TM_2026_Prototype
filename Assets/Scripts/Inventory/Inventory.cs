using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TM.Saving;
using TM.Items;

namespace TM.Inventory
{
    [Serializable]
    public class Inventory
    {
        public int slotNumber { get; private set; }
        public InventoryItem[] inventoryItems { get; private set; }
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
        public InventorySaveData Save()
        {
            InventoryItemSaveData[] inventoryItemSaveDatas = new InventoryItemSaveData[slotNumber];
            for (int i = 0; i < slotNumber; i++) 
            {
                inventoryItemSaveDatas[i] = this.inventoryItems[i].Save();
            }
            return new InventorySaveData
            {
                inventoryItems = inventoryItemSaveDatas,
                slotNumber = this.slotNumber,
            };
        }
        public void Load(InventorySaveData saveData)
        {
            this.slotNumber = saveData.slotNumber;
            InventoryItem[] savedItems = new InventoryItem[this.slotNumber];
            for (int i = 0; i < this.slotNumber; i++)
            {
                savedItems[i] = new InventoryItem();
            }
            //We need to use UuidFinder.FindMultiple() to use less ressources by getting through the SOs loop O(n) once instead of calling findUnique() at each InventoryItem.Load() which would result in O(n**2)
            UUID?[] uUIDs = saveData.inventoryItems.Select(item => item.itemDataUUID).ToArray();
            Dictionary<UUID, ScriptableObject> lookup = UuidFinder.findMultiple(uUIDs);
            for (int i = 0; i < this.slotNumber; i++)
            {
                UUID? uUID = saveData.inventoryItems[i].itemDataUUID;
                if(uUID != null)
                {
                    savedItems[i].Load(saveData.inventoryItems[i], (ItemData)lookup[uUID.Value]);
                }
            }
        }
    }
    [Serializable]
    public struct InventorySaveData
    {
        public InventoryItemSaveData[] inventoryItems;
        public int slotNumber;
    }
}