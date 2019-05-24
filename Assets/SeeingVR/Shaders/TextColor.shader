// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

Shader "Custom/TextColor" {
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
	_Color("Text Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off Cull Off ZWrite Off Fog{ Mode Off }

		Pass{
			Color[_Color]
			AlphaTest Greater 0.5
			Blend SrcColor DstColor
			BlendOp Sub
			SetTexture[_MainTex]
				{
				combine previous, texture * primary
				}
		}
	}
}
