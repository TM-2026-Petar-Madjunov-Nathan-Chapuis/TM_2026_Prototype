using System.ComponentModel.Design.Serialization;
using NUnit.Framework.Constraints;
using TM.Items;
using TM.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.Inventory.UI
{
    public class InventoryUIManager : GenericUITemplateManager
    {
        private GameObject player;
        private InventoryManager inventoryManager;
        private VisualElement itemsContainers;
        private VisualElement itemDescription;
        private VisualElement equipment;
        private ProgressBar weightBar;

        public override void SetRoot(VisualElement root)
        {
            base.SetRoot(root);
            this.player = GameObject.FindGameObjectWithTag("Player");
            if (this.player == null){   throw new UnityException("no player found by tag : Player");    }
            this.inventoryManager = this.player.GetComponent<InventoryManager>();

            itemsContainers = this.root.Q<VisualElement>("ItemsList");
            itemDescription = this.root.Q<VisualElement>("ItemDescription");
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
        }
        private void FillItems()
        {
        }
    }
}