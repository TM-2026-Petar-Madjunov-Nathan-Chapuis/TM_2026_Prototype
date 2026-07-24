
using TM.Input;
using TM.Inventory;
using TM.Items;
using Unity.Collections;
using Unity.VisualScripting;
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
        [SerializeField] private float MaxItemWidth;
        [SerializeField] private float MaxItemHeight;
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
            if (this.inventoryManager.itemWheelItems == null) return;
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
                wheelVectorImager.hoveredIndex = -1;
                root.style.display = DisplayStyle.Flex;
                itemWheelHolder.MarkDirtyRepaint();
            }
            else
            {
                //the item last select will be the new held item.
                playerItemController.SetHeldItem(this.wheelVectorImager.hoveredIndex == -1 ? null : this.inventoryManager.itemWheelItems[this.wheelVectorImager.hoveredIndex]);
                this.wheelVectorImager.hoveredIndex = -1;
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
                this.wheelVectorImager.hoveredIndex = -1;
            }
            else
            {
                Vector2 pos = new Vector2(mouseVector.x, -mouseVector.y) + new Vector2(this.itemWheelHolder.resolvedStyle.width / 2, this.itemWheelHolder.resolvedStyle.width / 2); //convert to correct space. (0,0 on top left)
                this.wheelVectorImager.hoveredIndex = this.wheelVectorImager.NearestItemWheelPosition(pos, this.itemWheelHolder.resolvedStyle.width);
            }
            itemWheelHolder.MarkDirtyRepaint();
        }
        private void LoadItems()
        {
            itemWheelHolder.Clear();
            Vector2[] positions = this.wheelVectorImager.getAllSlotCenters(this.itemWheelHolder.resolvedStyle.width);
            InventoryItem[] items = inventoryManager.itemWheelItems;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                AddItem(positions[i], items[i].itemData);
            }
        }
        private void AddItem(Vector2 pos, ItemData itemData)
        {
            VisualElement item = new();
            Vector2Int size = itemData.size;
            float x = MaxItemWidth / size.x * size.y;
            float y = MaxItemHeight / size.y * size.x;
            if (x < MaxItemHeight)
            {
                item.style.width = MaxItemWidth;
                item.style.height = x;
            }
            else
            {
                item.style.width = y;
                item.style.height = MaxItemHeight;
            }
            item.style.position = Position.Absolute;
            item.style.top = pos.y - item.style.height.value.value/2;
            item.style.left = pos.x - item.style.width.value.value/2;
            item.style.backgroundImage = Background.FromSprite(itemData.icon);
            item.AddToClassList("item-wheel-item");
            this.itemWheelHolder.Add(item);
        }
    }
}
