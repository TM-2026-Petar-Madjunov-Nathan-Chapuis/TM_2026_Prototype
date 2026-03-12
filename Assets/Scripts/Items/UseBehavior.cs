using UnityEngine;

namespace TM.Items
{
    public abstract class UseBehavior : ScriptableObject
    {
        public virtual void Use(GameObject user, ItemData item)
        {
            Debug.Log($"{item.itemName} was used by {user.name}, but no effect defined");
        }
    }
}
