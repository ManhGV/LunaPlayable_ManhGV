Shader "Custom/BlendTwoTextures"
{
    Properties
    {
        _MainTex ("Texture 1 (RGB)", 2D) = "white" {}
        _SecondTex ("Texture 2 (RGB)", 2D) = "white" {}
        [Enum(Y, 0, X, 1)] _BlendAxis ("Blend Axis", Float) = 0 // 0: Y-axis, 1: X-axis
        _BlendStartY ("Blend Start Y", Float) = -0.5 // Local Y coordinate where blend starts (if Axis=Y)
        _BlendEndY ("Blend End Y", Float) = 0.5   // Local Y coordinate where blend ends (if Axis=Y)
        _BlendStartX ("Blend Start X", Float) = -0.5 // Local X coordinate where blend starts (if Axis=X)
        _BlendEndX ("Blend End X", Float) = 0.5   // Local X coordinate where blend ends (if Axis=X)
        _InvertBlend ("Invert Blend Direction", Range(0, 1)) = 0 // 0: Blend Tex1 to Tex2, 1: Blend Tex2 to Tex1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)      // Uses TEXCOORD1
                float localY : TEXCOORD2; // Use TEXCOORD2 for local Y coordinate
                float localX : TEXCOORD3; // Use TEXCOORD3 for local X coordinate
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecondTex;
            float4 _SecondTex_ST;
            float _BlendAxis;
            float _BlendStartY;
            float _BlendEndY;
            float _BlendStartX;
            float _BlendEndX;
            float _InvertBlend;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // Use _MainTex tiling/offset
                o.localY = v.vertex.y; // Pass local Y coordinate
                o.localX = v.vertex.x; // Pass local X coordinate
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the textures
                fixed4 col1 = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_SecondTex, i.uv);

                // Select coordinate, start, and end points based on the chosen axis
                float coord = lerp(i.localY, i.localX, _BlendAxis); // 0 = Y, 1 = X
                float start = lerp(_BlendStartY, _BlendStartX, _BlendAxis);
                float end = lerp(_BlendEndY, _BlendEndX, _BlendAxis);

                // Calculate blend factor based on coordinate within the range
                float range = end - start;
                // Avoid division by zero or negative range, ensure a minimum range for calculation
                range = max(range, 0.0001);
                float blendFactor = saturate((coord - start) / range);

                // Apply smoothstep for smoother transition
                blendFactor = smoothstep(0.0, 1.0, blendFactor);

                // Invert blend direction if needed (Tex1 -> Tex2 vs Tex2 -> Tex1)
                blendFactor = lerp(blendFactor, 1.0 - blendFactor, _InvertBlend);

                // Blend the colors using the calculated blend factor
                fixed4 finalColor = lerp(col1, col2, blendFactor);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
}