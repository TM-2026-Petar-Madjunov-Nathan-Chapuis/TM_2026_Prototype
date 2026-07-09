using System.Collections.Generic;
using System.Linq;
using TM.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemWheelManager : MonoBehaviour
{
    [field: SerializeField] public float itemWidth {get; private set;}
    [field: SerializeField] public float itemHeight {get; private set;}
    private ItemWheelVectorImager vectorImager;
    private InventoryManager inventoryManager;
    private Vector2[] itemPos;
    private VisualElement itemWheelHolder;
    private VisualElement[] items;
    private void Start()
    {
        vectorImager = GameObject.FindAnyObjectByType<ItemWheelVectorImager>();
    }
    public void Enable(VisualElement ItemWheelHolder, InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
        if (this.itemWheelHolder != null && this.itemWheelHolder != ItemWheelHolder)
        {
            itemWheelHolder.UnregisterCallback<GeometryChangedEvent>(OnUILoaded);
        }
        this.itemWheelHolder = ItemWheelHolder;
        itemWheelHolder.generateVisualContent += vectorImager.Draw;
        itemWheelHolder.RegisterCallback<GeometryChangedEvent>(OnUILoaded);
        items = new VisualElement[this.inventoryManager.itemWheelItems.Count()];
        vectorImager.hoveredIndex = 0;
    }
    public void Disable()
    {
        itemWheelHolder.UnregisterCallback<GeometryChangedEvent>(OnUILoaded);
    }
    public void OnUILoaded(GeometryChangedEvent evt)
    {
        itemPos = vectorImager.getAllSlotCenters(itemWheelHolder.resolvedStyle.width);
        itemWheelHolder.Clear();
        items = new VisualElement[this.inventoryManager.itemWheelItems.Count()];
        LoadItems();
    }
    public void AddItem(InventoryItem inventoryItem, int index)
    {
        if (index < 0 || index > this.inventoryManager.itemWheelItems.Count()) return;
        this.inventoryManager.itemWheelItems[index] = inventoryItem;
        this.UpdateItem(index);
        this.ClearHighlight();
    }
    private void LoadItems()
    {
        for (int i = 0; i < this.inventoryManager.itemWheelItems.Count(); i++)
        {
            UpdateItem(i);
        }
    }
    private void UpdateItem(int index)
    {
        if (items[index] != null) {
            itemWheelHolder.Remove(items[index]);
        }

        if (this.inventoryManager.itemWheelItems[index] == null) return;
        VisualElement item = new VisualElement();
        items[index] = item;
        item.style.position = Position.Absolute;
        Vector2Int size = this.inventoryManager.itemWheelItems[index].itemData.size;
        float x = itemWidth / size.x * size.y;
        float y = itemHeight / size.y * size.x;
        if (x < itemHeight)
        {
            item.style.width = itemWidth;
            item.style.height = x;
        }
        else
        {
            item.style.width = y;
            item.style.height = itemHeight;
        }
        item.style.left = itemPos[index].x - (item.style.width.value.value / 2);
        item.style.top = itemPos[index].y - (item.style.height.value.value / 2);
        item.style.backgroundImage = Background.FromSprite(this.inventoryManager.itemWheelItems[index].itemData.icon);
        item.AddToClassList("inventory__item-wheel-item");
        itemWheelHolder.Add(item);
    }
    public void Highlight(int index)
    {
        vectorImager.hoveredIndex = index;
        itemWheelHolder.MarkDirtyRepaint();
    }
    public void ClearHighlight()
    {
        vectorImager.hoveredIndex = -1;
        itemWheelHolder.MarkDirtyRepaint();
    }
}