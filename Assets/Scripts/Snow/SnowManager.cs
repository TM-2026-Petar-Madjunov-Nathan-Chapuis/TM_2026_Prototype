using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TM.Player;
using Unity.Mathematics;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class SnowTerrain : MonoBehaviour
{
    [SerializeField] float chunkSize; // size of a chunk, used to calculate the chunk count and well responsible for the size
    [SerializeField] int subdivision; // number of division to the plane.
    [Tooltip("MUST NOT EXCEED TERRAIN'S CHUNK SIZE, BECAUSE IT WOULD NEED TOO MANY TERRAIN HEIGHTMAPS TO FUNCTION.")]
    [SerializeField] float snowRenderDistance;
    [SerializeField] Vector2Int snowTextureResolution;
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] Terrain mainTerrain;
    [SerializeField] Camera playerTopDownCamera;
    [SerializeField] Material snowMaterial;
    [SerializeField] Material channelPackerMaterial;
    [SerializeField] Material packerMaterial;
    private List<Terrain> terrains = new List<Terrain>();
    private Vector3 terrainSize; // The total size in world units of the terrain: width, height, and length. (unity docs)
    private Dictionary<Vector2Int, GameObject> chunks;
    private Mesh chunkMesh;
    private RenderTexture snowDisplacement;
    private Material mat1;
    private Material mat2;
    private Material mat3;
    private RenderTexture[] terrainTextures = new RenderTexture[4];
    private int currentTerrainQuadrant = 0; // stores the current terrain quadrants the camera is in, 1 top right, 2 top left, 3 bottom left, 4 bottom right
    private float terrainChunkSize;
    private Dictionary<Vector2, int> textureInfos; //the key vector2 is the materialpreoprtyblock's terrain center. contains at max 4 pair of key-value

    void Start()
    {
        terrainSize = mainTerrain.terrainData.size;
        Terrain.GetActiveTerrains(terrains);
        this.terrainChunkSize = mainTerrain.terrainData.size.x;//assumes all terrain are the same size and squares, which they are
        this.playerTopDownCamera.orthographicSize = this.snowRenderDistance;
        this.snowDisplacement = new RenderTexture(snowTextureResolution.x, snowTextureResolution.y, 0);
        this.snowDisplacement.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
        this.snowDisplacement.Create();
        this.playerTopDownCamera.targetTexture = this.snowDisplacement;

        for (int i = 0; i < 4; i++) //HEIGHTMAP TEXTURE AND CONTROL TEXTURE MUST BE THE SAME
        {
            terrainTextures[i] = new RenderTexture(
                mainTerrain.terrainData.heightmapTexture.width,
                mainTerrain.terrainData.heightmapTexture.height,
                0,
                RenderTextureFormat.ARGBFloat
            );

            terrainTextures[i].filterMode = FilterMode.Bilinear;
            terrainTextures[i].wrapMode = TextureWrapMode.Clamp;
            terrainTextures[i].Create();
        }

        //materials propreties
        this.channelPackerMaterial.SetTexture("_HeightMap", this.mainTerrain.terrainData.heightmapTexture);
        this.channelPackerMaterial.SetTexture("_LayerMask", this.mainTerrain.terrainData.terrainLayers[0].maskMapTexture);
        this.snowMaterial.SetTexture("_BaseMap", this.snowDisplacement);
        this.snowMaterial.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
        this.snowMaterial.SetVector("_OrthographicCameraSize", new Vector2(playerTopDownCamera.orthographicSize, playerTopDownCamera.orthographicSize));
        this.snowMaterial.SetFloat("_HeightMapMaxHeight", this.mainTerrain.terrainData.heightmapScale.y);
        this.mat1 = new Material(snowMaterial.shader);
        this.mat2 = new Material(snowMaterial.shader);
        this.mat3 = new Material(snowMaterial.shader);
        this.mat1.CopyMatchingPropertiesFromMaterial(snowMaterial);
        this.mat2.CopyMatchingPropertiesFromMaterial(snowMaterial);
        this.mat3.CopyMatchingPropertiesFromMaterial(snowMaterial);
        this.snowMaterial.SetTexture("_TerrainTexture", this.terrainTextures[0]);
        this.mat1.SetTexture("_TerrainTexture", this.terrainTextures[1]);
        this.mat2.SetTexture("_TerrainTexture", this.terrainTextures[2]);
        this.mat3.SetTexture("_TerrainTexture", this.terrainTextures[3]);

        this.chunkMesh = SubdividedMesh();
        this.chunks = CreateChunks();
        QuadrantCheck();
        

    }
    void Update()
    {
        this.snowMaterial.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);     
        QuadrantCheck();
    }

    private void QuadrantCheck()
    {
        Vector2 terrainOffset = new Vector2(Mathf.Repeat(playerTopDownCamera.transform.position.x, terrainChunkSize) - terrainChunkSize/2, 
                                            Mathf.Repeat(playerTopDownCamera.transform.position.z, terrainChunkSize) - terrainChunkSize/2);   
        int quadrant = terrainOffset switch
        {
            { x: >= 0, y: >= 0 } => 1,
            { x: < 0, y: >= 0 } => 2,
            { x: < 0, y: < 0 } => 3,
            { x: >= 0, y: < 0 } => 4,
            _ => throw new ArgumentException(),
        };
        if (quadrant != this.currentTerrainQuadrant)
        {
            this.currentTerrainQuadrant = quadrant;
            this.textureInfos = SetRenderTextures();
            UpdateChunkMaterials();
        }
    }

    private Dictionary<Vector2, int> SetRenderTextures()
    { 
        Vector2Int currentTerrainIndex = new (Mathf.FloorToInt(playerTopDownCamera.transform.position.x / terrainChunkSize), Mathf.FloorToInt(playerTopDownCamera.transform.position.z / terrainChunkSize));
        Vector2 position = currentTerrainQuadrant switch
        {  //gets the corner position of the currentTerrain the camera is in to get the four closest terrains we want and get no trouble (-1 because closest still the terrain we are in)
            1 => new Vector2(currentTerrainIndex.x * terrainChunkSize + terrainChunkSize/2 - 1, currentTerrainIndex.y * terrainChunkSize + terrainChunkSize/2 - 1), //top right
            2 => new Vector2(currentTerrainIndex.x * terrainChunkSize - terrainChunkSize/2 + 1, currentTerrainIndex.y * terrainChunkSize + terrainChunkSize/2 - 1), //top left
            3 => new Vector2(currentTerrainIndex.x * terrainChunkSize - terrainChunkSize/2 + 1, currentTerrainIndex.y * terrainChunkSize - terrainChunkSize/2 + 1), //bottom left
            4 => new Vector2(currentTerrainIndex.x * terrainChunkSize + terrainChunkSize/2 - 1, currentTerrainIndex.y * terrainChunkSize - terrainChunkSize/2 + 1), //bottom right
            _ => throw new ArgumentOutOfRangeException()
        }; 
        //takes the four closest terrain that will be used in rendering.
        Terrain[] closestTerrains = terrains.OrderBy(terrain => (terrain.transform.position - playerTopDownCamera.transform.position).sqrMagnitude).Take(4).ToArray();
        Dictionary<Vector2, int> result = new();
        for (int i = 0; i < closestTerrains.Length; i++)
        {
            SetRenderTexture(closestTerrains[i], i);
            result.Add(terrainCenter(terrainChunkSize, closestTerrains[i].transform.position), i);
        }
        return result;
    }
    private void SetRenderTexture(Terrain terrain, int slot)
    {
        RenderTexture heightmap = terrain.terrainData.heightmapTexture;
        Texture2D layerMask = terrain.terrainData.GetAlphamapTexture(0);

        channelPackerMaterial.SetTexture("_HeightMap", heightmap);
        channelPackerMaterial.SetTexture("_LayerMask", layerMask);

        packerMaterial.SetTexture("_LayerMask", layerMask);
        Graphics.Blit(heightmap, terrainTextures[slot], packerMaterial);
        this.snowMaterial.SetTexture("_TerrainTexture", this.terrainTextures[0]);
        this.mat1.SetTexture("_TerrainTexture", this.terrainTextures[1]);
        this.mat2.SetTexture("_TerrainTexture", this.terrainTextures[2]);
        this.mat3.SetTexture("_TerrainTexture", this.terrainTextures[3]);
    }

    private Vector2 terrainCenter(float chunkSize, Vector3 position)
    {
        return new Vector2(position.x + chunkSize/2, position.z + chunkSize/2);
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
                    chunk.GetComponent<Renderer>().sharedMaterial = snowMaterial;
                    chunk.GetComponent<Renderer>().localBounds = new Bounds(
                        new Vector3(
                            chunkSize / 2f,
                            mainTerrain.terrainData.heightmapScale.y / 2f,
                            chunkSize / 2f
                        ),
                        new Vector3(
                            chunkSize,
                            mainTerrain.terrainData.heightmapScale.y,
                            chunkSize
                        )
                    );
                    newChunks[new Vector2Int(globalX,globalY)] = chunk;
                }
            }
        }
        return newChunks;
    }
    private void UpdateChunkMaterials()
    {
        foreach (GameObject chunk in chunks.Values)
        {
            Vector2 chunkPosition = new(
                chunk.transform.position.x,
                chunk.transform.position.z
            );

            int index = -1;
            float closestDistance = float.MaxValue;

            foreach (var pair in textureInfos)
            {
                float dis = (pair.Key - chunkPosition).sqrMagnitude;
                if (dis < closestDistance)
                {
                    closestDistance = dis;
                    index = pair.Value;
                }
            }

            chunk.GetComponent<MeshRenderer>().material = index switch
            {
                0 => this.snowMaterial,
                1 => this.mat1,
                2 => this.mat2,
                3 => this.mat3,
                _ => throw new ArgumentException(),
            };
        }
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
