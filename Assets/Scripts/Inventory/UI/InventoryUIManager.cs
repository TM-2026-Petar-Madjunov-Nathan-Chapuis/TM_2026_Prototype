using System;
using System.ComponentModel.Design.Serialization;
using NUnit.Framework.Constraints;
using TM.Items;
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
        private VisualElement itemLayer;
        private VisualElement itemGrid;
        private VisualElement itemDescription;
        private VisualElement equipment;
        private ProgressBar weightBar;

        public override void SetRoot(VisualElement root)
        {
            base.SetRoot(root);
            this.player = GameObject.FindGameObjectWithTag("Player");
            if (this.player == null){   throw new UnityException("no player found by tag : Player");    }
            this.inventoryManager = this.player.GetComponent<InventoryManager>();

            itemLayer = this.root.Q<VisualElement>("ItemsLayer");
            itemDescription = this.root.Q<VisualElement>("ItemDescription");
            itemGrid = this.root.Q<VisualElement>("GridBackground");
            equipment = this.root.Q<VisualElement>("Equipement");
            weightBar = this.root.Q<ProgressBar>("WeightBar");
        }
        public override void OnEnable()
        {
            WeightBarUpdate();
            Grid();
            FillItems();
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
            itemGrid.style.height = (this.inventoryManager.gridSize.y) * this.inventoryManager.cellSize;
            itemLayer.style.width = this.inventoryManager.gridSize.x * this.inventoryManager.cellSize;
            itemLayer.style.height = (this.inventoryManager.gridSize.y) * this.inventoryManager.cellSize;
        }
        private void FillItems()
        {
        }
    }
}