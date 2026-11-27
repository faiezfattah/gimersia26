Shader "Custom/SpriteProceduralDiamondBG"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
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
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _BgColor, _ColorA, _ColorB;
            float _TileSize, _OuterSize, _InnerSize, _MinAlpha, _MaxAlpha;

            // Sin-free hash: avoids precision banding on mobile WebGL GPUs
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample _MainTex so SpriteRenderer alpha/masking works properly
                fixed texAlpha = tex2D(_MainTex, i.uv).a;

                // 1. Scale world coordinates to tile size
                float2 uv = i.worldPos / max(_TileSize, 0.01);

                // 2. Rotate 45 degrees (cos 45 = sin 45 = 0.70710678)
                float2 rotUV = float2(uv.x - uv.y, uv.x + uv.y) * 0.70710678;

                // 3. Split into unique cell ID and local [-0.5, 0.5] coords
                float2 id = floor(rotUV);
                float2 local = frac(rotUV) - 0.5;

                // 4. Hollow square SDF with screen-space anti-aliasing
                float d = max(abs(local.x), abs(local.y));
                float aa = fwidth(d);
                float outer = 1.0 - smoothstep(_OuterSize - aa, _OuterSize + aa, d);
                float inner = smoothstep(_InnerSize - aa, _InnerSize + aa, d);
                float shape = outer * inner;

                // 5. Randomize color and opacity per diamond
                float rndColor = hash21(id);
                float rndAlpha = hash21(id + 19.37);

                fixed3 diamondCol = lerp(_ColorA.rgb, _ColorB.rgb, rndColor);
                float alpha = lerp(_MinAlpha, _MaxAlpha, rndAlpha);

                fixed3 finalCol = lerp(_BgColor.rgb, diamondCol, shape * alpha);

                // Multiply by SpriteRenderer tint/alpha and texture alpha
                return fixed4(finalCol * i.color.rgb, texAlpha * i.color.a);
            }
            ENDCG
        }
    }
}