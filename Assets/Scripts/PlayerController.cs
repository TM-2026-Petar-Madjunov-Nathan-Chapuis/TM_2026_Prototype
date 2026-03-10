using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    [Header("References")]
    private CharacterController controller;
    [Header("MouvementSettings")] 
    private float speed = 5f;
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
        

        

    }

    private void MoveFunction() //mouvement + gravity
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 mouvement = new Vector3(move.x, 0, move.y);
        
        if(controller.isGrounded)
        {
            controller.Move(mouvement*speed*Time.deltaTime);
        }
        else;
        {
            controller.Move(new Vector3(0,-gravity*Time.deltaTime,0)); 
        }
    }
    private void JumpFunction()
    {
        if(controller.isGrounded)
        {
            //
        }

    }

}