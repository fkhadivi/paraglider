Shader "Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass" 
//Shader "Nature/Terrain/Toon"
{
		Properties{
			_Color("Main Color", Color) = (1,1,1,1)
			_OutlineColor("Outline Color", Color) = (0,0,0,1)
			_Outline("Outline width", Range(.002, 0.03)) = .005
			_MainTex("Base (RGB)", 2D) = "white" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "grey" {}
		}
			SubShader{
			Tags{ "RenderType" = "Opaque" }
			UsePass "Toon/Lighted/FORWARD"
			UsePass "Toon/Basic Outline/OUTLINE"
		}

			Fallback "Toon/Lighted"

	}