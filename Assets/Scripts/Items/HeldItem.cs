using TM.Items;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public UseBehavior useBehavior;
    public GameObject player;
    public ItemData itemData;
    public playerItemController playerItemController;
    private Collider hitboxCollider;

    void Start()
    {
        hitboxCollider = this.gameObject.GetComponent<Collider>();
        hitboxCollider.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        this.useBehavior.OnTriggerEnter(other, this);
    }
    public void EnableHitbox()
    {
        hitboxCollider.enabled = true;
    }
    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
    }
    public void Animate(string triggerName)
    {
        this.playerItemController.Animate(triggerName);
    }
}