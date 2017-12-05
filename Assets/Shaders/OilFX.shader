// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "custom/oil_fx"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }
 
    CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"
	#include "Noise.cginc"

    fixed4 _TextureSampleAdd;
 
    struct appdata
    {
        float4 vertex   : POSITION;
    };
 
    struct v2f
    {
        float4 vertex   : SV_POSITION;
		float4 worldPos : TEXCOORD0;
        float4  grabPos	: TEXCOORD1;
    };
 
    v2f vert(appdata i)
    {
        v2f o;
		o.worldPos = mul(unity_ObjectToWorld, i.vertex);
        o.vertex = UnityObjectToClipPos(i.vertex);
        o.grabPos = ComputeGrabScreenPos(o.vertex);

        return o;
    }
 
    uniform sampler2D _MainTex;
	sampler2D _BackgroundTexture;

	uniform float2 _TexelSize;

	float3 CalcNormal(float4 grabPos);

    fixed4 frag(v2f i) : SV_Target
    {
		float3 normal = CalcNormal(i.grabPos);

		float4 color = float4(snoise(i.worldPos.xz / 3 + float2(1000, 1000)), snoise(i.worldPos.xz / 3 + float2(2000, 2000)), snoise(i.worldPos.xz / 3 + float2(3000, 3000)), snoise(i.worldPos.xz / 100 + float2(5000, 5000)));

		float lit = pow(max(0, dot(normal, _WorldSpaceLightPos0)), 10);
		return float4(lit, lit, lit, (1 + snoise(i.worldPos.xz / 2)) / 2);
		//float4 bgcolor = tex2Dproj(_BackgroundTexture, grabPos);// *float4(normal, 1);


		return float4(normal, 1);
    }

	float calcLit(float4 grabPos, float2 offset)
	{
		grabPos.xy += offset;
		return length(tex2Dproj(_BackgroundTexture, grabPos)) / 3.0f;
	}

	float3 CalcNormal(float4 grabPos)
	{
		float2 texel = float2(1, 1) / _ScreenParams;

		float l = calcLit(grabPos, float2(texel.x, 0));
		float r = calcLit(grabPos, float2(-texel.x, 0));
		float t = calcLit(grabPos, float2(0, -texel.y));
		float b = calcLit(grabPos, float2(0, texel.y));


		float d1 = 0.5f + (l - r) * 100;
		float d2 = 0.5f + (t - b) * 100;

		float2 test = float2(d1, d2);
		float3 normal = normalize(float3(test.x, 1, test.y));

		return normal;
	}

    ENDCG
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

		GrabPass
		{
			"_BackgroundTexture"
		}
 
		Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        ENDCG
        }
    }
}
