using System;
using Newtonsoft.Json;
using NUnit.Framework;
using TM.Input;
using TM.Saving;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
        [SerializeField] private float thirdPersonSmoothFactor;
        [SerializeField] private float yClamp;
        private GameObject thirdPersonCameraPos = null;
        private float thirdPersonBaseY;
        private float thirdPersonVerticalOffset;
        public bool lookingAllowed = true;


        public void LoadData(string data)
        {
            PlayerCameraSaveData saveData = JsonConvert.DeserializeObject<PlayerCameraSaveData>(data);
            this.FOV = saveData.FOV;
            this.isFirstPerson = saveData.isFirstPerson;
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
            InputManager.Instance.RegisterListener("ToggleCamera", ToggleCamera, InputValueType.Button, false);
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
            thirdPersonCameraPos = new GameObject("thirdPersonCameraPos");
            thirdPersonCameraPos.transform.position = thirdPerson.gameObject.transform.position;
            thirdPersonCameraPos.transform.SetParent(transform);

            thirdPersonBaseY = thirdPersonCameraPos.transform.position.y - transform.position.y;
        }
        void OnDrawGizmos()
        {
            if (thirdPersonCameraPos == null) return;
            Gizmos.DrawLine(transform.position, thirdPersonCameraPos.transform.position);
        }
        public void ToggleCamera(InputValues inputValues) //Toggle camera called by input manager, input values is needed for type fulfilling but does nothing
        {
            this.isFirstPerson = !this.isFirstPerson;
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
        }
        void LateUpdate()
        {
            if (!isFirstPerson) //logic for the lerping of the third camera position to smooth movements
            {
                if (Vector3.Distance(thirdPerson.gameObject.transform.position, thirdPersonCameraPos.transform.position) > 0.1)
                {
                    thirdPerson.transform.position = Vector3.Lerp(thirdPerson.gameObject.transform.position, thirdPersonCameraPos.transform.position, thirdPersonSmoothFactor * Time.deltaTime);
                }
                thirdPerson.gameObject.transform.LookAt(this.gameObject.transform);
            }
        }
        private void Look(InputValues inputValues) // called by the InputManager at every triggered frame
        {
            if (lookingAllowed)
            {
                Vector2 look = inputValues.vector2Value;
                if (isFirstPerson)
                {
                    yRotation += look.x * ySensitivity * 0.01f; //horizontal
                    xRotation -= look.y * xSensitivity * 0.01f; //vertical

                    xRotation = Mathf.Clamp(xRotation, -89f, 89f); //avoid over vertical rotation by clamping

                    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
                    firstPerson.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                }
                else
                {
                    yRotation += look.x * ySensitivity * Time.deltaTime; //horizontal
                    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); //transforms the player's rotation directly and camera follows as its a child of parent

                    thirdPersonVerticalOffset -= look.y * xSensitivity * Time.deltaTime; //vertical
                    thirdPersonVerticalOffset = Mathf.Clamp(thirdPersonVerticalOffset, -yClamp, yClamp); //clamp max vertical "rotation" or movement

                    Vector3 pos = thirdPersonCameraPos.transform.position;
                    pos.y = thirdPersonBaseY + thirdPersonVerticalOffset + transform.position.y; //calculate camera pos based on the offset
                    thirdPersonCameraPos.transform.position = pos; //set the third person camera target that the acutual camera will learp towards to the pos.
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