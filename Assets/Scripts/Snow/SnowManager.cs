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
    [SerializeField] Material packerMaterial;
    [SerializeField] Material triplanarMaterial;
    [SerializeField] Material terrainMaterial;
    [SerializeField] int playerParticleEmmissionCount;
    [SerializeField] int animalParticleEmmissionCount;
    private List<Terrain> terrains = new List<Terrain>();
    private Vector3 terrainSize; // The total size in world units of the terrain: width, height, and length. (unity docs)
    private Dictionary<Vector2Int, GameObject> chunks;
    private Mesh chunkMesh;
    private RenderTexture snowDisplacement;
    private Material mat1;
    private Material mat2;
    private Material mat3;
    private RenderTexture[] terrainTextures = new RenderTexture[4];
    private RenderTexture[] triplanarTerrainTextures = new RenderTexture[4];
    private float terrainChunkSize;
    private Dictionary<Terrain, int> textureInfos;
    private Dictionary<GameObject, Terrain> chunkTerrains = new();

    void OnDisable()
    {
        terrainMaterial.SetInt("_ShowSnow", 1); //show snow again
        foreach (Terrain terrain in terrains)
        {
            terrain.materialTemplate.SetInteger("_ShowSnow", 1);
        }
    }

    void Start()
    {
        //particles emission scripts
        UnityEngine.Object[] animals = FindObjectsByType<AnimalSnowParticles>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (UnityEngine.Object emiter in animals)
        {
            (emiter as AnimalSnowParticles).particleEmitedCount = animalParticleEmmissionCount;
        }
        FindAnyObjectByType<PlayerSnowParticles>().particleEmitedCount = playerParticleEmmissionCount;

        //hides the snow layer, so that it saves perf
        terrainMaterial.SetInt("_ShowSnow", 0);

        terrainSize = mainTerrain.terrainData.size;
        Terrain.GetActiveTerrains(terrains);
        foreach (Terrain terrain in terrains)
        {
            terrain.materialTemplate = new Material(terrain.materialTemplate);
            terrain.materialTemplate.SetInt("_ShowSnow", 0);
        }
        this.terrainChunkSize = mainTerrain.terrainData.size.x;//assumes all terrain are the same size and squares, which they are
        this.playerTopDownCamera.orthographicSize = this.snowRenderDistance;
        this.snowDisplacement = new RenderTexture(snowTextureResolution.x, snowTextureResolution.y, 0);
        this.snowDisplacement.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
        this.snowDisplacement.Create();
        this.playerTopDownCamera.targetTexture = this.snowDisplacement;

        for (int i = 0; i < 4; i++) //HEIGHTMAP TEXTURE AND CONTROL TEXTURE MUST BE THE SAME SIZE
        {
            terrainTextures[i] = new RenderTexture(
                mainTerrain.terrainData.heightmapTexture.width,
                mainTerrain.terrainData.heightmapTexture.height,
                0,
                RenderTextureFormat.ARGBFloat
            );

            terrainTextures[i].useMipMap = true;
            terrainTextures[i].autoGenerateMips = true;
            terrainTextures[i].filterMode = FilterMode.Bilinear;
            terrainTextures[i].wrapMode = TextureWrapMode.Clamp;
            terrainTextures[i].Create();

            triplanarTerrainTextures[i] = new RenderTexture(
                mainTerrain.terrainData.heightmapTexture.width,
                mainTerrain.terrainData.heightmapTexture.height,
                0,
                RenderTextureFormat.ARGBFloat
            );
            triplanarTerrainTextures[i].filterMode = FilterMode.Bilinear;
            triplanarTerrainTextures[i].wrapMode = TextureWrapMode.Clamp;
            triplanarTerrainTextures[i].Create();
        }

        //materials propreties
        this.snowMaterial.SetTexture("_BaseMap", this.snowDisplacement);
        this.snowMaterial.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
        this.snowMaterial.SetVector("_OrthographicCameraSize", new Vector2(playerTopDownCamera.orthographicSize, playerTopDownCamera.orthographicSize));
        this.snowMaterial.SetFloat("_HeightMapMaxHeight", this.mainTerrain.terrainData.heightmapScale.y - 0.5f);
        this.snowMaterial.SetVector("_TerrainSize", new Vector2(this.terrainSize.x / 2, this.terrainSize.z / 2)); //needs to be divided by two for some reason.
        this.triplanarMaterial.SetVector("_TerrainSize", new Vector4(
            this.terrainSize.x,
            this.mainTerrain.terrainData.heightmapScale.y,
            this.terrainSize.z,
            0f
        ));
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
        this.mat1.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
        this.mat2.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
        this.mat3.SetVector("_OrthographicCameraPos", playerTopDownCamera.transform.position);
        QuadrantCheck();
    }

    private void QuadrantCheck()
    {
        Terrain[] closestTerrains = GetClosestTerrains();
        bool terrainSetChanged = this.textureInfos == null || !new HashSet<Terrain>(closestTerrains).SetEquals(this.textureInfos.Keys); //hashset.setequals checks if there is the same elements in both values regardless of order

        if (terrainSetChanged)
        {
            this.textureInfos = SetRenderTextures(closestTerrains);
            UpdateChunkMaterials();
        }
    }

    private Terrain[] GetClosestTerrains() //gets the 4 closest terrains
    {
        Vector3 playerPosition = playerTopDownCamera.transform.position; //camera is the same postion as the player just above

        return terrains.OrderBy(terrain => //orders by distance
            {
                Vector3 terrainPosition = terrain.transform.position;
                Vector3 size = terrain.terrainData.size;

                float closestX = Mathf.Clamp(playerPosition.x, terrainPosition.x, terrainPosition.x + size.x); //clamps player position x and z to terrain bounds so that it takes the closest point on the terrain
                float closestZ = Mathf.Clamp(playerPosition.z, terrainPosition.z, terrainPosition.z + size.z);

                return (new Vector2(closestX, closestZ) - new Vector2(playerPosition.x, playerPosition.z)).sqrMagnitude; //basically closest point on the terrain - player pos
            }).Take(4).ToArray(); //take the four closest
    }

    private Dictionary<Terrain, int> SetRenderTextures(Terrain[] closestTerrains)
    {
        Dictionary<Terrain, int> result = new();
        for (int i = 0; i < closestTerrains.Length; i++)
        {
            SetRenderTexture(closestTerrains[i], i);
            Vector3 terrainPos = terrainCenter(terrainChunkSize, closestTerrains[i].transform.position);
            switch (i)
            {
                case 0:
                    this.snowMaterial.SetVector("_TerrainPos", terrainPos);
                    break;
                case 1:
                    this.mat1.SetVector("_TerrainPos", terrainPos);
                    break;
                case 2:
                    this.mat2.SetVector("_TerrainPos", terrainPos);
                    break;
                case 3:
                    this.mat3.SetVector("_TerrainPos", terrainPos);
                    break;
                default: throw new ArgumentException();
            }
            result.Add(closestTerrains[i], i);
        }
        return result;
    }
    private void SetRenderTexture(Terrain terrain, int slot)
    {
        RenderTexture heightmap = terrain.terrainData.heightmapTexture;

        Graphics.Blit(heightmap, triplanarTerrainTextures[slot], triplanarMaterial);
        terrain.materialTemplate.SetTexture("_TriplanarLayer", triplanarTerrainTextures[slot]);

        packerMaterial.SetTexture("_LayerMask", triplanarTerrainTextures[slot]);
        Graphics.Blit(heightmap, terrainTextures[slot], packerMaterial);
    }

    private Vector3 terrainCenter(float chunkSize, Vector3 position)
    {
        return new Vector3(position.x + chunkSize / 2f, 0f, position.z + chunkSize / 2f);
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
                    chunk.GetComponent<Renderer>().localBounds = new Bounds( //these are the bounds for culling. important to save performance.
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
                    newChunks[new Vector2Int(globalX, globalY)] = chunk;
                    this.chunkTerrains[chunk] = terrain;
                }
            }
        }
        return newChunks;
    }
    private void UpdateChunkMaterials()
    {
        foreach (GameObject chunk in chunks.Values)
        {
            MeshRenderer renderer = chunk.GetComponent<MeshRenderer>();

             //if the current terrain isnt in the four closest just stop rendering
            if (!textureInfos.TryGetValue(chunkTerrains[chunk], out int index))
            {
                renderer.enabled = false;
                continue;
            }

            //set the new mat corresponding to the terrain its in to
            renderer.enabled = true;
            renderer.sharedMaterial = index switch
            {
                0 => snowMaterial,
                1 => mat1,
                2 => mat2,
                3 => mat3,
                _ => throw new ArgumentException()
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
