Shader "BallCam/StandardVertexAnim" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _AmountX ("Amount", Range(0,2)) = 0.1
         _AmountY ("Amount", Range(0,2)) = 0.1
         _AmountZ ("Amount", Range(0,2)) = 0.1
         _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
         _RimPower ("Rim Power", Float) = 1
         _RimWidth ("Rim Width", Float) = 1
         _Opacity ("Opacity", Range(0, 1)) = 1
         _Freq ("Freq", Range(0, 20)) = 2
         _Speed("Speed", Range(0, 2)) = 0.3

     }
     SubShader {
     Pass{
         Tags { "Queue"="Transparent" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
        
         CGPROGRAM

         #pragma vertex vert
         #pragma fragment frag

         #pragma target 3.0
         #pragma multi_compile_fog

         #include "UnityCG.cginc"
         #include "ClassicNoise3D.cginc"
         //#include "SimplexNoise3D.cginc"
         //#include "SimplexNoiseGrad3D.cginc"
         // Use shader model 3.0 target, to get nicer looking lighting

         uniform float4 _MainTex_ST;
 		float _AmountX;
 		float _AmountY;
 		float _AmountZ;
 		fixed4 _RimColor;
          uniform sampler2D _MainTex;
          uniform sampler2D _NoiseTex;
        uniform float4 _Color;
        float _RimWidth;
        float _Opacity;
        float _Freq;
        float _Speed;
        float _RimPower;

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 color : COLOR;
            UNITY_FOG_COORDS(1)
        };

        v2f vert(appdata_base v) {
		    // get a 3d noise using the position, low frequency
		    float b = pnoise(v.vertex * _Freq + _Time * _Speed, float3(1000, 1000, 1000));
		    // compose both noises
		    float displacementX = b * _AmountX;
		    float displacementY = b * _AmountY;
		    float displacementZ = b * _AmountZ;

         	float4 newPos = v.vertex + float4(v.normal.x * displacementX, v.normal.y * displacementY, v.normal.z * displacementZ, 1);

         	v2f o;
            o.pos = UnityObjectToClipPos (newPos);
           
            float3 viewDir = normalize(ObjSpaceViewDir(newPos));
            float dotProduct = 1 - dot(v.normal, viewDir);
            float rimWidth = _RimWidth;
            o.color = smoothstep(1 - rimWidth, 1.0, dotProduct);
           
            o.color *= _RimColor * _RimPower;
           
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

            UNITY_TRANSFER_FOG(o, o.pos);

         	return o;
         }

        float4 frag(v2f i) : COLOR {
            float4 texcol = tex2D(_MainTex, i.uv);
            texcol *= _Color;
            texcol.rgb += i.color;
            texcol.a = _Opacity;
            UNITY_APPLY_FOG(i.fogCoord, texcol);
            return texcol;
        }
 
         ENDCG
     }
     }
     FallBack "Diffuse"
}