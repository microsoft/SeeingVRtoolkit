// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


Shader "Custom/ContrastShader" {
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
	}

	SubShader
	{
		LOD 100

		Tags
	{
			"Queue" = "Overlay"
	}

		Pass
	{

		ZTest Always ZWrite On

		SetTexture[_MainTex]
	{
		Combine Texture * Primary
	}
	}
	}
}
