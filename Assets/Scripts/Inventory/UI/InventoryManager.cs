using System.Collections.Generic;
using TM.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.Inventory.UI
{
    public class InventoryManager : GenericUITemplateManager
    {
        private GameObject player;
        private Inventory.InventoryManager inventoryManager;
        private VisualElement[,] gridElements;
        private VisualElement[,] itemsElements;
        private VisualElement itemLayer;
        private VisualElement itemGrid;
        private VisualElement itemDescription;
        private VisualElement equipment;
        private VisualElement itemMain;
        private VisualElement playerView;
        private ProgressBar weightBar;
        private VisualElement weightBarHolder;
        private VisualElement dropItemIcon;
        private VisualElement headArmorSlot;
        private VisualElement chestArmorSlot;
        private VisualElement legArmorSlot;
        private VisualElement bootsArmorSlot;
        private VisualElement itemWheelHolder;
        private Dictionary<Vector2, Vector2Int> allParentToIndexPositions;
        private int switchFrameTime;
        //LOWER "MANAGERS"
        private GridHighlighter gridHighlighter;
        private ItemDescriptor itemDescriptor;
        private PlayerViewManager inventoryPlayerViewManager;
        private ItemWheelManager itemWheelManager;
        private DropAreaManager dropAreaManager;
        private ItemWheelVectorImager itemWheelVectorImager;

        public override void SetRoot(VisualElement root)
        {
            this.gridHighlighter = GameObject.FindAnyObjectByType<GridHighlighter>();
            this.itemDescriptor = GameObject.FindAnyObjectByType<ItemDescriptor>();
            this.itemWheelManager = GameObject.FindAnyObjectByType<ItemWheelManager>();
            this.dropAreaManager = GameObject.FindAnyObjectByType<DropAreaManager>();
            this.itemWheelVectorImager = GameObject.FindAnyObjectByType<ItemWheelVectorImager>();
            base.SetRoot(root);
            this.player = GameObject.FindGameObjectWithTag("Player");
            if (this.player == null) { throw new UnityException("no player found by tag : Player"); }
            this.inventoryManager = this.player.GetComponent<Inventory.InventoryManager>();

            itemLayer = this.root.Q<VisualElement>("ItemsLayer");
            itemDescription = this.root.Q<VisualElement>("ItemDescription");
            itemGrid = this.root.Q<VisualElement>("GridBackground");
            equipment = this.root.Q<VisualElement>("Equipement");
            weightBar = this.root.Q<ProgressBar>("WeightBar");
            weightBarHolder = this.root.Q<VisualElement>("WeightBarHolder");
            dropItemIcon = this.root.Q<VisualElement>("DropItemIcon");
            itemMain = this.root.Q<VisualElement>("Main");
            playerView = this.root.Q<VisualElement>("PlayerView");
            headArmorSlot = this.root.Q<VisualElement>("HeadArmorSlot");
            chestArmorSlot = this.root.Q<VisualElement>("ChestArmorSlot");
            legArmorSlot = this.root.Q<VisualElement>("LegArmorSlot");
            bootsArmorSlot = this.root.Q<VisualElement>("BootsArmorSlot");
            itemWheelHolder = this.root.Q<VisualElement>("ItemWheelHolder");
            allParentToIndexPositions = InventoryManagerHelper.AllParentToIndexPositions(this.inventoryManager);
            inventoryPlayerViewManager = GameObject.FindAnyObjectByType<PlayerViewManager>();
        }
        public override void OnEnable()
        {
            WeightBarUpdate();
            itemMain.RegisterCallback<GeometryChangedEvent>(ItemCallback); //needed because OnEnable might and does call before the UI even resolves for the user, leading to width = 0 on cellsize calculation. also it calls a redraw on every resolution change
            //ENABLE LOWER MANAGERS
            inventoryPlayerViewManager.Enable(playerView);
            itemDescriptor.Enable(this.root.Q<VisualElement>("ItemDescription"));
            itemWheelManager.Enable(itemWheelHolder, inventoryManager);
            dropAreaManager.Enable(weightBar, weightBarHolder, dropItemIcon);
        }
        private void ItemCallback(GeometryChangedEvent evt)
        {
            itemGrid.Clear();
            itemLayer.Clear();
            Grid();
            EquipementSlots();
            FillItems();
            allParentToIndexPositions = InventoryManagerHelper.AllParentToIndexPositions(this.inventoryManager);
        }
        public override void OnDisable()
        {
            itemDescriptor.Disable();
            itemGrid.Clear();
            itemLayer.Clear();
            inventoryPlayerViewManager.Disable();
            itemMain.UnregisterCallback<GeometryChangedEvent>(ItemCallback);
            this.dropAreaManager.Disable();
        }
        private void WeightBarUpdate()
        {

        }
        private void EquipementSlots()
        {
            this.headArmorSlot.style.width = this.inventoryManager.cellSize;
            this.headArmorSlot.style.height = this.inventoryManager.cellSize;
            this.chestArmorSlot.style.height = this.inventoryManager.cellSize;
            this.chestArmorSlot.style.width = this.inventoryManager.cellSize;
            this.legArmorSlot.style.width = this.inventoryManager.cellSize;
            this.legArmorSlot.style.height = this.inventoryManager.cellSize;
            this.bootsArmorSlot.style.height = this.inventoryManager.cellSize;
            this.bootsArmorSlot.style.width = this.inventoryManager.cellSize;
        }
        private void Grid()
        {
            float height = this.itemGrid.parent.resolvedStyle.height - this.inventoryManager.cellPosMargin * 2;
            float width = this.itemMain.resolvedStyle.width - 2 * this.inventoryManager.cellPosMargin;
            this.inventoryManager.UpdateCellsize(width, height);
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

                    slot.style.left = x * this.inventoryManager.cellSize + this.inventoryManager.cellPosMargin;
                    slot.style.top = y * this.inventoryManager.cellSize + this.inventoryManager.cellPosMargin;

                    slot.style.width = this.inventoryManager.cellSize;
                    slot.style.height = this.inventoryManager.cellSize;

                    if (x == 0)
                    {
                        slot.style.borderLeftWidth = 1;
                    }
                    if (y == this.inventoryManager.gridSize.y - 1)
                    {
                        slot.style.borderBottomWidth = 1;
                    }

                    itemGrid.Add(slot);
                    gridElements[x, y] = slot;
                }
            }
            itemGrid.style.width = this.inventoryManager.gridSize.x * this.inventoryManager.cellSize + 2 * this.inventoryManager.cellPosMargin;
            itemGrid.style.height = this.inventoryManager.gridSize.y * this.inventoryManager.cellSize + 2 * this.inventoryManager.cellPosMargin;
            itemLayer.style.width = this.inventoryManager.gridSize.x * this.inventoryManager.cellSize + 2 * this.inventoryManager.cellPosMargin;
            itemLayer.style.height = this.inventoryManager.gridSize.y * this.inventoryManager.cellSize + 2 * this.inventoryManager.cellPosMargin;
        }
        private void FillItems()
        {
            foreach (InventoryItem invItem in this.inventoryManager.inventoryGrid.itemsList)
            {
                CreateItem(invItem);
            }
            foreach (VisualElement item in this.itemsElements)
            { //add the drag and drop behaviour/manipulator
                ItemDragManipulator itemDragManipulator = new ItemDragManipulator(item);
                itemDragManipulator.OnDragEndEvent += HandleDragEnd;
                itemDragManipulator.OnDragMoveEvent += HandleDragging;
                itemDragManipulator.OnDragStartEvent += HandleDragStart;
                ManipulatorActivationFilter activator = new ManipulatorActivationFilter
                {
                    button = MouseButton.LeftMouse,
                };
                itemDragManipulator.activators.Add(activator);
                item.AddManipulator(itemDragManipulator);
                RightClickManipulator rightClickManipulator = new RightClickManipulator(item);
                rightClickManipulator.OnRightClickEvent += HandleRotate;
                item.AddManipulator(rightClickManipulator);
            }
        }
        private void CreateItem(InventoryItem inventoryItem)
        {
            VisualElement item = new VisualElement();

            item.AddToClassList("inventory__item");
            item.name = inventoryItem.itemData.name;

            item.userData = inventoryItem; //this stores the inventoryitem reference to the visualelement itself.

            item.style.position = Position.Absolute;
            Vector2 pos = InventoryManagerHelper.IndexToParentPos(inventoryItem.position, this.inventoryManager);
            item.style.left = pos.x;
            item.style.top = pos.y;
            SetVisualItem(item, inventoryItem);

            this.itemLayer.Add(item);
            this.itemsElements[inventoryItem.position.x, inventoryItem.position.y] = item;
        }
        private void HandleDragEnd(VisualElement item)
        {
            InventoryItem inventoryItem = item.userData as InventoryItem;
            gridHighlighter.ClearColors();
            itemWheelManager.ClearHighlight();
            if(dropAreaManager.hightlighted) //if its highlighted means the item was here when dropped.
            {
                this.inventoryManager.DropItem(inventoryItem);
                this.itemsElements[inventoryItem.position.x, inventoryItem.position.y] = null;
                itemLayer.Remove(item);
                dropAreaManager.SwitchToWeightBar();
                return;
            }
            dropAreaManager.SwitchToWeightBar();
            VisualElement parent = item.parent;
            if (parent == null) return;

            Vector2Int oldPos = inventoryItem.position;
            Vector2 pos = new Vector2(item.style.left.value.value, item.style.top.value.value);
            //IF POS < 0 THEN ITS ABOUT EITHER THE ARMOR OR THE ITEM WHEEL, SO DIFFERENT LOGIC
            if (pos.x < 0)
            {
                if (pos.y > parent.resolvedStyle.height / 2) //ITS ABOUT THE ARMOR
                {

                }
                else //ITS ABOUT THE ITEMWHEEL
                {
                    pos = parent.ChangeCoordinatesTo(itemWheelHolder, pos); //convert the positions to itemwheellocal space.
                    pos = new Vector2(pos.x + item.resolvedStyle.width / 2, pos.y + item.resolvedStyle.height / 2); //ajust to be the center of the item.
                    int index = GameObject.FindAnyObjectByType<ItemWheelVectorImager>().NearestItemWheelPosition(pos, itemWheelHolder.resolvedStyle.width);
                    this.itemWheelManager.AddItem(inventoryItem, index);
                    //GET THE ITEM BACK AS WE STORE ONLY THE REFERENCE.
                    Vector2 newPos = InventoryManagerHelper.IndexToParentPos(oldPos, this.inventoryManager);
                    bool result = this.inventoryManager.inventoryGrid.TryMoveItem(inventoryItem, oldPos);//checks if the item fits in the old space it occupied because it might have been rotated.
                    item.style.left = newPos.x;
                    item.style.top = newPos.y; //move the item back at the start position, providing the snapping.   
                    if (!result) //the item doesnt fit in the old space, which means it has beens rotated, so rotate it back to what it once was.
                    {
                        inventoryItem.Rotate();
                        SetVisualItem(item, inventoryItem); //update the item.
                    }
                }
            }

            (Vector2, Vector2Int, float) i = InventoryManagerHelper.NearestGridIndexedPosition(pos, allParentToIndexPositions);
            if (this.inventoryManager.inventoryGrid.TryMoveItem(inventoryItem, i.Item2))
            {
                Vector2 newPos = InventoryManagerHelper.IndexToParentPos(i.Item2, this.inventoryManager);
                item.style.left = newPos.x;
                item.style.top = newPos.y; //move the item in UI, providing the snapping.
                inventoryItem.SetPosition(i.Item2);
                itemsElements[oldPos.x, oldPos.y] = null;
                itemsElements[i.Item2.x, i.Item2.y] = item; // move the item on the 2d map of items.
            }
            else
            {
                Vector2 newPos = InventoryManagerHelper.IndexToParentPos(oldPos, this.inventoryManager);
                bool result = this.inventoryManager.inventoryGrid.TryMoveItem(inventoryItem, oldPos);//checks if the item fits in the old space it occupied because it might have been rotated.
                item.style.left = newPos.x;
                item.style.top = newPos.y; //move the item back at the start position, providing the snapping.   
                if (!result) //the item doesnt fit in the old space, which means it has beens rotated, so rotate it back to what it once was.
                {
                    inventoryItem.Rotate();
                    SetVisualItem(item, inventoryItem); //update the item.
                }
            }
        }
        private void HandleDragging(VisualElement item, Vector2 pointerPos)
        {
            if (switchFrameTime>=0) switchFrameTime++;
            if (switchFrameTime > 3) {switchFrameTime = -1; this.dropAreaManager.SwitchToDropArea();}//we consider this dragging and so put the option to drop items.
            gridHighlighter.OnDragMove(this.gridElements, item, this.inventoryManager, this.allParentToIndexPositions);
            Vector2 pos = new Vector2(item.resolvedStyle.left + item.resolvedStyle.width / 2, item.resolvedStyle.top + item.resolvedStyle.height / 2);
            this.dropAreaManager.Hightlight(pos, this.inventoryManager.gridSize, this.inventoryManager.cellSize);
            itemWheelManager.Highlight(this.itemWheelVectorImager.NearestItemWheelPosition(item.parent.ChangeCoordinatesTo(itemWheelHolder, pos), itemWheelHolder.resolvedStyle.width));
        }
        private void HandleDragStart(VisualElement item)
        {
            this.switchFrameTime = 0;
            ItemSelector.Select(item, item.userData as InventoryItem, inventoryManager, this.itemWheelHolder);
        }
        private void HandleRotate(VisualElement item)
        {

            InventoryItem inventoryItem = item.userData as InventoryItem;
            if (gridHighlighter.isGridHighlighted) //the item is dragging, so we hold its rotated state and the handledropdown gets it back if it fails.
            {
                inventoryItem.Rotate();
                SetVisualItem(item, inventoryItem);
                gridHighlighter.Refresh(this.gridElements, item, this.inventoryManager, this.allParentToIndexPositions);
            }
            else
            {
                ItemSelector.Select(item, inventoryItem, this.inventoryManager, this.itemWheelHolder);
                bool result = this.inventoryManager.inventoryGrid.TryRotateItem(inventoryItem);
                if (result)
                {
                    SetVisualItem(item, inventoryItem);
                }
                else
                {
                    Debug.Log("Failed to rotate"); //if we have time we can give here some visual and/or sound cue to the player for some feeback.
                }
            }
        }
        private VisualElement SetVisualItem(VisualElement item, InventoryItem inventoryItem)
        {
            Vector2Int size = inventoryItem.itemData.size;
            if (inventoryItem.rotated)
            {
                size = new Vector2Int(size.y, size.x);
            }
            item.style.width = size.x * this.inventoryManager.cellSize;
            item.style.height = size.y * this.inventoryManager.cellSize;

            item.style.backgroundImage = Background.FromSprite(inventoryItem.rotated ? inventoryItem.itemData.iconRotated : inventoryItem.itemData.icon);
            return item;
        }
    }
}