Shader "Unlit/NormalShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct FragmentData
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			FragmentData vert (VertexData v)
            {
				FragmentData i;
				i.vertex = UnityObjectToClipPos(v.vertex);
				i.normal = UnityObjectToWorldNormal(v.normal);
				return i;
            }

            fixed4 frag (FragmentData i) : SV_Target
            {
                fixed4 col = float4(abs(i.normal), 1);
                return col;
            }
            ENDCG
        }
    }
}
