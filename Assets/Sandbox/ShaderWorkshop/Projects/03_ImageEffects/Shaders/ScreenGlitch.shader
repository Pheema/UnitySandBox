Shader "Hidden/ScreenGlitch"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tile ("Tile", Float) = 10.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			#define M_2PI (6.28318530717958)
			inline float hash(float2 co)
			{
				return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
			}

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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Seed;
			float _Amplitude;
			float _Tile;

			fixed4 frag (v2f i) : SV_Target
			{

				i.uv.x += _Amplitude * (hash(float2(ceil(i.uv.y * _Tile) / _Tile, _Seed)) - 0.5);
				i.uv.x += 0.5 * _Amplitude * (hash(float2(ceil(i.uv.y * 1.761 * _Tile) / (1.761 * _Tile), _Seed)) - 0.5);
				i.uv.x += 0.25 * _Amplitude * (hash(float2(ceil(i.uv.y * 3.1781 * _Tile) / (3.1781 * _Tile), _Seed)) - 0.5);
				
				i.uv.y += _Amplitude * (hash(float2(ceil(i.uv.y * _Tile) / _Tile, _Seed)) - 0.5);

				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
