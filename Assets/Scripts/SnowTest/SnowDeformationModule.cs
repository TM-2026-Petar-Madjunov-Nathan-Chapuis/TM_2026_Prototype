using UnityEngine;

public class SnowDeformationModule : MonoBehaviour
{
    [SerializeField] private Camera topCamera;
    [SerializeField] private int textureResolution = 1024;

    public RenderTexture playerDisplacement;

    void Start()
    {
        playerDisplacement = new RenderTexture(textureResolution, textureResolution, 24);
        playerDisplacement.Create();
        topCamera.targetTexture = playerDisplacement;
        GL.Clear(true, true, Color.black);
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