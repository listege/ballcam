Shader "BallCam/FresnelShader" {
	Properties {
	 	_Shininess ("Shininess", Range (0.01, 3)) = 1

	 	_MyColor ("Shine Color", Color) = (1,1,1,1) 

		_MainTex ("Base (RGB)", 2D) = "white" {}

		_Bump ("Bump", 2D) = "bump" {}

		_Opacity ("Opacity", Range(0, 20)) = 1


	}
	SubShader {
		 Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Lambert alpha

		 #include "UnityCG.cginc"
         #include "ClassicNoise3D.cginc"

		sampler2D _MainTex;
		sampler2D _Bump;
		float _Shininess;
		fixed4 _MyColor; 
		float _Opacity;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
			half factor = dot(normalize(IN.viewDir),o.Normal);
			o.Albedo = c.rgb+_MyColor*(_Shininess-factor*_Shininess);
			o.Emission.rgb = _MyColor*(_Shininess-factor*_Shininess);
			o.Alpha = _Opacity;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}