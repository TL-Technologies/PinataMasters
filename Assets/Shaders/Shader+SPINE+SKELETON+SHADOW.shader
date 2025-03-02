Shader "MobileExtented/Shader+SPINE+SKELETON+SHADOW" 
{
    Properties 
    {       
        _MainTex ("Main Texture", 2D) = "black" {}
            
        _ShadowColorLight ("Shadow Color Light", Color) = (1,1,1,1)
        _ShadowColorMiddle ("Shadow Color Middle", Color) = (1,1,1,1)
        _ShadowColorDark ("Shadow Color Dark", Color) = (1,1,1,1)
    }
    
    SubShader 
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
               
        Pass 
        {      
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Lighting Off
   
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
                 
            struct appdata 
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            uniform fixed4 _ShadowColorLight;
            uniform fixed4 _ShadowColorMiddle; 
            uniform fixed4 _ShadowColorDark;

            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target 
            {
                fixed4 color = tex2D(_MainTex, i.uv);       
                float t = (color.r + color.g + color.b) / 3.0 / (color.a + 0.1f);

                if (t > 0.5)
                {
                    color.rgb = lerp(_ShadowColorMiddle.rgb, _ShadowColorLight.rgb, t);
                }
                else
                {
                    color.rgb = lerp(_ShadowColorDark.rgb, _ShadowColorMiddle.rgb, t);
                }              
                
                return color;
            }
            
            ENDCG
        }
    }
}
