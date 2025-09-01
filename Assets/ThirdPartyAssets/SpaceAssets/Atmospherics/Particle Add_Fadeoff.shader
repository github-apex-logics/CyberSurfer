// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Luminet/Additive_Fadeoff" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_BlendPos ("BlendPos", Vector) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha One
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
			float4 _BlendPos;
			

            struct appdata_t {
                float4 vertex : POSITION;				
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
				float4x4 sinerandom : TEXCOORD3;
                fixed4 color : COLOR;
				float4 position : TEXCOORD2;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.sinerandom = UNITY_MATRIX_MVP;
				o.position = clamp(distance(_WorldSpaceCameraPos.xyzz,_BlendPos)/1000,0,1);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				
                return o;
			
            }

 
            float _InvFade;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
                col.a = saturate(col.a); // alpha should not have double-brightness applied to it, but we can't fix that legacy behaior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)
				col *=  0.9;
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				col = i.position*col*1;
                return col;
            }
            ENDCG
        }
    }
}
}
