using System;
using TM.Saving;
using UnityEngine;
using Newtonsoft.Json;
using TM.Items;

namespace TM.Inventory
{
    public class InventoryManager : MonoBehaviour, ISaveable //sits on the player
    {
        public InventoryGrid inventoryGrid;
        [field: SerializeField] public int cellSize {get; private set; } = 64 ;

        [field: SerializeField, Range(0, 30f)]
        public int cellPosMargin {get; private set; }
        [field: SerializeField] public Vector2Int gridSize { get; private set; }
        public WeaponData exasdéflkj;
        public DrinkData whatdsaélkfjasédf;

        string ISaveable.UID => "InventoryManager";

        public void UpdateCellsize(float width, float height) //width is the total pixel width of the cell slots. 
        {
            int f = (int)width / this.gridSize.x;
            int j = (int)height / this.gridSize.y;
            this.cellSize = f > j ? j : f;
        }

        private void Start()
        {
            inventoryGrid = new InventoryGrid(gridSize);
            this.inventoryGrid.TryInsertItem(exasdéflkj, new Vector2Int(2,2), 4, false);
            this.inventoryGrid.TryInsertItem(exasdéflkj, new Vector2Int(6,2), 576, true);
            this.inventoryGrid.TryInsertItem(whatdsaélkfjasédf, new Vector2Int(5,5), 3, true);
        }

        public void LoadData(string data)
        {
            InventoryManagerSaveData saveData = JsonConvert.DeserializeObject<InventoryManagerSaveData>(data);
            this.inventoryGrid.Load(saveData.inventory);
            this.gridSize = saveData.gridSize;
        }

        public object SaveData()
        {
            return new InventoryManagerSaveData
            {
                inventory = this.inventoryGrid.Save(), //needs its own save implementation because it contains a Scriptable Object
                gridSize = this.gridSize, 
            };
        }
    }

    [Serializable]
    public struct InventoryManagerSaveData
    {
        public InventorySaveData inventory;
        public Vector2Int gridSize;
    }
}