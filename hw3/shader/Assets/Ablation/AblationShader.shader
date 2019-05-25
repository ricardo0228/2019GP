Shader "Unlit/AblationShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NoiseTex("Noise", 2D) = "white" {}
		_TotalTime("TotalTime", Range(0, 10)) = 5
		_EdgeLength("Edge Length", Range(0.0, 0.2)) = 0.1
		_RampTex("Ramp", 2D) = "white" {}
		_MinBorderY("Min Border Z", Float) = -0.5 
		_MaxBorderY("Max Border Z", Float) = 0.5  
		_DistanceEffect("Distance Effect", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }

		Pass
		{
			Cull Off 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityStandardBRDF.cginc"

			struct VertexData
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct FragmentData
			{
				float4 vertex : SV_POSITION;
				float2 uvMainTex : TEXCOORD0;
				float2 uvNoiseTex : TEXCOORD1;
				float2 uvRampTex : TEXCOORD2;
				float objPosY : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _TotalTime;
			float _EdgeLength;
			sampler2D _RampTex;
			float4 _RampTex_ST;
			int _Direction;
			float _MinBorderY;
			float _MaxBorderY;
			float _DistanceEffect;
			
			FragmentData vert (VertexData v)
			{
				FragmentData o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uvMainTex = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvNoiseTex = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.uvRampTex = TRANSFORM_TEX(v.uv, _RampTex);

				o.objPosY = v.vertex.y;

				return o;
			}
			
			fixed4 frag (FragmentData i) : SV_Target
			{
				float range = _MaxBorderY - _MinBorderY;
				float border = _MinBorderY;
				float threshold = _Time.y / _TotalTime;//消融系数随时间变化

				float dist = abs(i.objPosY - border);
				float normalizedDist = saturate(dist / range);

				fixed cutout = tex2D(_NoiseTex, i.uvNoiseTex).r * (1 - _DistanceEffect)
								+ normalizedDist * _DistanceEffect;
				clip(cutout - threshold);//利用透明通道测试实现消融效果

				float degree = saturate((cutout - threshold) / _EdgeLength);
				fixed4 edgeColor = tex2D(_RampTex, float2(degree, degree));//加载边缘处纹理

				fixed4 col = tex2D(_MainTex, i.uvMainTex);//加载基础纹理

				fixed4 finalColor = lerp(edgeColor, col, degree);
				return fixed4(finalColor.rgb, 1);
			}
			ENDCG
		}
	}
}
