using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour 
//smal script that sits on the model gameobject that has the animator, and redirects the animations events to their coresponding scripts
{
    public playerItemController playerItemController;

    public void HitboxEnabled()
    {
        playerItemController.HitboxEnable();
    }

    public void HitboxDisabled()
    {
        playerItemController.HitboxDisable();
    }

    public void AnimationEnd()
    {
        playerItemController.AnimationEnd();
    }
}