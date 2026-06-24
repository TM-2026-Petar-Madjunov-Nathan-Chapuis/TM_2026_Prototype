using System;
using TM.Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.UI
{
    public class PlayerViewManipulator : PointerManipulator
    {
        private bool isDragging;
        public event Action<Vector2> OnMoveAction;
        public PlayerViewManipulator(VisualElement target)
        {
            this.target = target;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnDragStart);
            target.RegisterCallback<PointerUpEvent>(OnDragEnd);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerCaptureOutEvent>(OnCaptureOut);
        }
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnDragStart);
            target.UnregisterCallback<PointerUpEvent>(OnDragEnd);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnCaptureOut);

        }
        private void OnDragStart(PointerDownEvent downEvent)
        {
            isDragging = true;
            target.CapturePointer(downEvent.pointerId);
            downEvent.StopPropagation();
        }
        private void OnDragEnd(PointerUpEvent upEvent)
        {
            if (!isDragging || !target.HasPointerCapture(upEvent.pointerId)) return;
            target.ReleasePointer(upEvent.pointerId);
            upEvent.StopPropagation();
        }
        private void OnPointerMove(PointerMoveEvent moveEvent)
        {
            if (!isDragging || !target.HasPointerCapture(moveEvent.pointerId)) return; // if not dragging or target hasnt captured the pointer yet return
            OnMoveAction.Invoke(moveEvent.deltaPosition);

            moveEvent.StopPropagation();
        }
        private void OnCaptureOut(PointerCaptureOutEvent captureOutEvent) //read more about the docs but this gets fired when the drag stop basically. (precisely when the target loses capture of the pointer, of the mouse's "focus")
        {
            isDragging = false;
        }
    }
}