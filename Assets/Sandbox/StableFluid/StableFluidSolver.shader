Shader "Hidden/StableFluidSolver"
{
    Properties
    {
		_MainTex ("", 2D) = "black" {}
        _PressureTex ("Pressure", 2D) = "black" {}
        _VelocityTex ("Velocity", 2D) = "black" {}
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


		float2 _MainTex_TexelSize;
        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
            o.uv = v.uv;
            return o;
        }

		sampler2D _MainTex;
        sampler2D_float _PressureTex;
        float2 _PressureTex_TexelSize;
        sampler2D_float _VelocityTex;
        float2 _VelocityTex_TexelSize;

		sampler2D_half _CameraMotionVectorsTexture;
		sampler2D _PrevImage;

        float _FixedDeltaTime;
		float _Density;

        float4 frag_addforce(v2f i) : SV_Target
        {
            return float4(0, 0, 0, 1);
        }

        float4 frag_diffuse(v2f i) : SV_Target
        {
            float3 dv = (float3)0.0;
            float2 d = _VelocityTex_TexelSize.xy;
            dv += tex2D(_VelocityTex, i.uv + float2(d.x, 0)).xyz;
            dv += tex2D(_VelocityTex, i.uv - float2(d.x, 0)).xyz;
            dv += tex2D(_VelocityTex, i.uv + float2(0, d.y)).xyz;
            dv += tex2D(_VelocityTex, i.uv - float2(0, d.y)).xyz;
            dv -= 4.0 * tex2D(_VelocityTex, i.uv);
			dv *= _FixedDeltaTime * 1.0e-6 / (d.x * d.x);
            float3 v = tex2D(_VelocityTex, i.uv) + dv;
            return float4(v ,1);
        }

        float4 frag_advection(v2f i) : SV_Target
        {
            float3 prevVelocity = tex2D(_VelocityTex, i.uv).rgb;
            float3 nextTex = tex2D(_MainTex, i.uv - prevVelocity * _FixedDeltaTime).rgb;
            return float4(nextTex, 1);
        }

        float4 frag_poisson(v2f i) : SV_Target
        {
			float3 p = (float3)0.0;
            float2 d = _VelocityTex_TexelSize;
            float3 pRight = tex2D(_PressureTex, i.uv + float2(d.x, 0));
            float3 pLeft  = tex2D(_PressureTex, i.uv - float2(d.x, 0));
            float3 pUp    = tex2D(_PressureTex, i.uv + float2(0, d.y));
            float3 pDown  = tex2D(_PressureTex, i.uv - float2(0, d.y));
            
			float vxRight = tex2D(_VelocityTex, i.uv + float2(d.x, 0)).x;
			float vxLeft  = tex2D(_VelocityTex, i.uv).x;
			float vyUp    = tex2D(_VelocityTex, i.uv + float2(0, d.y)).y;
			float vyDown  = tex2D(_VelocityTex, i.uv).y;
            float divV = (vxRight - vxLeft) / d.x + (vyUp - vyDown) / d.y;

			p = (pRight + pLeft + pUp + pDown - _Density * d.x * d.x / _FixedDeltaTime * divV) * 0.25;
            return float4(p, 1);
        }

		float4 frag_projection(v2f i) : SV_Target
		{
			float2 d = _PressureTex_TexelSize.xy;
			float3 prevVelocity = tex2D(_VelocityTex, i.uv);
			float pRight = tex2D(_PressureTex, i.uv).r;
			float pLeft  = tex2D(_PressureTex, i.uv - float2(d.x, 0)).r;
			float pUp    = tex2D(_PressureTex, i.uv).r;
			float pDown  = tex2D(_PressureTex, i.uv - float2(0, d.y)).r;

			float3 gradP = float3((pRight - pLeft) / d.x, (pUp - pDown) / d.y, 0);
			float3 nextVelocity = prevVelocity - _FixedDeltaTime / _Density * gradP;
			return float4(nextVelocity, 1);
		}

		float4 frag_extf(v2f i) : SV_Target
		{
			float2 motionVec = tex2D(_CameraMotionVectorsTexture, i.uv).rg * 10.0;

			float4 velocity = tex2D(_VelocityTex, i.uv);
			float4 f = (float4)0.0;
			float l = length(i.uv - float2(0.5, 0.0));
			float amp = saturate(lerp(1.0, 0.0, l * 50.0));
			// f = 98 * amp * float4(_CosTime.w, _SinTime.w, 0, 0);
			f = 98 * amp * float4(0, 1, 0, 0);
			// velocity += f * _FixedDeltaTime;
			velocity += float4(motionVec, 0, 0);
			velocity *= 0.95f;
			return float4(velocity.xyz, 1.0);
		}

		float4 frag_advectionImage(v2f i) : SV_Target
		{
			float2 flipedUV = float2(i.uv.x, 1.0 - i.uv.y);
			float3 prevVelocity = tex2D(_VelocityTex, flipedUV).rgb;
			float3 prevImage = tex2D(_PrevImage, i.uv - prevVelocity * _FixedDeltaTime);
			float3 nextImage = tex2D(_MainTex, i.uv).rgb;
			nextImage += 11 * prevImage;
			nextImage /= 12.0;
			return float4(nextImage, 1);
		}

        ENDCG

        // Pass0: Add Force
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_addforce
            ENDCG
        }

        // Pass1: Diffuse
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_diffuse
            ENDCG
        }

        // Pass2: Advection
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_advection
            ENDCG
        }

        // Pass3: Solve poisson eqn
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_poisson
            ENDCG
        }
		
		// Pass4: Projection
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_projection
			ENDCG
		}

		// Pass5: External force
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_extf
			ENDCG
		}

		// Pass6: Advect image
		Pass
		{
			Stencil
			{
				Ref 2
				Comp NotEqual
				Pass Keep
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_advectionImage
			ENDCG
		}
    }
}
