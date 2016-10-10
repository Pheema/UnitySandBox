// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/GridDebugger"
{
	Properties
	{
		_LineInterval("LineInterval", Vector) = (1, 1, 1, 0)
		_Threshold("Threshold", Range(0, 1)) = 0.1
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 spos : TEXCOORD1;
			};


			float3 _LineInterval;
			float _Threshold;
			float4x4 _MainCameraVP;

			float3 mod(float3 v, float3 m)
			{
				return v - m * floor(v / m);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.spos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
				o.spos = mul(_MainCameraVP, o.spos);
				// o.spos = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				
				i.spos.xyz /= i.spos.w;
				i.spos.z = Linear01Depth(i.spos.z);
				float3 repCoord = i.spos.xyz;
				// repCoord = step(repCoord, _Threshold);
				float4 col = float4(repCoord, 1.0);
				return col;
			}
			ENDCG
		}
	}
}
