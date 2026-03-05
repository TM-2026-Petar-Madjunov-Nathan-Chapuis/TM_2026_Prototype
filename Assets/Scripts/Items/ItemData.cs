
using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    
    public string itemName { get; private set; }
    public int itemId;
    public string description { get; private set; }
    public Sprite icon;
    public UseBehavior useBehavior;

    public virtual void Use()
    {
        Debug.Log("Using item");
    }
}