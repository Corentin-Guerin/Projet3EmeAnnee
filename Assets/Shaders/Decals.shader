Shader "Custom/Tuto_Decal"
{
    Properties
    {
        [HDR] _Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
        "RenderType"="Transparent" 
        "Queue"="Transparent-400" 
        "DisableBatching"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 ray : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            sampler2D_float _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.position = UnityWorldToClipPos(worldPos);
                o.ray = worldPos - _WorldSpaceCameraPos;
                o.screenPos = ComputeScreenPos(o.position);
                return o;
            }

            float3 getProjectedObjectPos(float2 screenPos, float3 worldRay)
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenPos);
                depth = Linear01Depth(depth) * _ProjectionParams.z;

                worldRay = normalize(worldRay);

                worldRay /= dot(worldRay, -UNITY_MATRIX_V[2].xyz);

                float3 worldPos = _WorldSpaceCameraPos + worldRay * depth;
                float3 objectPos = mul(unity_WorldToObject, float4(worldPos,1)).xyz;

                clip(0.5 - abs(objectPos));

                objectPos += 0.5;

                return objectPos;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUv = i.screenPos.xy / i.screenPos.w;
                float2 uv = getProjectedObjectPos(screenUv, i.ray).xz;

                fixed4 col = tex2D(_MainTex, uv);

                col *= _Color;

                return col;
            }

            ENDCG
        }
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
    }
}
