using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/Drink")]
    public class DrinkBehaviour : UseBehavior
    {
        public override void Use(HeldItem heldItem, bool pressed)
        {
            if (!pressed) return;
            if (heldItem.itemData is DrinkData drinkData)
            { // essaie de cast l'item en drinkData
                Debug.Log($"{drinkData.drinkAmmount} was used by the player, but no effect defined");
            }
            else
            {
                base.Use(heldItem, pressed);
            }
        }
    }
}