// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:True,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:744,x:37325,y:31956,varname:node_744,prsc:2|normal-1661-OUT,emission-1650-OUT,alpha-4155-OUT;n:type:ShaderForge.SFN_Cubemap,id:3739,x:36476,y:32510,ptovrint:False,ptlb:cubemap,ptin:_cubemap,varname:_cubemap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:9a05250de96f90449a664ebda8d4bb18,pvfc:1;n:type:ShaderForge.SFN_Slider,id:4155,x:36463,y:32850,ptovrint:False,ptlb:opacity,ptin:_opacity,varname:_opacity,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4375673,max:1;n:type:ShaderForge.SFN_ValueProperty,id:8380,x:36160,y:32418,ptovrint:False,ptlb: tile 1,ptin:_tile1,varname:_tile1,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Tex2d,id:7902,x:36858,y:31797,varname:_no4,prsc:2,tex:aba06f8af729c9e4fa841a4dc7d578c5,ntxv:3,isnm:True|UVIN-6239-OUT,TEX-8173-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:8173,x:36599,y:31705,ptovrint:False,ptlb:normal,ptin:_normal,varname:_normal,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:aba06f8af729c9e4fa841a4dc7d578c5,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:4018,x:36858,y:31987,varname:_node_4018,prsc:2,tex:aba06f8af729c9e4fa841a4dc7d578c5,ntxv:0,isnm:False|UVIN-6059-OUT,TEX-8173-TEX;n:type:ShaderForge.SFN_ValueProperty,id:7834,x:36198,y:32127,ptovrint:False,ptlb: tile 2,ptin:_tile2,varname:_tile2,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Time,id:9424,x:36274,y:31663,varname:node_9424,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9071,x:36443,y:31846,varname:node_9071,prsc:2|A-6151-OUT,B-9424-TSL;n:type:ShaderForge.SFN_ValueProperty,id:6151,x:36274,y:31846,ptovrint:False,ptlb:speed 1,ptin:_speed1,varname:_speed1,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:5938,x:36261,y:31992,ptovrint:False,ptlb:speed 2,ptin:_speed2,varname:_speed2,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1855,x:36443,y:31978,varname:node_1855,prsc:2|A-5938-OUT,B-9424-TSL;n:type:ShaderForge.SFN_Add,id:6239,x:36825,y:32153,varname:node_6239,prsc:2|A-9071-OUT,B-173-OUT;n:type:ShaderForge.SFN_Add,id:6059,x:36825,y:32330,varname:node_6059,prsc:2|A-1855-OUT,B-8707-OUT;n:type:ShaderForge.SFN_NormalBlend,id:1661,x:37116,y:31881,varname:node_1661,prsc:2|BSE-7902-RGB,DTL-4018-RGB;n:type:ShaderForge.SFN_TexCoord,id:7231,x:36125,y:32230,varname:node_7231,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:173,x:36481,y:32188,varname:node_173,prsc:2|A-7231-UVOUT,B-7834-OUT;n:type:ShaderForge.SFN_Multiply,id:8707,x:36642,y:32245,varname:node_8707,prsc:2|A-7231-UVOUT,B-8380-OUT;n:type:ShaderForge.SFN_Color,id:3423,x:36605,y:32671,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:1,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:5996,x:36784,y:32639,varname:node_5996,prsc:2|A-3423-RGB,B-3423-RGB;n:type:ShaderForge.SFN_Multiply,id:1650,x:36976,y:32584,varname:node_1650,prsc:2|A-3739-RGB,B-5996-OUT;proporder:3739-4155-8380-8173-7834-6151-5938-3423;pass:END;sub:END;*/

Shader "Almgp/cristal/cristalWater" {
    Properties {
        _cubemap ("cubemap", Cube) = "_Skybox" {}
        _opacity ("opacity", Range(0, 1)) = 0.4375673
        _tile1 (" tile 1", Float ) = 4
        _normal ("normal", 2D) = "bump" {}
        _tile2 (" tile 2", Float ) = 4
        _speed1 ("speed 1", Float ) = 1
        _speed2 ("speed 2", Float ) = 1
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform samplerCUBE _cubemap;
            uniform fixed _opacity;
            uniform fixed _tile1;
            uniform sampler2D _normal; uniform float4 _normal_ST;
            uniform fixed _tile2;
            uniform half _speed1;
            uniform half _speed2;
            uniform half4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9424 = _Time + _TimeEditor;
                float2 node_6239 = ((_speed1*node_9424.r)+(i.uv0*_tile2));
                float3 _no4 = UnpackNormal(tex2D(_normal,TRANSFORM_TEX(node_6239, _normal)));
                float2 node_6059 = ((_speed2*node_9424.r)+(i.uv0*_tile1));
                float3 _node_4018 = UnpackNormal(tex2D(_normal,TRANSFORM_TEX(node_6059, _normal)));
                float3 node_1661_nrm_base = _no4.rgb + float3(0,0,1);
                float3 node_1661_nrm_detail = _node_4018.rgb * float3(-1,-1,1);
                float3 node_1661_nrm_combined = node_1661_nrm_base*dot(node_1661_nrm_base, node_1661_nrm_detail)/node_1661_nrm_base.z - node_1661_nrm_detail;
                float3 node_1661 = node_1661_nrm_combined;
                float3 normalLocal = node_1661;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float3 emissive = (texCUBE(_cubemap,viewReflectDirection).rgb*(_Color.rgb+_Color.rgb));
                float3 finalColor = emissive;
                return fixed4(finalColor,_opacity);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
