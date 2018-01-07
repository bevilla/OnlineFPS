Shader "Custom/HealthShader" {
	Properties {
		_fgColor ("Color1", Color) = (0,1,0,1)
		_bgColor ("Color2", Color) = (1,0,0,1)
		_Health ("Health", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 localPos;
		};

		fixed4 _fgColor;
		fixed4 _bgColor;
		float _Health;

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = (1 - IN.localPos.x - 0.5) <= _Health ? _fgColor : _bgColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
