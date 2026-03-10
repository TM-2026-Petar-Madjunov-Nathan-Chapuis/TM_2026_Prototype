using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
    public class WeaponData : ItemData
    {
        public int damage;
        public int durability;

        public WeaponData(string name, string description, Sprite icon, UseBehavior useBehavior, int damage, int durability) : base(name, description, icon, useBehavior)
        {
            this.damage = damage;
            this.durability = durability;
        }
    }
}