Shader "Unlit/WaterShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Speed("Wave Speed", Range(0, 20)) = 7
		_Length("Wave Length", Range(0, 1)) = 0.2
		_Amplitude("Wave Amplitude", Range(0, 2)) = 0.3
		_Shininess("Shininess", Range(10, 20)) = 15
		_Ks("Ks", Range(0, 1)) = 0.08
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
				float3 normal : NORMAL;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Speed;
			float _Length;
			float _Amplitude;
			float _Shininess;
			float _Ks;

			FragmentData vert(VertexData v)
			{
				FragmentData i;
				i.vertex = UnityObjectToClipPos(v.vertex);
				i.normal = normalize(UnityObjectToWorldNormal(v.normal));

				float valueA = (v.vertex.x - _Speed * _Time) / _Length;

				i.vertex += float4(0, _Amplitude * sin(valueA), 0, 0);
				i.normal = normalize(i.normal - float3(cos(valueA), 0, 0));
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return i;
			}

			float4 frag(FragmentData i) : SV_Target
			{
				float3 lightDir = _WorldSpaceLightPos0.xyz;//外界光照方向（原点指向光源）
				float3 lightColor = _LightColor0.rgb;//外界光照颜色
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);//视线方向（表面指向相机）

				float4 col = tex2D(_MainTex, i.uv);
				float3 diffuse = col.rgb * DotClamped(lightDir, i.normal);
				float3 ambient = col.rgb * UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 reflection = normalize(dot(lightDir, i.normal) * 2.0 * i.normal - lightDir);//R + L = R·L * 2N，反射方向
				float3 specular = pow(max(0, dot(viewDir, reflection)), _Shininess) * lightColor * _Ks;
				return float4(diffuse + ambient + specular, 1);
			}
			ENDCG
		}
	}
}
