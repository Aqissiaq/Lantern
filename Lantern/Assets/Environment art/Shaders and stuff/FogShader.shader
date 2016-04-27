Shader "Custom/FogShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_XSpeed ("XSpeed", Range(-0.1, 0.1)) = 0
		_YSpeed("YSpeed", Range(-0.1, 0.1)) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 100
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float _XSpeed;
			float _YSpeed;

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float xOffset = mul(_XSpeed, _Time[1]);
				float yOffset = mul(_YSpeed, sin(10 * i.uv.x));
				float2 d = float2(fmod(i.uv.x + xOffset, 3840), fmod(i.uv.y + yOffset, 2500));
				fixed4 col = tex2D(_MainTex, d);
				return col;
			}
			ENDCG
		}
	}
}
