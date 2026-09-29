using System.Collections.Generic;
using TM.Misc;
using UnityEngine;

//since the script of the falling tree requires gameobjects, and that unity's tree placing methods doesnt feature them, we must take all the tree in the terrain and
//convert them back to normal gameobject. this is not performant and optimized, but works. the other approach would involve much more complex raycasting and tree falling handling.
public class TreeConverterManager : MonoBehaviour
{
    private List<Terrain> terrains = new List<Terrain>();

    private void Start()
    {
        Terrain.GetActiveTerrains(terrains);
        foreach (Terrain terrain in terrains)
        {
            terrain.terrainData = TerrainDataCloner.Clone(terrain.terrainData);
            terrain.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;

            TerrainData terrainData = terrain.terrainData;
            TreePrototype[] treePrototypes = terrainData.treePrototypes;
            TreeInstance[] treeInstances = terrainData.treeInstances;
            SpawnTrees(terrain, terrainData, treePrototypes, treeInstances);

            terrainData.treeInstances = new TreeInstance[0];
            terrain.Flush();
        }
    }
    public void ClearAllTerrainTreeInstances()
    {
        List<Terrain> terrains = new List<Terrain>();

        Terrain.GetActiveTerrains(terrains);

        foreach(Terrain terrain in terrains)
        {
            terrain.terrainData.treeInstances = new List<TreeInstance>().ToArray();
            terrain.terrainData.SetTreeInstances(new TreeInstance[0], false);
            terrain.Flush();
        }
    }
    private void SpawnTrees(Terrain terrain, TerrainData terrainData, TreePrototype[] treePrototypes, TreeInstance[] treeInstances)
    {
        for (int i = 0; i < treeInstances.Length; i++)
        {
            TreeInstance treeInstance = treeInstances[i];

            GameObject treePrefab = treePrototypes[treeInstance.prototypeIndex].prefab;

            Vector3 localPosition = Vector3.Scale(treeInstance.position, terrainData.size); //turn the [0-1] into the size of the terrain
            Vector3 worldPosition = terrain.transform.TransformPoint(localPosition); //gets the world pos from the local

            GameObject tree = Instantiate(treePrefab, this.gameObject.transform); //initiate the tree under this gameobject
            tree.transform.SetPositionAndRotation(worldPosition, Quaternion.Euler(0, treeInstance.rotation, 0));

        }
    }
}