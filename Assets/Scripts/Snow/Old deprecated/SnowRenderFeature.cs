using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class SnowRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Material channelPacker;
    private SnowPackingRenderPass snowPackingRenderPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (channelPacker == null)
            return;
        renderer.EnqueuePass(snowPackingRenderPass);
    }

    public override void Create()
    {
        snowPackingRenderPass = new SnowPackingRenderPass(channelPacker);
    }
}
