Shader "Solutena/Sprite/Blur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DestBlend", Float) = 10
        
        _Blur ("Blur", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend [_SrcBlend] [_DstBlend]

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment Frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            
            float4 _MainTex_TexelSize;
            float _Blur;

            fixed4 Frag(v2f IN) : SV_Target
            {
                if(_Blur <= 0)
                    return SampleSpriteTexture(IN.texcoord);

                fixed4 color = 0;
                float sum = 0;

                float kernelSize = ceil(_Blur * 3.0) * 2.0 + 1.0;
                float halfKernelSize = floor(kernelSize * 0.5);

                float sigma = 2.0 * _Blur * _Blur;
                for (int x = -halfKernelSize; x <= halfKernelSize; x++)
                {
                    for (int y = -halfKernelSize; y <= halfKernelSize; y++)
                    {
                        float weight = min(1,exp( -(x * x + y * y) / sigma) / (3.14159265358979323846 * sigma));
                        sum += weight;
                        color += SampleSpriteTexture(IN.texcoord + float2(x, y) * _MainTex_TexelSize) * weight;
                    }
                }

                return color / max(1,sum);
            }
        ENDCG
        }
    }
}
