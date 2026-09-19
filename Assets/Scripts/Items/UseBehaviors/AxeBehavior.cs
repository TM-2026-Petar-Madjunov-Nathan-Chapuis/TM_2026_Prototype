using TM.Inventory;
using TM.Misc;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/Axe")]
    public class AxeBehavior : UseBehavior
    {
        public string animationTrigger; //set pour chaque axeweaponbehavior.
        public override void Use(HeldItem heldItem, bool pressed)
        {
            Debug.Log("using axe");
            if (!pressed) return;
            if (heldItem.itemData is WeaponData)
            {
                heldItem.Animate(animationTrigger);
            }
            else
            {
                base.Use(heldItem, pressed);
            }
        }
        public override void OnTriggerEnter(Collider collider, HeldItem heldItem, GameObject player)
        {
            if (collider.gameObject.CompareTag("Tree"))
            {
                collider.gameObject.GetComponent<TreeFall>().Fall(player.GetComponent<InventoryManager>(), player.transform.position);
            }
        }
    }
}