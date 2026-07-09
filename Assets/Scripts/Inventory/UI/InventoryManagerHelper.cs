using System;
using System.Collections.Generic;
using System.Linq;
using TM.Inventory;
using Unity.Mathematics;
using UnityEngine;
namespace TM.Inventory.UI
{
    public static class InventoryManagerHelper
    {
        public static float MaxDistanceTreshold = 100;
        public static int NearestItemWheelPosition(Vector2 pos, Vector2[] positions)
        {
            (float, int) dis = (9999999, -99);
            for (int i = 0; i < positions.Count(); i++)
            {
                float distance = Vector2.Distance(pos, positions[i]);
                if (distance < dis.Item1) dis = (distance, i);
            }
            return dis.Item2;
        }
        
        public static (Vector2, Vector2Int, float) NearestGridIndexedPosition(Vector2 pos, Dictionary<Vector2, Vector2Int> allParentToIndexPosition)
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
            if(i.Item3 > MaxDistanceTreshold)
            {
                i.Item3 = MaxDistanceTreshold;
                i.Item1 = new Vector2(-1, -1);
                i.Item2 = new Vector2Int(-1, -1); //going to get discard in the grid hilighter checks.
            }
            return i;
        }
        public static Dictionary<Vector2, Vector2Int> AllParentToIndexPositions(Inventory.InventoryManager inventoryManager)
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
        public static Vector2 IndexToParentPos(Vector2Int vector2Int, Inventory.InventoryManager inventoryManager)
        {
            return new Vector2(vector2Int.x * inventoryManager.cellSize + inventoryManager.cellPosMargin, vector2Int.y * inventoryManager.cellSize + inventoryManager.cellPosMargin);
        }
    }
}