using System.Collections.Generic;
using TM.Inventory;
using TM.Inventory.UI;
using TM.UI;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.Inventory.UI
{
    public class ItemDescriptor : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset itemStatTemplate;
        [SerializeField] private float ItemPreviewHeight = 180;
        private Label itemName;
        private Label itemDescription;
        private Label statisticsLabel;
        private VisualElement itemStatsHolder;
        private VisualElement root;
        private VisualElement itemPreviewHolder;
        private InventoryManager inventoryManager;

        public void Enable(VisualElement root)
        {
            this.root = root;
            this.itemName = root.Q<Label>("ItemName");
            this.itemDescription = root.Q<Label>("ItemDescriptionText");
            this.itemStatsHolder = root.Q<VisualElement>("ItemStatsHolder");
            this.statisticsLabel = root.Q<Label>("StatsLabel");
            this.itemPreviewHolder = root.Q<VisualElement>("ItemPreviewHolder");
            ItemSelector.OnSelectedChange += UpdateSelected;
            this.Hide();
        }
        public void Disable()
        {
            ItemSelector.OnSelectedChange -= UpdateSelected;
        }
        public void UpdateSelected(InventoryItem selected)
        {
            if (selected == null) { this.Hide(); return; }
            this.itemName.text = selected.itemData.name;
            this.itemDescription.text = selected.itemData.description;
            this.Show();
            this.CreateStats(selected);
            this.CreateItemView(selected);
        }
        private void CreateStats(InventoryItem selected)
        {
            List<ItemUIStat> itemUIStats = selected.itemData.GetItemUIStats();
            itemUIStats.Add(new ItemUIStat("Type", selected.itemData.GetItemType().ToString()));
            if (itemUIStats.Count == 0)
            {
                itemStatsHolder.style.display = DisplayStyle.None;
                statisticsLabel.style.display = DisplayStyle.None;
                return;
            }
            itemStatsHolder.style.display = DisplayStyle.Flex;
            statisticsLabel.style.display = DisplayStyle.Flex;
            itemStatsHolder.Clear();
            foreach (ItemUIStat itemUIStat in itemUIStats)
            {
                VisualElement itemStat = itemStatTemplate.Instantiate();
                itemStat.Q<Label>("StatTitleTemplate").text = itemUIStat.title;
                itemStat.Q<Label>("StatValueTemplate").text = itemUIStat.value;
                itemStatsHolder.Add(itemStat);
            }
            foreach (TemplateContainer templateContainer in this.itemStatsHolder.Children())
            {
                templateContainer.style.width = new StyleLength(Length.Percent(100));
            }
        }
        private void CreateItemView(InventoryItem selected)
        {
            this.itemPreviewHolder.Clear();
            VisualElement item = new VisualElement();

            Vector2Int size = selected.itemData.size;
            int cellSize = (int)this.ItemPreviewHeight / size.y;
            item.style.width = size.x * cellSize;
            item.style.height = size.y * cellSize;

            item.AddToClassList("");
            item.style.backgroundImage = Background.FromSprite(selected.itemData.icon);

            this.itemPreviewHolder.Add(item);
        }
        private void Hide()
        {
            foreach (VisualElement visualElement in this.root.Children())
            {
                visualElement.style.display = DisplayStyle.None;
            }
            ItemSelector.ClearSelected();
        }
        private void Show()
        {
            foreach (VisualElement visualElement in this.root.Children())
            {
                visualElement.style.display = DisplayStyle.Flex;
            }
        }
    }
}