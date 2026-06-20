using System.Collections.Generic;
using TM.Inventory;
using Unity.Mathematics;
using UnityEngine;

public static class InventoryUIHelper
{
    public static (Vector2, Vector2Int, float) NearestIndexedPosition(Vector2 pos, Dictionary<Vector2, Vector2Int> allParentToIndexPosition)
    {
        (Vector2, Vector2Int, float) i = (new Vector2(), new Vector2Int(), math.INFINITY);
        foreach (Vector2 key in allParentToIndexPosition.Keys)
        {
            float dis = Vector2.Distance(key, pos);
            if (dis < i.Item3)
            {
                i.Item3 = dis;
                i.Item1 = key;
                i.Item2 = allParentToIndexPosition[key];
            }
        }
        return i;
    }
    public static Dictionary<Vector2, Vector2Int> AllParentToIndexPositions(InventoryManager inventoryManager)
    {
        Dictionary<Vector2, Vector2Int> dict = new();
        for (int x = 0; x < inventoryManager.gridSize.x; x++)
        {
            for (int y = 0; y < inventoryManager.gridSize.y; y++)
            {
                Vector2Int index = new Vector2Int(x, y);
                dict.Add(IndexToParentPos(index, inventoryManager), index);
            }
        }
        return dict;
    }
    public static Vector2 IndexToParentPos(Vector2Int vector2Int, InventoryManager inventoryManager)
    {
        return new Vector2(vector2Int.x * inventoryManager.cellSize + inventoryManager.cellPosMargin, vector2Int.y * inventoryManager.cellSize + inventoryManager.cellPosMargin);
    }
}