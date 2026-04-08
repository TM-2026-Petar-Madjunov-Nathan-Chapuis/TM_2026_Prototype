using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    [Header("References")]
    private CharacterController controller;
    [Header("MouvementSettings")] 
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float verticalSpeed = 0f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float mouseSensibilityHorizontal = 100f;
    [SerializeField] private float mouseSensibilityVertical = 50f;
    [SerializeField] private float maxUpCameraRotation = 90f;
    [SerializeField] private float maxDownCameraRotation = -90f;

    
<<<<<<< Updated upstream
    [SerializeField]float cameraXRotation = 0f;
    private Camera playerCamera;
=======
    private new Camera camera;
>>>>>>> Stashed changes

    public InputActionAsset InputActions;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    
    private void OnEnable()
    {
        InputActions.FindActionMap("player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("player").Disable();
    }
    private void Awake()
    {
       moveAction = InputSystem.actions.FindAction("Move");
       lookAction = InputSystem.actions.FindAction("Look");
       jumpAction = InputSystem.actions.FindAction("Jump");


    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        MoveFunction();
        JumpFunction();
        LookFunction();
    }

    private void MoveFunction() //mouvement + gravity
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 xZmouvement = new Vector3(move.x, 0, move.y);
        Vector3 xYZmouvement = new Vector3(xZmouvement.x*speed*Time.deltaTime, verticalSpeed*Time.deltaTime, xZmouvement.z*speed*Time.deltaTime);
        Vector3 xYZPlayerMouvement = transform.TransformDirection(xYZmouvement);

        controller.Move(xYZPlayerMouvement);
        
        if(controller.isGrounded && verticalSpeed <= 0)
        {
            verticalSpeed = -1f;
        }
        else
        {
            verticalSpeed += -gravity*Time.deltaTime;
        }
    }
    private void JumpFunction()
    {
        if(controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalSpeed = jumpForce;
        }
    }
    private void LookFunction()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();
        

        if(lookAction.triggered)
        {
            cameraXRotation += -look.y*mouseSensibilityVertical*Time.deltaTime;
            cameraXRotation = Mathf.Clamp(cameraXRotation,maxDownCameraRotation,maxUpCameraRotation);
            
            transform.Rotate(0,look.x*mouseSensibilityHorizontal*Time.deltaTime,0);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraXRotation,0f,0f);
            //playerCamera.transform.Rotate(cameraXRotation,0,0);            
        }
    }    
}