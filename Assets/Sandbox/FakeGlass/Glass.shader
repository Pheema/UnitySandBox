Shader "Cumtom/Glass"
{
    Properties
    {
        _EnvTex ("EnvTex", CUBE) = "white" {}
        _IOR("IOR", Float) = 1.4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float3 normal : NORMAL;
            float3 wpos : TEXCOORD1;
        };

		sampler2D_float _GrabTexture;
        samplerCUBE _EnvTex;    // 背景キューブマップテクスチャ
        float _IOR;	            // 屈折率


        v2f vert(appdata v)
        {
            v2f o;
            // Unity5.4以前はunity_ObjectToWorldを_Object2Worldに置き換える
            o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
            o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.normal = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz;
            return o;
        }
            
		float4 frag_back(v2f i) : SV_Target
		{
			float3 normal = 0.5 + 0.5 * normalize(i.normal);
			float linearDepth = Linear01Depth(i.vertex.z);
			return float4(normal, linearDepth);
		}

		float4 frag_front(v2f i) : SV_Target
		{
			float3 cam2wpos = normalize(i.wpos - _WorldSpaceCameraPos);
			i.normal = normalize(i.normal);
			float3 back_normal = (float3)0.0;

			// 屈折光を計算
			// 物体の表面のみを考慮しているため疑似的な屈折効果になることに注意
			float3 refractedRay = refract(cam2wpos, i.normal, 1.0 / _IOR);

			refractedRay = normalize(refractedRay);

			float t_min = 0.0;
			float t_max = 3.0;

			float3 temp = (float3)0.0;
#if 1
			float4 spos_mid;
			for (uint j = 0; j < 32; j++)
			{
				float t_mid = 0.5 * (t_min + t_max);
				float3 wpos_mid = i.wpos + t_mid * refractedRay;
				spos_mid = mul(UNITY_MATRIX_VP, float4(wpos_mid, 1.0));
				spos_mid.xyz /= spos_mid.w;
				spos_mid.xy = (float2)0.5 + 0.5 * spos_mid.xy;
				spos_mid.y = 1.0 - spos_mid.y;
				float linearDepth = tex2D(_GrabTexture, spos_mid.xy).a;
				
				if (linearDepth > 0.9999)
				{
					t_max = t_mid;
					continue;
				}

				float linearDepth_mid = Linear01Depth(spos_mid.z);
				if (linearDepth > linearDepth_mid)
				{
					t_min = t_mid;
				}
				else
				{
					t_max = t_mid;
				}
			}

			back_normal = 2.0 * tex2D(_GrabTexture, spos_mid.xy).rgb - (float3)1.0;
			back_normal = normalize(back_normal);
			
			temp = refractedRay;
			refractedRay = refract(refractedRay, -back_normal, _IOR);
#endif

			float4 refractedColor = texCUBE(_EnvTex, refractedRay);
			
			// 内部で全反射した場合
			// 反射した方向のCubeMapを参照する（ごまかし）
			if (length(refractedRay) == 0.0) refractedColor.rgb = 0.6 * texCUBE(_EnvTex, reflect(temp, -back_normal));

			// 反射光を計算
			float3 reflectedRay = reflect(cam2wpos, i.normal);
			float4 reflectedColor = texCUBE(_EnvTex, reflectedRay);

			// フレネル反射
			// https://ja.wikipedia.org/wiki/フレネルの式
			float f0 = (_IOR - 1) / (_IOR + 1);
			f0 *= f0;
			float coief = saturate(1.0 - dot(i.normal, -cam2wpos));
			float coief5 = coief * coief * coief * coief * coief;
			float fresnel = f0 + (1.0 - f0) * coief5;

			// float4 hoge = mul(UNITY_MATRIX_VP, float4(i.wpos, 1.0));
			// return float4(hoge.xy / hoge.w, 0.0, 1.0);
			// return float4(abs(back_normal), t_max - t_min);
			// return float4(spos_mid.xy, 0, t_max - t_min);
			// return float4(tex2D(_GrabTexture, spos_mid.xy).xyz, 1);

			// 屈折光と反射光をmixして出力
			return lerp(refractedColor, reflectedColor, fresnel);
		}
			ENDCG

		Pass
		{
			Offset 0, -1
		}

		Pass
		{
			Cull Front
			
			ZTest GEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_back
			ENDCG
		}

		GrabPass
		{
		}

		Pass
		{
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_front
			ENDCG
		}
    }
}
