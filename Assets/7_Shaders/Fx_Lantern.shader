// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Expedition/Fx_Lantern"
{
	Properties
	{
		_Color("Color", 2D) = "white" {}
		_EmissiveIntensity("Emissive Intensity", Float) = 15
		_WaveNoise("Wave Noise", 2D) = "gray" {}
		_WaveAmount("Wave Amount", Float) = 0.3
		_EmissiveColorMultiplier("Emissive Color Multiplier", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZTest Always
			ZWrite On
		}

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend One One , One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _EmissiveIntensity;
		uniform sampler2D _Color;
		uniform float _WaveAmount;
		uniform sampler2D _WaveNoise;
		uniform float4 _EmissiveColorMultiplier;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord6 = i.uv_texcoord * float2( 0.2,0.3 );
			float2 panner3 = ( 1.0 * _Time.y * float2( 0.1,-0.1 ) + uv_TexCoord6);
			float2 uv_TexCoord19 = i.uv_texcoord * float2( 0.2,0.3 );
			float2 panner20 = ( 1.0 * _Time.y * float2( -0.15,-0.1 ) + uv_TexCoord19);
			o.Emission = ( _EmissiveIntensity * tex2D( _Color, ( ( _WaveAmount * ( tex2D( _WaveNoise, panner3 ).r * tex2D( _WaveNoise, panner20 ).r ) * i.uv_texcoord.y ) + i.uv_texcoord ) ) * _EmissiveColorMultiplier ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
382;215;1105;710;1572.086;383.4168;1.3;True;False
Node;AmplifyShaderEditor.Vector2Node;17;-3041.808,485.8674;Inherit;False;Constant;_Vector1;Vector 1;1;0;Create;True;0;0;False;0;0.2,0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;5;-3151.193,-231.0763;Inherit;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;0.2,0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;2;-2973.664,-65.74426;Inherit;False;Constant;_Vector2;Vector 2;1;0;Create;True;0;0;False;0;0.1,-0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;18;-2864.28,651.1995;Inherit;False;Constant;_Vector3;Vector 3;1;0;Create;True;0;0;False;0;-0.15,-0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2822.095,484.1096;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-2931.479,-232.834;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;16;-3060.083,188.7762;Inherit;True;Property;_WaveNoise;Wave Noise;3;0;Create;True;0;0;False;0;867ba9783543a334bb28620364b7274a;867ba9783543a334bb28620364b7274a;False;gray;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;3;-2666.061,-155.4942;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;20;-2556.677,561.4495;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;21;-2326.17,532.0723;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-2435.554,-184.8713;Inherit;True;Property;_MasterNoise;MasterNoise;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1856.899,335.6253;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2037.944,48.95071;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1957.594,-149.9723;Inherit;False;Property;_WaveAmount;Wave Amount;4;0;Create;True;0;0;False;0;0.3;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1596.126,73.37187;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-1104.226,32.36115;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-895.8224,1.362318;Inherit;True;Property;_Color;Color;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-621.3354,-165.6122;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;2;0;Create;True;0;0;False;0;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-890.8856,256.1832;Inherit;False;Property;_EmissiveColorMultiplier;Emissive Color Multiplier;5;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-343.8268,-51.1652;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-166.0424,-53.13388;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;_Expedition/Fx_Lantern;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;True;7;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;17;0
WireConnection;6;0;5;0
WireConnection;3;0;6;0
WireConnection;3;2;2;0
WireConnection;20;0;19;0
WireConnection;20;2;18;0
WireConnection;21;0;16;0
WireConnection;21;1;20;0
WireConnection;10;0;16;0
WireConnection;10;1;3;0
WireConnection;24;0;10;1
WireConnection;24;1;21;1
WireConnection;11;0;12;0
WireConnection;11;1;24;0
WireConnection;11;2;8;2
WireConnection;9;0;11;0
WireConnection;9;1;8;0
WireConnection;1;1;9;0
WireConnection;14;0;15;0
WireConnection;14;1;1;0
WireConnection;14;2;25;0
WireConnection;0;2;14;0
ASEEND*/
//CHKSM=9B12206E71E6C19AFCA83224CA6146073E24D8E8