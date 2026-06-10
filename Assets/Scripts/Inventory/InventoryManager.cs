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
        [field: SerializeField] public Vector2Int gridSize { get; private set; }
        public WeaponData exasdéflkj;

        string ISaveable.UID => "InventoryManager";


        private void Start()
        {
            inventoryGrid = new InventoryGrid(gridSize);
            this.inventoryGrid.TryInsertItem(exasdéflkj, new Vector2Int(2,2), 4);
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