Shader "Unlit/BlobShadow"
{
    Properties
    {
        _ShadowColor("Shadow Color", Color) = (0,0,0,0.3)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry-1" "RenderPipeline"="UniversalPipeline" }
        Cull Back
        ZWrite Off
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BlobShadow"
            Tags { "LightMode"="UniversalForward" }

            Stencil
            {
                Ref 0
                Comp Equal
                Pass IncrSat
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _ShadowColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 Frag() : SV_Target
            {
                return _ShadowColor;
            }
            ENDHLSL
        }
    }
}
