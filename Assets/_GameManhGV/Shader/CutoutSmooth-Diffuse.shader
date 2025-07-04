Shader "Horus/Transparent/Cutout smoothly/Diffuse"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
            [Enum(Both, 0, Back, 1, Front, 2)] _Cull("Show", float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest+100" "IgnoreProjector"="True" "RenderType"="TransparentCutout"
        }
        LOD 200

        Cull[_Cull]
        Offset -1, 1

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}