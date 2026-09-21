using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/MeleeWeapon")]
    public class MeleeWeaponBehavior : UseBehavior
    {
        public string animationTrigger; //set pour chaque meleeeweaponbehavior.
        public override void Use(HeldItem heldItem, bool pressed)
        {
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
        public override void Collision(Collider collider, HeldItem heldItem, GameObject player)
        {
            if (heldItem.itemData is WeaponData weaponData) 
            {
                Vector3 playerpos = heldItem.player.gameObject.transform.position;
                Vector3 relative =  collider.gameObject.transform.position - playerpos;
                if (collider.gameObject.GetComponent<HealthManager>() && collider.gameObject.GetComponent<Rigidbody>())
                {
                    collider.gameObject.GetComponent<HealthManager>().TakeDamage(weaponData.damage, "Was stuck by " + weaponData.name);
                    collider.gameObject.GetComponent<Rigidbody>().AddForce(relative.normalized * weaponData.knockbackForce);
                }
            }
        }
 
    }
}