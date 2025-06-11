Shader "Horus/Emission Mask Only"
{
    Properties
    {
        [Header(Texture)]
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor("Texture Color", Color) = (1,1,1,1)
        _SpecialMask("R : Unlit Mask, G : Emission Mask", 2D) = "black"{}
        [HDR]_EmissionColor("Emisson Color", color) = (0,0,0,1)
        _Stencil ("Stencil Value", Float) = 200
        _DissolveNoise("Dissolve Noise", 2D) = "white"{}
        _DissolveAmount("Dissolve Amount", Range(0, 1)) = 0
        [HDR]_OutlineColor("Dissolve Color", Color) = (16,0.451018184,0,1)
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
        LOD 150

        CGPROGRAM
        #include "Lighting.cginc"
        #pragma surface surf CustomLambert noforwardadd

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
        float4 _MainColor;
        sampler2D _UnlitMask;
        sampler2D _SpecialMask;
        float4 _EmissionColor;
        sampler2D _DissolveNoise;

        float _DissolveAmount;
        float4 _OutlineColor;
        float _Outline;

        float Remap(float main, float minIn, float maxIn, float minOut, float maxOut)
        {
            return minOut + (main - minIn) * (maxOut - minOut) / (maxIn - minIn);
        }

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float dissolveNoise = tex2D(_DissolveNoise, IN.uv_MainTex).r;
            float dissolveAmount = Remap(_DissolveAmount, 0, 1, 0, 2);

            float step2Value = step(Remap(dissolveAmount, 0, 1, 0, 0.5), dissolveNoise);
            clip(step2Value - 0.5);


            float smoothstepValue = smoothstep(0, Remap(dissolveAmount, 0, 1, 0, 0.9), dissolveNoise);
            float step1Value = step(dissolveAmount, dissolveNoise);

            float dissolveMask = step(0.01, dissolveAmount);
            float edgeValue = smoothstepValue - step1Value;
            float baseValue = 1 - edgeValue * dissolveMask;
            edgeValue = pow(saturate(edgeValue), 15);


            float4 edgeColor = edgeValue * _OutlineColor * dissolveMask;

            float4 mask = tex2D(_SpecialMask, IN.uv_MainTex);
            float unlitMask = mask.r;
            float emissionMask = mask.g;

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _MainColor;

            o.Specular = unlitMask; // Không dùng Specular nên lấy nó để chứa unlit mask
            o.Emission = emissionMask * _EmissionColor;
            o.Albedo = c.rgb * baseValue + edgeColor.rgb;
            o.Alpha = step2Value;
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
            #pragma fragmentoption ARB_precision_hint_fastest
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
                // Old Logic
                // float objectOriginY = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).y + _ShadowHeight;
                // // v.vertex.y=0;
                // float4 vPosWorld = mul(unity_ObjectToWorld, v.vertex);
                // float opposite = vPosWorld.y - objectOriginY;
                // float cosTheta = -_LightDirection.y;
                // float hypotenuse = opposite / cosTheta;
                // float3 vPos = vPosWorld.xyz + (_LightDirection * hypotenuse);
                // o.pos = mul(UNITY_MATRIX_VP, float4(vPos.x, objectOriginY + shadowHeight, vPos.z, 1));
                
                float4 vPosWorld = mul(unity_ObjectToWorld, v.vertex);
                
                float3 lightDirNormalized = normalize(_LightDirection.xyz);
                float shadowOffset = -lightDirNormalized.y * _ShadowHeight;
                o.pos = mul(UNITY_MATRIX_VP, float4(vPosWorld.x, shadowOffset, vPosWorld.z, 1));
                #else
                o.pos = UnityObjectToClipPos(v.vertex);
                #endif
                return o;
            }

            fixed4 frag(vsOut i) : COLOR
            {
                float4 col = lerp(_ShadowColor, float4(0, 0, 0, 0), _DissolveAmount);
                #if _ENABLESHADOW_ON
                return col;
                #endif

                discard;
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}