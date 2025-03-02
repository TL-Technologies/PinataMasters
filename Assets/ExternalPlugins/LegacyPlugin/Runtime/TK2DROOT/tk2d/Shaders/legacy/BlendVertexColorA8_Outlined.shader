// unlit, vertex colour, alpha blended
// cull off

Shader "tk2d/BlendVertexColorA8_Outlined" 
{
	Properties 
	{
		_MainTex ("Value (A)", 2D) = "white" {}
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineTreshold("Outline Treshold", float) = 0.6
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float4 _OutlineColor;
			uniform float _OutlineTreshold;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);

				col.rgb = i.color.rgb;

				if (col.a > 0 && col.a < _OutlineTreshold)
				{
				   col = _OutlineColor;
				}


				if (col.a > 0.01)
				{
					col.a = 1;
				}
				else
				{
					col.a = 0;
				}

				col.a *= i.color.a;

				return col;
			}
			
			ENDCG
		} 
	}
}
