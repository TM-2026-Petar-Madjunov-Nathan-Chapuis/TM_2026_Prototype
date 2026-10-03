 using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TM.Input;
using TM.Saving;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TM.Player
{
    public class PlayerController : MonoBehaviour, ISaveable
    {
        [Header("References")]
        private CharacterController controller;
        [Header("MouvementSettings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float verticalSpeed = 0f;
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float stickToGroundFactor = 400f;
        [SerializeField] private Animator animator;
        private static readonly int JumpHash = Animator.StringToHash("jump"); // better performance according to unity UNT0041 warning (store hash reference to parameter rather than repeated id lookup)
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private static readonly int FallingHash = Animator.StringToHash("falling");

        public string UID => "PlayerController";

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }
        private void OnEnable()
        {
            InputManager.Instance.RegisterListener("Move", Move, InputValueType.Vector2, false);
            InputManager.Instance.RegisterListener("Jump", Jump, InputValueType.Button, true);
        }
        private void OnDisable()
        {
            InputManager.Instance.UnRegisterListener("Move", Move);
            InputManager.Instance.UnRegisterListener("Jump", Jump);
        }
        private void Move(InputValues inputValues) //mouvement + gravity | ran every frame by the input listener in input manager
        {
            if (Time.deltaTime <= 0f)
            {
                return;
            }

            bool isGrounded = controller.isGrounded;

            if (isGrounded && verticalSpeed <= 0)
            {
                verticalSpeed = -stickToGroundFactor * Time.deltaTime;
                animator.SetBool(FallingHash, false);
            }
            else
            {
                verticalSpeed += -gravity * Time.deltaTime;
                animator.SetBool(FallingHash, verticalSpeed < 0);
            }

            Vector2 move = inputValues.vector2Value;
            Vector3 xZmouvement = new Vector3(move.x, 0, move.y);
            Vector3 xYZmouvement = new Vector3(xZmouvement.x * speed * Time.deltaTime, verticalSpeed * Time.deltaTime, xZmouvement.z * speed * Time.deltaTime);
            Vector3 xYZPlayerMouvement = transform.TransformDirection(xYZmouvement);

            controller.Move(xYZPlayerMouvement);
            animator.SetFloat(SpeedHash, Math.Clamp(Vector3.Magnitude(controller.velocity) / speed, 0, 1)); // divide by default speed to get 0 -> 1 value (above default speed will default to 1)
        }
        private void Jump(InputValues inputValues) //only called when jump button is pressed (and parameter is just for type checking, doesnt do anything)
        {
            if (controller.isGrounded)
            {
                verticalSpeed = jumpForce;
                animator.SetTrigger(JumpHash);
            }
        }

        public object SaveData()
        {
            Vector3 rootPosition = transform.position;
            PlayerControllerSaveData saveData = new PlayerControllerSaveData
            {
                rootPositionX = rootPosition.x,
                rootPositionY = rootPosition.y,
                rootPositionZ = rootPosition.z,
            };
            return saveData;
        }

        public void LoadData(string data)
        {
            PlayerControllerSaveData saveData = JsonConvert.DeserializeObject<PlayerControllerSaveData>(data);
            Vector3 savedPosition = new Vector3(saveData.rootPositionX, saveData.rootPositionY, saveData.rootPositionZ);

            CharacterController characterController = GetComponent<CharacterController>();
            characterController.enabled = false; //in order to move the player, character controller must be disabled

            transform.position = savedPosition;
            verticalSpeed = 0f;

            characterController.enabled = true; //reenable
        }
    }
    [Serializable]
    public struct PlayerControllerSaveData
    {
        public float rootPositionX;
        public float rootPositionY;
        public float rootPositionZ;
    }
}