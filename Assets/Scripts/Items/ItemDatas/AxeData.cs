using System;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Axe", menuName = "Inventory/Axe")]
    public class AxeData : ItemData
    {
        public int damage;
        public float weight;
        public float knockbackForce;
        public override List<ItemUIStat> GetItemUIStats()
        {
            return new List<ItemUIStat>()
            {
                new ItemUIStat("Weight", this.weight.ToString()),
                new ItemUIStat("Damage", this.damage.ToString()),
                new ItemUIStat("KnockbackForce", this.knockbackForce.ToString())
            };
        }
        public override ItemType GetItemType()
        {
            return ItemType.Weapon;
        }
    }
}