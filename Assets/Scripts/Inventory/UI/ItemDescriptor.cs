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
        private Label itemName;
        private Label itemDescription;
        private Label itemTypeValue;
        [SerializeField] private VisualTreeAsset itemStatTemplate;
        private Label statisticsLabel;
        private VisualElement itemStatsHolder;
        private VisualElement root;
        public void Enable(VisualElement root)
        {
            this.root = root;
            this.itemName = root.Q<Label>("ItemName");
            this.itemDescription = root.Q<Label>("ItemDescriptionText");
            this.itemTypeValue = root.Q<Label>("ItemTypeValue");
            this.itemStatsHolder = root.Q<VisualElement>("ItemStatsHolder");
            this.statisticsLabel = root.Q<Label>("StatsLabel");
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
            this.itemName.text = selected.itemData.itemName;
            this.itemDescription.text = selected.itemData.description;
            this.itemTypeValue.text = selected.itemData.GetItemType().ToString();
            this.Show();
            this.CreateStats(selected);
        }
        private void CreateStats(InventoryItem selected)
        {
            List<ItemUIStat> itemUIStats = selected.itemData.GetItemUIStats();
            if (itemUIStats.Count == 0)
            {
                itemStatsHolder.style.display = DisplayStyle.None;
                statisticsLabel.style.display = DisplayStyle.None;
                return;
            }
            itemStatsHolder.style.display = DisplayStyle.Flex;
            statisticsLabel.style.display = DisplayStyle.Flex;
            itemStatsHolder.Clear();
            foreach (ItemUIStat itemUIStat in selected.itemData.GetItemUIStats())
            {
                VisualElement itemStat = itemStatTemplate.Instantiate();
                itemStat.Q<Label>("StatTitleTemplate").text = itemUIStat.title;
                itemStat.Q<Label>("StatValueTemplate").text = itemUIStat.value;
                itemStatsHolder.Add(itemStat);
            }
        }
        private void Hide()
        {
            foreach (VisualElement visualElement in this.root.Children())
            {
                visualElement.style.display = DisplayStyle.None;
            }
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