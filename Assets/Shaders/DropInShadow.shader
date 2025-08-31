Shader "Unlit/DropInShadow"
{
    Properties
    {
        _ShadowColor("Shadow Color", Color) = (0,0,0,0.4)
        _WorldOffset("World Offset (XY)", Vector) = (0.1, -0.1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off
        ZWrite Off
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
        float4 _ShadowColor;
        float4 _WorldOffset;
        CBUFFER_END

        struct Attributes { float4 positionOS : POSITION; };
        struct Varyings   { float4 positionCS : SV_POSITION; };

        Varyings Vert(Attributes IN)
        {
            Varyings OUT;
            float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
            posWS.xy += _WorldOffset.xy;
            OUT.positionCS = TransformWorldToHClip(posWS);
            return OUT;
        }

        half4 Frag() : SV_Target
        {
            return _ShadowColor;
        }
        ENDHLSL

        Pass
        {
            Name "Shadow"
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
            ENDHLSL
        }
    }
}