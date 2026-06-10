using System;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
    public class WeaponData : ItemData
    {
        public int damage;
        public int durability;

        public WeaponData(string name, string description, Sprite icon, Vector2Int size, UseBehavior useBehavior, int damage, int durability) : base(name, description, icon, size, useBehavior)
        {
            this.damage = damage;
            this.durability = durability;
        }
    }
}