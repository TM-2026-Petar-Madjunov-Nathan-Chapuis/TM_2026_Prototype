using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.Misc
{
    public class BlurManager : MonoBehaviour //inspired by unity's manual documentation found at https://docs.unity.cn/6000.2/Documentation/Manual/urp/renderer-features/create-custom-renderer-feature.html
    {
        private static readonly int horizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
        private static readonly int verticalBlurId = Shader.PropertyToID("_VerticalBlur");
        private Material material;
        private RenderTexture temp; // after research cause : temp and result needed because writing and reading on/from the same file at the same times leads to errors, that's why we use different files for the passes
        private RenderTexture result;
        [SerializeField] private Shader blurShader;
        [Range(0f, 1f)]
        [SerializeField] private float horizontalBlur;
        [Range(0f, 1f)]
        [SerializeField] private float verticalBlur;
        void Awake()
        {
            material = new Material(blurShader);
        }
        public RenderTexture Blur(RenderTexture input)
        {
            UpdateRTs(input);

            material.SetFloat(horizontalBlurId, horizontalBlur);
            material.SetFloat(verticalBlurId, verticalBlur);

            material.SetTexture("_MainTex", input);

            Graphics.Blit(input, temp, material, 0);

            material.SetTexture("_MainTex", temp);

            Graphics.Blit(temp, result, material, 1);

            return result;
        }
        private void UpdateRTs(RenderTexture input)
        {
            if (temp == null || temp.height != input.height || temp.width != input.width)
            {
                this.temp?.Release();
                this.temp = new RenderTexture(input);
                this.temp.Create();
            }
            if (result == null || result.height != input.height || result.width != input.width)
            {
                this.result?.Release();
                this.result = new RenderTexture(input);
                this.result.Create();
            }
        }
    }
}
