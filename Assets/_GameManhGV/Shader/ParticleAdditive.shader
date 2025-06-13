Shader "Horus/Transparent/ParticleAdditive_BuiltIn"
{
    Properties
    {
        [Toggle]_UseLerpColor("Use Lerp Color", Range(0, 1)) = 0
        [HDR]_Color1("BrightColor", Color) = (1, 1, 1, 1)
        _Color2("DarkColor", Color) = (1, 1, 1, 1)
        [Header(Base)][HDR]_TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
        _Boost ("Boost", Range(0, 10)) = 2
        _Clip("Clip", Range(0,1)) = 0.01
        _Pull("Pull", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)]_Cull ("Cull", Range(0, 1)) = 0
        [Toggle]_EnableFog ("Fog", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        Cull [_Cull]
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma shader_feature _ENABLEFOG_ON
            #pragma shader_feature _USELERPCOLOR_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float4 _Color1;
            float4 _Color2;
            float _Boost;
            float _Clip;
            float _Pull;

            struct appdata_t
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                half4 color : COLOR;

                #if _ENABLEFOG_ON
                float fogFactor : TEXCOORD1;
                #endif
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                float3 cameraPosition = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz;
                float3 cameraDirection = normalize(cameraPosition - v.position.xyz);

                float4 worldPos = mul(unity_ObjectToWorld, v.position);
                worldPos.xyz += cameraDirection * _Pull;

                o.position = UnityObjectToClipPos(worldPos);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;

                #if _ENABLEFOG_ON
                o.fogFactor = ComputeFogFactor(o.position.z);
                #endif
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float4 mainTexture = tex2D(_MainTex, i.texcoord);

                float4 combineColor;
                #if _USELERPCOLOR_ON
                combineColor = lerp(_Color2, _Color1, mainTexture.r);
                #else
                combineColor = _TintColor;
                #endif

                float4 col = _Boost * i.color * combineColor * mainTexture;
                clip(mainTexture.r - _Clip);

                #if _ENABLEFOG_ON
                col.rgb = lerp(unity_FogColor, col.rgb, i.fogFactor);
                #endif
                return col;
            }
            ENDCG
        }
    }
}