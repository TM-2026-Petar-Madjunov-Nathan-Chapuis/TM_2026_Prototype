using UnityEngine;

public class snowtesselationquickscript : MonoBehaviour
{
    [SerializeField] private Camera camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Renderer>().material.SetTexture("_BaseMap", camera.activeTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
