using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class DrinkData : ItemData
{
    public int drinkAmmount;

    public DrinkData(string name, string description, Sprite icon, UseBehavior useBehavior, int drinkAmmount) : base (name, description, icon, useBehavior)
    {
        this.drinkAmmount = drinkAmmount;
    }
}
