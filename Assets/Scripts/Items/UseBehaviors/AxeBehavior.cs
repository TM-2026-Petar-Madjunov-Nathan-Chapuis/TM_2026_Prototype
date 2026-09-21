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
            if (!pressed) return;
            if (heldItem.itemData is AxeData)
            {
                heldItem.Animate(animationTrigger);
            }
            else
            {
                base.Use(heldItem, pressed);
            }
        }
        public override void Collision(Collider collider, HeldItem heldItem, GameObject player)
        {
            if (heldItem.itemData is AxeData axeData)
            {
                if (collider.gameObject.CompareTag("Tree"))
                {
                    collider.gameObject.GetComponent<TreeFall>().Fall(player.GetComponent<InventoryManager>(), player.transform.position);
                }
                if (collider.gameObject.CompareTag("Knockbackable"))
                {
                    Vector3 playerpos = heldItem.player.gameObject.transform.position;
                    Vector3 relative = collider.gameObject.transform.position - playerpos;
                    collider.gameObject.GetComponent<Rigidbody>().AddForce(relative.normalized * axeData.knockbackForce);
                }
                if (collider.gameObject.GetComponent<HealthManager>())
                {
                    Vector3 playerpos = heldItem.player.gameObject.transform.position;
                    Vector3 relative = collider.gameObject.transform.position - playerpos;
                    collider.gameObject.GetComponent<HealthManager>().TakeDamage(axeData.damage, "Was stuck by " + axeData.name);
                    if (collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                    {
                        rigidbody.AddForce(relative.normalized * axeData.knockbackForce);
                    }
                }

            }
        }
    }
}