using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(menuName = "Inventory/Behaviors/Drink")]
    public class DrinkBehaviour : UseBehavior
    {
        public override void Use(GameObject user, ItemData item)
        {
            if (item is DrinkData drinkData)
            { // essaie de cast l'item en drinkData
                Debug.Log($"{drinkData.drinkAmmount} was used by {user.name}, but no effect defined");
            }
            else
            {
                base.Use(user, item);
            }
        }
    }
}