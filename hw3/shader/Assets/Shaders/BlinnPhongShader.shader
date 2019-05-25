﻿Shader "Unlit/BlinnPhongShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Shininess("Shininess", Range(1, 20)) = 10
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
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float _Shininess;
			float4 _MainTex_ST;

			FragmentData vert(VertexData v)
			{
				FragmentData i;
				i.vertex = UnityObjectToClipPos(v.vertex);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return i;
			}

			fixed4 frag(FragmentData i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);//基础纹理

				float3 lightDir = _WorldSpaceLightPos0.xyz;//外界光照方向（原点指向光源）
				float3 lightColor = _LightColor0.rgb;//外界光照颜色

				float3 diffuse = col.rgb * DotClamped(lightDir, i.normal);//漫反射效果

				float3 ambient = col.rgb * UNITY_LIGHTMODEL_AMBIENT.xyz;//环境光效果

				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);//视线方向（表面指向相机）
				float3 halfVector = normalize(lightDir + viewDir);//仅在平行光情况下L等于外界光照方向
				float3 specular = pow(max(0, dot(halfVector, i.normal)), _Shininess) * lightColor;//反射光效果

				return float4(diffuse + ambient + specular, 1);
			}
			ENDCG
		}
	}
}