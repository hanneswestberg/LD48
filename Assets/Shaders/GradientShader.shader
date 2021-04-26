Shader "Unlit/GradientShader"
{
    Properties
    {
        _ColorStart("Start Color", Color) = (1, 1, 1, 1)
        _ColorEnd("End Color", Color) = (1, 1, 1, 1)
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

            #include "UnityCG.cginc"

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
                o.uv = v.uv;
                return o;
            }

            fixed4 _ColorStart;
            fixed4 _ColorEnd;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = lerp(_ColorStart, _ColorEnd, i.uv.y);
                return col;
            }
            ENDCG
        }
    }
}
