using System;
using Newtonsoft.Json;
using TM.Input;
using TM.Saving;
using Unity.Mathematics;
using UnityEngine;

namespace TM.Player
{
    public class PlayerCamera : MonoBehaviour, ISaveable
    {
        public Camera firstPerson;
        public Camera thirdPerson;
        [SerializeField] private int FOV = 90;
        public bool isFirstPerson = true;
        public string UID => "cameraToggle";
        [SerializeField] private float xRotation = 0f;
        [SerializeField] private float yRotation = 0f;

        [SerializeField] private float xSensitivity = 100f;
        [SerializeField] private float ySensitivity = 50f;
        [SerializeField] private float thirdPersonDistance = 5f;
        [SerializeField] private float thirdPersonSmoothFactor;
        [SerializeField] private float castRadius = 0.2f;
        [SerializeField] private CrosshairManager crosshairManager;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject firstPersonSocket;
        public bool lookingAllowed = true;
        private PlayerController playerController;

        public void LoadData(string data)
        {
            PlayerCameraSaveData saveData = JsonConvert.DeserializeObject<PlayerCameraSaveData>(data);
            this.FOV = saveData.FOV;
            this.isFirstPerson = !saveData.isFirstPerson;
            this.ToggleCamera(new InputValues());
            this.lookingAllowed = true;
        }

        public object SaveData()
        {
            return new PlayerCameraSaveData
            {
                FOV = this.FOV,
                isFirstPerson = this.isFirstPerson,
            };
        }
        private void OnEnable()
        {
            InputManager.Instance.RegisterListener("Look", Look, InputValueType.Vector2, true);
            InputManager.Instance.RegisterListener("ToggleCamera", ToggleCamera, InputValueType.Button, true);
        }
        private void OnDisable()
        {
            InputManager.Instance.UnRegisterListener("Look", Look);
            InputManager.Instance.UnRegisterListener("ToggleCamera", ToggleCamera);
        }

        void Start()
        {
            if (isFirstPerson)
            {
                firstPerson.gameObject.SetActive(true);
                thirdPerson.gameObject.SetActive(false);
            }
            else
            {
                firstPerson.gameObject.SetActive(false);
                thirdPerson.gameObject.SetActive(true);
            }
            playerController = this.gameObject.GetComponent<PlayerController>();
        }
        void LateUpdate()
        {
            if (isFirstPerson && playerController != null && !playerController.IsGrounded && firstPersonSocket != null)
            {
                firstPerson.transform.position = firstPersonSocket.transform.position;
            }
        }
        public void ToggleCamera(InputValues inputValues) //Toggle camera called by input manager, input values is needed for type fulfilling but does nothing
        {
            this.isFirstPerson = !this.isFirstPerson;
            if (isFirstPerson)
            {
                crosshairManager.FirstP();
                firstPerson.gameObject.SetActive(true);
                thirdPerson.gameObject.SetActive(false);
            }
            else
            {
                crosshairManager.ThirdP();
                firstPerson.gameObject.SetActive(false);
                thirdPerson.gameObject.SetActive(true);
            }
        }
        private void Look(InputValues inputValues) // called by the InputManager at every triggered frame
        {
            if (lookingAllowed)
            {
                Vector2 look = inputValues.vector2Value;
                yRotation += look.x * ySensitivity * 0.01f; //horizontal
                xRotation -= look.y * xSensitivity * 0.01f; //vertical
                xRotation = Mathf.Clamp(xRotation, -85f, 85f); //avoid over vertical rotation by clamping
                transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); //transforms the player's rotation directly and camera follows as its a child of parent

                firstPerson.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //just tilt the first person camera
                if (!this.isFirstPerson)
                {
                    float dis = thirdPersonDistance;
                    if (Physics.SphereCast(firstPerson.transform.position, this.castRadius, -firstPerson.transform.forward, out RaycastHit hit, thirdPersonDistance, layerMask))
                    {
                        dis = hit.distance;
                    }

                    Vector3 direction = new Vector3(0f, math.sin(math.TORADIANS * -xRotation), math.cos(math.TORADIANS * -xRotation)).normalized;
                    thirdPerson.transform.localPosition = direction * -math.abs(dis) + firstPerson.transform.localPosition;
                    thirdPerson.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                }
            }
        }
    }
    [Serializable]
    public struct PlayerCameraSaveData
    {
        public int FOV;
        public bool isFirstPerson;
    }
}