using TM.Inventory;
using TM.Player;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/Food")]
    public class FoodBehavior : UseBehavior
    {
        public string animationTrigger;
        public override void Use(HeldItem heldItem, bool pressed)
        {
            if (!pressed) return;
            if (heldItem.itemData is FoodData foodData)
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
            if (heldItem.itemData is FoodData foodData)
            {
                PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
                playerInfo.playerHunger += foodData.satietyAmmount;
                Mathf.Clamp(0,100, playerInfo.playerHunger);    

                GameObject.Destroy(heldItem.gameObject);
                player.GetComponent<InventoryManager>().DestroyItem(inventoryItem);
            }
        }
    }
}