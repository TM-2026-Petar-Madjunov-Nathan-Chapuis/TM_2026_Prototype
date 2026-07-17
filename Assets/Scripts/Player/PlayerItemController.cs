using System.Collections.Generic;
using TM.Input;
using TM.Inventory;
using Unity.VisualScripting;
using UnityEngine;

public class playerItemController : MonoBehaviour
{
    [SerializeField] private GameObject itemSocket;
    [SerializeField] private Animator animator;
    public InventoryItem heldItem {get; private set;}
    public GameObject heldItemGameobject {get; private set;}

    void Start()
    {
        InputManager.Instance.RegisterListener("Attack", UseItem, InputValueType.Button, true);
    }

    public void SetHeldItem(InventoryItem inventoryItem) {
        if (this.heldItemGameobject) Destroy(this.heldItemGameobject);
        if(inventoryItem == null)
        {
            heldItem = null;
            this.heldItemGameobject = null;
        }
        else
        {
            heldItem = inventoryItem;
            this.heldItemGameobject = Instantiate(inventoryItem.itemData.heldPrefab, itemSocket.transform); //clone the prefab into the itemSocket.
            HeldItem heldItemComponent = this.heldItemGameobject.GetComponent<HeldItem>();
            heldItemComponent.useBehavior = this.heldItem.itemData.useBehavior;
            heldItemComponent.player = this.gameObject;
            heldItemComponent.itemData = this.heldItem.itemData;
            heldItemComponent.playerItemController = this;
        }
    }
    public void Animate(string triggerName)
    {
        this.animator.SetTrigger(triggerName);
    }

    public void UseItem(InputValues inputValues)
    {
        if(this.heldItem == null) return;

        this.heldItem.itemData.Use(this.heldItemGameobject.GetComponent<HeldItem>(), inputValues.pressed);
    }
    public void HitboxEnable()
    {
        this.heldItemGameobject.GetComponent<HeldItem>().EnableHitbox();
    }
    public void HitboxDisable()
    {
        this.heldItemGameobject.GetComponent<HeldItem>().DisableHitbox();
    }
    public void AnimationEnd()
    {
        this.heldItem.itemData.useBehavior.OnAnimationEnd(this.heldItemGameobject.GetComponent<HeldItem>());
    }
}