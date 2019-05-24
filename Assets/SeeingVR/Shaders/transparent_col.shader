// Based on a solution on Unity Community
 // https://answers.unity.com/questions/1041383/set-one-color-transparent-in-texture-shader.html

Shader "Custom/transparent_col" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_TransparentColor("Transparent Color", Color) = (1,1,1,1)
		_Threshold("Threshhold", Float) = 0.1
		_MainTex("Albedo (RGBA)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		
		ZTest Always ZWrite On

		CGPROGRAM
#pragma surface surf Lambert alpha

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	fixed4 _Color;
	fixed4 _TransparentColor;
	half _Threshold;

	void surf(Input IN, inout SurfaceOutput o) {
		// Read color from the texture
		half4 c = tex2D(_MainTex, IN.uv_MainTex);

		// Output colour will be the texture color * the vertex colour
		half4 output_col = c * (1, 1, 1, 1);

		//calculate the difference between the texture color and the transparent color
		//note: we use 'dot' instead of length(transparent_diff) as its faster, and
		//although it'll really give the length squared, its good enough for our purposes!
		half3 transparent_diff = c.xyz - _TransparentColor.xyz;
		half transparent_diff_squared = dot(transparent_diff,transparent_diff);

		//if colour is too close to the transparent one, discard it.
		//note: you could do cleverer things like fade out the alpha
		if (transparent_diff_squared < _Threshold)
		{
			discard;
		}
		else {
			o.Alpha = 1;
		}

		//output albedo and alpha just like a normal shader
		o.Albedo = _Color.rgb;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
