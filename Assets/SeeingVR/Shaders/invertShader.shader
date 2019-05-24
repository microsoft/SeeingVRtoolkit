// Modified from Unity documentation
 // https://docs.unity3d.com/540/Documentation/Manual/SL-GrabPass.html

Shader "GrabPassInvert"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_Color("Text Color", Color) = (1,1,1,1)
	}

	
	SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
	{
		"_BackgroundTexture"
	}

		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		// Render the object with the texture generated above, and invert the colors
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

		struct v2f
	{
		float4 grabPos : TEXCOORD0;
		float4 pos : SV_POSITION;
		float2 texcoord : TEXCOORD1;
		fixed4 color : COLOR;
	};
		sampler2D _MainTex;

	v2f vert(appdata_t v) {
		v2f o;
		// use UnityObjectToClipPos from UnityCG.cginc to calculate 
		// the clip-space of the vertex
		o.pos = UnityObjectToClipPos(v.vertex);
		// use ComputeGrabScreenPos function from UnityCG.cginc
		// to get the correct texture coordinate
		o.grabPos = ComputeGrabScreenPos(o.pos);
		o.texcoord = v.texcoord;
		o.color = v.color;
		return o;
	}

	sampler2D _BackgroundTexture;

	inline float4 contrastColor(half4 color)
	{
		int d = 0;

		float a = 1 - (0.299 * color.r + 0.587 * color.g + 0.114 * color.b);
		if (a <= 0.6)
		{
			d = 0;
		}
		else
			d = 1;

		return float4(d, d, d, 1);
	}

	half4 frag(v2f i) : SV_Target
	{
		float4 startColor = tex2D(_MainTex, i.texcoord);
		half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
		return half4((1-bgcolor).rgb, startColor.a);
	}
		ENDCG
	}

	}

		
}
