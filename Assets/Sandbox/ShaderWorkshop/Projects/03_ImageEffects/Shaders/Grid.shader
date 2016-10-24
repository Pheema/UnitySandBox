Shader "Custom/Grid"
{
	Properties
	{
		_GridColor0("GridColor0", Color) = (0, 0, 0, 1)
		_GridColor1("GridColor1", Color) = (1, 1, 1, 1)
		_GridTile("Tile", Vector) = (1, 1, 0, 0)
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			float2 mod(float2 v, float2 m)
			{
				return v - m * floor(v / m);
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _GridColor0;
			float4 _GridColor1;
			float2 _GridTile;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 repUV = 0.5 * mod(i.uv * _GridTile, (float2)2.0);
				uint2 grid01 = step(repUV, 0.5);
				float gridType = grid01.x ^ grid01.y;
				return lerp(_GridColor0, _GridColor1, gridType);
			}
			ENDCG
		}
	}
	FallBack "Standard"
}
