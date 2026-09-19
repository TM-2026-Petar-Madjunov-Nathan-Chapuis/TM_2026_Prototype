using UnityEngine;

namespace TM.Items
{
    public abstract class UseBehavior : ScriptableObject
    {
        public virtual void Use(HeldItem heldItem, bool pressed)
        {
            Debug.Log($"{heldItem.itemData.name} was used by {heldItem.itemData.name}, but no effect is defined");
        }
        public virtual void OnTriggerEnter(Collider collision, HeldItem heldItem, GameObject player)
        {
            Debug.Log($"collided without effect");
        }
        public virtual void OnAnimationEnd(HeldItem heldItem) { }
    }
}
