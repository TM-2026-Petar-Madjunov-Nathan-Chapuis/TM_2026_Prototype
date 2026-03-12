using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    [Header("References")]
    private CharacterController controller;
    [Header("MouvementSettings")] 
    private float speed = 5f;
    private float jumpForce = 5f;

    [SerializeField] private float verticalSpeed = 0f; //modifiable
    [SerializeField] private float gravity = 9.81f;

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

        controller.Move(xYZmouvement);
        
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
        
    }
}