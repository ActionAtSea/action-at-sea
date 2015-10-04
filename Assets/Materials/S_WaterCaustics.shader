// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-3588-OUT;n:type:ShaderForge.SFN_Color,id:1794,x:31554,y:32058,ptovrint:False,ptlb:node_1794,ptin:_node_1794,varname:_node_1794,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3762976,c2:0.6179066,c3:0.6397059,c4:1;n:type:ShaderForge.SFN_Tex2d,id:825,x:31554,y:32247,varname:node_825,prsc:2,tex:8c5694045711d0e4cb093459a9961205,ntxv:0,isnm:False|UVIN-7373-UVOUT,TEX-7611-TEX;n:type:ShaderForge.SFN_Panner,id:7373,x:31164,y:31999,varname:node_7373,prsc:2,spu:1,spv:1|DIST-8944-OUT;n:type:ShaderForge.SFN_Time,id:2042,x:30664,y:32093,varname:node_2042,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8944,x:30999,y:31994,varname:node_8944,prsc:2|A-2042-TSL,B-4210-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4210,x:30779,y:31953,ptovrint:False,ptlb:Pan Speed,ptin:_PanSpeed,varname:_PanSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:3588,x:32148,y:32213,varname:node_3588,prsc:2|A-1794-RGB,B-4581-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7611,x:31305,y:32463,ptovrint:False,ptlb:node_7611,ptin:_node_7611,varname:_node_7611,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8c5694045711d0e4cb093459a9961205,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4501,x:31554,y:32411,varname:node_4501,prsc:2,tex:8c5694045711d0e4cb093459a9961205,ntxv:0,isnm:False|UVIN-1276-UVOUT,TEX-7611-TEX;n:type:ShaderForge.SFN_Multiply,id:1754,x:31808,y:32423,varname:node_1754,prsc:2|A-825-RGB,B-4501-RGB,C-8874-OUT;n:type:ShaderForge.SFN_Panner,id:1276,x:31164,y:32253,varname:node_1276,prsc:2,spu:1,spv:1|UVIN-5542-OUT,DIST-9764-OUT;n:type:ShaderForge.SFN_Multiply,id:9764,x:30961,y:32253,varname:node_9764,prsc:2|A-2042-TSL,B-3332-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3332,x:30783,y:32297,ptovrint:True,ptlb:Pan Speed23,ptin:_PanSpeed23,varname:_PanSpeed23,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:8734,x:30674,y:32446,varname:node_8734,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:9136,x:30658,y:32625,varname:node_9136,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:5542,x:30928,y:32476,varname:node_5542,prsc:2|A-8734-UVOUT,B-9136-OUT;n:type:ShaderForge.SFN_Slider,id:8874,x:31317,y:32708,ptovrint:False,ptlb:node_8874,ptin:_node_8874,varname:_node_8874,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0.6194599,max:10;n:type:ShaderForge.SFN_Power,id:4581,x:31997,y:32423,varname:node_4581,prsc:2|VAL-1754-OUT,EXP-7671-OUT;n:type:ShaderForge.SFN_Slider,id:7671,x:31667,y:32713,ptovrint:False,ptlb:node_8874_copy,ptin:_node_8874_copy,varname:_node_8874_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6194599,max:10;proporder:1794-4210-7611-3332-8874-7671;pass:END;sub:END;*/

Shader "Shader Forge/S_WaterC" {
    Properties {
        _node_1794 ("node_1794", Color) = (0.3762976,0.6179066,0.6397059,1)
        _PanSpeed ("Pan Speed", Float ) = 1
        _node_7611 ("node_7611", 2D) = "white" {}
        _PanSpeed23 ("Pan Speed23", Float ) = 1
        _node_8874 ("node_8874", Range(-10, 10)) = 0.6194599
        _node_8874_copy ("node_8874_copy", Range(0, 10)) = 0.6194599
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
            uniform float4 _TimeEditor;
            uniform float4 _node_1794;
            uniform float _PanSpeed;
            uniform sampler2D _node_7611; uniform float4 _node_7611_ST;
            uniform float _PanSpeed23;
            uniform float _node_8874;
            uniform float _node_8874_copy;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
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
                float4 node_2042 = _Time + _TimeEditor;
                float2 node_7373 = (i.uv0+(node_2042.r*_PanSpeed)*float2(1,1));
                float4 node_825 = tex2D(_node_7611,TRANSFORM_TEX(node_7373, _node_7611));
                float2 node_1276 = ((i.uv0*0.5)+(node_2042.r*_PanSpeed23)*float2(1,1));
                float4 node_4501 = tex2D(_node_7611,TRANSFORM_TEX(node_1276, _node_7611));
                float3 diffuseColor = (_node_1794.rgb+pow((node_825.rgb*node_4501.rgb*_node_8874),_node_8874_copy));
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
            uniform float4 _TimeEditor;
            uniform float4 _node_1794;
            uniform float _PanSpeed;
            uniform sampler2D _node_7611; uniform float4 _node_7611_ST;
            uniform float _PanSpeed23;
            uniform float _node_8874;
            uniform float _node_8874_copy;
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
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 node_2042 = _Time + _TimeEditor;
                float2 node_7373 = (i.uv0+(node_2042.r*_PanSpeed)*float2(1,1));
                float4 node_825 = tex2D(_node_7611,TRANSFORM_TEX(node_7373, _node_7611));
                float2 node_1276 = ((i.uv0*0.5)+(node_2042.r*_PanSpeed23)*float2(1,1));
                float4 node_4501 = tex2D(_node_7611,TRANSFORM_TEX(node_1276, _node_7611));
                float3 diffuseColor = (_node_1794.rgb+pow((node_825.rgb*node_4501.rgb*_node_8874),_node_8874_copy));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
