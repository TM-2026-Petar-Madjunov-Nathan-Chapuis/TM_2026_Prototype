using System;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Food", menuName = "Inventory/Food")]
    public class FoodData : ItemData
    {
        public int satietyAmmount;
        public float weight;
        public override List<ItemUIStat> GetItemUIStats()
        {
            return new List<ItemUIStat>
            {
                new ItemUIStat("Weight", weight.ToString()),
                new ItemUIStat("Satiety Ammout", satietyAmmount.ToString())
            };
        }
        public override ItemType GetItemType()
        {
            return ItemType.Food;
        }
    }
}