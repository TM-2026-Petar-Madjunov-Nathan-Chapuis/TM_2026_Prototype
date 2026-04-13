using System;
using Newtonsoft.Json;
using TM.Saving;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour, ISaveable
{
    public Camera firstPerson;
    public Camera thirdPerson;
    [SerializeField] private int FOV = 90;
    public bool isFirstPerson = true;
    public string UID => "cameraToggle";
    private InputAction lookAction;
    private InputAction toggleCamera;
    [SerializeField] private InputActionAsset inputActionAsset;

    [SerializeField] private float xRotation = 0f;
    [SerializeField] private float yRotation = 0f;

    [SerializeField] private float xSensitivity = 100f;
    [SerializeField] private float ySensitivity = 50f;
    [SerializeField] private float thirdPersonSmoothFactor;
    [SerializeField] private float yClamp;
    private GameObject thirdPersonCameraPos = null;
    private float thirdPersonBaseY;
    private float thirdPersonVerticalOffset;



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
    void Awake()
    {
        InputActionMap playerActionMap = inputActionAsset.FindActionMap("Player", true);
        lookAction = playerActionMap.FindAction("Look", true);
        toggleCamera = playerActionMap.FindAction("ToggleCamera", true);
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
    public void ToggleCamera()
    {
        this.isFirstPerson =  !this.isFirstPerson;
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
    void Update()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();
        if (toggleCamera.triggered)
        {
            ToggleCamera();
        }
        if (lookAction.triggered)
        {
            Look(look);
        }
    }
    void LateUpdate()
    {
        if (!isFirstPerson)
        {
            if (Vector3.Distance(thirdPerson.gameObject.transform.position, thirdPersonCameraPos.transform.position) > 0.1)
            {
                thirdPerson.transform.position = Vector3.Lerp(thirdPerson.gameObject.transform.position, thirdPersonCameraPos.transform.position, thirdPersonSmoothFactor * Time.deltaTime);
            }
            thirdPerson.gameObject.transform.LookAt(this.gameObject.transform);
        }
    }
    private void Look(Vector2 look)
    {
        if (isFirstPerson)
        {
            yRotation += look.x * ySensitivity * Time.deltaTime; //horizontal
            xRotation -= look.y * xSensitivity * Time.deltaTime; //vertical

            xRotation = Mathf.Clamp(xRotation, -89f, 89f); //avoid over vertical rotation by clamping

            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
            firstPerson.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);   
        }
        else
        {
            yRotation += look.x * ySensitivity * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

            thirdPersonVerticalOffset -= look.y * xSensitivity * Time.deltaTime;
            thirdPersonVerticalOffset = Mathf.Clamp(thirdPersonVerticalOffset, -yClamp, yClamp);

            Vector3 pos = thirdPersonCameraPos.transform.position;
            pos.y = thirdPersonBaseY + thirdPersonVerticalOffset + transform.position.y;
            thirdPersonCameraPos.transform.position = pos;
        }
    }
}
    [Serializable]
    public struct PlayerCameraSaveData
    {
        public int FOV;
        public bool isFirstPerson;
    }
