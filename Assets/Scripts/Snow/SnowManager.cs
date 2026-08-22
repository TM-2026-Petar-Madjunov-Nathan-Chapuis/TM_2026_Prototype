using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class SnowTerrain : MonoBehaviour
{
    [SerializeField] float chunkSize; // size of a chunk, used to calculate the chunk count and well responsible for the size
    [SerializeField] int subdivision; // number of division to the plane.
    [SerializeField] float snowRenderDistance;
    [SerializeField] Vector2Int snowTextureResolution;
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] Terrain mainTerrain;
    [SerializeField] Camera playerTopDownCamera;
    [SerializeField] Material snowMaterial;
    [SerializeField] Material channelPackerMaterial;
    private List<Terrain> terrains = new List<Terrain>();
    private Vector3 terrainSize; // The total size in world units of the terrain: width, height, and length. (unity docs)
    private Dictionary<Vector2Int, GameObject> chunks;
    private Mesh chunkMesh;
    private RenderTexture snowDisplacement;


    void Start()
    {
        terrainSize = mainTerrain.terrainData.size;
        Terrain.GetActiveTerrains(terrains);
        this.chunkMesh = SubdividedMesh();
        this.chunks = CreateChunks();
        this.playerTopDownCamera.orthographicSize = this.snowRenderDistance;
        this.snowDisplacement = new RenderTexture(snowTextureResolution.x, snowTextureResolution.y, 0);
        this.snowDisplacement.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
        this.snowDisplacement.Create();
        this.playerTopDownCamera.targetTexture = this.snowDisplacement;
        this.channelPackerMaterial.SetTexture("_HeightMap", this.mainTerrain.terrainData.heightmapTexture);
        this.channelPackerMaterial.SetTexture("_LayerMask", this.mainTerrain.terrainData.terrainLayers[0].maskMapTexture);
        this.snowMaterial.SetTexture("_BaseMap", this.snowDisplacement);
        this.snowMaterial.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
        this.snowMaterial.SetVector("_OrthographicCameraSize", new Vector2(playerTopDownCamera.orthographicSize, playerTopDownCamera.orthographicSize));
    }
    void Update()
    {
        this.snowMaterial.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
    }

    private Dictionary<Vector2Int, GameObject> CreateChunks()
    {
        //assumes the terrain is square
        int chunkCount = Mathf.CeilToInt(terrainSize[0] / chunkSize);
        Dictionary<Vector2Int, GameObject> newChunks = new Dictionary<Vector2Int, GameObject>();
        Debug.Log(chunkCount);
        foreach (Terrain terrain in terrains)
        {
            Vector3 terrainPos = terrain.transform.position;
            
            for (int x = 0; x < chunkCount; x++)
            {
                for (int y = 0; y < chunkCount; y++)
                {
                    Vector3 position = new Vector3(terrainPos.x + x * chunkSize, terrainPos.y, terrainPos.z + y * chunkSize);

                    int globalX = Mathf.FloorToInt(position.x / chunkSize);//this is responsible for the global indexing. 
                    int globalY = Mathf.FloorToInt(position.z / chunkSize);//each postion in the world is given a grid tiling, found like this.

                    GameObject chunk = GameObject.Instantiate(chunkPrefab, position, Quaternion.identity, this.gameObject.transform);
                    chunk.GetComponent<MeshFilter>().mesh = this.chunkMesh;
                    chunk.gameObject.transform.localScale = new Vector3(chunkSize, 1, chunkSize);
                    chunk.name = $"{globalX}:{globalY}";
                    newChunks[new Vector2Int(globalX,globalY)] = chunk;
                }
            }
        }
        return newChunks;
    }

    //this function subdivide a plane mesh into more vertices.
    private Mesh SubdividedMesh()
    {
        Mesh newMesh = new();
        int resolution = (int)Mathf.Pow(2, subdivision);//resolution based on subdivions is 2^x, where x is subdivisions
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)]; //vertices count is (resolution + 1)^2, where x is the subdivision count
        int[] triangles = new int[resolution * resolution * 2 * 3]; // triangles is resolution^2 * 2 * 3, because each triangle is 3 ints.
        int triangleCount = 0;

        for (int y = 0; y < resolution + 1; y++)
        {
            for (int x = 0; x < resolution + 1; x++)
            {
                int index = y * (resolution + 1) + x;
                //runs for each square at x + 1 / y + 1

                //creates the top left vertice of the square. gives the total grid of vertices because x+1/y+1
                vertices[index] = new Vector3((float)x / resolution, 0, (float)y / resolution);//gives 0 for x=0 and 1 for x=resolution
                if (x < resolution && y < resolution)//avoid the extra square layer
                {
                    //abcd assuming counterclockwise triangle square summit naming
                    //first triangle
                    triangles[triangleCount++] = index; //                    A
                    triangles[triangleCount++] = index + resolution + 1;//    B
                    triangles[triangleCount++] = index + 1;//                 D
                    //second triangle
                    triangles[triangleCount++] = index + resolution + 1;//    C
                    triangles[triangleCount++] = index + resolution + 1 + 1;//B
                    triangles[triangleCount++] = index + 1;//                 D
                }
            }
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }
        newMesh.normals = normals;
        newMesh.name = "SnowChunk";
        return newMesh;
    }
}