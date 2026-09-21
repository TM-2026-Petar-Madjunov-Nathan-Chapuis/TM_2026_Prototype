using System;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Dead Fox", menuName = "Inventory/Dead")]
    public class DeadFoxData : ItemData
    {
        public override List<ItemUIStat> GetItemUIStats()
        {
            return new List<ItemUIStat>();
        }
        public override ItemType GetItemType()
        {
            return ItemType.RawMeat;
        }
    }
}