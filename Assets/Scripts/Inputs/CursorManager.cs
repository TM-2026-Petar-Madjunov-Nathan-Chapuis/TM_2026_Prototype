using UnityEngine;
using UnityEngine.InputSystem;

namespace TM.Input
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance {get; private set;}
        void Awake() // attempt at a "singleton" but MUST BE SET IN THE SCRIPT ORDERING AT FIRST IN UNITY SETTINGS BECAUSE THIS IS BEING CALLED IN SOME ONENABLE() SCRIPTS
        {
            if (Instance != null && Instance != this) //avoid multiple instances
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

