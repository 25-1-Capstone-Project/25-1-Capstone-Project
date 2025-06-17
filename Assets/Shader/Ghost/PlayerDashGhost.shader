Shader "Custom/Afterimage_UVRect"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _UVRect ("UV Rect", Vector) = (0, 0, 1, 1) // x: offset, z: size
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST; // Needed by Unity
            float4 _Color;
            float4 _UVRect; // xy = offset, zw = size

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // default UV
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 adjustedUV = i.uv * _UVRect.zw + _UVRect.xy; // scale + offset
                fixed4 col = tex2D(_MainTex, adjustedUV);
                return col * _Color;
            }
            ENDCG
        }
    }
}