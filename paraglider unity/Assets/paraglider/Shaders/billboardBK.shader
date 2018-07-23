Shader "Unlit/billboardBK"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_AlphaTex("Alpha Texture", 2D) = "white" {}
		_Scale("Scale", float) = 1.0
		_Rotation("Rotation ", Float) = 0.0
		_Xoffset("x offset ", Float) = 0.0
		_Yoffset("Y offset ", Float) = 0.0
	}

		Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		//Cull Off

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
		sampler2D _AlphaTex;
	fixed4 _TintColor;
	float _Scale;
	float _Xoffset;
	float _Yoffset;

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

	float _Rotation;

	v2f vert(appdata_t v)
	{
		float PiRot = _Rotation*0.017453;
		float sinX = sin(-PiRot);
		float cosX = cos(-PiRot);
		float sinY = sin(-PiRot);
		float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
		v.vertex.xy = mul(v.texcoord.xy, rotationMatrix);
		v.vertex.x += -0.5*sinX - 0.5*cosX + _Xoffset;
		v.vertex.y += -0.5*cosX + 0.5*sinX + _Yoffset;

		sinX = sin(PiRot);
		cosX = cos(PiRot);
		sinY = sin(PiRot); 

		rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
		

		v2f o;
		o.vertex = mul(	UNITY_MATRIX_P,   
						mul( UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0 ))
				   + float4(v.vertex.x, v.vertex.y, 0.0, 0.0)*_Scale);
		v.vertex.xy *= 1 / 1;

		v.vertex.xy = mul(v.vertex.xy, rotationMatrix);
		v.vertex.x -= _Xoffset;
		v.vertex.y -= _Yoffset;

		

		o.color = v.color;
		o.texcoord = float2(v.vertex.x + 0.5, v.vertex.y + 0.5);
		return o;
	}


	fixed4 frag(v2f i) : COLOR
	{
		fixed4 c = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
		c.a *= (tex2D(_AlphaTex, i.texcoord).r+tex2D(_AlphaTex, i.texcoord).g+tex2D(_AlphaTex, i.texcoord).b)/3;
		return c;
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
