using System;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
    public class DrinkData : ItemData
    {
        public int drinkAmmount;
        public float weight;
        public override List<ItemUIStat> GetItemUIStats()
        {
            return new List<ItemUIStat>
            {
                new ItemUIStat("Weight", weight.ToString()),
                new ItemUIStat("Drink Ammout", drinkAmmount.ToString())
            };
        }
        public override ItemType GetItemType()
        {
            return ItemType.Drink;
        }
    }
}