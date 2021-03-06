﻿Shader "Custom/DarknessShader"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AdditionTex("Additional smoke", 2D) = "white" {}

	_Pixels("Pixels in a quad", Float) = 2048

		_Transmission("Transmission", Vector) = (5, 0, 0, 0)
		_Dissipation("Dissipation", Range(0,1)) = 0.1
		_AdditionOffset("Additive offset", Vector) = (0, 0, 0, 0)

	}

		SubShader
	{
		// Required to work
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		Pass
	{
		CGPROGRAM

		//#pragma vertex vert_img
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
	sampler2D _AdditionTex;
	float4 _AdditionOffset;
	half _Pixels;

	half4 _Transmission;
	half _Dissipation;
	const float pi = 3.1415;
	const float e = 2.71828;


	struct vertOutput {
		float4 pos : SV_POSITION;	// Clip space
		float2 uv : TEXCOORD0;		// UV data
		float3 wPos : TEXCOORD1;	// World position
	};

	vertOutput vert(appdata_full v)
	{
		vertOutput o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		o.wPos = mul(_Object2World, v.vertex).xyz;
		return o;
	}


	//float4 frag(v2f_img i) : COLOR
	float4 frag(vertOutput i) : COLOR
	{
		// Cell centre
		fixed2 uv = round(i.uv * _Pixels) / _Pixels;

	half s = 1 / _Pixels;

	float cl = tex2D(_MainTex, uv + fixed2(-s, 0)).a;	// Centre Left
	float tc = tex2D(_MainTex, uv + fixed2(-0, -s)).a;	// Top Centre
	float cc = tex2D(_MainTex, uv + fixed2(0, 0)).a;	// Centre Centre
	float bc = tex2D(_MainTex, uv + fixed2(0, +s)).a;	// Bottom Centre
	float cr = tex2D(_MainTex, uv + fixed2(+s, 0)).a;	// Centre Right



	float factor =
		_Dissipation *
		(
			(
				cl * _Transmission.x +
				tc * _Transmission.y +
				bc * _Transmission.z +
				cr * _Transmission.w
				)
			- (_Transmission.x + _Transmission.y + _Transmission.z + _Transmission.w) * cc
			);

	if (factor >= -0.003 && factor < 0.0)
		factor = -0.003;
	cc += factor;

	cc += tex2D(_AdditionTex, i.uv + _AdditionOffset.xy).a * .01;


	return float4(0.01, 0.01, 0.01, cc);
	}
		ENDCG
	}
	}
}
