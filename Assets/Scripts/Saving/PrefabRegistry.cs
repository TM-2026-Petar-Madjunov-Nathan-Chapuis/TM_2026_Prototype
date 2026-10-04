using System.Collections.Generic;
using UnityEngine;

//not ideal solution, but as we have a low number of prefabs, its fine.
//it assumes that all prefab have different naming, as it uses them as id.
public class PrefabRegistry : MonoBehaviour
{
    public List<GameObject> prefabs;
    private string cloneSubString = "(Clone)";

    public GameObject GetPrefab(string id)
    {
        if (id.Contains(cloneSubString))
        {
            id = id.Replace(cloneSubString, string.Empty).Trim(); //removes the substring if it exists
        }
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.name == id) return prefab;
        }

        return null;
    }
}