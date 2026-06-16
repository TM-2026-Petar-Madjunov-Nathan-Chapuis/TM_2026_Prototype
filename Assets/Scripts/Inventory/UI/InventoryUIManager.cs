using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TM.UI;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.Inventory.UI
{
    public class InventoryUIManager : GenericUITemplateManager
    {
        private GameObject player;
        private InventoryManager inventoryManager;
        private VisualElement[,] gridElements;
        private VisualElement[,] itemsElements;
        private VisualElement itemLayer;
        private VisualElement itemGrid;
        private VisualElement itemDescription;
        private VisualElement equipment;
        private ProgressBar weightBar;
        private InventoryGridHighlighter gridHighlighter;
        private Dictionary<Vector2, Vector2Int> allParentToIndexPositions;

        public override void SetRoot(VisualElement root)
        {
            this.gridHighlighter = GameObject.FindAnyObjectByType<InventoryGridHighlighter>();
            base.SetRoot(root);
            this.player = GameObject.FindGameObjectWithTag("Player");
            if (this.player == null){   throw new UnityException("no player found by tag : Player");    }
            this.inventoryManager = this.player.GetComponent<InventoryManager>();

            itemLayer = this.root.Q<VisualElement>("ItemsLayer");
            itemDescription = this.root.Q<VisualElement>("ItemDescription");
            itemGrid = this.root.Q<VisualElement>("GridBackground");
            equipment = this.root.Q<VisualElement>("Equipement");
            weightBar = this.root.Q<ProgressBar>("WeightBar");
            allParentToIndexPositions = InventoryUIHelper.AllParentToIndexPositions(this.inventoryManager);
        }
        public override void OnEnable()
        {
            WeightBarUpdate();
            Grid();
            FillItems();
        }
        public override void OnDisable()
        {
            itemGrid.Clear();
            itemLayer.Clear();
        }
        private void WeightBarUpdate()
        {
            
        }
        private void Grid()
        {
            gridElements = new VisualElement[
                inventoryManager.gridSize.x,
                inventoryManager.gridSize.y
            ];
            itemsElements = new VisualElement[
                inventoryManager.gridSize.x,
                inventoryManager.gridSize.y
            ];
            for (int y = 0; y < this.inventoryManager.gridSize.y; y++)
            {
                for (int x = 0; x < this.inventoryManager.gridSize.x; x++)
                {
                    VisualElement slot = new VisualElement();

                    slot.AddToClassList("inventory__slot");
                    slot.style.position = Position.Absolute;
                
                    slot.style.left = x * this.inventoryManager.cellSize;
                    slot.style.top = y * this.inventoryManager.cellSize;

                    slot.style.width = this.inventoryManager.cellSize;
                    slot.style.height = this.inventoryManager.cellSize;


                    itemGrid.Add(slot);
                    gridElements[x,y] = slot;
                }
            }
            itemGrid.style.width = this.inventoryManager.gridSize.x * this.inventoryManager.cellSize;
            itemGrid.style.height = this.inventoryManager.gridSize.y * this.inventoryManager.cellSize;
            itemLayer.style.width = this.inventoryManager.gridSize.x * this.inventoryManager.cellSize;
            itemLayer.style.height = this.inventoryManager.gridSize.y * this.inventoryManager.cellSize;
        }
        private void FillItems()
        {
            Debug.Log("items filled");
            foreach (InventoryItem invItem in this.inventoryManager.inventoryGrid.itemsList)
            {
                CreateItem(invItem);
            }
            foreach (VisualElement item in this.itemsElements) { //add the drag and drop behaviour/manipulator
                ItemDragManipulator itemDragManipulator = new ItemDragManipulator(item);
                itemDragManipulator.OnDragEndEvent += HandleDragEnd;
                itemDragManipulator.OnDragMoveEvent += HandleDragging;
                item.AddManipulator(itemDragManipulator);
            }
        }
        private void CreateItem(InventoryItem inventoryItem)
        {
            VisualElement item = new VisualElement();

            item.AddToClassList("inventory__item");
            item.name = inventoryItem.itemData.name;

            item.userData = inventoryItem; //this stores the inventoryitem reference to the visualelement itself.

            item.style.position = Position.Absolute;
            Vector2 pos = InventoryUIHelper.IndexToParentPos(inventoryItem.position, this.inventoryManager);
            item.style.left = pos.x;
            item.style.top = pos.y;
            item.style.width = inventoryItem.itemData.size.x * this.inventoryManager.cellSize;
            item.style.height = inventoryItem.itemData.size.y * this.inventoryManager.cellSize;
            item.style.backgroundImage = Background.FromSprite(inventoryItem.itemData.icon);

            this.itemLayer.Add(item);
            this.itemsElements[inventoryItem.position.x, inventoryItem.position.y] = item;
        }
        private void HandleDragEnd(VisualElement item)
        {
            VisualElement parent = item.parent;
            if (parent == null) return;

            InventoryItem inventoryItem = item.userData as InventoryItem;

            Vector2 pos = new Vector2(item.style.left.value.value, item.style.top.value.value);
            (Vector2, Vector2Int, float) i = InventoryUIHelper.NearestIndexedPosition(pos, allParentToIndexPositions);
            Vector2Int oldPos = inventoryItem.position;
            if (this.inventoryManager.inventoryGrid.TryMoveItem(inventoryItem, i.Item2))
            {
                Vector2 newPos = InventoryUIHelper.IndexToParentPos(i.Item2, this.inventoryManager);
                item.style.left = newPos.x;
                item.style.top = newPos.y; //move the item in UI, providing the snapping.

                itemsElements[oldPos.x, oldPos.y] = null;
                itemsElements[i.Item2.x, i.Item2.y] = item; // move the item on the 2d map of items.
            }
            else
            {
                Vector2 newPos = InventoryUIHelper.IndexToParentPos(inventoryItem.position, this.inventoryManager);
                item.style.left = newPos.x;
                item.style.top = newPos.y; //move the item back at the start position, providing the snapping.
            }
        }
        private void HandleDragging(VisualElement item, Vector2 pointerPos)
        {
            gridHighlighter.OnDragMove(this.gridElements, item, this.inventoryManager, this.allParentToIndexPositions);
            Debug.Log("moved");
        }
    }
}