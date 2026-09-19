using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/MeleeWeapon")]
    public class MeleeWeaponBehavior : UseBehavior
    {
        public string animationTrigger; //set pour chaque meleeeweaponbehavior.
        public float knowbackForce = 10000f;
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
        public override void OnTriggerEnter(Collider collider, HeldItem heldItem, GameObject player)
        {
            Debug.Log("handle damage assignement and knoback here");
            Vector3 playerpos = heldItem.player.gameObject.transform.position;
            Vector3 relative =  collider.gameObject.transform.position - playerpos;
            if (collider.gameObject.GetComponent<Rigidbody>())
            {
            collider.gameObject.GetComponent<Rigidbody>().AddForce(relative * knowbackForce);
            collider.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0));             
            }
        }
    }
}