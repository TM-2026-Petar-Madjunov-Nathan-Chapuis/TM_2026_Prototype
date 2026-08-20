using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using static UnityEngine.Rendering.RenderGraphModule.Util.RenderGraphUtils;

public class SnowPackingRenderPass : ScriptableRenderPass
{
    [SerializeField]
    private Material channelPackerMaterial;
    private SnowPackingRenderPass pass;
    private RTHandle packedTexture;
    public RTHandle PackedTexture => packedTexture;

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
    {
        UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();

        TextureHandle sourceTexture = resourceData.activeColorTexture; //camera current texture

        TextureDesc destinationDesc = renderGraph.GetTextureDesc(sourceTexture); //create destination texture with same size. this is a descriptor and texture created below
        destinationDesc.name = "Packed Camera Texture";
        destinationDesc.depthBufferBits = 0; //force color only
        TextureHandle destinationTexture = renderGraph.CreateTexture(destinationDesc);//actually create it

        BlitMaterialParameters blitParams = new RenderGraphUtils.BlitMaterialParameters(sourceTexture, destinationTexture, channelPackerMaterial, 0);
        renderGraph.AddBlitPass(blitParams, "ChannelPacking pass");
    }
}