// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

Shader "Custom/PostMagnify" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

		CGINCLUDE

#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};


	uniform half4 _Color;

	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	half4 _MainTex_ST;

	
	half _Magnification;


	v2f vertRobert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		float2 uv = v.texcoord.xy;
		o.uv[0] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1 - uv.y;
#endif
		return o;
	}


		half4 fragRobert(v2f i) : SV_Target{

		
		return tex2D(_MainTex, i.uv[0]/_Magnification + 0.5 - 0.5/_Magnification);
	

	}

	

		ENDCG

		Subshader {
		
			Pass{
			ZTest Always Cull Off ZWrite On

			CGPROGRAM
#pragma vertex vertRobert
#pragma fragment fragRobert
			ENDCG
		}
			
			
	}

	Fallback off

} // shader
