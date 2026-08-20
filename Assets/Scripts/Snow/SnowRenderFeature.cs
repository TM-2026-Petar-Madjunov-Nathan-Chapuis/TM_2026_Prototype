using UnityEngine;
using UnityEngine.Rendering.Universal;


public class SnowRenderFeature : ScriptableRendererFeature
{
    private SnowPackingRenderPass snowPackingRenderPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(snowPackingRenderPass);
    }

    public override void Create()
    {
        snowPackingRenderPass = new SnowPackingRenderPass();
    }
}
