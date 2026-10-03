using System.Collections.Generic;
using System.Linq;
using TM.Inventory;
using TM.Inventory.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemWheelManager : MonoBehaviour
{
    [field: SerializeField] public float maxItemWidth {get; private set;}
    [field: SerializeField] public float maxItemHeight {get; private set;}
    private ItemWheelVectorImager vectorImager;
    private TM.Inventory.InventoryManager inventoryManager;
    private Vector2[] itemPos;
    private VisualElement itemWheelHolder;
    private VisualElement[] items;
    private void Start()
    {
        vectorImager = GameObject.FindAnyObjectByType<ItemWheelVectorImager>();
    }
    public void Enable(VisualElement ItemWheelHolder, TM.Inventory.InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
        if (this.itemWheelHolder != null)
        {
            itemWheelHolder.generateVisualContent -= vectorImager.Draw;
            itemWheelHolder.UnregisterCallback<GeometryChangedEvent>(OnUILoaded);
        }
        this.itemWheelHolder = ItemWheelHolder;
        itemWheelHolder.generateVisualContent += vectorImager.Draw;
        itemWheelHolder.RegisterCallback<GeometryChangedEvent>(OnUILoaded);
        items = new VisualElement[this.inventoryManager.ItemWheelIds.Count()];
        vectorImager.SetHoveredIndex(-1, null);
    }
    public void Disable()
    {
        itemWheelHolder.generateVisualContent -= vectorImager.Draw;
        itemWheelHolder.UnregisterCallback<GeometryChangedEvent>(OnUILoaded);
    }
    public void OnUILoaded(GeometryChangedEvent evt)
    {
        itemPos = vectorImager.getAllSlotCenters(itemWheelHolder.resolvedStyle.width);
        itemWheelHolder.Clear();
        items = new VisualElement[this.inventoryManager.ItemWheelIds.Count()];
        LoadItems();
    }
    public void AddItem(InventoryItem inventoryItem, int index)
    {
        if (index < 0 || index > this.inventoryManager.ItemWheelIds.Count()) return;
        if (this.inventoryManager.ItemWheelIds.Contains(inventoryItem.id))
        {
            int idx = -1;
            for (int i = 0; i < this.inventoryManager.ItemWheelIds.Length; i++)
            {
                if (this.inventoryManager.ItemWheelIds[i] == inventoryItem.id)
                {
                    idx = i;
                    break;
                }
            }
            itemWheelHolder.Remove(this.items[idx]);
            this.items[idx] = null;
            this.inventoryManager.RemoveFromWheel(inventoryItem.id);
            this.UpdateItem(idx);    
            itemWheelHolder.MarkDirtyRepaint();
            
        }
        this.inventoryManager.ItemWheelIds[index] = inventoryItem.id;
        this.UpdateItem(index);
        this.ClearHighlight();
    }
    private void LoadItems()
    {
        for (int i = 0; i < this.inventoryManager.ItemWheelIds.Count(); i++)
        {
            UpdateItem(i);
        }
    }
    private void UpdateItem(int index)
    {
        if (items[index] != null) {
            itemWheelHolder.Remove(items[index]);
        }

        if (this.inventoryManager.ItemWheelIds[index] == null || this.inventoryManager.GetItemFromGuid(this.inventoryManager.ItemWheelIds[index]) == null) return;
        VisualElement item = new VisualElement();
        items[index] = item;
        item.style.position = Position.Absolute;
        Vector2Int size = this.inventoryManager.GetItemFromGuid(this.inventoryManager.ItemWheelIds[index]).itemData.size;
        float scale = Mathf.Min(maxItemWidth / size.x, maxItemHeight / size.y);
        item.style.width = size.x * scale;
        item.style.height = size.y * scale;
        item.style.left = itemPos[index].x - (item.style.width.value.value / 2);
        item.style.top = itemPos[index].y - (item.style.height.value.value / 2);
        item.style.backgroundImage = Background.FromSprite(this.inventoryManager.GetItemFromGuid(this.inventoryManager.ItemWheelIds[index]).itemData.icon);
        item.AddToClassList("inventory__item-wheel-item");
        itemWheelHolder.Add(item);
    }
    public void Highlight(int index)
    {
        vectorImager.SetHoveredIndex(index, null);
        itemWheelHolder.MarkDirtyRepaint();
    }
    public void ClearHighlight()
    {
        if (this.inventoryManager.ItemWheelIds.Contains(ItemSelector.selectedItem.id)) {
                int index = -1;
                for (int i = 0; i < inventoryManager.ItemWheelIds.Length; i++)
                {
                    if (inventoryManager.ItemWheelIds[i] == ItemSelector.selectedItem.id)
                    {
                        index = i;
                        break;
                    }
                }
                vectorImager.SetHoveredIndex(index, itemWheelHolder);
        } else vectorImager.SetHoveredIndex(-1, null);
        
        itemWheelHolder.MarkDirtyRepaint();
    }
}