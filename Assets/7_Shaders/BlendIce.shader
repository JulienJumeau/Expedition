// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Expedition/BlendIce"
{
	Properties
	{
		_PrimaryAlbedo("Primary Albedo", 2D) = "gray" {}
		_PrimaryMetallicSmoothness("Primary MS", 2D) = "black" {}
		[Normal]_PrimaryNormalMap("Primary NormalMap", 2D) = "bump" {}
		_RimPower("RimPower", Range( 0 , 3)) = 1.5
		_SecondaryAlbedo("Secondary Albedo", Color) = (0.7244571,0.7675065,0.8301887,1)
		_RimAlbedo("Rim Albedo", Color) = (0.8509804,0.9162218,0.9254902,1)
		_SecondaryMetallic("Secondary Metallic", Range( 0 , 1)) = 0.2
		_SecondarySmoothness("Secondary Smoothness", Range( 0 , 1)) = 0.8952819
		[Normal]_SecondaryNormalMap("Secondary NormalMap", 2D) = "bump" {}
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 6
		_HeightMap("HeightMap", 2D) = "black" {}
		_Displacementamount("Displacement amount", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 5.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform sampler2D _HeightMap;
		uniform float4 _HeightMap_ST;
		uniform float _Displacementamount;
		uniform sampler2D _PrimaryNormalMap;
		uniform float4 _PrimaryNormalMap_ST;
		uniform sampler2D _SecondaryNormalMap;
		uniform float4 _SecondaryNormalMap_ST;
		uniform sampler2D _PrimaryAlbedo;
		uniform float4 _PrimaryAlbedo_ST;
		uniform float4 _SecondaryAlbedo;
		uniform float4 _RimAlbedo;
		uniform half _RimPower;
		uniform sampler2D _PrimaryMetallicSmoothness;
		uniform float4 _PrimaryMetallicSmoothness_ST;
		uniform float _SecondaryMetallic;
		uniform float _SecondarySmoothness;
		uniform float _TessValue;

		float4 tessFunction( )
		{
			return _TessValue;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float temp_output_43_0 = saturate( ( -ase_worldNormal.y * 5 ) );
			float2 uv_HeightMap = v.texcoord * _HeightMap_ST.xy + _HeightMap_ST.zw;
			float4 appendResult59 = (float4(0.0 , -tex2Dlod( _HeightMap, float4( uv_HeightMap, 0, 0.0) ).r , 0.0 , 0.0));
			float4 transform71 = mul(unity_WorldToObject,( ( temp_output_43_0 * appendResult59 ) * _Displacementamount ));
			v.vertex.xyz += transform71.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PrimaryNormalMap = i.uv_texcoord * _PrimaryNormalMap_ST.xy + _PrimaryNormalMap_ST.zw;
			float2 uv_SecondaryNormalMap = i.uv_texcoord * _SecondaryNormalMap_ST.xy + _SecondaryNormalMap_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float temp_output_43_0 = saturate( ( -ase_worldNormal.y * 5 ) );
			float3 lerpResult24 = lerp( UnpackNormal( tex2D( _PrimaryNormalMap, uv_PrimaryNormalMap ) ) , UnpackNormal( tex2D( _SecondaryNormalMap, uv_SecondaryNormalMap ) ) , temp_output_43_0);
			float3 normalizeResult18 = normalize( lerpResult24 );
			o.Normal = normalizeResult18;
			float2 uv_PrimaryAlbedo = i.uv_texcoord * _PrimaryAlbedo_ST.xy + _PrimaryAlbedo_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV31 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode31 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV31, _RimPower ) );
			float4 lerpResult34 = lerp( _SecondaryAlbedo , _RimAlbedo , fresnelNode31);
			float4 lerpResult13 = lerp( tex2D( _PrimaryAlbedo, uv_PrimaryAlbedo ) , lerpResult34 , temp_output_43_0);
			o.Albedo = lerpResult13.rgb;
			float2 uv_PrimaryMetallicSmoothness = i.uv_texcoord * _PrimaryMetallicSmoothness_ST.xy + _PrimaryMetallicSmoothness_ST.zw;
			float4 tex2DNode2 = tex2D( _PrimaryMetallicSmoothness, uv_PrimaryMetallicSmoothness );
			float lerpResult14 = lerp( tex2DNode2.r , _SecondaryMetallic , temp_output_43_0);
			o.Metallic = lerpResult14;
			float lerpResult15 = lerp( tex2DNode2.g , _SecondarySmoothness , temp_output_43_0);
			o.Smoothness = lerpResult15;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
318;-1004;1105;710;2460.571;601.1298;2.482122;True;False
Node;AmplifyShaderEditor.WorldNormalVector;45;-2947.819,606.4995;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;42;-2631.533,602.8647;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;75;-2974.915,1328.813;Inherit;True;Property;_HeightMap;HeightMap;14;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;41;-2427.656,595.9285;Inherit;True;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;53;-2565.723,1347.766;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;-2164.597,682.486;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-3075.811,521.0667;Half;False;Property;_RimPower;RimPower;3;0;Create;True;0;0;False;0;1.5;1.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-2276.462,1321.89;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-1724.365,1094.97;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FresnelNode;31;-2642.79,338.2264;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1708.199,1254.212;Inherit;False;Property;_Displacementamount;Displacement amount;15;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-1505.207,1503.48;Inherit;True;Property;_SecondaryNormalMap;Secondary NormalMap;8;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-2637.294,-69.79575;Inherit;False;Property;_SecondaryAlbedo;Secondary Albedo;4;0;Create;True;0;0;False;0;0.7244571,0.7675065,0.8301887,1;0.7244571,0.7675065,0.8301887,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1659.444,-108.7128;Inherit;True;Property;_PrimaryNormalMap;Primary NormalMap;2;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;32;-2641.143,138.8036;Inherit;False;Property;_RimAlbedo;Rim Albedo;5;0;Create;True;0;0;False;0;0.8509804,0.9162218,0.9254902,1;0.8509804,0.9162218,0.9254902,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-1387.672,1079.807;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;1;-1660.804,-382.3249;Inherit;True;Property;_PrimaryAlbedo;Primary Albedo;0;0;Create;True;0;0;False;0;-1;None;e70a4cc9a27a530468623a76c6c025fe;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1658.248,261.3998;Inherit;True;Property;_PrimaryMetallicSmoothness;Primary MS;1;0;Create;False;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-1658.651,779.6992;Inherit;False;Property;_SecondaryMetallic;Secondary Metallic;6;0;Create;True;0;0;False;0;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;24;-756.2945,1113.549;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;34;-2045.122,149.0856;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1661.446,905.4861;Inherit;False;Property;_SecondarySmoothness;Secondary Smoothness;7;0;Create;True;0;0;False;0;0.8952819;0.8952819;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;15;-697.0784,251.3937;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;-694.283,75.29184;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;18;-452.9156,750.5678;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;13;-632.7863,-223.802;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;71;-265.1526,616.7625;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-5.323671,70.81544;Float;False;True;-1;7;ASEMaterialInspector;0;0;Standard;_Expedition/BlendIce;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;1;6;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;9;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;45;2
WireConnection;41;0;42;0
WireConnection;53;0;75;1
WireConnection;43;0;41;0
WireConnection;59;1;53;0
WireConnection;54;0;43;0
WireConnection;54;1;59;0
WireConnection;31;3;30;0
WireConnection;73;0;54;0
WireConnection;73;1;74;0
WireConnection;24;0;4;0
WireConnection;24;1;11;0
WireConnection;24;2;43;0
WireConnection;34;0;33;0
WireConnection;34;1;32;0
WireConnection;34;2;31;0
WireConnection;15;0;2;2
WireConnection;15;1;9;0
WireConnection;15;2;43;0
WireConnection;14;0;2;1
WireConnection;14;1;10;0
WireConnection;14;2;43;0
WireConnection;18;0;24;0
WireConnection;13;0;1;0
WireConnection;13;1;34;0
WireConnection;13;2;43;0
WireConnection;71;0;73;0
WireConnection;0;0;13;0
WireConnection;0;1;18;0
WireConnection;0;3;14;0
WireConnection;0;4;15;0
WireConnection;0;11;71;0
ASEEND*/
//CHKSM=8DC66C2BC1EB99A5CF75F23C5D7C47C2869B3105