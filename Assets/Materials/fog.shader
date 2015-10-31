Shader "FogShader" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", float) = 1.0
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        ZWrite Off
        Lighting Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha 

		Pass 
		{

	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #include "UnityCG.cginc"

	        fixed4 _Color;
	        float _Alpha;
	        sampler2D _MainTex;
	        float4 _MainTex_ST;

	        struct v2f 
	        {
	            float4 pos : SV_POSITION;
	            float2 uv : TEXCOORD0;
	        };

	        v2f vert (appdata_base v)
	        {
	            v2f o;
	            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	            return o;
	        }

	        fixed4 frag(v2f i) : SV_Target
	        {
	            fixed4 texcol = tex2D (_MainTex, i.uv) * _Color;
                texcol.a *= _Alpha;
	            return texcol;
	        }
	        
	        ENDCG
        }
    }
}