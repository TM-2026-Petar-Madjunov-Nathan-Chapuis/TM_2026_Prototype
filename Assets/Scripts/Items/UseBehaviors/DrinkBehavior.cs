using TM.Inventory;
using TM.Player;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/Drink")]
    public class DrinkBehaviour : UseBehavior
    {
        public string animationTrigger;
        public GameObject crushedDrink; 
        public float throwForce;
        public float throwAngle;
        public override void Use(HeldItem heldItem, bool pressed)
        {
            if (!pressed) return;
            if (heldItem.itemData is DrinkData drinkData)
            { // essaie de cast l'item en drinkData
                heldItem.Animate(this.animationTrigger);
            }
            else
            {
                base.Use(heldItem, pressed);
            }
        }
        public override void OnAnimationUse(HeldItem heldItem, GameObject player, InventoryItem inventoryItem)
        {
            if (heldItem.itemData is DrinkData drinkData)
            {
                PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
                playerInfo.playerThirst += drinkData.drinkAmmount;
                Mathf.Clamp(0,100, playerInfo.playerThirst);    

                GameObject crushedCan = Instantiate(crushedDrink);
                crushedCan.transform.position = heldItem.transform.position + player.transform.up * 1.4f; //small vertical offset
                Vector3 direction = Quaternion.AngleAxis(-throwAngle, player.transform.right) * (-player.transform.forward);
                crushedCan.GetComponent<Rigidbody>().AddForce(direction * throwForce);
                crushedCan.GetComponent<WorldItem>().inventoryManager = player.GetComponent<InventoryManager>();
                GameObject.Destroy(heldItem.gameObject);
                player.GetComponent<InventoryManager>().DestroyItem(inventoryItem);
            }
        }
    }
}