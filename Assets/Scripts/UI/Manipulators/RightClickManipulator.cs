
using System;
using TM.Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

//inspired from unity's official tutorial about manipulators found on youtube. and the docs.
namespace TM.UI
{
    public class RightClickManipulator : PointerManipulator
    {
        public event Action<VisualElement> OnRightClickEvent;
        public RightClickManipulator(VisualElement target)
        {
            this.target = target;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnDownEvent);
        }
        //and this is where you clean them up. eveything is handled by unity which is convinient and why we are using it.
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnDownEvent);
        }
        private void OnDownEvent(PointerDownEvent evt)
        {
            if (evt.button == (int)UnityEngine.UIElements.MouseButton.RightMouse)
            {
                OnRightClickEvent.Invoke(this.target);
            }
        }
    }
}
