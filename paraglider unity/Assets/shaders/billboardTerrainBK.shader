Shader "Nature/Terrain/billboardBK"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_Scale("Scale", float) = 1.0
	}

		Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		Cull Off

		// important
		ZWrite off

		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		sampler2D _MainTex;
	fixed4 _TintColor;
	float _Scale;

	struct appdata_t
	{
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_P,
			mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
			+ float4(v.vertex.x, v.vertex.y, 0.0, 0.0)*_Scale);

		o.color = v.color;
		o.texcoord = float2(v.vertex.x + 0.5, v.vertex.y + 0.5);
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
	}
		ENDCG
	}
	}
	}
	/*
	Properties{
		//_MainTex("Texture Image", 2D) = "white" {}
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" { }
	_ScaleX("Scale X", Float) = 1.0
		_ScaleY("Scale Y", Float) = 1.0
	}
		SubShader{
		Pass{
		CGPROGRAM

#pragma vertex vert  
#pragma fragment frag

		// User-specified uniforms            
		uniform sampler2D _MainTex;
	uniform float _ScaleX;
	uniform float _ScaleY;

	struct vertexInput {
		float4 vertex : POSITION;
		float4 tex : TEXCOORD0;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 tex : TEXCOORD0;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		output.pos = mul(UNITY_MATRIX_P,
			mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
			+ float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
			* float4(_ScaleX, _ScaleY, 1.0, 1.0));

		output.tex = input.tex;

		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		return tex2D(_MainTex, float2(input.tex.xy));
	}

		ENDCG
	}
	}*/
}
