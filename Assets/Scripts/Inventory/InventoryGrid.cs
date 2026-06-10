using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TM.Saving;
using TM.Items;
using UnityEngine.UIElements;
using Unity.Collections;
using Unity.VisualScripting;

namespace TM.Inventory
{
    [Serializable]
    public class InventoryGrid
    {
        public Vector2Int gridSize { get; private set; }
        public InventoryItem[,] inventoryItems { get; private set; } // [,] means 2d array, [x,y]
        public InventoryGrid(Vector2Int gridSize)
        {
            this.gridSize = gridSize;
            this.inventoryItems = new InventoryItem[gridSize.x, gridSize.y];
        }
        public bool CanPlace(InventoryItem inventoryItem, Vector2Int pos)
        {
            Vector2Int size = inventoryItem.itemData.size;

            //boundary check
            if (pos.x < 0 || pos.y < 0) return false;
            if (pos.x + size.x > gridSize.x || pos.y + size.y > gridSize.y)
            {
                return false; //out of bounds
            }

            for (int x = pos.x; x > x+size.x; x++) //check every square the item would occupy and if another item is in there just return false
            {
                for (int y = pos.y; y > y+size.y; y++)
                {
                    if (inventoryItems[x,y] != null) return false; 
                }
            }
            return true;
        }
        public bool TryInsertItem(ItemData itemData, Vector2Int pos, int count)
        {
            InventoryItem item = new InventoryItem();
            item.SetCount(count);
            item.SetItemData(itemData);

            if(!CanPlace(item, pos)) return false;
            Vector2Int size = item.itemData.size;
            for(int x = pos.x; x < x + size.x;)
            {
                for (int y = pos.y; y < y + size.y;)
                {
                    
                    inventoryItems[x,y] = item;
                }
            }
            return true;
        }
        public void RemoveItem(InventoryItem item)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                for(int y = 0; y < gridSize.y; y++)
                {
                    if(inventoryItems[x,y] == item) inventoryItems[x,y] = null;
                }
            }
        }
        public InventorySaveData Save()
        {
            InventoryItemSaveData[,] inventoryItemSaveDatas = new InventoryItemSaveData[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    inventoryItemSaveDatas[x,y] = this.inventoryItems[x,y] != null ? this.inventoryItems[x,y].Save() : default;
                }
            }
            return new InventorySaveData
            {
                inventoryItems = inventoryItemSaveDatas,
                gridSize = this.gridSize,
            };
        }
        public void Load(InventorySaveData saveData)
        {
            this.gridSize = saveData.gridSize;
            InventoryItem[,] savedItems = new InventoryItem[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    savedItems[x,y] = new InventoryItem();
                }
            }
            //We need to use UuidFinder.FindMultiple() to use less ressources by getting through the SOs loop O(n) once instead of calling findUnique() at each InventoryItem.Load() which would result in O(n**2)
            UUID?[] uUIDs = saveData.inventoryItems //lists all UUID
                .Cast<InventoryItemSaveData>() //flattens the 2d array
                .Select(item => item.itemDataUUID)
                .ToArray();
            Dictionary<UUID, ScriptableObject> lookup = UuidFinder.findMultiple(uUIDs); //for each UUID, finds the correct Scriptable Object item data, discards null values
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    UUID? uUID = saveData.inventoryItems[x,y].itemDataUUID;
                    if (uUID != null)
                    {
                        savedItems[x,y].Load(saveData.inventoryItems[x,y], (ItemData)lookup[uUID.Value]);
                    }
                }
            }
            this.inventoryItems = savedItems;
        }
    }
    [Serializable]
    public struct InventorySaveData
    {
        public InventoryItemSaveData[,] inventoryItems;
        public Vector2Int gridSize;
    }
}