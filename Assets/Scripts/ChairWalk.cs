using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class CairWalk : MonoBehaviour
{
    public InputActionAsset InputActions;
    
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction juumpAction;

    private Vector2 moveAMT;
    private Vector2 lookAMT;
    private Rigidbody rrigidbody;

    public float WalkSpeed = 10;
    public float RotateSpeed = 10;
    public float JumpSpeed = 10;

    private void OnEnable()
    {
        InputActions.FindActionMap("player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("player").Enable();
    }
    private void Awake()
    {
       moveAction = InputSystem.actions.FindAction("Move");
       lookAction = InputSystem.actions.FindAction("Look");
       juumpAction = InputSystem.actions.FindAction("Jump");

       rrigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        moveAMT = moveAction.ReadValue<Vector2>();
        lookAMT = lookAction.ReadValue<Vector2>();

        if (juumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }
    private void Jump()
    {
        rrigidbody.AddForceAtPosition(new Vector3(0, 5f, 0), Vector3.up, ForceMode.Impulse);
    }
    private void FixedUpdate()
    {
        Walking();
        Rotating();
    }
    private void Walking()
    {
        rrigidbody.MovePosition(rrigidbody.position + transform.forward*moveAMT.y*WalkSpeed*Time.deltaTime);
    }
    private void Rotating()
    {
        if (moveAMT.y != 0)
        {
            float rotationAmount = lookAMT.x * RotateSpeed * Time.deltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
            rrigidbody.MoveRotation(rrigidbody.rotation * deltaRotation);
        }
    }












}
