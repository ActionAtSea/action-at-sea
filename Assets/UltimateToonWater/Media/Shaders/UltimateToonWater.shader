Shader "FX/Ultimate Toon Water" {
	Properties {
		_Color ("Water Color", Color) = (0.0,0.25,0.5,0.0)
		_Color2 ("Shoreline Color", Color) = (1.0,1.0,1.1,1.0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex ("Second (RGB)", 2D) = "white" {}
		_ColorBias("Color Mixer",Range (0,1)) = 0.5
		_TextureBias("Texture Mixer",Range (0,1)) = 0.5
        _HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
        _HighlightThresholdRange("Highlight Threshold Range", Range (0,1)) = 0.5 //Max difference for intersections
        
        _UseGPU("Use GPU",int) = 1
        
        _WaveScale1("Wave Scale 1",Float) = 1
        _WaveScale2("Wave Scale 2",Float) = 1
        _Wavelength("Wavelength",Vector) = (0,0,0,0)
        _WaveSpeed("Wave Speed",Vector) = (0,0,0,0)
        _WaveOffset("Wave offset",Vector) = (0,0,0,0)
        _WaveGain("Wave gain",Vector) = (0,0,0,0)
        _GameTime("Game Time",Float) = 0
        _HeightFactor("Height Factor",Float) = 1
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "AutoLight.cginc"
	#include "Lighting.cginc"
	ENDCG
	
	SubShader {
		Tags { "RenderType"="Transparent"  "Queue" = "Geometry+1" }
		LOD 200
		
		// Normal pass
        Pass
        {
		    Lighting On
		    Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ZTest LEqual
            Cull Back
            AlphaTest Off
		    Tags {"LightMode" = "ForwardBase" }
		
			CGPROGRAM
			#pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_fwdbase
            //#pragma multi_compile_fwdadd_fullshadows
	        
	        uniform sampler2D _CameraDepthTexture; //Depth Texture
			uniform sampler2D _MainTex;
			uniform sampler2D _DetailTex;
			uniform float _TextureBias;
			uniform float _ColorBias;
			uniform float4 _Color;
			uniform float4 _Color2;
			uniform float _HighlightThresholdMax;
			uniform float _HighlightThresholdRange;
			uniform float _HeightFactor;
			uniform int _UseGPU;
			
			uniform float4 _MainTex_ST;
			uniform float4 _DetailTex_ST;
			
        	uniform  float _WaveScale1;
        	uniform  float _WaveScale2;
        	uniform  float4 _Wavelength;
        	uniform  float4 _WaveOffset;
        	uniform  float4 _WaveGain;
        	uniform  float4 _WaveSpeed;
			uniform float _GameTime;
	        
		    struct v2f
		    {
		    	float2 uv_MainTex : TEXCOORD0;
		    	float2 uv_DetailTex : TEXCOORD1;
		        float4 pos : SV_POSITION;
		        float4 projPos : TEXCOORD2; //Screen position of pos.
		        float4 color : COLOR;
		        LIGHTING_COORDS(3, 4)
		    };
		    
		    
		    v2f vert(appdata_base v)
		    {
		        v2f o;
		        
		        o.color.xyz = _Color;
		        o.color.w = _Color.a;

		        v.vertex.y = 0.0f;
		        
		        if (_UseGPU >= 1) {
			        	const float TWOPI = 6.28318530718;
			       		float3 worldPos = mul (_Object2World, v.vertex).xyz;
			       		float yC1 = (_GameTime * _WaveSpeed.g * TWOPI) + ((worldPos.z * TWOPI) / _Wavelength.g) + _WaveOffset.g;
			       		float xC1 = (_GameTime * _WaveSpeed.r * TWOPI) + ((worldPos.x * TWOPI) / _Wavelength.r) + _WaveOffset.r;
						float xyComp = (cos(yC1) + cos(xC1))/2;
						if (_WaveGain.r > 1 && xyComp >= 0) {
							xyComp = pow(xyComp,_WaveGain.r);
						} else {
							if (_WaveGain.g > 1) {
								xyComp = -1*pow(xyComp*-1,_WaveGain.g);
							}
						}
						float normFactor = 0;
						if (_UseGPU == 2) {
							float yC2 = (_GameTime * _WaveSpeed.a * TWOPI) + ((worldPos.z * TWOPI) / _Wavelength.a) + _WaveOffset.a;
							float xC2 = (_GameTime * _WaveSpeed.b * TWOPI) + ((worldPos.x * TWOPI) / _Wavelength.b) + _WaveOffset.b;
							float xyComp2 = (cos(yC2) + cos(xC2))/2;
							if (_WaveGain.b > 1 && xyComp2 >= 0) {
								xyComp2 = pow(xyComp2,_WaveGain.b)*pow(2,_WaveGain.b);
							} else {
								if (_WaveGain.a > 1) {
									xyComp2 = -1*pow(xyComp2*-1,_WaveGain.a);
								}
							}
							float waveFactor = _WaveScale1/(_WaveScale1+_WaveScale2);
							
							v.vertex.y += _WaveScale1 * xyComp + _WaveScale2 * xyComp2;
							normFactor = abs(-1 * (waveFactor*(sin(yC1)+sin(xC1)) + (1-waveFactor)*(sin(yC2)+sin(xC2))));
						} else {
							v.vertex.y +=  _WaveScale1 * xyComp;
							normFactor = abs(-1 * (sin(yC1)+sin(xC1)));
						}
						v.vertex.y += 1.0;
						o.color.xyz += (normFactor * _HeightFactor * _Color2.rgb);
		        }

		        
		        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		        o.projPos = ComputeScreenPos(o.pos);
		        o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
		        o.uv_DetailTex = TRANSFORM_TEX(v.texcoord, _DetailTex);

		        TRANSFER_VERTEX_TO_FRAGMENT(o)
		        return o;
		    }

		    half4 frag(v2f i) : COLOR
		    {
		    	half4 ret;
		    	half4 c = tex2D (_MainTex, i.uv_MainTex.xy);
		    	half4 d = tex2D (_DetailTex, i.uv_DetailTex.xy);
		    	ret.rgb = (_ColorBias * i.color.rgb) + ((1-_ColorBias) * (c.rgb*_TextureBias+d.rgb*(1-_TextureBias)));
		    	ret.a = i.color.a;

		        //Get the distance to the camera from the depth buffer for this point
		        float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));

		        //Actual distance to the camera
		        float partZ = i.projPos.z;

		        //If the two are similar, then there is an object intersecting with our object
		        float diff = (abs(sceneZ - partZ)) / _HighlightThresholdMax;

		        if(diff <= 1 && diff >= _HighlightThresholdRange)
		        {              
					ret.rgb += (_Color2.rgb * (1-diff));
		        }

		        
		        float3 lightColor = _LightColor0.rgb;
		        float3 lightDir = _WorldSpaceLightPos0;
				float atten = LIGHT_ATTENUATION(i) - _Color2.a;
				float3 N = float3(0.0f, 1.0f, 0.0f);
				float NL = saturate(dot(N, lightDir));

		        ret.rgb += atten * UNITY_LIGHTMODEL_AMBIENT * NL;
		        
				return ret;
		    }
	        
	        ENDCG
		}
	} 
	
	
	FallBack "Diffuse"
}