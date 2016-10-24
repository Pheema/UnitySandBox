Shader "Custom/GridStandard" {
	Properties {
		[HideInInspector] _MainTex ("", 2D) = "white" {}

		_GridColor0("GridColor0", Color) = (0, 0, 0, 1)
		_GridColor1("GridColor1", Color) = (1, 1, 1, 1)
		_GridTile("Tile", Vector) = (1, 1, 0, 0)
		_ShiftSpeed("ShiftSpeed", Vector) = (0, 0, 0, 0)

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		float2 mod(float2 v, float2 m)
		{
			return v - m * floor(v / m);
		}

		float4 _GridColor0;
		float4 _GridColor1;
		float2 _GridTile;
		float2 _ShiftSpeed;

		half _Glossiness;
		half _Metallic;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 uv = IN.uv_MainTex;
			uv -= _ShiftSpeed * _Time.y;
			float2 repUV = 0.5 * mod(uv * _GridTile, (float2)2.0);
			uint2 grid01 = step(repUV, 0.5);
			float gridType = grid01.x ^ grid01.y;
			o.Albedo = lerp(_GridColor0, _GridColor1, gridType);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
