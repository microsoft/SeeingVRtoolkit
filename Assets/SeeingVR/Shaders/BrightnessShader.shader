// Modified from Unity standard assets: Legacy Image effects
 // https://assetstore.unity.com/packages/essentials/legacy-image-effects-83913

Shader "Custom/BrightnessShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

		// Shader code pasted into all further CGPROGRAM blocks	
		CGINCLUDE

#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _MainTexBlurred;

	float4 _MainTex_TexelSize;
	half4 _MainTex_ST;

	half4 _MainTexBlurred_ST;

	float intensity;

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy, _MainTex_ST);
#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1 - o.uv.y;
#endif			
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half c = 1;
		half4 color = tex2D(_MainTex, i.uv);
		
		half red = c * pow(color.r, intensity);
		if (red < 0) red = 0;
		if (red > 1) red = 1;
		half green = c * pow(color.g, intensity);
		if (green < 0) green = 0;
		if (green > 1) green = 1;
		half blue = c * pow(color.b, intensity);
		if (blue < 0) blue = 0;
		if (blue > 1) blue = 1;

		return half4(red, green, blue, color.a);
	}

		ENDCG

		Subshader {
		Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}
