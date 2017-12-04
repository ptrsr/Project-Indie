Shader "custom/LaserFX"
{
	 CGINCLUDE
		 #include "UnityCG.cginc"
	 ENDCG
 
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"  "ForceNoShadowCasting" = "True" }
		
		ZWrite Off
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
			};
             
			struct v2f
			{
				float4 vertex			: POSITION1;
				float4 pos				: SV_POSITION;
                float  uv				: TEXCOORD0;
			};


			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = length(o.pos);

				return o;
			}

			uniform float  _time;
			uniform float  _effectMulti;
			uniform float  _darken;
			uniform float4 _color;
	
			uniform float3 _lastPos;
			uniform float _lastDist;
			uniform float _maxDist;
			uniform float2 _direction;


			float hash(float n);
			float noise3D(float3 x);

			half4 frag (v2f i) : COLOR
			{
				float fallOff = saturate(1 - (_lastDist + distance(_lastPos, i.vertex.xyz)) / _maxDist);

				float alpha = noise3D(float3((i.pos.xy + _direction * _time) / _effectMulti, 0));
				float4 color = float4(_color.xyz * pow(clamp(alpha, 0.3f, 1), _darken) * fallOff, _color.a * fallOff);

				return color;
			}

			float hash(float n)
			{
				return frac(sin(n)*43758.5453);
			}
 
			float noise3D(float3 x)
			{
				// The noise function returns a value in the range -1.0f -> 1.0f
 
				float3 p = floor(x);
				float3 f = frac(x);
 
				f       = f*f*(3.0-2.0*f);
				float n = p.x + p.y*57.0 + 113.0*p.z;
 
				return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
							   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
						   lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
							   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
			}
 

			ENDCG
		}
	}
	FallBack "Diffuse"
}
