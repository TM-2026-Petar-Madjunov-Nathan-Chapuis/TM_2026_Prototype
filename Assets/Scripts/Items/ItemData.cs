using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName { get; private set; } //unique
    public string description { get; private set; }
    public Sprite icon;
    public UseBehavior useBehavior;

    public ItemData(string name, string description, Sprite icon, UseBehavior useBehavior)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.useBehavior = useBehavior;
    }

    public virtual void Use(GameObject user)
    {
        UnityEngine.Debug.Log("Using item");
        useBehavior.Use(user, this);
    }
}