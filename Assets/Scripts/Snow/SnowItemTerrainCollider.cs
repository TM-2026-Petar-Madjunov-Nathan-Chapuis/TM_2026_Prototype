using UnityEngine;


//sits on the terrain and create a collider the height of the snow for items to consider.
[RequireComponent(typeof(MeshCollider))]
public class SnowTerrainCollider : MonoBehaviour
{
    [SerializeField] private Terrain currentTerrain;
    [SerializeField] private float verticalOffset = 0.5f;
    [SerializeField] private int resolution = 128;

    void Awake()
    {
        Mesh terrainMesh = CreateTerrainMesh(
            currentTerrain.terrainData,
            resolution
        );

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = terrainMesh;
    }

    //same as the function generate mesh for snowmanager, but sampling height from terraindata
    private Mesh CreateTerrainMesh(TerrainData terrainData, int resolution)
    {
        Mesh newMesh = new();
        resolution = Mathf.Max(2, resolution); //avoid to low res

        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)]; //vertices count is (resolution + 1)^2, where x is the subdivision count
        int[] triangles = new int[resolution * resolution * 2 * 3]; // triangles is resolution^2 * 2 * 3, because each triangle is 3 ints.
        int triangleCount = 0;

        //following the example at : https://docs.unity3d.com/6000.6/Documentation/ScriptReference/TerrainData.GetAlphamaps.html;
        float[,,] alphamaps = terrainData.GetAlphamaps(
            0,
            0,
            terrainData.alphamapWidth,
            terrainData.alphamapHeight
        );

        for (int y = 0; y < resolution + 1; y++)
        {
            for (int x = 0; x < resolution + 1; x++)
            {
                int index = y * (resolution + 1) + x;
                //runs for each square at x + 1 / y + 1

                //get height from height data
                float xcoord = (float)x / resolution;
                float zcoord = (float)y / resolution;
                float vertexHeight = terrainData.GetInterpolatedHeight(xcoord, zcoord) + this.verticalOffset;

                int alphaX = Mathf.Clamp(Mathf.RoundToInt(xcoord * (terrainData.alphamapWidth - 1)), 0, terrainData.alphamapWidth - 1);
                int alphaZ = Mathf.Clamp(Mathf.RoundToInt(zcoord * (terrainData.alphamapHeight - 1)), 0, terrainData.alphamapHeight - 1);

                vertexHeight *= alphamaps[alphaZ, alphaX, 0]; //multiplies by snow alpha.

                //creates the top left vertice of the square. gives the total grid of vertices because x+1/y+1
                vertices[index] = new Vector3( xcoord * terrainData.size.x, vertexHeight, zcoord * terrainData.size.z); //terrain space

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
        newMesh.name = "TerrainItemCollider";
        return newMesh;
    }
}