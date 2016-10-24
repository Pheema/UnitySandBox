Shader "Hidden/JFA"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
		#include "UnityCG.cginc"

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
		// R, G: 一番近い母点のUV座標
		// B: 一番近い母点との距離
		// A: 母点を発見しているかどうか
		sampler2D_float _VoronoiTex;
		float2 _VoronoiTex_TexelSize;

		float _JumpSize;

		float4 frag_init(v2f i) : SV_Target
		{
			float4 col = tex2D(_MainTex, i.uv);
			float4 val = float4(-1, -1, 1000000, 0);
			if (any(col.rgb)) val = float4(i.uv, 0, 1);
			// if (col.a > 0) val = float4(i.uv, 0, 1);
			return val;
		}

		float4 frag_JFA(v2f i) : SV_Target
		{
			float d = _JumpSize;
			
			float4 myInfo = tex2D(_VoronoiTex, i.uv);
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					float2 uv = i.uv + d * int2(x, y);
					float4 neighborInfo = tex2D(_VoronoiTex, uv);
					if (length(neighborInfo.rg - i.uv) < myInfo.b)
					{
						myInfo.rg = neighborInfo.rg;
						myInfo.b = length(neighborInfo.rg - i.uv);
						myInfo.a = 1;
					}
				}
			}

			return myInfo;
		}

		float4 frag_show(v2f i) : SV_Target
		{
			float4 col = tex2D(_MainTex, i.uv);
			float val = col.b * 2.0;
			return float4((float3)val, 1);
		}
		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_init
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma	fragment frag_JFA
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma	fragment frag_show
			ENDCG
		}
	}
}
