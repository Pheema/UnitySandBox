Shader "Custom/Outline"
{
	Properties
	{
		_Color ("LineColor", Color) = (0, 0, 0, 1)
		_Thickness ("LineThickness", Float) = 1.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Front

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float  _Thickness;

			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz += normalize(v.normal.xyz) * _Thickness;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(_Color.xyz, 1.0);
			}
			ENDCG
		}
	}
}
