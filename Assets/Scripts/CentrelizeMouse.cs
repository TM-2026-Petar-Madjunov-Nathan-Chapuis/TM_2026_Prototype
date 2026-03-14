using UnityEngine;
using UnityEngine.InputSystem;

public class CentrelizeMouse : MonoBehaviour
{
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
