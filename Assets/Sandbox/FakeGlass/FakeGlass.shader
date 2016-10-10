Shader "Cumtom/FakeGlass"
{
    Properties
    {
        _EnvTex ("EnvTex", CUBE) = "white" {}
        _IOR("IOR", Float) = 1.4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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

            samplerCUBE _EnvTex;    // 背景キューブマップテクスチャ
            float _IOR;	            // 屈折率

            v2f vert (appdata v)
            {
                v2f o;
                // Unity5.4以前はunity_ObjectToWorldを_Object2Worldに置き換える
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 cam2wpos = normalize(i.wpos - _WorldSpaceCameraPos);
                i.normal = normalize(i.normal);
                
                // 屈折光を計算
                // 物体の表面のみを考慮しているため疑似的な屈折効果になることに注意
                float3 refractedRay = refract(cam2wpos, i.normal, 1.0 / _IOR);
                fixed4 refractedColor = texCUBE(_EnvTex, refractedRay);

                // 反射光を計算
                float3 reflectedRay = reflect(cam2wpos, i.normal);
                fixed4 reflectedColor = texCUBE(_EnvTex, reflectedRay);

                // フレネル反射
                // https://ja.wikipedia.org/wiki/フレネルの式
                float f0 = (_IOR - 1) / (_IOR + 1);
                f0 *= f0;
                float coief = saturate(1.0 - dot(i.normal, -cam2wpos));
                float coief5 = coief * coief * coief * coief * coief;
                float fresnel = f0 + (1.0 - f0) * coief5;

                // 屈折光と反射光をmixして出力
                return lerp(refractedColor, reflectedColor, fresnel);
            }
            ENDCG
        }
    }
}
