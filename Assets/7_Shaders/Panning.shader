// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Panning"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "gray" {}
		_PrimaryMetallicSmoothness1("MetallicSmoothness", 2D) = "black" {}
		[Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		_Speed("Speed", Vector) = (0,0,0,0)
		_Normalpower("Normal power", Range( 0 , 2)) = 1
		_Smoothnesspower("Smoothness power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Normalpower;
		uniform sampler2D _NormalMap;
		uniform float2 _Speed;
		uniform sampler2D _Albedo;
		uniform sampler2D _PrimaryMetallicSmoothness1;
		uniform float _Smoothnesspower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner5 = ( 1.0 * _Time.y * _Speed + i.uv_texcoord);
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, panner5 ), _Normalpower );
			o.Albedo = tex2D( _Albedo, panner5 ).rgb;
			float4 tex2DNode3 = tex2D( _PrimaryMetallicSmoothness1, panner5 );
			o.Metallic = tex2DNode3.r;
			o.Smoothness = ( tex2DNode3.a * _Smoothnesspower );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
318;-1004;1105;710;2285.496;673.4825;2.444784;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1371.017,-10.7714;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;6;-1345.073,142.1347;Inherit;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;0,0;0.4,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;5;-1057.089,9.517883;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;3;-756.66,389.0525;Inherit;True;Property;_PrimaryMetallicSmoothness1;MetallicSmoothness;1;0;Create;False;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-750.4077,597.6591;Inherit;False;Property;_Smoothnesspower;Smoothness power;5;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1094.949,181.8895;Inherit;False;Property;_Normalpower;Normal power;4;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-759.116,-123.6807;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;e70a4cc9a27a530468623a76c6c025fe;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-756.723,134.1283;Inherit;True;Property;_NormalMap;NormalMap;2;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-407.5222,491.6458;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Panning;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;4;0
WireConnection;5;2;6;0
WireConnection;3;1;5;0
WireConnection;2;1;5;0
WireConnection;1;1;5;0
WireConnection;1;5;7;0
WireConnection;8;0;3;4
WireConnection;8;1;9;0
WireConnection;0;0;2;0
WireConnection;0;1;1;0
WireConnection;0;3;3;1
WireConnection;0;4;8;0
ASEEND*/
//CHKSM=A4AC87B33396E7F6E8ECC57D9BCBC98B8B843A1E