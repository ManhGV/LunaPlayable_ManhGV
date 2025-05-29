Shader "Horus/BotPart"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _IsWeakness ("Is Weakness", Float) = 0
        _WeaknessColor ("Weakness Color", Color) = (1, 0, 0, 1)
        _WeaknessScale("Weakness Scale", Float) = 1
		_WeaknessBias("Weakness Bias", Float) = 0
        _WeaknessBlinkSpeed("Weakness Blink Speed", Float) = 10
        // POTENTIAL: destroy mode, this reduces amount of materials in scene
        [Toggle] _IsDestroyed ("Is Destroyed", Float) = 0
        _BurnTex ("Burn Texture", 2D) = "gray" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Stencil 
        {
            Ref 200
            Comp GEqual
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // #pragma shader_feature _ISWEAKNESS_ON
            // #pragma shader_feature _ISDESTROYED_ON
            // make fog work
            #pragma multi_compile_fog

            #include "Assets/_GameManhGV/Shader/Bot/MyLib.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normals : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 view_dir : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            fixed _IsWeakness;
            fixed4 _WeaknessColor;
            float _WeaknessScale;
            float _WeaknessBias;
            float _WeaknessBlinkSpeed;

            fixed _IsDestroyed;
            sampler2D _BurnTex;
            float4 _BurnTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                if (_IsDestroyed)
                    o.uv = TRANSFORM_TEX(v.uv, _BurnTex);
                else
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.view_dir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                if (_IsDestroyed)
                {
                    col = tex2D(_BurnTex, i.uv);
                }
                else
                {
                    col = tex2D(_MainTex, i.uv);
                    if (_IsWeakness)
                    {
                        const float3 normal = normalize(i.normal);
                        const float3 view_dir = normalize(i.view_dir);
                        const float weakness_display_strength = _WeaknessBias + _WeaknessScale * pow(1.0 - dot(normal, view_dir ), (sin(_Time.y * _WeaknessBlinkSpeed) + 2.0 ) / 2.0  ) ;
                        const fixed4 weakness_display = weakness_display_strength * _WeaknessColor;
                        col = col + weakness_display;
                    }
                }
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
