Shader "Custom/ToonShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Shininess("Shininess", Range(1, 20)) = 10
		_Outline("Outline", Range(0, 0.2)) = 0.1
		_OutlineCol("OutlineCol", Color) = (1, 1, 1, 1)
	}
    SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}

		Pass
		{
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USE_STROKE

			#include "UnityCG.cginc"

			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct FragmentData
			{
				float4 pos : SV_POSITION;
			};
			float _Outline;
			float4 _OutlineCol;

			FragmentData vert(VertexData v)
			{
				FragmentData o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(vnormal.xy);
#if USE_STROKE
				o.pos.xy += offset * _Outline; 
#endif
				return o;
			}

			fixed4 frag(FragmentData i) : SV_Target
			{
				return _OutlineCol;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USE_TOON
			#pragma shader_feature USE_SPECULAR

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
				float4 col = tex2D(_MainTex, i.uv);
				float3 lightDir = _WorldSpaceLightPos0.xyz;//外界光照方向（原点指向光源）
				float3 lightColor = _LightColor0.rgb;//外界光照颜色
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);//视线方向（表面指向相机）

				//diffuse light
				float NdotL = DotClamped(lightDir, i.normal);
				float3 diffuse = col.rgb * NdotL;
#if USE_TOON
				if (NdotL > 0.85f) diffuse = lerp(col.rgb, float3(1, 1, 1), 0.5);
				else if (NdotL > 0.5f) diffuse = col.rgb * 0.8;
				else if (NdotL > 0.0f) diffuse = lerp(col.rgb, float3(0, 0, 0), 0.5);
				else diffuse = 0;
#endif
				//ambient light
				float3 ambient = col.rgb * UNITY_LIGHTMODEL_AMBIENT.xyz;
				//specular light
				float3 specular = float3(0, 0, 0);
#if USE_SPECULAR
				float3 reflection = normalize(dot(lightDir, i.normal) * 2.0 * i.normal - lightDir);//R + L = R·L * 2N，反射方向
				float Ks = pow(max(0, dot(viewDir, reflection)), _Shininess);
#if USE_TOON
				if (Ks > 0.85f) Ks = 1;
				else Ks = 0;
#endif
				specular = Ks * lightColor;
#endif
				return float4(diffuse + ambient + specular, 1);
			}
			ENDCG
		}
    }
    FallBack "Diffuse"
	CustomEditor "CustomShaderGUI"
}
