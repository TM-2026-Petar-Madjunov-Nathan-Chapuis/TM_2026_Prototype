using System;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Items
{
    [CreateAssetMenu(fileName = "New Combustible", menuName = "Inventory/Combustible")]
    public class CombustibleData : ItemData
    {
        public override List<ItemUIStat> GetItemUIStats()
        {
            return new List<ItemUIStat>();
        }
        public override ItemType GetItemType()
        {
            return ItemType.Combustible;
        }
    }
}