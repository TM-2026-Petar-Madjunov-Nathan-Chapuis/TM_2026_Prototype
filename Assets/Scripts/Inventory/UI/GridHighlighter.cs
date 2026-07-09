using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TM.Inventory;
using UnityEngine;
using UnityEngine.UIElements;
namespace TM.Inventory.UI
{
    public class GridHighlighter : MonoBehaviour
    {
        [SerializeField] private Color availableColor;
        [SerializeField] private Color occupiedColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private int selectedBorderWidth;
        private Vector2Int currentIndex;
        private List<VisualElement> coloredOnes;
        public bool isGridHighlighted;
        private void Start()
        {
            ItemSelector.selectedBorderColor = this.selectedColor;
            ItemSelector.selectedBorderWidth = this.selectedBorderWidth;
            coloredOnes = new();
            isGridHighlighted = false;
        }
        public void OnDragMove(VisualElement[,] slots, VisualElement item, Inventory.InventoryManager inventoryManager, Dictionary<Vector2, Vector2Int> allParentToIndexPositions)
        {
            (Vector2, Vector2Int, float) index = InventoryManagerHelper.NearestGridIndexedPosition(new Vector2(item.style.left.value.value, item.style.top.value.value), allParentToIndexPositions);
            if (currentIndex != index.Item2)
            {
                this.currentIndex = index.Item2;
                this.ClearColors();
                (List<Vector2Int>, List<Vector2Int>) overlaping = inventoryManager.inventoryGrid.OverLapingItems((InventoryItem)item.userData, currentIndex);
                foreach (Vector2Int position in overlaping.Item1)
                {
                    VisualElement visualElement = slots[position.x, position.y];
                    visualElement.style.unityBackgroundImageTintColor = occupiedColor;
                    coloredOnes.Add(visualElement);
                }
                foreach (Vector2Int position in overlaping.Item2)
                {
                    VisualElement visualElement = slots[position.x, position.y];
                    visualElement.style.unityBackgroundImageTintColor = availableColor;
                    coloredOnes.Add(visualElement);
                }
                isGridHighlighted = true;
            }
        }
        public void ClearColors()
        {
            foreach (VisualElement visualElement in coloredOnes)
            {
                visualElement.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 1);
            }
            isGridHighlighted = false;
        }
        public void ResetIndex()
        {
            this.currentIndex = new Vector2Int(-9999, -9999); //will never fire in the check
        }
        public void Refresh(VisualElement[,] slots, VisualElement item, Inventory.InventoryManager inventoryManager, Dictionary<Vector2, Vector2Int> allParentToIndexPositions)
        {
            this.ClearColors();
            this.ResetIndex();
            this.OnDragMove(slots, item, inventoryManager, allParentToIndexPositions);
        }
    }
}