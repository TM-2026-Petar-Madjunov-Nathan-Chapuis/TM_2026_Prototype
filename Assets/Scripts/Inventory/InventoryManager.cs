using System;
using TM.Saving;
using UnityEngine;
using Newtonsoft.Json;
using TM.Items;

namespace TM.Inventory
{
    public class InventoryManager : MonoBehaviour, ISaveable //sits on the player
    {
        public Inventory inventory;
        [field: SerializeField] public int slotNumber { get; private set; }
        public WeaponData exasdéflkj;

        string ISaveable.UID => "InventoryManager";


        private void Start()
        {
            inventory = new Inventory(slotNumber);
            this.inventory.ModifySlot(3, exasdéflkj, 4);
        }

        public void LoadData(string data)
        {
            InventoryManagerSaveData saveData = JsonConvert.DeserializeObject<InventoryManagerSaveData>(data);
            this.inventory.Load(saveData.inventory);
            this.slotNumber = saveData.slotNumber;
        }

        public object SaveData()
        {
            return new InventoryManagerSaveData
            {
                inventory = this.inventory.Save(), //needs its own save implementation because it contains a Scriptable Object
                slotNumber = this.slotNumber, 
            };
        }
        public void IncreaseSlots(int increase)
        {
            slotNumber += increase;
            inventory.IncreaseSlots(increase);
        }
    }

    [Serializable]
    public struct InventoryManagerSaveData
    {
        public InventorySaveData inventory;
        public int slotNumber;
    }
}