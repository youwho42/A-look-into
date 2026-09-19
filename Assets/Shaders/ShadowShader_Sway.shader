// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Custom/ShadowShader_Sway" {
	Properties {
		[PerRendererData] _MainTex ( "Sprite Texture", 2D ) = "white" {}

		_Color ( "Tint", Color ) = ( 0, 0, 0, 0.5 )

		// -- Wind sway (same variables/defaults as the SwayGraph shader graph) --
		_WindMovement ( "Wind Movement", Vector ) = ( 1.5, 0, 0, 0 )
		_WindDensity ( "Wind Density", Float ) = 2.0
		_WindStrength ( "Wind Strength", Float ) = 0.1
	}

	SubShader {
		Tags { "Queue"="Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "TransparentCutout"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True" }

		Pass {

			Stencil {
				Ref 0
				Comp Equal
				Pass DecrWrap
			}
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Always

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			fixed4 _Color;

			uniform float2 _WindMovement;
			uniform float _WindDensity;
			uniform float _WindStrength;

			struct v2f {
				half4 pos : POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			// --- Gradient noise, same algorithm as Shader Graph's Gradient Noise node ---
			float2 unity_gradientNoise_dir(float2 p) {
				p = fmod(p, 289.0);
				float x = fmod((34.0 * p.x + 1.0) * p.x, 289.0) + p.y;
				x = fmod((34.0 * x + 1.0) * x, 289.0);
				x = frac(x / 41.0) * 2.0 - 1.0;
				return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
			}

			float unity_gradientNoise(float2 p) {
				float2 ip = floor(p);
				float2 fp = frac(p);
				float d00 = dot(unity_gradientNoise_dir(ip), fp);
				float d01 = dot(unity_gradientNoise_dir(ip + float2(0.0, 1.0)), fp - float2(0.0, 1.0));
				float d10 = dot(unity_gradientNoise_dir(ip + float2(1.0, 0.0)), fp - float2(1.0, 0.0));
				float d11 = dot(unity_gradientNoise_dir(ip + float2(1.0, 1.0)), fp - float2(1.0, 1.0));
				fp = fp * fp * fp * (fp * (fp * 6.0 - 15.0) + 10.0);
				return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
			}

			v2f vert(appdata_img v) {
				v2f o;

				// Same steps as the graph: object-space position -> tiled/scrolled noise
				// sample -> displacement on X -> blended in by UV.y (root stays put, tip sways).
				float3 objPos = v.vertex.xyz;
				float2 noiseUV = (objPos.xy + _Time.y * _WindMovement) * _WindDensity;
				float displacement = unity_gradientNoise(noiseUV) * _WindStrength;

				float3 swayedPos = float3(objPos.x + displacement, objPos.y, objPos.z);
				float3 finalPos = lerp(objPos, swayedPos, v.texcoord.y);

				o.pos = UnityObjectToClipPos(float4(finalPos, 1.0));
				half2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
				o.uv = uv;
				o.color = _Color;
				return o;
			}

			half4 frag (v2f i) : COLOR {
				half4 color = tex2D(_MainTex, i.uv);
				if (color.a <= 0.3)
					discard;
				color = _Color;
				return color;
			}
			ENDCG
		}
	}

	Fallback off
}
