Shader "MobileExtented/Shader+MULTIFUNC"
{
	Properties
	{      
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Color texture", 2D) = "white" {}
        
        [ShaderFeatureToggle(F_AO, Ambient Occlusion)] _F_ao("Ambient Occlusion", Int) = 0
        [ShaderFeature(F_AO)] _AO ("Ambient Occlusion map", 2D) = "white" {}
        
        [ShaderFeatureToggle(F_NORMAL_MAP, Normal map)] _F_normal_map("Normal Map", Int) = 0
        [ShaderFeature(F_NORMAL_MAP)] _NormalMap ("Normal Map", 2D) = "white" {}
        
        [ShaderFeatureToggle(F_DIFFUSE, Diffuse)] _F_diffuse("Diffuse", Int) = 0
        [ShaderFeature(F_DIFFUSE)] _DiffuseRampTex ("Diffuse", 2D) = "white" {}
        
        [ShaderFeatureToggle(F_SPECULAR, Specular)] _F_specular("Specular", Int) = 0
        [ShaderFeature(F_SPECULAR)] _SpecTex ("Specular Texture", 2D) =  "white" {} 
        [ShaderFeature(F_SPECULAR)] _SpecShininess ("Specular Shininess", Range(0, 1)) = 0.5
        [ShaderFeature(F_SPECULAR)] _SpecIntensity ("Specular Intensity", Range(0, 10)) = 1

        [ShaderFeatureToggle(F_REFLECTION, Cubemap)] _F_reflection("Cubemap", Int) = 0
        [ShaderFeature(F_REFLECTION)] _ReflectionTex ("Cubemap Texture", 2D) =  "white" {} 
        [ShaderFeature(F_REFLECTION)] _ReflectionCube ("Cubemap Cube", Cube) = "" {}

        [ShaderFeatureToggle(F_EMISSION, Emission)] _F_emission_map("Emission", Int) = 0
        [ShaderFeature(F_EMISSION)] _EmissionMap("Emission Map", 2D) = "black" {}
        [ShaderFeature(F_EMISSION)] _EmissionMapColor("Emission Map Color", Color) = (0,0,0,1)
        [ShaderFeature(F_EMISSION)] _EmissionColor("Emission Color", Color) = (0,0,0,1)  
        
        [ShaderFeatureToggle(F_FALLOFF, Falloff)] _F_falloff("Falloff", Int) = 0
        [ShaderFeature(F_FALLOFF)] _FalloffColor ("Falloff Color", Color) = (1,1,1,1)  
        [ShaderFeature(F_FALLOFF)] _FalloffTex ("Falloff map", 2D) =  "white" {} 
        [ShaderFeature(F_FALLOFF)] _FalloffPower ("Falloff Power", Range(0, 10)) = 3.0
        [ShaderFeatureKeywordEnum(F_FALLOFF, Additive, Multiply)] _FalloffBlend ("Falloff Blend mode", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
                        
        ZTest Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        LOD 100
        
		Pass
		{        
			CGPROGRAM
            
            #pragma shader_feature F_AO
            #pragma shader_feature F_NORMAL_MAP
			#pragma shader_feature F_DIFFUSE
            #pragma shader_feature F_SPECULAR
            #pragma shader_feature F_REFLECTION
            #pragma shader_feature F_EMISSION
            #pragma shader_feature F_FALLOFF
            #pragma multi_compile _FALLOFFBLEND_ADDITIVE _FALLOFFBLEND_MULTIPLY   
             
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase


			#pragma vertex vert
			#pragma fragment frag
            
                     
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform fixed4 _Color;
            
            #if F_AO
            uniform sampler2D _AO;
            uniform fixed4 _AO_ST;
            #endif

            #if F_NORMAL_MAP
            uniform sampler2D _NormalMap;
            #endif                     

			#if F_DIFFUSE
            uniform fixed4 _DiffuseColor;
            sampler1D _DiffuseRampTex;
            #endif
           
            #if F_SPECULAR
            sampler2D _SpecTex;
            float _SpecShininess;
            float _SpecIntensity;
            #endif
            
            #if F_REFLECTION
            uniform sampler2D _ReflectionTex;
            uniform samplerCUBE _ReflectionCube;
            uniform fixed4 _ReflectionColor;             
            #endif
            
            #if F_EMISSION
            uniform sampler2D _EmissionMap;
            uniform fixed4 _EmissionMapColor;
            uniform fixed4 _EmissionColor;
            #endif
            
            #if F_FALLOFF
            uniform fixed4 _FalloffColor;
            sampler2D _FalloffTex;
            uniform float _FalloffPower;
            float _FalloffBlend;
            #endif
            

			struct vertexInput 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
                float4 tangent : TANGENT;

				float2 uv : TEXCOORD0;

				#if F_AO
				float2 uvAO : TEXCOORD1;
				#endif                                                                                             
			};

			struct vertexOutput 
			{
				float4 pos : SV_POSITION;

				float4 posWorld : TEXCOORD0;
				float2 uvMain : TEXCOORD1;
				float3 normalDir : TEXCOORD2;                

				#if F_AO
				float2 uvAO : TEXCOORD3;
				#endif
                
                #if F_NORMAL_MAP                       
                float3 tangentWorld : TEXCOORD4;  
                float3 normalWorld : TEXCOORD5;
                float3 binormalWorld : TEXCOORD6;             
                #endif
                
                LIGHTING_COORDS(7,8)
			};
            
            
			vertexOutput vert(vertexInput v) 
			{
				vertexOutput output;
                
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
                                                    
				output.posWorld = mul(modelMatrix, v.vertex);
				output.normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
				output.pos = UnityObjectToClipPos(v.vertex);

				output.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
                
                #if F_AO
                output.uvAO = TRANSFORM_TEX(v.uvAO, _AO);
                #endif
                
                #if F_NORMAL_MAP
                output.tangentWorld = UnityObjectToWorldDir(v.tangent.xyz);
                output.normalWorld = UnityObjectToWorldNormal(v.normal);
                output.binormalWorld = normalize(cross(output.normalWorld, output.tangentWorld) * v.tangent.w);
                #endif
                
                TRANSFER_VERTEX_TO_FRAGMENT(output);

				return output;
			}


			float4 frag(vertexOutput input) : SV_Target 
			{
                half3 worldNormal = normalize(input.normalDir);
                
                #if F_NORMAL_MAP               
                half3 tnormal = tex2D(_NormalMap, input.uvMain).rgb * 2 - 1;
                float3x3 local2WorldTranspose = float3x3(input.tangentWorld, input.binormalWorld, input.normalWorld);
                
                worldNormal = normalize(mul(tnormal, local2WorldTranspose));            
                #endif              
                            

				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
                
                float attenuation = 1.0;
                                
                fixed4 resultColor = tex2D(_MainTex, input.uvMain) * _Color;
                
                
                #if F_AO
                half3 ao = tex2D(_AO, input.uvAO);
                resultColor.rgb *= ao.rgb;
                #endif 
                
                
                #if F_SPECULAR
                float3 specularReflection;
                
                if (dot(worldNormal, lightDirection) < 0.0) 
                {
                    specularReflection = float3(0.0, 0.0, 0.0);
                } 
                else 
                {
                    fixed4 specularColor = tex2D(_SpecTex, input.uvMain);
                    specularReflection = _LightColor0.xyz * specularColor.rgb * _SpecIntensity * pow(max(0.0, dot(reflect(-lightDirection, worldNormal), viewDirection)), 199.0 * _SpecShininess + 1.0);
                }
                
                resultColor.rgb += specularReflection; 
                #endif  
                
                
                #if F_DIFFUSE
                half diff = 0.5 - dot(worldNormal, lightDirection) * 0.5;
                fixed3 diffuseReflection = tex1D(_DiffuseRampTex, diff).rgb;            
                resultColor.rgb *= diffuseReflection * _LightColor0.xyz * tex1D(_DiffuseRampTex, 1.0 - LIGHT_ATTENUATION(input)).rgb;
                #else
                resultColor.rgb *= LIGHT_ATTENUATION(input);                                
                #endif
                
                
                #if F_FALLOFF
                fixed4 falloffPower = tex2D(_FalloffTex, input.uvMain);
                
                float falloff = 1 - saturate(dot(normalize(viewDirection), worldNormal));
                float3 falloffLighting = attenuation * falloffPower.rgb * pow(falloff, _FalloffPower);
                
                #if _FALLOFFBLEND_ADDITIVE
                resultColor.rgb += falloffLighting * _FalloffColor.rgb * _LightColor0.w;
                #endif

                #if _FALLOFFBLEND_MULTIPLY                
                resultColor.rgb *= lerp(fixed3(1.0, 1.0, 1.0), _FalloffColor.rgb * _LightColor0.w, falloffLighting);
                #endif
                
                #endif
                
                
                #if F_REFLECTION
                fixed4 reflectionColor = tex2D(_ReflectionTex, input.uvMain);
                
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(input.posWorld));                
                half3 worldReflection = reflect(-worldViewDir, worldNormal);

                resultColor.rgb += reflectionColor.r * _LightColor0.w * texCUBE(_ReflectionCube, worldReflection).rgb;
                #endif
                
                
                #if F_EMISSION
                fixed3 emission = tex2D(_EmissionMap, input.uvMain).rgb * _EmissionMapColor.rgb + _EmissionColor.rgb;
                resultColor.rgb += emission;
                #endif
            
				return resultColor;
			}

			ENDCG
		}
        
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
