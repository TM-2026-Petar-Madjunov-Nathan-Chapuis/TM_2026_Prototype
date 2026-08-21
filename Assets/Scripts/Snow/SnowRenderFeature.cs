using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class SnowRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Material channelPacker;
    public RTHandle packedTexture => snowPackingRenderPass?.PackedTexture;
    private SnowPackingRenderPass snowPackingRenderPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(snowPackingRenderPass);
    }

    public override void Create()
    {
        snowPackingRenderPass = new SnowPackingRenderPass(channelPacker);
    }
    protected override void Dispose(bool disposing)
    {
        snowPackingRenderPass?.Dispose();
    }
}
