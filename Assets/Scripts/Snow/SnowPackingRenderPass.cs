using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using static UnityEngine.Rendering.RenderGraphModule.Util.RenderGraphUtils;

public class SnowPackingRenderPass : ScriptableRenderPass
{
    private Material material;
    public SnowPackingRenderPass(Material material)
    {
        this.material = material;
    }
    private SnowPackingRenderPass pass;
    private RTHandle packedTexture;
    public RTHandle PackedTexture => packedTexture;

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
    {
        UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameContext.Get<UniversalCameraData>();

        TextureHandle sourceTexture = resourceData.activeColorTexture; //camera current texture

        RenderTextureDescriptor packedTextureDescriptor = cameraData.cameraTargetDescriptor; //create destination texture with same size. this is a descriptor and texture created below
        packedTextureDescriptor.depthBufferBits = 0; //force color only
        packedTextureDescriptor.msaaSamples = 1;
        RenderingUtils.ReAllocateHandleIfNeeded(
            ref packedTexture,
            packedTextureDescriptor,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "Packed Camera Texture"
        );
        TextureHandle destinationTexture = renderGraph.ImportTexture(packedTexture);//actually create it

        BlitMaterialParameters blitParams = new RenderGraphUtils.BlitMaterialParameters(sourceTexture, destinationTexture, material, 0);
        renderGraph.AddBlitPass(blitParams, "ChannelPacking pass");
    }
    public void Dispose()
    {
        packedTexture?.Release();
        packedTexture = null;
    }
}