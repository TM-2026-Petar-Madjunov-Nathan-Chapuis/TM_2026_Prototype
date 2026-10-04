using Newtonsoft.Json;
using UnityEngine;
using TM.Saving;
using System;
using System.Collections.Generic;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class ItemData : ScriptableObject, IUUID
    {

        [SerializeField, TextArea(3, 10)]
        public string description;
        public Vector2Int size;
        [SerializeField, JsonProperty] private UUID uuid;
        public UUID UUID => uuid;
        public Sprite icon;
        public Sprite iconRotated;
        public UseBehavior useBehavior;
        public GameObject heldPrefab;
        public GameObject worldPrefab;

        private void OnValidate() // runs when asset is created/modified in editor
        {
            if (uuid.Equals(default)) // Checks if uuid is at default value (0 in case of int structs i think)
            {
                uuid = UUID.NewUUID(); // if so we must change it
            }
        }
        public virtual ItemType GetItemType()
        {
            return ItemType.None;
        }
        public virtual List<ItemUIStat> GetItemUIStats() //implement in childrens, used in ui item description.
        {
            return new List<ItemUIStat>();
        }

        public virtual void Use(HeldItem heldItem, bool pressed)
        {
            useBehavior.Use(heldItem, pressed);
        }
    }
}