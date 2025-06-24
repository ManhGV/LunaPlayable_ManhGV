Shader "Custom/WeaknessLighting"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        [Space]
        [Header(Weakness Blink)]
        [HDR]_WeaknessColor ("Weakness Color", Color) = (1, 0, 0, 1)
        _WeaknessBlinkSpeed("Weakness Blink Speed", Float) = 10
        _WeaknessSlider("Slider", Range(-1, 1)) = 0

        [Space]
        [Header(Shadow)]
        [Toggle]_EnableShadow("Enable Shadow", Float) = 0
        _ShadowColor("Shadow Color", Color) = (0,0,0,0.25)
        _ShadowHeight("Shadow Height", Float) = 0.01
        _LightDirection("Light Direction", Vector) = (0.15, 0.7, 0.3, 0.3)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        CGPROGRAM
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma vertex vert
        // #pragma fragment frag
        #pragma surface surf CustomLambert noforwardadd

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        half4 LightingCustomLambert(SurfaceOutput s, half3 lightDir, half atten)
        {
            half NdotL = dot(s.Normal, lightDir);
            half4 c;
            half3 litColor = s.Albedo * _LightColor0.rgb * (NdotL * atten);
            c.rgb = lerp(litColor, s.Albedo, s.Specular);
            c.a = s.Alpha;
            return c;
        }

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 normal;
            float3 viewDir;
        };

        struct MeshData
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
            float3 normal : NORMAL;
        };

        //
        // struct v2f
        // {
        //     
        // };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _WeaknessColor;
        float _WeaknessBlinkSpeed;
        float _WeaknessSlider;

        float Remap(float In, float2 InMinMax, float2 OutMinMax)
        {
            return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        // v2f vert (MeshData v)
        // {
        //     v2f o;
        //     return o;
        // }
        //
        // float4 frag(v2f i)
        // {
        //     
        //     return 
        // }

        void vert(inout MeshData v, out Input o)
        {
            o.uv_MainTex = v.texcoord;
            o.normal = normalize(UnityObjectToWorldNormal(v.normal));
            o.viewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 finalColor = mainColor;

            float weaknessFade = sin(_Time.y * _WeaknessBlinkSpeed) * 0.5 + 0.5;
            float weaknessMask = smoothstep(_WeaknessSlider, 1, 1 - max(0, dot(IN.normal, IN.viewDir)));
            float4 weaknessColor = _WeaknessColor * weaknessMask * weaknessFade;
            //lerp(_WeaknessColor * weaknessMask, float4(0, 0, 0, 0), weaknessFade);
            finalColor += weaknessColor;

            o.Albedo = finalColor.rgb;
            o.Alpha = 1;
        }
        ENDCG

        Pass
        {
            Tags
            {
                "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent"
            }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            Stencil
            {
                Ref 0
                Comp Equal
                Pass IncrWrap
                ZFail Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma fragmentoption ARB_precision_hint_fastest
            #pragma shader_feature _ENABLESHADOW_ON
            #include "UnityCG.cginc"

            struct vsOut
            {
                float4 pos : SV_POSITION;
            };

            float _ShadowHeight;
            fixed4 _ShadowColor;
            fixed4 _LightDirection;
            float _DissolveAmount;

            vsOut vert(appdata_base v)
            {
                vsOut o;
                float shadowHeight = _ShadowHeight;
                #if _ENABLESHADOW_ON
                float objectOriginY = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).y + _ShadowHeight;
                // v.vertex.y=0;
                float4 vPosWorld = mul(unity_ObjectToWorld, v.vertex);
                float opposite = vPosWorld.y - objectOriginY;
                float cosTheta = -_LightDirection.y;
                float hypotenuse = opposite / cosTheta;
                float3 vPos = vPosWorld.xyz + (_LightDirection * hypotenuse);
                o.pos = mul(UNITY_MATRIX_VP, float4(vPos.x, objectOriginY + shadowHeight, vPos.z, 1));
                #else
                o.pos = UnityObjectToClipPos(v.vertex);
                #endif
                return o;
            }

            fixed4 frag(vsOut i) : COLOR
            {
                //float4 col = lerp(_ShadowColor, float4(0, 0, 0, 0), _DissolveAmount);
                #if _ENABLESHADOW_ON
                return _ShadowColor;
                #endif

                discard;
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}