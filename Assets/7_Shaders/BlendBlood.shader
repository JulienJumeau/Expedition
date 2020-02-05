// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Expedition/BlendBlood"
{
	Properties
	{
		_PrimaryAlbedo("Primary Albedo", 2D) = "gray" {}
		_PrimaryMetallicSmoothness("Primary MS", 2D) = "black" {}
		[Normal]_PrimaryNormalMap("Primary NormalMap", 2D) = "bump" {}
		_SecondaryAlbedo("Secondary Albedo", Color) = (0.754717,0.1103596,0.1103596,0)
		_SecondaryMetallic("Secondary Metallic", Range( 0 , 1)) = 0.2
		_SecondarySmoothness("Secondary Smoothness", Range( 0 , 1)) = 0.8952819
		[Normal]_SecondaryNormalMap("Secondary NormalMap", 2D) = "bump" {}
		_SplatterMap("SplatterMap", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _PrimaryNormalMap;
		uniform float4 _PrimaryNormalMap_ST;
		uniform sampler2D _SecondaryNormalMap;
		uniform float4 _SecondaryNormalMap_ST;
		uniform sampler2D _SplatterMap;
		uniform float4 _SplatterMap_ST;
		uniform sampler2D _PrimaryAlbedo;
		uniform float4 _PrimaryAlbedo_ST;
		uniform float4 _SecondaryAlbedo;
		uniform sampler2D _PrimaryMetallicSmoothness;
		uniform float4 _PrimaryMetallicSmoothness_ST;
		uniform float _SecondaryMetallic;
		uniform float _SecondarySmoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PrimaryNormalMap = i.uv_texcoord * _PrimaryNormalMap_ST.xy + _PrimaryNormalMap_ST.zw;
			float2 uv_SecondaryNormalMap = i.uv_texcoord * _SecondaryNormalMap_ST.xy + _SecondaryNormalMap_ST.zw;
			float2 uv_SplatterMap = i.uv_texcoord * _SplatterMap_ST.xy + _SplatterMap_ST.zw;
			float temp_output_25_0 = ( ( 1.0 - i.vertexColor.r ) * tex2D( _SplatterMap, uv_SplatterMap ).r );
			float3 lerpResult24 = lerp( UnpackNormal( tex2D( _PrimaryNormalMap, uv_PrimaryNormalMap ) ) , UnpackNormal( tex2D( _SecondaryNormalMap, uv_SecondaryNormalMap ) ) , temp_output_25_0);
			float3 normalizeResult18 = normalize( lerpResult24 );
			o.Normal = normalizeResult18;
			float2 uv_PrimaryAlbedo = i.uv_texcoord * _PrimaryAlbedo_ST.xy + _PrimaryAlbedo_ST.zw;
			float4 lerpResult13 = lerp( tex2D( _PrimaryAlbedo, uv_PrimaryAlbedo ) , _SecondaryAlbedo , temp_output_25_0);
			o.Albedo = lerpResult13.rgb;
			float2 uv_PrimaryMetallicSmoothness = i.uv_texcoord * _PrimaryMetallicSmoothness_ST.xy + _PrimaryMetallicSmoothness_ST.zw;
			float4 tex2DNode2 = tex2D( _PrimaryMetallicSmoothness, uv_PrimaryMetallicSmoothness );
			float lerpResult14 = lerp( tex2DNode2.r , _SecondaryMetallic , temp_output_25_0);
			o.Metallic = lerpResult14;
			float lerpResult15 = lerp( tex2DNode2.g , _SecondarySmoothness , temp_output_25_0);
			o.Smoothness = lerpResult15;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
318;-1004;1105;710;3004.507;-40.87471;1.995811;True;False
Node;AmplifyShaderEditor.VertexColorNode;7;-2370.076,626.6311;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-2367.144,791.8929;Inherit;True;Property;_SplatterMap;SplatterMap;7;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;28;-2155.006,658.1182;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-1659.444,-123.1083;Inherit;True;Property;_PrimaryNormalMap;Primary NormalMap;2;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1580.484,1170.676;Inherit;True;Property;_SecondaryNormalMap;Secondary NormalMap;6;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1933.868,711.3947;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1661.446,905.4861;Inherit;False;Property;_SecondarySmoothness;Secondary Smoothness;5;0;Create;True;0;0;False;0;0.8952819;0.9;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1658.414,133.1457;Inherit;True;Property;_PrimaryMetallicSmoothness;Primary MS;1;0;Create;False;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-1666.225,394.3991;Inherit;False;Property;_SecondaryAlbedo;Secondary Albedo;3;0;Create;True;0;0;False;0;0.754717,0.1103596,0.1103596,0;0.7547169,0.1103595,0.1103595,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;24;-756.2945,1113.549;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1658.651,779.6992;Inherit;False;Property;_SecondaryMetallic;Secondary Metallic;4;0;Create;True;0;0;False;0;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1660.804,-382.3249;Inherit;True;Property;_PrimaryAlbedo;Primary Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;18;-452.9156,750.5678;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;13;-632.7863,-223.802;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;15;-697.0784,251.3937;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;29;-1899.777,989.0623;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;-694.283,75.29184;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_Expedition/BlendBlood;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;7;1
WireConnection;25;0;28;0
WireConnection;25;1;27;1
WireConnection;24;0;4;0
WireConnection;24;1;11;0
WireConnection;24;2;25;0
WireConnection;18;0;24;0
WireConnection;13;0;1;0
WireConnection;13;1;8;0
WireConnection;13;2;25;0
WireConnection;15;0;2;2
WireConnection;15;1;9;0
WireConnection;15;2;25;0
WireConnection;14;0;2;1
WireConnection;14;1;10;0
WireConnection;14;2;25;0
WireConnection;0;0;13;0
WireConnection;0;1;18;0
WireConnection;0;3;14;0
WireConnection;0;4;15;0
ASEEND*/
//CHKSM=8363A4A6C964DDD0FAECFF02C6FDC46CBD98404F