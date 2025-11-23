// Assets/Shaders/URP2D_WhiteSilhouette_PerRenderer_Stable_NoBatch.shader
Shader "URP2D/WhiteSilhouette_PerRenderer_Stable_NoBatch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color (Material Tint)", Color) = (1,1,1,1)

        // SpriteRenderer.color 가 자동 주입되는 PerRenderer 데이터
        [PerRendererData] _RendererColor ("Renderer Color", Color) = (1,1,1,1)

        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags{
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="Transparent" "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "DisableBatching"="True"
        }
        Pass
        {
            Name "WhiteSilhouette"
            Tags{ "LightMode"="Universal2D" }
            Cull Off ZWrite Off ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct A { float4 pos: POSITION; float2 uv: TEXCOORD0; };
            struct V { float4 pos: SV_Position; float2 uv: TEXCOORD0; };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            float4 _Color;          // 머티리얼 틴트
            float4 _RendererColor;  // SpriteRenderer.color
            float  _Cutoff;

            V Vert(A v){
                V o;
                o.pos = TransformObjectToHClip(v.pos.xyz);
                o.uv  = v.uv;
                return o;
            }

            float4 Frag(V i):SV_Target
            {
                // 알파만 사용해 실루엣 마스크 생성
                float a = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, i.uv, 0).a;

                // 테두리 깨짐 방지용 컷오프(이진화)
                a = step(_Cutoff, a);

                // 최종 색 = (재질 색 × 렌더러 색) 의 RGB, 알파는 원본 알파 × 두 색의 알파
                float3 tintRGB = (_Color.rgb * _RendererColor.rgb);
                float  tintA   = saturate(_Color.a * _RendererColor.a);

                return float4(tintRGB, a * tintA);
            }
            ENDHLSL
        }
    }
}
