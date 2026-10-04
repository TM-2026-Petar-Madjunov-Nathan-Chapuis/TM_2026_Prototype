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
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        //we must remove the (clone), and (1, 2, 3, ...) suffixes to get the prefab name to match the gameobject name
        id = id.Replace(cloneSubString, string.Empty).Trim(); //removes the (clone)
        int suffixStart = id.LastIndexOf(" ("); //if there is whitespace( and a ) at the end
        if (suffixStart >= 0 && id.EndsWith(")"))
        {
            id = id.Substring(0, suffixStart); //cut it out, takign the part before.
        }

        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null && prefab.name == id) return prefab;
        }

        return null;
    }
}