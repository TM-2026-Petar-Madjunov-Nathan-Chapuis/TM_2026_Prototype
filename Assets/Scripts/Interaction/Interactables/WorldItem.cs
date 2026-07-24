using TM.Inventory;
using UnityEngine;

namespace TM.Items
{
    public class WorldItem : MonoBehaviour, IInteractable
    {
        [SerializeField] public InventoryManager inventoryManager;
        [SerializeField] public ItemData itemData;
        public InteractionType GetInteractionType() => InteractionType.PickUp;
        public void Interact()
        {
            if (inventoryManager.TryAddItem(itemData))
            {
                Destroy(this.gameObject);
            }
        }
    }
}