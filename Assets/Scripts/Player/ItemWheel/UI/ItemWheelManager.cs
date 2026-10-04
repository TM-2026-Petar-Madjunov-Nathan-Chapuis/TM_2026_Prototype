
using System;
using TM.Input;
using TM.Inventory;
using TM.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace TM.Player.ItemWheel.UI
{
    public class ItemWheelManager : MonoBehaviour
    {
        [SerializeField] private UIDocument itemWheelUIDoc;
        [Range(0, 1)]
        [SerializeField] private float maxWidthPercentage;
        [Range(0, 1)]
        [SerializeField] private float maxHeightPercentage;
        [Range(0, 1)]
        [SerializeField] private float sensitivity = 0.5f;
        [SerializeField] private float deadZoneRadius = 20;
        [SerializeField] private float clampRadius = 30;
        [SerializeField] private float maxItemWidth;
        [SerializeField] private float maxItemHeight;
        [SerializeField] private float itemSizeWidthRatio;
        [SerializeField] private float itemSizeHeightRatio;
        [SerializeField] private ItemWheelVectorImager wheelVectorImager;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private playerItemController playerItemController;
        private bool MenuOpened = false;
        private VisualElement root;
        private VisualElement itemWheelHolder;
        private Vector2 mouseVector;

        void Awake()
        {
            root = itemWheelUIDoc.rootVisualElement;
            root.style.display = DisplayStyle.None;
            itemWheelHolder = root.Q<VisualElement>("ItemWheelHolder");
            Resize(null);
            itemWheelHolder.generateVisualContent += wheelVectorImager.Draw;
        }
        void OnEnable()
        {
            itemWheelHolder.RegisterCallback<GeometryChangedEvent>(Resize);
            InputManager.Instance.RegisterListener("ItemWheel", ToggleMenu, InputValueType.UpAndDownButton, true);
        }

        void OnDisable()
        {
            itemWheelHolder.UnregisterCallback<GeometryChangedEvent>(Resize);
            InputManager.Instance.UnRegisterListener("ItemWheel", ToggleMenu);
        }
        void Update()
        {
            if (MenuOpened)
            {
                this.HandleMouseMovement();
            }
        }
        public void Resize(GeometryChangedEvent changedEvent)
        {
            float width = itemWheelHolder.parent.resolvedStyle.width;
            float height = itemWheelHolder.parent.resolvedStyle.height;
            if (width <= 0 || height <= 0) return;

            float x = width * maxWidthPercentage;
            float y = height * maxHeightPercentage;
            if (x < y)
            {
                itemWheelHolder.style.width = x;
                itemWheelHolder.style.height = x;
            }
            else
            {
                itemWheelHolder.style.width = y;
                itemWheelHolder.style.height = y;
            }
            if (this.inventoryManager == null) return;
            if (this.itemWheelHolder == null) return;
            if (this.inventoryManager.ItemWheelIds == null) return;
            this.LoadItems();
        }
        public void ToggleMenu(InputValues inputValues)
        {
            if (inputValues.pressed) MenuOpened = true;
            else MenuOpened = false;

            if (MenuOpened)
            {
                GameStateManager.Instance.SlowTime();
                GameStateManager.Instance.FreezeLooking();
                playerCamera.lookingAllowed = false;
                mouseVector = Vector2.zero;
                wheelVectorImager.SetHoveredIndex(-1, null);
                root.style.display = DisplayStyle.Flex;
                itemWheelHolder.MarkDirtyRepaint();
            }
            else
            {
                //the item last select will be the new held item.
                playerItemController.SetHeldItem(this.wheelVectorImager.hoveredIndex == -1 ? null : this.inventoryManager.GetItemFromGuid(inventoryManager.ItemWheelIds[this.wheelVectorImager.hoveredIndex]));
                this.wheelVectorImager.SetHoveredIndex(-1, null);
                root.style.display = DisplayStyle.None;
                playerCamera.lookingAllowed = true;
                GameStateManager.Instance.UnFreezeTime();
            }
        }
        private void HandleMouseMovement()
        {
            Vector2 delta = Mouse.current.delta.value * sensitivity;
            mouseVector += delta;
            mouseVector = Vector2.ClampMagnitude(mouseVector, clampRadius);
            float magnitude = mouseVector.magnitude;
            if (magnitude < deadZoneRadius)
            {
                this.wheelVectorImager.SetHoveredIndex(-1, null);
            }
            else
            {
                Vector2 pos = new Vector2(mouseVector.x, -mouseVector.y) + new Vector2(this.itemWheelHolder.resolvedStyle.width / 2, this.itemWheelHolder.resolvedStyle.width / 2); //convert to correct space. (0,0 on top left)
                this.wheelVectorImager.SetHoveredIndex(this.wheelVectorImager.NearestItemWheelPosition(pos, this.itemWheelHolder.resolvedStyle.width), null);
            }
            itemWheelHolder.MarkDirtyRepaint();
        }
        private void LoadItems()
        {
            itemWheelHolder.Clear();
            Vector2[] positions = this.wheelVectorImager.getAllSlotCenters(this.itemWheelHolder.resolvedStyle.width);
            for (int i = 0; i < inventoryManager.ItemWheelIds.Length; i++)
            {
                if (inventoryManager.ItemWheelIds[i] == null) continue;
                AddItem(positions[i], inventoryManager.GetItemFromGuid(inventoryManager.ItemWheelIds[i]).itemData);
            }
        }
        private void AddItem(Vector2 pos, ItemData itemData)
        {
            VisualElement item = new();
            Vector2Int size = itemData.size;
            float itemWidth = size.x * itemSizeWidthRatio;
            float itemHeight = size.y * itemSizeHeightRatio;

            //if the item is wider than tall, then multiply so that it fits in the max width if taller then multiply for max height
            float scale = Mathf.Min(maxItemWidth / itemWidth, maxItemHeight / itemHeight);
            item.style.width = itemWidth * scale;
            item.style.height = itemHeight * scale;

            item.style.position = Position.Absolute;
            item.style.top = pos.y - item.style.height.value.value/2;
            item.style.left = pos.x - item.style.width.value.value/2;
            item.style.backgroundImage = Background.FromSprite(itemData.icon);
            item.AddToClassList("item-wheel-item");
            this.itemWheelHolder.Add(item);
        }
    }
}
