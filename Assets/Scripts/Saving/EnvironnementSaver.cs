using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TM.Saving;
using UnityEngine;
using UnityEngine.AI;

//the aim of this class is to save all the current gameobjects forming the environement, such as worlditems and trees. 
// Basically responsible of the saving of the world of the player
public class EnvironnementSaver : MonoBehaviour, ISaveable
{
    [SerializeField] private PrefabRegistry prefabRegistry;
    public string UID => "EnvironementSaver";

    public object SaveData()
    {
        List<EnvironnementObjectSaveData> saveDatas = new List<EnvironnementObjectSaveData>();

        foreach(Transform child in this.transform)
        {
            Debug.Log(child.gameObject.name);
            saveDatas.Add(SaveObject(child.gameObject));
        }

        return saveDatas;
    }

    public void LoadData(string data)
    {
        //destroy all current childrens
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (data == null) return;

        //load the saved childrens back
        List<EnvironnementObjectSaveData> saveDatas = JsonConvert.DeserializeObject<List<EnvironnementObjectSaveData>>(data);
        foreach(EnvironnementObjectSaveData saveData in saveDatas)
        {
            this.LoadObject(saveData, this.transform);
        }
    }
    public EnvironnementObjectSaveData SaveObject(GameObject gameObject)
    {
        EnvironnementObjectSaveData saveData = new EnvironnementObjectSaveData
        {
            positionX = gameObject.transform.position.x,
            positionY = gameObject.transform.position.y,
            positionZ = gameObject.transform.position.z,

            rotationX = gameObject.transform.eulerAngles.x,
            rotationY = gameObject.transform.eulerAngles.y,
            rotationZ = gameObject.transform.eulerAngles.z,

            scaleX = gameObject.transform.localScale.x,
            scaleY = gameObject.transform.localScale.y,
            scaleZ = gameObject.transform.localScale.z,

            name = gameObject.name, //assuming all environement childrens are the same naming as the prefabs, or with the (clone) added thing
        };
        return saveData;
    }
    public void LoadObject(EnvironnementObjectSaveData saveData, Transform parent)
    {
        GameObject prefab = prefabRegistry.GetPrefab(saveData.name);
        if (prefab == null)
        {
            Debug.LogError("Could not find prefab for saved environment object '" + saveData.name + "'.");
            return;
        }

        GameObject gameObject = GameObject.Instantiate(prefab, parent, false);
        Vector3 position = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        Vector3 scale = new Vector3(saveData.scaleX, saveData.scaleY, saveData.scaleZ);
        Quaternion rotation = Quaternion.Euler(saveData.rotationX, saveData.rotationY, saveData.rotationZ);

        gameObject.transform.position = position;
        gameObject.transform.localScale = scale;
        gameObject.transform.rotation = rotation;

        //if the object is a fox or another animal (navmeshagent), we need to snap it to the navmesh mesh
        NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null && NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position; //nearest navmesh position
            navMeshAgent.Warp(hit.position); //warp to it
            return;
        }

        gameObject.SetActive(true);
    }
}

[Serializable]
public struct EnvironnementObjectSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float scaleX;
    public float scaleY;
    public float scaleZ;

    public string name;
}