using Newtonsoft.Json;
using UnityEngine;
using TM.Saving;

namespace TM.Items
{
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject, IUUID
{
    public string itemName { get; private set; } //unique
    public string description { get; private set; }

    [JsonProperty] private UUID uuid;
    public UUID UUID => uuid;

    private void OnValidate() // runs when asset is created/modified in editor
    {
        if (uuid.Equals(default)) // Checks if uuid is at default value (0 in case of int structs i think)
        {
            uuid = UUID.NewUUID(); // if so we must change it
        }
    }

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
}