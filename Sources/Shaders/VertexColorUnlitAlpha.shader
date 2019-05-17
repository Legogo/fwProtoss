Shader "FWP/Vertex color unlit (alpha)" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}
 
Category {

  Tags {Queue=Transparent}
	//Tags { "Queue"="Geometry" }
	
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha 

  Lighting Off

	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
 
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				Combine texture * primary DOUBLE
			}
		}
	}
}
}