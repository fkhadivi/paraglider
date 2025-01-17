Shader "Toon/Lit Outline BK" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.0005, 0.1)) = .005
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			UsePass "Toon/LitBK/FORWARD"
			UsePass "Toon/Basic Outline/OUTLINE"
	}

		Fallback "Toon/Lit"
}