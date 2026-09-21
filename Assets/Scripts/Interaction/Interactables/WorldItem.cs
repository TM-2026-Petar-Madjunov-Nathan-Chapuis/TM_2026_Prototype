using TM.Inventory;
using UnityEngine;

namespace TM.Items
{
    public class WorldItem : MonoBehaviour, IInteractable
    {
        [SerializeField] public ItemData itemData;
        public InteractionType GetInteractionType() => InteractionType.PickUp;
        public void Interact(GameObject player)
        {
            if (player.GetComponent<InventoryManager>().TryAddItem(itemData))
            {
                Destroy(this.gameObject);
            }
        }
    }
}