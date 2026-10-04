using TM.Inventory;
using TM.Items;
using TM.Player;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public UseBehavior useBehavior;
    public GameObject player;
    public ItemData itemData;
    public playerItemController playerItemController;
    public void Collision(Collider collider) {
        this.itemData.useBehavior.Collision(collider, this, player);
    }
    public void AnimationUse(InventoryItem inventoryItem) {
        this.itemData.useBehavior.OnAnimationUse(this, player, inventoryItem);
    }
    public void Animate(string triggerName)
    {
        this.playerItemController.Animate(triggerName);
    }
}