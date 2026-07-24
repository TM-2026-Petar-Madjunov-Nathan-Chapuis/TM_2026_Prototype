using TM.Input;
using TM.Player;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteracter : MonoBehaviour
{
    [SerializeField] private float sphereRadius = 0.2f;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionMask;
    [SerializeField] private PlayerCamera playerCamera;
    void Start()
    {
    }
    void OnDisable()
    {
        InputManager.Instance.UnRegisterListener("Interact", Raycast);
    }
    void OnEnable()
    {
        InputManager.Instance.RegisterListener("Interact", Raycast, InputValueType.Button, false);
    }
    private void Raycast(InputValues inputValues)
    {
        if (Physics.SphereCast(playerCamera.firstPerson.gameObject.transform.position, sphereRadius, playerCamera.firstPerson.transform.forward, out RaycastHit hit, interactionDistance, interactionMask))
        {
            if (inputValues.pressed)
            {
                hit.collider.gameObject.GetComponent<IInteractable>().Interact();   
            }
        }
    }
}


//outlines docs : https://ameye.dev/notes/rendering-outlines/