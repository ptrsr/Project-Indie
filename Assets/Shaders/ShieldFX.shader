Shader "custom/ShieldFX"
{
	 CGINCLUDE
		 #include "UnityCG.cginc"
		#include "Noise.cginc"

	 ENDCG
 
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"  "ForceNoShadowCasting" = "True" }
		
		GrabPass
		{
			"_BackgroundTexture"
		}

        Blend SrcAlpha OneMinusSrcAlpha

		Pass 
		{ 
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			struct appdata
			{
				half4	vertex	:	POSITION;
				half3   normal  :   NORMAL;
				half2   uv		:	TEXCOORD0;
			};
             
			struct v2f
			{
				float4 pos				: SV_Position;
				float4 vertex			: TEXCOORD0;

				float3 normal			: TEXCOORD1;
                float2 uv				: TEXCOORD2;
				float4 grabPos			: TEXCOORD3;

			};


			v2f vert (appdata v)
			{
				v2f o;
				
				o.vertex = mul(unity_ObjectToWorld, v.vertex);
				
				o.pos = UnityObjectToClipPos(v.vertex);
				
				o.uv = v.uv;
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				o.grabPos = ComputeGrabScreenPos(o.pos);

				return o;
			}

			uniform sampler2D _MainTex;
			uniform sampler2D _BackgroundTexture;

			uniform float _tiling;
			uniform float _fallOff;
			uniform float _warp;
			uniform float _noiseMulti;
			uniform float4 _baseColor;
			uniform float _time;
			uniform float _noisePow;

			half4 frag (v2f i) : COLOR
			{
				float4 hex = tex2D(_MainTex, i.uv / _tiling);
				float3 camToFrag = normalize(_WorldSpaceCameraPos - i.vertex);

				float product = max(0, pow(1 - dot(camToFrag, i.normal), _fallOff));

				float2 screenNormal = mul(UNITY_MATRIX_V, i.normal);

				float hexValue = saturate(1 - length(hex.rgb));

				float noise = pow(min(1 + snoise((screenNormal + _time) * _noiseMulti) / 2, 1), _noisePow);

				float4 grabPos = i.grabPos + float4(screenNormal.x, screenNormal.y, 0, 0) * _warp * (1 + product * 3);
				float3 background = tex2Dproj(_BackgroundTexture, grabPos);

				float3 color = lerp(background, _baseColor.rgb, (hexValue + product) * noise);

				return float4(color, 1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
