using System;
using TM.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.Inventory.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uIDocument;
        private VisualElement root;
        private bool isOpen = true;
        void Awake()
        {
            root = uIDocument.rootVisualElement;
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