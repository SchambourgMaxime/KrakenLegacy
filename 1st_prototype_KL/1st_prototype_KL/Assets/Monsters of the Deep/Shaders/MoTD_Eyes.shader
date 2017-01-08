// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:Bumped Specular,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-50-OUT,spec-19-OUT,gloss-24-OUT,lwrap-96-OUT;n:type:ShaderForge.SFN_Cubemap,id:4,x:33383,y:32652,ptlb:Reflection,ptin:_Reflection,cube:16301b190bd755c4ba26ad57de1d2c63,pvfc:0|DIR-14-OUT;n:type:ShaderForge.SFN_Fresnel,id:6,x:33516,y:32853|EXP-68-OUT;n:type:ShaderForge.SFN_NormalVector,id:7,x:33966,y:32721,pt:False;n:type:ShaderForge.SFN_Multiply,id:14,x:33562,y:32652|A-16-OUT,B-7-OUT;n:type:ShaderForge.SFN_ViewVector,id:16,x:33966,y:32557;n:type:ShaderForge.SFN_Slider,id:19,x:33810,y:33010,ptlb:Specular,ptin:_Specular,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:24,x:33810,y:33086,ptlb:Gloss,ptin:_Gloss,min:0,cur:0.472495,max:1;n:type:ShaderForge.SFN_Tex2d,id:26,x:33966,y:32398,ptlb:Diffuse,ptin:_Diffuse,tex:72622ef605100764986d58e2009c170a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:50,x:33030,y:32633|A-26-RGB,B-4-RGB,T-6-OUT;n:type:ShaderForge.SFN_Slider,id:68,x:33810,y:32916,ptlb:Fresnel,ptin:_Fresnel,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:84,x:33230,y:32955,ptlb:SSS Tint,ptin:_SSSTint,glob:False,c1:0.4132785,c2:0.6007338,c3:0.6176471,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:95,x:33230,y:33144,ptlb:SSS Factor,ptin:_SSSFactor,glob:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:96,x:32991,y:33003|A-84-RGB,B-95-OUT;proporder:26-4-68-19-24-84-95;pass:END;sub:END;*/

Shader "Monsters of the Deep/MotD Eyes" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Reflection ("Reflection", Cube) = "_Skybox" {}
        _Fresnel ("Fresnel", Range(0, 1)) = 0
        _Specular ("Specular", Range(0, 1)) = 1
        _Gloss ("Gloss", Range(0, 1)) = 0.472495
        _SSSTint ("SSS Tint", Color) = (0.4132785,0.6007338,0.6176471,1)
        _SSSFactor ("SSS Factor", Float ) = 3
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
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
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Reflection;
            uniform float _Specular;
            uniform float _Gloss;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Fresnel;
            uniform float4 _SSSTint;
            uniform float _SSSFactor;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = (_SSSTint.rgb*_SSSFactor)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 diffuse = forwardLight * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_108 = i.uv0;
                finalColor += diffuseLight * lerp(tex2D(_Diffuse,TRANSFORM_TEX(node_108.rg, _Diffuse)).rgb,texCUBE(_Reflection,(viewDirection*i.normalDir)).rgb,pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel));
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Reflection;
            uniform float _Specular;
            uniform float _Gloss;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Fresnel;
            uniform float4 _SSSTint;
            uniform float _SSSFactor;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = (_SSSTint.rgb*_SSSFactor)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 diffuse = forwardLight * attenColor;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_109 = i.uv0;
                finalColor += diffuseLight * lerp(tex2D(_Diffuse,TRANSFORM_TEX(node_109.rg, _Diffuse)).rgb,texCUBE(_Reflection,(viewDirection*i.normalDir)).rgb,pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel));
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Bumped Specular"
    CustomEditor "ShaderForgeMaterialInspector"
}
