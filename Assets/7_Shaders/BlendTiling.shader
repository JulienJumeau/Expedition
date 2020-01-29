// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Expedition/BlendTiling"
{
	Properties
	{
		_PrimaryAlbedo("Primary Albedo", 2D) = "gray" {}
		_PrimaryMetallicSmoothness("Primary MS", 2D) = "black" {}
		[Normal]_PrimaryNormalMap("Primary NormalMap", 2D) = "bump" {}
		_SecondaryAlbedo("Secondary Albedo", 2D) = "gray" {}
		_PrimaryMetallicSmoothness1("Secondary MS", 2D) = "black" {}
		[Normal]_SecondaryNormalMap("Secondary NormalMap", 2D) = "bump" {}
		_SplatterMap("SplatterMap", 2D) = "white" {}
		_Bordersharpness("Border sharpness", Range( 0 , 1)) = 1
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
		uniform float _Bordersharpness;
		uniform sampler2D _SplatterMap;
		uniform float4 _SplatterMap_ST;
		uniform sampler2D _PrimaryAlbedo;
		uniform float4 _PrimaryAlbedo_ST;
		uniform sampler2D _SecondaryAlbedo;
		uniform float4 _SecondaryAlbedo_ST;
		uniform sampler2D _PrimaryMetallicSmoothness;
		uniform float4 _PrimaryMetallicSmoothness_ST;
		uniform sampler2D _PrimaryMetallicSmoothness1;
		uniform float4 _PrimaryMetallicSmoothness1_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PrimaryNormalMap = i.uv_texcoord * _PrimaryNormalMap_ST.xy + _PrimaryNormalMap_ST.zw;
			float2 uv_SecondaryNormalMap = i.uv_texcoord * _SecondaryNormalMap_ST.xy + _SecondaryNormalMap_ST.zw;
			float temp_output_44_0 = ( _Bordersharpness * 0.5 );
			float2 uv_SplatterMap = i.uv_texcoord * _SplatterMap_ST.xy + _SplatterMap_ST.zw;
			float smoothstepResult42 = smoothstep( temp_output_44_0 , ( 1.0 - temp_output_44_0 ) , saturate( ( ( 1.0 - i.vertexColor.r ) * tex2D( _SplatterMap, uv_SplatterMap ).r ) ));
			float temp_output_39_0 = saturate( smoothstepResult42 );
			float4 lerpResult24 = lerp( tex2D( _PrimaryNormalMap, uv_PrimaryNormalMap ) , tex2D( _SecondaryNormalMap, uv_SecondaryNormalMap ) , temp_output_39_0);
			float4 normalizeResult18 = normalize( lerpResult24 );
			o.Normal = normalizeResult18.rgb;
			float2 uv_PrimaryAlbedo = i.uv_texcoord * _PrimaryAlbedo_ST.xy + _PrimaryAlbedo_ST.zw;
			float2 uv_SecondaryAlbedo = i.uv_texcoord * _SecondaryAlbedo_ST.xy + _SecondaryAlbedo_ST.zw;
			float4 lerpResult13 = lerp( tex2D( _PrimaryAlbedo, uv_PrimaryAlbedo ) , tex2D( _SecondaryAlbedo, uv_SecondaryAlbedo ) , temp_output_39_0);
			o.Albedo = lerpResult13.rgb;
			float2 uv_PrimaryMetallicSmoothness = i.uv_texcoord * _PrimaryMetallicSmoothness_ST.xy + _PrimaryMetallicSmoothness_ST.zw;
			float4 tex2DNode2 = tex2D( _PrimaryMetallicSmoothness, uv_PrimaryMetallicSmoothness );
			float2 uv_PrimaryMetallicSmoothness1 = i.uv_texcoord * _PrimaryMetallicSmoothness1_ST.xy + _PrimaryMetallicSmoothness1_ST.zw;
			float4 tex2DNode37 = tex2D( _PrimaryMetallicSmoothness1, uv_PrimaryMetallicSmoothness1 );
			float lerpResult14 = lerp( tex2DNode2.r , tex2DNode37.r , temp_output_39_0);
			o.Metallic = lerpResult14;
			float lerpResult15 = lerp( tex2DNode2.a , tex2DNode37.a , temp_output_39_0);
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
244;-985;1258;760;3552.881;879.4073;2.770165;True;False
Node;AmplifyShaderEditor.VertexColorNode;7;-3810.43,332.2908;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-3226.206,61.84895;Inherit;False;Property;_Bordersharpness;Border sharpness;7;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3139.12,263.919;Inherit;False;Constant;_DivideBy2;DivideBy2;8;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-3766.629,560.188;Inherit;True;Property;_SplatterMap;SplatterMap;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;28;-3553.319,357.3376;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-3370.812,455.1037;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-2830.207,221.0491;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;46;-2632.005,535.0493;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;47;-3059.122,460.7191;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-2427.406,444.6488;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-1659.709,1088.304;Inherit;True;Property;_SecondaryNormalMap;Secondary NormalMap;5;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1662.411,-127.5159;Inherit;True;Property;_PrimaryNormalMap;Primary NormalMap;2;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;39;-2062.323,405.315;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1662.348,131.1785;Inherit;True;Property;_PrimaryMetallicSmoothness;Primary MS;1;0;Create;False;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;-1660.494,834.9907;Inherit;True;Property;_PrimaryMetallicSmoothness1;Secondary MS;4;0;Create;False;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;24;-756.2945,1113.549;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1664.804,-385.3249;Inherit;True;Property;_PrimaryAlbedo;Primary Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;-1661.514,589.3397;Inherit;True;Property;_SecondaryAlbedo;Secondary Albedo;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;13;-632.7863,-223.802;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;18;-452.9156,750.5678;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;14;-670.154,169.9519;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;15;-672.9494,316.3565;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_Expedition/BlendTiling;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;7;1
WireConnection;25;0;28;0
WireConnection;25;1;27;1
WireConnection;44;0;43;0
WireConnection;44;1;48;0
WireConnection;46;0;44;0
WireConnection;47;0;25;0
WireConnection;42;0;47;0
WireConnection;42;1;44;0
WireConnection;42;2;46;0
WireConnection;39;0;42;0
WireConnection;24;0;4;0
WireConnection;24;1;11;0
WireConnection;24;2;39;0
WireConnection;13;0;1;0
WireConnection;13;1;36;0
WireConnection;13;2;39;0
WireConnection;18;0;24;0
WireConnection;14;0;2;1
WireConnection;14;1;37;1
WireConnection;14;2;39;0
WireConnection;15;0;2;4
WireConnection;15;1;37;4
WireConnection;15;2;39;0
WireConnection;0;0;13;0
WireConnection;0;1;18;0
WireConnection;0;3;14;0
WireConnection;0;4;15;0
ASEEND*/
//CHKSM=98070AE308162EF07C0F60745B758CE890E33782