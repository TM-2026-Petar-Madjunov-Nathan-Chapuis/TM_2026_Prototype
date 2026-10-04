using System.Collections.Generic;
using TM.Input;
using TM.Inventory;
using TM.Misc;
using UnityEngine;

namespace TM.Player
{
    public class playerItemController : MonoBehaviour
    {
        [SerializeField] private float raycastRadius; //makes the targeting easier
        [SerializeField] private float raycastDistance; //reach
        [SerializeField] private LayerMask raycastMask; //exclude the Player layer
        [SerializeField] private GameObject itemSocket;
        [SerializeField] private Animator animator;
        public InventoryItem heldItem { get; private set; }
        public GameObject heldItemGameobject { get; private set; }
        private bool itemActionInProgress;

        void Start()
        {
            InputManager.Instance.RegisterListener("Attack", UseItem, InputValueType.Button, true);
        }

        public void SetHeldItem(InventoryItem inventoryItem)
        {
            itemActionInProgress = false;

            if (this.heldItemGameobject) Destroy(this.heldItemGameobject);
            if (inventoryItem == null)
            {
                heldItem = null;
                this.heldItemGameobject = null;
                this.animator.SetBool("grabbing", false);
            }
            else
            {
                this.animator.SetBool("grabbing", true);
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
            if (itemActionInProgress) return;

            itemActionInProgress = true;
            this.animator.SetTrigger(triggerName);
        }

        public void UseItem(InputValues inputValues)
        {
            if (this.heldItem == null) return;

            this.heldItem.itemData.Use(this.heldItemGameobject.GetComponent<HeldItem>(), inputValues.pressed);
        }
        void OnDrawGizmos()
        {
            PlayerCamera playerCamera = this.gameObject.GetComponent<PlayerCamera>(); //as this script, player camera sits on the player
            if (playerCamera == null || playerCamera.firstPerson == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCamera.firstPerson.transform.position, raycastRadius);
            Vector3 right = playerCamera.firstPerson.transform.position + raycastRadius * Vector3.Cross(playerCamera.firstPerson.transform.forward, playerCamera.firstPerson.transform.up);
            Gizmos.DrawLine(right, right + playerCamera.firstPerson.transform.forward * raycastDistance);
            Vector3 left = playerCamera.firstPerson.transform.position + raycastRadius * Vector3.Cross(playerCamera.firstPerson.transform.up, playerCamera.firstPerson.transform.forward);
            Gizmos.DrawLine(left, left + playerCamera.firstPerson.transform.forward * raycastDistance);
            Gizmos.DrawWireSphere(playerCamera.firstPerson.transform.position + playerCamera.firstPerson.transform.forward * (raycastDistance - raycastRadius), raycastRadius);
        }
        public void CastRay() //THIS IS CALLED DURING THE ANIMATION OF THE ITEM, USING EVENTS (through playeranimationevents.cs)
        {
            PlayerCamera playerCamera = this.gameObject.GetComponent<PlayerCamera>(); //as this script, player camera sits on the player
            Camera camera = playerCamera.firstPerson; //gets the first person camera even if in 3d person

            RaycastHit[] hits = Physics.SphereCastAll(
                camera.transform.position, //origin
                raycastRadius,             //radius
                camera.transform.forward,  //direction
                raycastDistance,           //distance
                raycastMask,               //layermask
                QueryTriggerInteraction.Ignore); //if the raycast hits trigger collider or not

            System.Array.Sort(hits, (first, second) => first.distance.CompareTo(second.distance)); //https://www.reddit.com/r/Unity3D/comments/za14u3/remember_raycastalls_sorting_is_undefined/
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform == this.gameObject.transform || hit.collider.transform.IsChildOf(this.gameObject.transform)) continue; //if player (normally exluded), continue
                bool isHealthTarget = hit.collider.GetComponentInParent<HealthManager>() != null;
                bool isTreeTarget = hit.collider.GetComponentInParent<TreeFall>() != null;
                bool isInteractableTarget = hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable");
                if (!isHealthTarget && !isTreeTarget && !isInteractableTarget) continue;

                Debug.Log(hit.collider.gameObject.name);
                this.heldItemGameobject.GetComponent<HeldItem>().Collision(hit.collider); //register to the object a hit
                break;
            }
        }
        public void AnimationEnd() //THIS IS CALLED DURING THE ANIMATION OF THE ITEM, USING EVENTS (through playeranimationevents.cs)
        {
            itemActionInProgress = false;
            if (this.heldItem == null || this.heldItemGameobject == null) return;

            this.heldItem.itemData.useBehavior.OnAnimationEnd(this.heldItemGameobject.GetComponent<HeldItem>());
        }
        public void AnimationUse()
        {
            this.heldItemGameobject.GetComponent<HeldItem>().AnimationUse(heldItem);
        }
    }
}
