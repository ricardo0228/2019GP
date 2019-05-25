Shader "Unlit/TextureShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
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
				float2 uv : TEXCOORD0;
			};

			struct FragmentData
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			FragmentData vert(VertexData v)
			{
				FragmentData i;
				i.vertex = UnityObjectToClipPos(v.vertex);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			fixed4 frag(FragmentData i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
        }
    }
}
