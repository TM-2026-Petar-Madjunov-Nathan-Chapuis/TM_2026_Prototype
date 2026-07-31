using System;
using TM.Saving;
using UnityEngine;
using Newtonsoft.Json;
using TM.Items;
using System.Linq;
using System.Collections.Generic;

namespace TM.Inventory
{
    public class InventoryManager : MonoBehaviour, ISaveable //sits on the player
    {
        public InventoryGrid inventoryGrid;
        [field: SerializeField] public int cellSize { get; private set; } = 64;

        [field: SerializeField, Range(0, 30f)]
        public int cellPosMargin { get; private set; }
        [field: SerializeField] public Vector2Int gridSize { get; private set; }
        public WeaponData exasdéflkj;
        public DrinkData whatdsaélkfjasédf;
        public Guid?[] ItemWheelIds;
        private int itemWheelSlotNumber;
        string ISaveable.UID => "InventoryManager";

        public void UpdateCellsize(float width, float height) //width is the total pixel width of the cell slots. 
        {
            int f = (int)width / this.gridSize.x;
            int j = (int)height / this.gridSize.y;
            this.cellSize = f > j ? j : f;
        }

        private void Start()
        {
            itemWheelSlotNumber = GameObject.FindAnyObjectByType<ItemWheelVectorImager>().slotNumber;
            ItemWheelIds = new Guid?[itemWheelSlotNumber];
            inventoryGrid = new InventoryGrid(gridSize);
            this.inventoryGrid.TryInsertItem(exasdéflkj, new Vector2Int(2, 2), false);
            this.inventoryGrid.TryInsertItem(exasdéflkj, new Vector2Int(6, 2), true);
            this.inventoryGrid.TryInsertItem(whatdsaélkfjasédf, new Vector2Int(6, 5), false);
        }

        public void LoadData(string data)
        {
            InventoryManagerSaveData saveData = JsonConvert.DeserializeObject<InventoryManagerSaveData>(data);
            this.inventoryGrid.Load(saveData.inventory);
            this.gridSize = saveData.gridSize;
            this.ItemWheelIds = saveData.ItemWheelIds;
        }

        public object SaveData()
        {

            InventoryManagerSaveData inventoryManagerSaveData = new InventoryManagerSaveData
            {
                inventory = this.inventoryGrid.Save(), //needs its own save implementation because it contains a Scriptable Object
                gridSize = this.gridSize,
                ItemWheelIds = this.ItemWheelIds,
            };
            return inventoryManagerSaveData;
        }

        public bool TryAddItem(ItemData itemData) //tries to add an item. it searches for a space. first left to right then top to bottom. not optimized at all.brute force. 
        {
            InventoryItem item = new InventoryItem();
            item.SetItemData(itemData);


            for (int y = 0; y < this.gridSize.y; y++)
            {
                for (int x = 0; x < this.gridSize.x; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (this.inventoryGrid.CanPlace(item, pos)) { this.inventoryGrid.TryInsertItem(itemData, pos, false); return true; }
                    ;
                    item.Rotate();
                    if (this.inventoryGrid.CanPlace(item, pos)) { this.inventoryGrid.TryInsertItem(itemData, pos, true); return true; }
                    item.Rotate();
                }
            }
            return false;
        }
        public void RemoveFromWheel(Guid id)
        {
            for(int i = 0; i < ItemWheelIds.Length; i++)
            {
                if(ItemWheelIds[i] == id)
                    ItemWheelIds[i] = null;
            }
        }
        public InventoryItem GetItemFromGuid(Guid? id)
        {
            return this.inventoryGrid.itemsList.FirstOrDefault(x => x.id == id);
        }
        public void DropItem(InventoryItem inventoryItem)
        {
            this.inventoryGrid.RemoveItem(inventoryItem);
            FindAnyObjectByType<ItemWheelManager>().OnUILoaded(new UnityEngine.UIElements.GeometryChangedEvent()); //trigger refresh on item wheel, a bit messy.
            this.RemoveFromWheel(inventoryItem.id);
            GameObject worldItem = GameObject.Instantiate(inventoryItem.itemData.worldPrefab);
            worldItem.GetComponent<WorldItem>().inventoryManager = this;
            worldItem.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        }
    }

    [Serializable]
    public struct InventoryManagerSaveData
    {
        public InventorySaveData inventory;
        public Vector2Int gridSize;
        public Guid?[] ItemWheelIds;
    }
}