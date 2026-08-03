using UnityEngine;

public class SnowDeformationModule : MonoBehaviour
{
    [SerializeField] private Camera topCamera;
    [SerializeField] private int textureResolution = 1024;
    [SerializeField] private GameObject snowGameobject;
    private Material snowMaterial;

    public RenderTexture playerDisplacement;

    void Awake()
    {
        playerDisplacement = new RenderTexture(textureResolution, textureResolution, 24);
        playerDisplacement.Create();
        topCamera.targetTexture = playerDisplacement;
        GL.Clear(true, true, Color.black);
        this.snowMaterial = snowGameobject.GetComponent<Renderer>().material;
        this.snowMaterial.SetTexture("_BaseMap", playerDisplacement);
        this.snowMaterial.SetVector("_OrthographicCameraPos", topCamera.transform.position);
        this.snowMaterial.SetVector("_OrthographicCameraSize", new Vector2(topCamera.orthographicSize, topCamera.orthographicSize));
    }

    void OnDestroy()
    {
        if (playerDisplacement != null)
        {
            playerDisplacement.Release();
            Destroy(playerDisplacement);
        }
    }
}