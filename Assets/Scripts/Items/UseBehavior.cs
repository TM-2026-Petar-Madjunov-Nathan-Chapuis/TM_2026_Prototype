using TM.Inventory;
using UnityEngine;

namespace TM.Items
{
    public abstract class UseBehavior : ScriptableObject
    {
        public virtual bool usesRaycast => false;
        public virtual void Use(HeldItem heldItem, bool pressed)
        {
            Debug.Log($"{heldItem.itemData.name} was used by {heldItem.itemData.name}, but no effect is defined");
        }
        public virtual void Collision(Collider collision, HeldItem heldItem, GameObject player)
        {
            Debug.Log($"collided without effect");
        }
        public virtual void OnAnimationUse(HeldItem heldItem, GameObject player, InventoryItem inventoryItem)
        {
            Debug.Log($"Used during the animation with no");
        }
        public virtual void OnAnimationEnd(HeldItem heldItem) { }
    }
}
