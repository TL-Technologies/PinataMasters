Shader "MobileExtented/Shader+SPRITES+SHADOW"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ShadowColorLight ("Shadow Color Light", Color) = (1,1,1,1)
        _ShadowColorMiddle ("Shadow Color Middle", Color) = (1,1,1,1)
        _ShadowColorDark ("Shadow Color Dark", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0        
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            
            uniform fixed4 _ShadowColorLight;
            uniform fixed4 _ShadowColorMiddle;
            uniform fixed4 _ShadowColorDark;
                        
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color = SampleSpriteTexture (IN.texcoord) * IN.color;
                color.rgb *= color.a;
                
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
