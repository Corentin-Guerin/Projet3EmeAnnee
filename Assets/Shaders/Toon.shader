// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CelShading/Toon"
{
	Properties
	{
		
		[Header(Albedo)]
		_Albedo("Main Texture", 2D) = "white" {}
		[Space(10)]
	
		[Header(Ambient)]
		[HDR] _AmbientColor("Ambient Color", Color) = (0.4, 0.4, 0.4, 1)
		[Space(10)]

		[Header(Specular)]
		[HDR] _SpecularColor("Specular Color", Color) = (0.9, 0.9, 0.9, 1)
		[Space(10)]

		[Header(Glossiness)]
		_Glossiness("Glossiness", Float) = 32
		[Space(10)]

		[Header(Rim Light)]
		[Toggle(_)] _EnableRim("Enable Rim Light", float) = 0
		[HDR] _RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimTreshold("Rim Threshold", Range(0, 1)) = 0.1
		[Space(10)]

		[Header(Emissive)]
		_EmissionMap("Emission Map", 2D) = "black" {}
		[HDR]_EmissionColor("Emmission Color", Color) = (0, 0, 0, 1)
		_EmissionStrenght("Emission Strenght", Range(0, 1)) = 0
		[Space(10)]

		[Header(Outline)]
		_OutlineSize("Outline Size", Float) = 0.03
		_OutlineColor("OutlineColor", Color) = (0, 0, 0, 1)		
		
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				//"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			Stencil
			{
				Ref 4
				Comp Always
				Pass replace
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				//float4 position : POSITION;
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD1;
				float4 color : COLOR;
				//SHADOW_COORDS(2)
				LIGHTING_COORDS(1,2)
			};

			sampler2D _Albedo;
			float4 _Albedo_ST;

			

			v2f vert (appdata v)
			{
				v2f o;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Albedo);
				o.viewDir = WorldSpaceViewDir(v.vertex);

				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}
			
			float4 _Color;
			float4 _AmbientColor;
			float _Glossiness;
			float4 _SpecularColor;
			float4 _RimColor;
			float _RimAmount;
			float _EnableRim;
			float _RimTreshold;
			half _TintAlpha;

			sampler2D _EmissionMap;
			float4 _EmissionColor;
			float _EmissionStrenght;


			float4 frag (v2f i) : SV_Target
			{
				float4 sample = tex2D(_Albedo, i.uv);

				if (sample.a < _TintAlpha)
				{
					discard;
					return 0;
				}

				float viewDir = normalize(i.viewDir);

				//Light Calculation
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float NdotL2 = dot(_WorldSpaceLightPos0, i.worldNormal);

				float shadow = SHADOW_ATTENUATION(i);
				
				float lightIntensity = smoothstep(0, 0.002, NdotL * shadow);
				float lightIntensity2 = smoothstep(0, 0.002, (NdotL2 + 0.92) * shadow);
		
				float attenuation = LIGHT_ATTENUATION(i);

				float4 light = lightIntensity * _LightColor0 * attenuation / 20;
				float4 light2 = lightIntensity2 * _LightColor0 * attenuation;
				//End Light Calculation
				
				//Specular
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;
				//End Specular

				//Rim Light
				float4 rimDot = (1 - abs(dot(i.viewDir, i.worldNormal))) * _EnableRim;

				float rimIntensity = rimDot * pow(NdotL2, _RimTreshold); 
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;
				//End Rim Light

				//Emissive
				half4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor * (_EmissionStrenght * 8);
				sample.rgb += emission.rgb;
				half4 texColor = tex2D(_EmissionMap, i.uv);
				//End Emissive

				
				
				//Output
				if(texColor[0] == texColor[1] == texColor[2] && _EmissionStrenght > 0)
				{
					return sample * (_AmbientColor + specular + rim);
				}
				else 
				{
					return sample * (_AmbientColor + light + light2 + specular + rim);
					//return light;
				}
				
				
			}
			ENDCG
		}
	
			//UsePass "Standard/SHADOWCASTER"
		Pass 
        {
            Tags 
            {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }

        Pass 
        {
            Cull OFF
            ZWrite OFF
            ZTest ON

            Stencil        
			{
                Ref 4 
                Comp notequal
                Fail keep
                Pass replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
            #include "UnityCG.cginc"

            uniform float4 _OutlineColor;
            uniform float _OutlineSize;
            uniform float _OutlineExtrusion;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float4 screenCoord : TEXCOORD0;
                float4 color : COLOR;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;

                float4 newPos = input.vertex;

                float3 normal = normalize(input.normal);
                newPos += float4(normal, 0.0) * _OutlineSize;

                output.pos = UnityObjectToClipPos(newPos);

                output.color = _OutlineColor;
                return output;
            }


            float4 frag(vertexOutput input) : COLOR
            {
                float4 color = _OutlineColor;

                return input.color;
            }

            ENDCG
		}
			
		}
		
		Fallback "Diffuse"
	}
	
