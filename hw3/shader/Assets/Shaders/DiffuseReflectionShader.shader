Shader "Unlit/DiffuseReflectionShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityStandardBRDF.cginc"

			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct FragmentData
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST; 

			FragmentData vert (VertexData v)
			{
				FragmentData i;
				i.vertex = UnityObjectToClipPos(v.vertex);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			fixed4 frag(FragmentData i) : SV_Target
			{
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float4 col = tex2D(_MainTex, i.uv);
				float3 diffuse = col.rgb * DotClamped(lightDir, i.normal);
				return float4(diffuse, 1);
			}
			ENDCG
		}
    }
}
