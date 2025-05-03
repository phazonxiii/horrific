Shader "HorrorEngine/PreRenderedBackground"
{
    Properties
    {
        _ColorTex ("Color", 2D) = "white" {}
        _DepthTex ("Depth", 2D) = "white" {}
       // _ShadowBias("ShadowBias", Float) = 0.001
        _DepthFactor("DepthFactor", Float) = 100
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
        }
        Cull Off Zwrite On
        
        Pass
        {
            Name "FORWARD"
            Lighting On
            Tags {"LightMode" = "ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodynlightmap novertexlight
        
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // shadow parameters to pass from vertex
            };

            sampler2D _DepthTex;
            float4 _DepthTex_ST;
            sampler2D _ColorTex;
            float4 _ColorTex_ST;
            
            //float _ShadowBias;
            //float _ShadowFactor;
            sampler2D _CameraDepthTexture;
            
            v2f vert (appdata_tan  v)
            {
                v2f o;
                // A regular quad goes from -0.5 to 0.5 so multiply by 2 to cover all clip space. Also inverse y
                o.pos = float4(v.vertex.x*2, -v.vertex.y*2, 0, 1);
                o.uv = TRANSFORM_TEX(v.texcoord, _ColorTex);
                
                TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
                return o;
            }

            fixed4 frag (v2f i, out float depth : SV_Depth) : SV_Target
            {
                fixed4 col = tex2D(_ColorTex, i.uv);
                
                fixed4 depthCol = SAMPLE_DEPTH_TEXTURE(_DepthTex, i.uv);
                depth = depthCol.r;

                UNITY_LIGHT_ATTENUATION(atten, i, 0)

                 float sceneDepth = tex2D(_CameraDepthTexture, i.uv).r;

                // TODO - This is a very dodgy shadow implementation. Ideally we use a normal buffer and do ndotL, not having shadows on the color render               
                float3 finalCol = _LightColor0.rgb * saturate(atten);
                finalCol.rgb += UNITY_LIGHTMODEL_AMBIENT;
                finalCol.rgb *= col.rgb;
                col.rgb = lerp(min(finalCol.rgb, col.rgb), col.rgb, saturate(atten+0.5 + 1 - saturate((depth - sceneDepth))));

                return float4(col.rgb, 1);
            }
            ENDCG
        }
        
         // Forward additive pass (needed for additional lights).
         Pass
         {
            Name "FORWARD_DELTA"
            
            Tags { "LightMode" = "ForwardAdd" }

            Blend One One
            ZWrite Off
            ZTest Always
            
            CGPROGRAM
           
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1)
                float3 ray : TEXCOORD2;
            };

            sampler2D _DepthTex;
            float4 _DepthTex_ST;
            
            sampler2D _ColorTex;
            float4 _ColorTex_ST;

            sampler2D _CameraDepthTexture;
            float _ShadowBias;
            float _DepthFactor;

            v2f vert(appdata_full v)
            {
                v2f o;
                // A regular quad goes from -0.5 to 0.5 so multiply by 2 to cover all clip space. Also inverse y
                o.pos = float4(v.vertex.x * 2, -v.vertex.y * 2, 1, 1);
                o.uv = TRANSFORM_TEX(v.texcoord, _ColorTex);
                
                // view space vertex direction
                float4 p = v.vertex;
                p.x = v.texcoord.x *2-1;
                p.y = v.texcoord.y *2-1;
                p.z = -1;
                p.w = -1;
                o.ray = mul(unity_CameraInvProjection, p) * _ProjectionParams.z;

                TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 col = tex2D(_ColorTex, i.uv);

                float nlDepth = tex2D(_DepthTex, i.uv).r;
                
                // then later in fragment shader, use eye space direction with linear depth to get eye space position
                float lDepth = Linear01Depth(nlDepth);
                float3 viewPos = i.ray * lDepth;
                
                // use eye space position to get world space position
                float3 worldPos = mul(unity_CameraToWorld, float4(viewPos.xyz, 1)).xyz;
                
                UNITY_LIGHT_ATTENUATION(atten, i, worldPos)
                fixed4 c = atten;
               
                float sceneDepth = tex2D(_CameraDepthTexture, i.uv).r;
               
               // float3 viewPos2 = i.ray * Linear01Depth(sceneDepth);
                //float zDiff = length(viewPos.xyz - viewPos2.xyz);
               
                float lSceneDepth = Linear01Depth(sceneDepth);
                return float4(lerp(c.rgb * col.rgb * _LightColor0.rgb, 0, lSceneDepth < lDepth), 1);
            } 

            ENDCG
        }
     
    }
}
