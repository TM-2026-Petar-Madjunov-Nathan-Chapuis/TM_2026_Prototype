using System;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
    public class DrinkData : ItemData
    {
        public int drinkAmmount;

        public DrinkData(string name, string description, Sprite icon, Vector2Int size, UseBehavior useBehavior, int drinkAmmount) : base(name, description, icon, size, useBehavior)
        {
            this.drinkAmmount = drinkAmmount;
        }
    }
}