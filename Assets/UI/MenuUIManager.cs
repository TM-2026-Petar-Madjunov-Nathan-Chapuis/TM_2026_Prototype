using System;
using TM.Input;
using TM.Inventory.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.UI
{
    public class MenuUIManager : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset inventoryTemplate;
        [SerializeField] private UIDocument uIDocument;
        private VisualElement root;
        private VisualElement templateHolder;
        private bool isOpen = true;
        void Awake()
        {
            root = uIDocument.rootVisualElement;
            templateHolder = root.Q<VisualElement>("MenuTemplateHolder");
            var instance = inventoryTemplate.CloneTree();
            instance.style.flexGrow = 1;
            instance.style.flexShrink = 1;
            instance.style.alignSelf = Align.Stretch;
            instance.style.width = Length.Percent(100);
            instance.style.height = Length.Percent(100);
            templateHolder.Add(instance);
            InventoryUIManager inventoryUIManager = new();
            inventoryUIManager.SetRoot(templateHolder);
            inventoryUIManager.OnEnable();
        }
        private void OnEnable()
        {
            InputManager.Instance.RegisterListener("OpenInventory", ToggleHideShow, InputValueType.Button, false);
        }
        private void OnDisable()
        {
            InputManager.Instance.UnRegisterListener("OpenInventory", ToggleHideShow);
        }
        void Start()
        {
             
        }
        void Update()
        {
            
        }
        private void ToggleHideShow(InputValues inputValues)// called by the input manager, the argument is for type checking and is useless | for now simple hide and display but will need more logic when menu gets more populated with tabs
        {
            root.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex; //using display none, but many more available option at https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-best-practices-for-managing-elements.html
            isOpen = !isOpen;
        }
    }
}