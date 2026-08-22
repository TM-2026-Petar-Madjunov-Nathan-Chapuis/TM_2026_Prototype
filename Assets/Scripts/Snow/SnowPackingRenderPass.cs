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
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
    {
        UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameContext.Get<UniversalCameraData>();

        TextureHandle sourceTexture = resourceData.activeColorTexture; //camera current texture

        TextureDesc desc = resourceData.cameraDepthTexture.GetDescriptor(renderGraph);
        desc.depthBufferBits = 0;
        TextureHandle destination = renderGraph.CreateTexture(desc);

        //packs the texture into destination
        BlitMaterialParameters blitParams = new RenderGraphUtils.BlitMaterialParameters(sourceTexture, destination, material, 0);
        renderGraph.AddBlitPass(blitParams, "ChannelPacking pass");
        
        //takes the destination texture then outputs it into the camera
        BlitMaterialParameters copyBackParams = new RenderGraphUtils.BlitMaterialParameters(
            destination,
            sourceTexture,
            Blitter.GetBlitMaterial(TextureDimension.Tex2D),
            0
        );

        renderGraph.AddBlitPass( copyBackParams,"Copy Packed Texture To Camera");
    }
}