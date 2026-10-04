using System;
using System.Linq;
using TM.Inventory;
using UnityEngine;
using UnityEngine.UIElements;
namespace TM.Inventory.UI
{
    public static class ItemSelector
    {
        public static InventoryItem selectedItem;
        public static Vector2Int selectedPosition => new Vector2Int(selectedItem.position.x, selectedItem.position.y);
        public static VisualElement itemVE;
        public static VisualElement itemWheelHolder;
        public static Color selectedBorderColor;
        public static int selectedBorderWidth;
        private static Color oldBorderColor;
        public static event Action<InventoryItem> OnSelectedChange;
        public static void Select(VisualElement itemVEIn, InventoryItem item, Inventory.InventoryManager inventoryManager, VisualElement itemwheelholder)
        {
            if (item == selectedItem) return;
            ClearSelected();
            if (inventoryManager.ItemWheelIds.Contains(item.id))
            {
                int index = -1;
                for (int i = 0; i < inventoryManager.ItemWheelIds.Length; i++)
                {
                    if (inventoryManager.ItemWheelIds[i] == item.id)
                    {
                        index = i;
                        break;
                    }
                }
                GameObject.FindAnyObjectByType<ItemWheelVectorImager>().SetHoveredIndex(index, itemwheelholder);
                itemwheelholder.MarkDirtyRepaint();
            }
            itemWheelHolder = itemwheelholder;
            selectedItem = item;
            itemVE = itemVEIn;
            oldBorderColor = itemVE.resolvedStyle.borderTopColor;
            setItemVE(selectedBorderWidth, selectedBorderColor);
            OnSelectedChange.Invoke(item);
        }
        private static void setItemVE(int borderWidth, Color color)
        {
            if (itemVE == null) return;
            itemVE.style.borderTopWidth = borderWidth;
            itemVE.style.borderBottomWidth = borderWidth;
            itemVE.style.borderRightWidth = borderWidth;
            itemVE.style.borderLeftWidth = borderWidth;
            itemVE.style.borderBottomColor = color;
            itemVE.style.borderTopColor = color;
            itemVE.style.borderRightColor = color;
            itemVE.style.borderLeftColor = color;
        }
        public static void ClearSelected()
        {
            selectedItem = null;

            setItemVE(1, oldBorderColor);
            GameObject.FindAnyObjectByType<ItemWheelVectorImager>().SetHoveredIndex(-1, null);
            if (itemWheelHolder != null) itemWheelHolder.MarkDirtyRepaint();
           
            itemVE = null;
        }
    }
}