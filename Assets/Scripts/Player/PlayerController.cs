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
    [SerializeField] private Animator animator;

    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    
    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }
    private void Awake()
    {
        InputActionMap playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move", true);
        jumpAction = playerMap.FindAction("Jump", true);
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        JumpFunction();
        MoveFunction();
    }

    private void MoveFunction() //mouvement + gravity
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 xZmouvement = new Vector3(move.x, 0, move.y);
        Vector3 xYZmouvement = new Vector3(xZmouvement.x*speed*Time.deltaTime, verticalSpeed*Time.deltaTime, xZmouvement.z*speed*Time.deltaTime);
        Vector3 xYZPlayerMouvement = transform.TransformDirection(xYZmouvement);

        controller.Move(xYZPlayerMouvement);
        animator.SetFloat("speed", Vector3.Magnitude(controller.velocity)/speed);
        
        if(controller.isGrounded && verticalSpeed <= 0)
        {
            verticalSpeed = -1f;
            animator.SetBool("falling", false);
        }
        else
        {
            verticalSpeed += -gravity*Time.deltaTime;
            animator.SetBool("falling", true);
        }
    }
    private void JumpFunction()
    {
        if(controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalSpeed = jumpForce;
            animator.SetBool("jump", true);
        }
        if(animator.GetBool("jump"))
        {
            animator.SetBool("jump", false);
        }
    }
}