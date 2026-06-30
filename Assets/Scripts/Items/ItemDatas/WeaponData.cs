using System;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
    public class WeaponData : ItemData
    {
        public int damage;
        public float weight;
        public override List<ItemUIStat> GetItemUIStats()
        {
            return new List<ItemUIStat>()
            {
                new ItemUIStat("Weight", this.weight.ToString()),
                new ItemUIStat("Damage", this.damage.ToString()),
            };
        }
        public override ItemType GetItemType()
        {
            return ItemType.Weapon;
        }
    }
}