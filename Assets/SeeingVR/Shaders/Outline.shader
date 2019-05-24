// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


Shader "Custom/Outline" {
	Properties {
		_Color ("Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline color", Color) = (0, 0, 1, 1)
		_OutlineWidth("Outine width", Range(1.0, 5.0)) = 2.01
		_CenterToPivot("Center", Vector) = (0, 0, 0, 1)
		_Origin("Origin", Vector) = (0, 0, 0)
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos: POSITION;
		float4 color: COLOR;
		float3 normal: NORMAL;
	};

	float _OutlineWidth;
	float4 _OutlineColor;
	float3 _CenterToPivot;

	v2f vert(appdata v) {
		float3 center = mul(unity_WorldToObject, _CenterToPivot);
		float3 tmp = v.vertex.xyz - center.xyz;
		tmp *= _OutlineWidth;
		v.vertex.xyz = tmp + center.xyz; 

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.color = _OutlineColor;
		return o;
	}

	ENDCG



	SubShader {
		Tags{"Queue" = "Transparent"}

		Pass //Render Outline
		{
			ZWrite Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				half4 frag(v2f i): COLOR {
					return i.color;
				}
			ENDCG
		} 

		Pass //Normnal render
		{
			ZWrite On
			Material {
				Diffuse[_Color]
				Ambient[_Color]
			}

			Lighting On
			SetTexture[_MainTex] {
				ConstantColor[_Color]
			}

			SetTexture[_MainTex] {
				Combine previous * primary DOUBLE
			}
		}
		
	}

}
