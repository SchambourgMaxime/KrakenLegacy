#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:Bumped Specular,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:10,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32266,y:32157|diff-2-RGB,spec-747-OUT,gloss-426-OUT,normal-8-RGB,emission-412-OUT,transm-655-OUT,lwrap-765-RGB;n:type:ShaderForge.SFN_Tex2d,id:2,x:34088,y:32543,ptlb:Diffuse (Spec A),ptin:_DiffuseSpecA,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8,x:34088,y:32353,ptlb:Normal,ptin:_Normal,tex:9383a15144b8542488fbe967b3f334b8,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:20,x:33145,y:33104,ptlb:Specularity,ptin:_Specularity,min:0,cur:0.4618056,max:1;n:type:ShaderForge.SFN_Tex2d,id:79,x:34088,y:32859,ptlb:EmissionMap,ptin:_EmissionMap,tex:68b7916e2184d7949ab21cb386d56973,ntxv:0,isnm:False|UVIN-91-UVOUT;n:type:ShaderForge.SFN_Panner,id:91,x:34329,y:32859,spu:0,spv:0.0025;n:type:ShaderForge.SFN_VertexColor,id:183,x:33765,y:33061;n:type:ShaderForge.SFN_Lerp,id:351,x:33465,y:32889|A-354-OUT,B-783-OUT,T-183-R;n:type:ShaderForge.SFN_Vector3,id:354,x:33465,y:32813,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Slider,id:411,x:33145,y:33186,ptlb:PhotoforeEmission,ptin:_PhotoforeEmission,min:0,cur:2.717949,max:3;n:type:ShaderForge.SFN_Multiply,id:412,x:33135,y:32852|A-351-OUT,B-411-OUT;n:type:ShaderForge.SFN_Slider,id:426,x:33145,y:33281,ptlb:Gloss,ptin:_Gloss,min:0,cur:0.5943498,max:1;n:type:ShaderForge.SFN_Vector1,id:655,x:32889,y:32522,v1:0.35;n:type:ShaderForge.SFN_Multiply,id:747,x:32963,y:32023|A-2-A,B-20-OUT;n:type:ShaderForge.SFN_Color,id:765,x:32845,y:33186,ptlb:SSS Color,ptin:_SSSColor,glob:False,c1:0.9137255,c2:0.6862745,c3:0.3215686,c4:0;n:type:ShaderForge.SFN_Color,id:782,x:34088,y:33053,ptlb:Emission Tint,ptin:_EmissionTint,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:783,x:33765,y:32931|A-79-RGB,B-782-RGB;proporder:2-8-782-79-411-20-426-765;pass:END;sub:END;*/

Shader "Monsters of the Deep/MotD_Body" {
    Properties {
        _DiffuseSpecA ("Diffuse (Spec A)", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _EmissionTint ("Emission Tint", Color) = (0.5,0.5,0.5,1)
        _EmissionMap ("EmissionMap", 2D) = "white" {}
        _PhotoforeEmission ("PhotoforeEmission", Range(0, 3)) = 2.717949
        _Specularity ("Specularity", Range(0, 1)) = 0.4618056
        _Gloss ("Gloss", Range(0, 1)) = 0.5943498
        _SSSColor ("SSS Color", Color) = (0.9137255,0.6862745,0.3215686,0)
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
            uniform float4 _TimeEditor;
            uniform sampler2D _DiffuseSpecA; uniform float4 _DiffuseSpecA_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Specularity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float _PhotoforeEmission;
            uniform float _Gloss;
            uniform float4 _SSSColor;
            uniform float4 _EmissionTint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                float3 shLight : TEXCOORD7;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.shLight = ShadeSH9(float4(mul(_Object2World, float4(v.normal,0)).xyz * 1.0,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_812 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_812.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = _SSSColor.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float node_655 = 0.35;
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_655,node_655,node_655);
                float3 diffuse = (forwardLight+backLight) * attenColor;
////// Emissive:
                float4 node_813 = _Time + _TimeEditor;
                float2 node_91 = (node_812.rg+node_813.g*float2(0,0.0025));
                float3 emissive = (lerp(float3(0,0,0),(tex2D(_EmissionMap,TRANSFORM_TEX(node_91, _EmissionMap)).rgb*_EmissionTint.rgb),i.vertexColor.r)*_PhotoforeEmission);
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_747 = (tex2D(_DiffuseSpecA,TRANSFORM_TEX(node_812.rg, _DiffuseSpecA)).a*_Specularity);
                float3 specularColor = float3(node_747,node_747,node_747);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                diffuseLight += i.shLight; // Per-Vertex Light Probes / Spherical harmonics
                finalColor += diffuseLight * tex2D(_DiffuseSpecA,TRANSFORM_TEX(node_812.rg, _DiffuseSpecA)).rgb;
                finalColor += specular;
                finalColor += emissive;
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
            uniform float4 _TimeEditor;
            uniform sampler2D _DiffuseSpecA; uniform float4 _DiffuseSpecA_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Specularity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float _PhotoforeEmission;
            uniform float _Gloss;
            uniform float4 _SSSColor;
            uniform float4 _EmissionTint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_814 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_814.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = _SSSColor.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float node_655 = 0.35;
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_655,node_655,node_655);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_747 = (tex2D(_DiffuseSpecA,TRANSFORM_TEX(node_814.rg, _DiffuseSpecA)).a*_Specularity);
                float3 specularColor = float3(node_747,node_747,node_747);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_DiffuseSpecA,TRANSFORM_TEX(node_814.rg, _DiffuseSpecA)).rgb;
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
