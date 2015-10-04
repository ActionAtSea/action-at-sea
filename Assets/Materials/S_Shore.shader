// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7241-RGB,custl-267-OUT,alpha-267-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32279,y:32502,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:1029,x:31965,y:32642,ptovrint:False,ptlb:Transperancy,ptin:_Transperancy,varname:_Transperancy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c9f8e60af0e8e5c49a75efbfe38c70a2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8285,x:31965,y:32865,ptovrint:False,ptlb:ripple,ptin:_ripple,varname:_ripple,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:755f7bb940011894899bd1f93e80cc3f,ntxv:0,isnm:False|UVIN-6215-UVOUT;n:type:ShaderForge.SFN_Multiply,id:8069,x:32228,y:32798,varname:node_8069,prsc:2|A-1029-A,B-8285-A;n:type:ShaderForge.SFN_Panner,id:6215,x:31709,y:32865,varname:node_6215,prsc:2,spu:1,spv:0|DIST-4673-OUT;n:type:ShaderForge.SFN_Time,id:1580,x:31325,y:32761,varname:node_1580,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4673,x:31499,y:32888,varname:node_4673,prsc:2|A-1580-T,B-2047-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2047,x:31325,y:32977,ptovrint:False,ptlb:Ripple Speed,ptin:_RippleSpeed,varname:_RippleSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Tex2d,id:5496,x:31966,y:33370,ptovrint:False,ptlb:ripple_copy,ptin:_ripple_copy,varname:_ripple_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:755f7bb940011894899bd1f93e80cc3f,ntxv:0,isnm:False|UVIN-2332-UVOUT;n:type:ShaderForge.SFN_Panner,id:2332,x:31710,y:33334,varname:node_2332,prsc:2,spu:1,spv:0|DIST-424-OUT;n:type:ShaderForge.SFN_Time,id:3868,x:31327,y:33230,varname:node_3868,prsc:2;n:type:ShaderForge.SFN_Multiply,id:424,x:31501,y:33357,varname:node_424,prsc:2|A-3868-T,B-89-OUT;n:type:ShaderForge.SFN_ValueProperty,id:89,x:31327,y:33446,ptovrint:False,ptlb:Ripple Speed_copy,ptin:_RippleSpeed_copy,varname:_RippleSpeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.2;n:type:ShaderForge.SFN_Multiply,id:3728,x:32228,y:33322,varname:node_3728,prsc:2|A-5660-A,B-5496-A;n:type:ShaderForge.SFN_Tex2d,id:5660,x:31966,y:33132,ptovrint:False,ptlb:Transperancy_copy,ptin:_Transperancy_copy,varname:_Transperancy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c9f8e60af0e8e5c49a75efbfe38c70a2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:267,x:32404,y:33061,varname:node_267,prsc:2|A-8069-OUT,B-3728-OUT;proporder:7241-1029-8285-2047-5496-89-5660;pass:END;sub:END;*/

Shader "ActionAtSea/S_Shore" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Transperancy ("Transperancy", 2D) = "white" {}
        _ripple ("ripple", 2D) = "white" {}
        _RippleSpeed ("Ripple Speed", Float ) = 0.1
        _ripple_copy ("ripple_copy", 2D) = "white" {}
        _RippleSpeed_copy ("Ripple Speed_copy", Float ) = -0.2
        _Transperancy_copy ("Transperancy_copy", 2D) = "white" {}
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Transperancy; uniform float4 _Transperancy_ST;
            uniform sampler2D _ripple; uniform float4 _ripple_ST;
            uniform float _RippleSpeed;
            uniform sampler2D _ripple_copy; uniform float4 _ripple_copy_ST;
            uniform float _RippleSpeed_copy;
            uniform sampler2D _Transperancy_copy; uniform float4 _Transperancy_copy_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float4 _Transperancy_var = tex2D(_Transperancy,TRANSFORM_TEX(i.uv0, _Transperancy));
                float4 node_1580 = _Time + _TimeEditor;
                float2 node_6215 = (i.uv0+(node_1580.g*_RippleSpeed)*float2(1,0));
                float4 _ripple_var = tex2D(_ripple,TRANSFORM_TEX(node_6215, _ripple));
                float4 _Transperancy_copy_var = tex2D(_Transperancy_copy,TRANSFORM_TEX(i.uv0, _Transperancy_copy));
                float4 node_3868 = _Time + _TimeEditor;
                float2 node_2332 = (i.uv0+(node_3868.g*_RippleSpeed_copy)*float2(1,0));
                float4 _ripple_copy_var = tex2D(_ripple_copy,TRANSFORM_TEX(node_2332, _ripple_copy));
                float node_267 = ((_Transperancy_var.a*_ripple_var.a)+(_Transperancy_copy_var.a*_ripple_copy_var.a));
                float3 finalColor = emissive + float3(node_267,node_267,node_267);
                return fixed4(finalColor,node_267);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
