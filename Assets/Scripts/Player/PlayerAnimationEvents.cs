using UnityEngine;

namespace TM.Player
{
    public class PlayerAnimationEvents : MonoBehaviour
    //smal script that sits on the model gameobject that has the animator, and redirects the animations events to their coresponding scripts
    {
        public playerItemController playerItemController;

        public void CastRay()
        {
            playerItemController.CastRay();
        }

        public void AnimationEnd()
        {
            playerItemController.AnimationEnd();
        }

        public void AnimationUse()
        {
            playerItemController.AnimationUse();
        }
    }
}
