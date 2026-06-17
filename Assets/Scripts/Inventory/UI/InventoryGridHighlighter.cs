using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TM.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryGridHighlighter : MonoBehaviour
{
    [SerializeField] private Color availableColor;
    [SerializeField] private Color occupiedColor;
    private Vector2Int currentIndex;
    private List<VisualElement> coloredOnes;
    private void Start()
    {
        coloredOnes = new();
    }
    public void OnDragMove(VisualElement[,] slots, VisualElement item, InventoryManager inventoryManager, Dictionary<Vector2, Vector2Int> allParentToIndexPositions)
    {
        (Vector2, Vector2Int, float) index = InventoryUIHelper.NearestIndexedPosition(new Vector2(item.style.left.value.value, item.style.top.value.value), allParentToIndexPositions);
        if (currentIndex != index.Item2)
        {
            this.currentIndex = index.Item2;
            this.ClearColors();
            (List<Vector2Int>,List<Vector2Int>) overlaping = inventoryManager.inventoryGrid.OverLapingItems((InventoryItem)item.userData, currentIndex);
            foreach(Vector2Int position in overlaping.Item1)
            {
                VisualElement visualElement = slots[position.x, position.y];
                visualElement.style.unityBackgroundImageTintColor = occupiedColor;
                coloredOnes.Add(visualElement);
            }
            foreach(Vector2Int position in overlaping.Item2)
            {
                VisualElement visualElement = slots[position.x, position.y];
                visualElement.style.unityBackgroundImageTintColor = availableColor;
                coloredOnes.Add(visualElement);
            }
        }
    }
    public void ClearColors()
    {
        foreach (VisualElement visualElement in coloredOnes)
        {
            visualElement.style.unityBackgroundImageTintColor = new Color(0,0,0,1);
        }
    }
}