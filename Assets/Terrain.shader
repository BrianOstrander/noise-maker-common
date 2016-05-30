// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33162,y:32543,varname:node_4013,prsc:2|diff-6903-OUT;n:type:ShaderForge.SFN_Tex2d,id:351,x:32162,y:32237,ptovrint:False,ptlb:HeightMap,ptin:_HeightMap,varname:node_351,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:210,x:32313,y:32808,ptovrint:False,ptlb:Terrain1,ptin:_Terrain1,varname:node_210,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9318,x:32162,y:32595,ptovrint:False,ptlb:Terrain2,ptin:_Terrain2,varname:node_9318,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:66321cc856b03e245ac41ed8a53e0ecc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:8976,x:32313,y:32709,ptovrint:False,ptlb:Start1,ptin:_Start1,varname:node_8976,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.4;n:type:ShaderForge.SFN_ValueProperty,id:1811,x:32162,y:32427,ptovrint:False,ptlb:Start2,ptin:_Start2,varname:node_1811,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.6;n:type:ShaderForge.SFN_If,id:6903,x:32860,y:32317,varname:node_6903,prsc:2|A-351-R,B-1811-OUT,GT-9318-RGB,EQ-9318-RGB,LT-3175-OUT;n:type:ShaderForge.SFN_If,id:3175,x:32626,y:32678,varname:node_3175,prsc:2|A-351-R,B-8976-OUT,GT-210-RGB,EQ-210-RGB,LT-3038-RGB;n:type:ShaderForge.SFN_Tex2d,id:3038,x:32313,y:33033,ptovrint:False,ptlb:Terrain0,ptin:_Terrain0,varname:node_3038,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8295e63d56936e045b12cf42e6e62d43,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:3022,x:32162,y:32505,ptovrint:False,ptlb:End2,ptin:_End2,varname:node_3022,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_Lerp,id:2716,x:32920,y:32571,varname:node_2716,prsc:2|A-9318-RGB,B-3175-OUT,T-8086-OUT;n:type:ShaderForge.SFN_Subtract,id:5246,x:32416,y:32481,varname:node_5246,prsc:2|A-3022-OUT,B-1811-OUT;n:type:ShaderForge.SFN_Subtract,id:8302,x:32485,y:32326,varname:node_8302,prsc:2|A-1811-OUT,B-351-R;n:type:ShaderForge.SFN_Divide,id:5161,x:32601,y:32481,varname:node_5161,prsc:2|A-8302-OUT,B-5246-OUT;n:type:ShaderForge.SFN_Clamp01,id:8086,x:32754,y:32526,varname:node_8086,prsc:2|IN-5161-OUT;proporder:351-210-9318-1811-8976-3038;pass:END;sub:END;*/

Shader "Shader Forge/Terrain" {
    Properties {
        _HeightMap ("HeightMap", 2D) = "white" {}
        _Terrain1 ("Terrain1", 2D) = "bump" {}
        _Terrain2 ("Terrain2", 2D) = "white" {}
        _Start2 ("Start2", Float ) = 0.6
        _Start1 ("Start1", Float ) = 0.4
        _Terrain0 ("Terrain0", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _HeightMap; uniform float4 _HeightMap_ST;
            uniform sampler2D _Terrain1; uniform float4 _Terrain1_ST;
            uniform sampler2D _Terrain2; uniform float4 _Terrain2_ST;
            uniform float _Start1;
            uniform float _Start2;
            uniform sampler2D _Terrain0; uniform float4 _Terrain0_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _HeightMap_var = tex2D(_HeightMap,TRANSFORM_TEX(i.uv0, _HeightMap));
                float node_6903_if_leA = step(_HeightMap_var.r,_Start2);
                float node_6903_if_leB = step(_Start2,_HeightMap_var.r);
                float node_3175_if_leA = step(_HeightMap_var.r,_Start1);
                float node_3175_if_leB = step(_Start1,_HeightMap_var.r);
                float4 _Terrain0_var = tex2D(_Terrain0,TRANSFORM_TEX(i.uv0, _Terrain0));
                float3 _Terrain1_var = UnpackNormal(tex2D(_Terrain1,TRANSFORM_TEX(i.uv0, _Terrain1)));
                float3 node_3175 = lerp((node_3175_if_leA*_Terrain0_var.rgb)+(node_3175_if_leB*_Terrain1_var.rgb),_Terrain1_var.rgb,node_3175_if_leA*node_3175_if_leB);
                float4 _Terrain2_var = tex2D(_Terrain2,TRANSFORM_TEX(i.uv0, _Terrain2));
                float3 diffuseColor = lerp((node_6903_if_leA*node_3175)+(node_6903_if_leB*_Terrain2_var.rgb),_Terrain2_var.rgb,node_6903_if_leA*node_6903_if_leB);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _HeightMap; uniform float4 _HeightMap_ST;
            uniform sampler2D _Terrain1; uniform float4 _Terrain1_ST;
            uniform sampler2D _Terrain2; uniform float4 _Terrain2_ST;
            uniform float _Start1;
            uniform float _Start2;
            uniform sampler2D _Terrain0; uniform float4 _Terrain0_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _HeightMap_var = tex2D(_HeightMap,TRANSFORM_TEX(i.uv0, _HeightMap));
                float node_6903_if_leA = step(_HeightMap_var.r,_Start2);
                float node_6903_if_leB = step(_Start2,_HeightMap_var.r);
                float node_3175_if_leA = step(_HeightMap_var.r,_Start1);
                float node_3175_if_leB = step(_Start1,_HeightMap_var.r);
                float4 _Terrain0_var = tex2D(_Terrain0,TRANSFORM_TEX(i.uv0, _Terrain0));
                float3 _Terrain1_var = UnpackNormal(tex2D(_Terrain1,TRANSFORM_TEX(i.uv0, _Terrain1)));
                float3 node_3175 = lerp((node_3175_if_leA*_Terrain0_var.rgb)+(node_3175_if_leB*_Terrain1_var.rgb),_Terrain1_var.rgb,node_3175_if_leA*node_3175_if_leB);
                float4 _Terrain2_var = tex2D(_Terrain2,TRANSFORM_TEX(i.uv0, _Terrain2));
                float3 diffuseColor = lerp((node_6903_if_leA*node_3175)+(node_6903_if_leB*_Terrain2_var.rgb),_Terrain2_var.rgb,node_6903_if_leA*node_6903_if_leB);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
