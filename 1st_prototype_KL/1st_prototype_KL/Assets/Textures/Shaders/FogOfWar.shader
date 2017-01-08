Shader "Custom/Shader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_FogRadius ("FogRadius", Float) = 10.0
		_ObjectFogRadius("ObjectFogRadius", Float) = 10.0
		_SpecialObjectRadius ("SpecialObjectFogRadius", Float) = 10.0
		_FogMaxRadius ("FogMaxRadius", Float) = 0.5
		_Player_Pos ("Player_Pos", Vector) = (0,0,0,1)
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alpha:blend
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		float _FogRadius;
		float _ObjectFogRadius;
		float _SpecialObjectRadius;
		float _FogMaxRadius;
		float4 _Player_Pos;
		uniform float4 _Points[5];
		uniform float4 _SpecialObjectPos;
		
		struct Input {
			float2 uv_MainTex;
			float2 location;
		};

		float powerForPos(float4 pos, float2 nearVertex);
		float powerForObjectPos(float4 pos, float2 nearVertex);
		float powerForSpecialObjectPos(float4 pos, float2 nearVertex);

		void vert(inout appdata_full vertexData, out Input outData)
		{
			float4 pos = mul(UNITY_MATRIX_MVP, vertexData.vertex);
			float4 posWorld = mul(_Object2World, vertexData.vertex);
			outData.uv_MainTex = vertexData.texcoord;
			outData.location = posWorld.xy;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			float tampon = (powerForPos(_Player_Pos, IN.location));

			for (int i = 0; i < 4; i++)
			{				
				//if(step(0, _Point_Exist[i]))
					tampon += (powerForObjectPos(_Points[i], IN.location));
			}

			float alpha = 1.0 - (tampon + powerForSpecialObjectPos(_SpecialObjectPos, IN.location));

			o.Albedo = baseColor.rgb;
			o.Alpha = alpha;
		}

		float powerForSpecialObjectPos(float4 pos, float2 nearVertex)
		{
			float atten = clamp(_SpecialObjectRadius - length(pos.xy - nearVertex.xy), 0.0, _SpecialObjectRadius);
			return (1.0 / _FogMaxRadius) * atten / _SpecialObjectRadius;
		}

		float powerForObjectPos(float4 pos, float2 nearVertex)
		{
			float atten = clamp(_ObjectFogRadius - length(pos.xy - nearVertex.xy), 0.0, _ObjectFogRadius);
			return (1.0 / _FogMaxRadius) * atten / _ObjectFogRadius;
		}

		float powerForPos(float4 pos, float2 nearVertex)
		{
			float atten = clamp(_FogRadius - length(pos.xy - nearVertex.xy), 0.0, _FogRadius);
			return (1.0 / _FogMaxRadius) * atten / _FogRadius;
		}
		ENDCG
	}
	FallBack "Transparent/VertexLit"
}
