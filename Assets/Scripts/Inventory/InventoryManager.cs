using System;
using TM.Saving;
using UnityEngine;

namespace TM.Inventory
{
    public class InventoryManager : MonoBehaviour, ISaveable //sits on the player
    {
        public Inventory inventory;
        [field: SerializeField] public int slotNumber { get; private set; }

        string ISaveable.UID => "InventoryManager";


        private void Start()
        {
            inventory = new Inventory(slotNumber);
            WeaponData exalibur = new WeaponData("exalibur", "The best sword in the world", null, null, 10, 100);
            inventory.ModifySlot(3, exalibur, 50);
        }

        public void LoadData(string data)
        {
            InventorySaveData inventorySaveData = JsonUtility.FromJson<InventorySaveData>(data);
            this.inventory = inventorySaveData.inventory;
            this.slotNumber = inventorySaveData.slotNumber;
        }

        public object SaveData()
        {
            return new InventorySaveData
            {
                inventory = this.inventory,
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
    public struct InventorySaveData
    {
        public Inventory inventory;
        public int slotNumber;
    }
}