using UnityEngine;
using UnityEngine.InputSystem;

namespace TM.Input
{
    public class CentrelizeMouse : MonoBehaviour
    {
        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

