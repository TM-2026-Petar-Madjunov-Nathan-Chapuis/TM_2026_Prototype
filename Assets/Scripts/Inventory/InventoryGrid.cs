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
        public InventoryItem[,] itemGridMap { get; private set; } // [,] means 2d array, [x,y]
        public List<InventoryItem> itemsList { get; private set; }
        public InventoryGrid(Vector2Int gridSize)
        {
            this.gridSize = gridSize;
            this.itemGridMap = new InventoryItem[gridSize.x, gridSize.y];
            this.itemsList = new();
        }
        public bool CanPlace(InventoryItem inventoryItem, Vector2Int pos)
        {
            Vector2Int size = inventoryItem.itemData.size;

            //boundary check
            if (pos.x < 0 || pos.y < 0) return false; //out of bounds
            if (pos.x + size.x > gridSize.x || pos.y + size.y > gridSize.y) return false; //out of bounds

            for (int x = pos.x; x < pos.x+size.x; x++) //check every square the item would occupy and if another item is in there just return false
            {
                for (int y = pos.y; y < pos.y+size.y; y++)
                {
                    if (itemGridMap[x,y] != null && itemGridMap[x,y] != inventoryItem) return false; 
                }
            }
            Debug.Log("passed");
            return true;
        }
        public (List<Vector2Int>, List<Vector2Int>) OverLapingItems(InventoryItem inventoryItem, Vector2Int pos)
        {
            Vector2Int size = inventoryItem.itemData.size;
            List<Vector2Int> occupied = new();
            List<Vector2Int> availiable = new();

            for (int x = pos.x; x < pos.x+size.x; x++) //check every square the item would occupy and if another item is in there just return false
            {
                for (int y = pos.y; y < pos.y+size.y; y++)
                {
                    if (pos.x < 0 || pos.y < 0)  break; //out of bounds
                    if (pos.x + size.x > gridSize.x || pos.y + size.y > gridSize.y) break; //out of bounds
                    if (itemGridMap[x,y] != null) occupied.Add(new Vector2Int(x,y)); 
                    else availiable.Add(new Vector2Int(x,y));
                }
            }
            return (occupied, availiable);
        }
        public bool TryInsertItem(ItemData itemData, Vector2Int pos, int count)
        {
            InventoryItem item = new InventoryItem();
            item.SetCount(count);
            item.SetItemData(itemData);
            item.SetPosition(pos);

            if(!CanPlace(item, pos)) return false;
            Vector2Int size = item.itemData.size;
            for(int x = pos.x; x < pos.x + size.x; x++)
            {
                for (int y = pos.y; y < pos.y + size.y; y++)
                {
                    itemGridMap[x,y] = item;
                }
            }
            this.itemsList.Add(item);
            return true;
        }
        public bool TryMoveItem(InventoryItem item, Vector2Int pos)
        {
            if(!CanPlace(item, pos)) return false;
            RemoveItem(item);
            Vector2Int size = item.itemData.size;
            item.SetPosition(pos);
            for(int x = pos.x; x < pos.x + size.x; x++)
            {
                for (int y = pos.y; y < pos.y + size.y; y++)
                {
                    itemGridMap[x,y] = item;
                }
            }
            this.itemsList.Add(item);
            return true;
        }
        public void RemoveItem(InventoryItem item)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                for(int y = 0; y < gridSize.y; y++)
                {
                    if(itemGridMap[x,y] == item) itemGridMap[x,y] = null;
                }
            }
            this.itemsList.Remove(item);
        }
        public InventorySaveData Save()
        {
            List<InventoryItemSaveData> saveDatas = new();
            foreach(InventoryItem inventoryItem in this.itemsList)
            {
                saveDatas.Add(inventoryItem.Save());
            }            
            return new InventorySaveData
            {
                itemSaveDatas = saveDatas,
                gridSize = this.gridSize,
            };
        }
        public void Load(InventorySaveData saveData)
        {
            this.itemGridMap = new InventoryItem[gridSize.x, gridSize.y];
            this.itemsList = new();

            this.gridSize = saveData.gridSize;

            //We need to use UuidFinder.FindMultiple() to use less ressources by getting through the SOs loop O(n) once instead of calling findUnique() at each InventoryItem.Load() which would result in O(n**2)
            UUID?[] uUIDs = saveData.itemSaveDatas.Select(item => item.itemDataUUID).ToArray();
            Dictionary<UUID, ScriptableObject> lookup = UuidFinder.findMultiple(uUIDs);
            foreach(InventoryItemSaveData inventoryItemSaveData in saveData.itemSaveDatas)
            {
                InventoryItem item = new();
                item.Load(inventoryItemSaveData, (ItemData)lookup[inventoryItemSaveData.itemDataUUID.Value]);
                this.itemsList.Add(item);
            }
            foreach(InventoryItem item in this.itemsList)
            {
                Vector2Int pos = item.position;
                for(int x = pos.x; x < pos.x + item.itemData.size.x; x++)
                {
                    for(int y = pos.y; y < pos.y + item.itemData.size.y; y++)
                    {
                        if (itemGridMap[x,y] != null)
                        {
                            Debug.LogError($"another item is already at {x}-{y}");
                        }
                        itemGridMap[x,y] = item;      
                    }
                }
            }
        }
    }
    [Serializable]
    public struct InventorySaveData
    {
        public List<InventoryItemSaveData> itemSaveDatas;
        public Vector2Int gridSize;
    }
}