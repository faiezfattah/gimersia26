Shader "Custom/DiamondPattern"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _BgColor ("Background Color", Color) = (0.62, 0.88, 0.76, 1)
        _ColorA ("Diamond Color A", Color) = (0.75, 0.95, 0.85, 1)
        _ColorB ("Diamond Color B", Color) = (0.52, 0.80, 0.68, 1)
        _TileSize ("Tile Size (World Units)", Float) = 1.0
        _OuterSize ("Outer Radius", Range(0.1, 0.5)) = 0.34
        _InnerSize ("Inner Radius", Range(0.0, 0.45)) = 0.20
        _MinAlpha ("Min Opacity", Range(0, 1)) = 0.15
        _MaxAlpha ("Max Opacity", Range(0, 1)) = 0.65
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4  color      : COLOR;
                float2 uv         : TEXCOORD0;
                half2  lightingUV : TEXCOORD1;
                float2 worldPos   : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _BgColor, _ColorA, _ColorB;
                float _TileSize, _OuterSize, _InnerSize, _MinAlpha, _MaxAlpha;
            CBUFFER_END

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.worldPos = TransformObjectToWorld(v.positionOS).xy;
                o.uv = v.uv;
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
                o.color = v.color;
                return o;
            }

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                half texAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                // Procedural diamond pattern
                float2 uv = i.worldPos / max(_TileSize, 0.01);
                float2 rotUV = float2(uv.x - uv.y, uv.x + uv.y) * 0.70710678;

                float2 id = floor(rotUV);
                float2 local = frac(rotUV) - 0.5;

                float d = max(abs(local.x), abs(local.y));
                float aa = fwidth(d);
                float outer = 1.0 - smoothstep(_OuterSize - aa, _OuterSize + aa, d);
                float inner = smoothstep(_InnerSize - aa, _InnerSize + aa, d);
                float shape = outer * inner;

                float rndColor = hash21(id);
                float rndAlpha = hash21(id + 19.37);

                half3 diamondCol = lerp(_ColorA.rgb, _ColorB.rgb, rndColor);
                half alpha = lerp(_MinAlpha, _MaxAlpha, rndAlpha);
                half3 finalCol = lerp(_BgColor.rgb, diamondCol, shape * alpha) * i.color.rgb;

                // Feed into URP 2D lighting & shadows
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(finalCol, texAlpha * i.color.a, mask, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);

                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }
    }
}