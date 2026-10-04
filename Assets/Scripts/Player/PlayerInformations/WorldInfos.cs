using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


public class WorldInfos : MonoBehaviour
{
    public float ambientTemperature {get; private set;} = 0f;
    public List<GameObject> environnementObjects {get; private set;} = new List<GameObject>();
    public GameObject environnementObjectsFolder;

    void Awake()
    {
            
    }
    void Update()
    {
        GetAllEnvironnementObjects();
    }

    void GetAllEnvironnementObjects()
    {
        environnementObjects.Clear();

        environnementObjectsFolder = GameObject.Find("ENVIRONNEMENTS OBJECTS");

        foreach (Transform transform in environnementObjectsFolder.GetComponentInChildren<Transform>(true))
        {
            environnementObjects.Add(transform.gameObject);
        }
    }
}

