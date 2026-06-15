using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator
{
    private bool isDragging;
    private Vector2 startPointerPos;
    private Vector2 startTargetWorldPos;

    public DragAndDropManipulator(VisualElement target)
    {
        this.target = target;
    }
    //this is where you register the callbacks for events
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnDragStart);
        target.RegisterCallback<PointerUpEvent>(OnDragEnd);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove); //fires when mouse moves
        target.RegisterCallback<PointerCaptureOutEvent>(OnCaptureOut); //read docs about capture envents for more information but basically this fires when the mouse isnt "focused" on the element
    }
    //and this is where you clean them up. eveything is handled by unity which is convinient and why we are using it.
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
        target.style.position = Position.Absolute; //switch to absolute position.

        this.startPointerPos = downEvent.position;
        this.startTargetWorldPos = target.worldBound.position;

        target.BringToFront(); //brings to the front of the scene.
        target.CapturePointer(downEvent.pointerId); //this is related to capture events so reads the docs, but basically this makes the dragged element still "focused" when the mouse leaves its area

        downEvent.StopPropagation(); //read more about event and their propagation (stopping propagation maybe not needed but advised)
    }
    private void OnDragEnd(PointerUpEvent upEvent)
    {
        if (!isDragging || !target.HasPointerCapture(upEvent.pointerId)) return; // if not dragging or target hasnt captured the pointer yet return

        target.ReleasePointer(upEvent.pointerId); //release the pointer so that on cpature out fires.

        upEvent.StopPropagation();
    }
    private void OnPointerMove(PointerMoveEvent moveEvent)
    {
        if (!isDragging || !target.HasPointerCapture(moveEvent.pointerId)) return; // if not dragging or target hasnt captured the pointer yet return

        VisualElement parent = target.parent; //this is needed because we use position.absolute and so the coords are in the parent's space
        if (parent == null) return;

        Vector2 pointerPos = (Vector2)moveEvent.position;
        Vector2 delta = pointerPos - this.startPointerPos; //how far the pointer moved from the oringinal position

        Vector2 newWorlPos = this.startTargetWorldPos + delta; //adds the delta to the orginal target position
        Vector2 newLocalPos = parent.WorldToLocal(newWorlPos); //convert to the parent absolute space

        target.style.left = newLocalPos.x;
        target.style.top = newLocalPos.y;

        moveEvent.StopPropagation();
    }
    private void OnCaptureOut(PointerCaptureOutEvent captureOutEvent) //read more about the docs but this gets fired when the drag stop basically. (precisely when the target loses capture of the pointer, of the mouse's "focus")
    {
        isDragging = false;
    }
}