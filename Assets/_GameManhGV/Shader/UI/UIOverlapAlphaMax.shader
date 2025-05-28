// Shader "Custom/StencilMergedAlpha_Ordered"
// Shader này tạo hiệu ứng alpha hợp nhất (không cộng dồn) cho các sprite chồng lấn.
// QUAN TRỌNG: Sprite bạn muốn "nằm trên" (quyết định hình dạng ở vùng chồng lấn)
// PHẢI ĐƯỢC RENDER ĐẦU TIÊN trong nhóm các sprite sử dụng Material này
// với cùng một giá trị Stencil Reference.
Shader "Custom/StencilMergedAlpha_Ordered" {
    Properties{
        _MainTex("Sprite Texture (Ảnh Sprite)", 2D) = "white" {}
        _Color("Tint & Alpha (Màu phủ & Alpha gốc)", Color) = (1,1,1,0.5) // Đặt Alpha ở đây, ví dụ 0.5 cho 50%
        _StencilRef("Stencil Reference (Tham chiếu Stencil)", Float) = 1
            // Tất cả sprite trong nhóm cần dùng cùng _StencilRef và cùng Material này (hoặc Material có Stencil y hệt)
    }

        SubShader{
            Tags {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off // Rất quan trọng cho đối tượng trong suốt và Stencil
            Blend SrcAlpha OneMinusSrcAlpha // Chế độ hoà trộn tiêu chuẩn

            Pass {
                Stencil {
                    Ref[_StencilRef] // Giá trị tham chiếu để ghi và so sánh
                    Comp NotEqual     // Chỉ vẽ nếu giá trị stencil hiện tại KHÔNG BẰNG _StencilRef
                                      // (tức là pixel này CHƯA được vẽ bởi sprite khác trong nhóm ĐÃ RENDER TRƯỚC ĐÓ)
                    Pass Replace      // Nếu Stencil Test và Depth Test (nếu có) đều qua:
                                      // thay thế giá trị stencil tại pixel này bằng _StencilRef.
                                      // Điều này "đánh dấu" pixel này đã được chiếm.
                    Fail Keep         // Nếu Stencil Test thất bại: giữ nguyên giá trị stencil
                    ZFail Keep        // Nếu Stencil Test qua nhưng Depth Test thất bại: giữ nguyên giá trị stencil
                }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                fixed4 _Color; // Màu và alpha từ Properties

                v2f vert(appdata_t IN) {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = IN.color * _Color; // Nhân màu đỉnh (thường là trắng) với _Color (bao gồm alpha)
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target {
                    fixed4 texColor = tex2D(_MainTex, IN.texcoord);
                    fixed4 finalColor = texColor * IN.color;

                    finalColor.a = saturate(finalColor.a); // Đảm bảo alpha trong khoảng [0,1]

                    // Rất quan trọng: Clip (không vẽ) các pixel hoàn toàn trong suốt của sprite.
                    // Điều này ngăn không cho các vùng trong suốt của sprite ghi vào Stencil Buffer.
                    clip(finalColor.a - 0.001); // Giá trị trừ rất nhỏ để tránh lỗi làm tròn của số thực

                    return finalColor;
                }
                ENDCG
            }
        }
            Fallback "Sprites/Default"
}
