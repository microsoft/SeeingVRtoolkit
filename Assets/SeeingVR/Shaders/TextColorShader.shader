// based on https://www.reddit.com/r/Unity3D/comments/1onw43/invert_colors_shader/

Shader "ddShaders/ddInvert" {
//ddInvert shader: Daniel DeEntremont
//Apply this shader to a mesh and watch all pixels behind the mesh become inverted!
    Properties
        {
            _Color ("Tint Color", Color) = (1,1,1,1)
        }
   
        SubShader
        {
            Tags { "Queue"="Transparent" }
 
            Pass
            {
               ZWrite On
               ColorMask 0
            }
	    	Pass
	        {
			Blend OneMinusDstColor OneMinusSrcAlpha 
			BlendOp Add
		}
		
         }//end subshader
}//end shader