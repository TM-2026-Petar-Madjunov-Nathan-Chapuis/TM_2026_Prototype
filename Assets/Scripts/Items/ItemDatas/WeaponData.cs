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

        public WeaponData(string name, string description, Sprite icon, Vector2Int size, UseBehavior useBehavior, int damage, float weight) : base(name, description, icon, size, useBehavior)
        {
            this.damage = damage;
            this.weight = weight;
        }
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