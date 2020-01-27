// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Expedition/FX_Fire"
{
	Properties
	{
		Fire_Noise_Mask("MasterNoises", 2D) = "gray" {}
		_EmissiveIntensity("Emissive Intensity", Float) = 50
		_MaskSmoothness("Mask Smoothness", Range( 0 , 1)) = 0.53
		_BorderIntensity("Border Intensity", Float) = 5
		_BorderThreshold("Border Threshold", Float) = 4
		_BorderSmoothness("Border Smoothness", Float) = 2
		_WaveSize("Wave Size", Range( 0 , 50)) = 3
		_WaveAmount("Wave Amount", Range( 0 , 1)) = 0.01
		_WaveSpeed("Wave Speed", Range( 0 , 20)) = 2
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
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D Fire_Noise_Mask;
		uniform float _WaveSize;
		uniform float _WaveSpeed;
		uniform float _WaveAmount;
		uniform float _EmissiveIntensity;
		uniform float _BorderThreshold;
		uniform float _BorderSmoothness;
		uniform float _BorderIntensity;
		uniform float _MaskSmoothness;


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1);
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1);
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = float3( 0, 1, 0 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( float4( v.normal , 0 ), rotationCamMatrix )).xyz;
			v.vertex.x *= length( unity_ObjectToWorld._m00_m10_m20 );
			v.vertex.y *= length( unity_ObjectToWorld._m01_m11_m21 );
			v.vertex.z *= length( unity_ObjectToWorld._m02_m12_m22 );
			v.vertex = mul( v.vertex, rotationCamMatrix );
			v.vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
			//Need to nullify rotation inserted by generated surface shader;
			v.vertex = mul( unity_WorldToObject, v.vertex );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			Gradient gradient20 = NewGradient( 0, 5, 2, float4( 0.3137255, 0, 0, 0 ), float4( 1, 0, 0, 0.07353323 ), float4( 0.9260406, 0.4294769, 0.06137696, 0.6588235 ), float4( 0.8584906, 0.8217357, 0.1174351, 0.923537 ), float4( 1, 0.9558154, 0.5707547, 1 ), 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float mulTime81 = _Time.y * _WaveSpeed;
			float2 appendResult83 = (float2(( sin( ( ( i.uv_texcoord.y * _WaveSize ) + mulTime81 ) ) * _WaveAmount * i.uv_texcoord.y ) , 0.0));
			float2 uv_TexCoord9 = i.uv_texcoord * float2( 0.8,0.9 ) + appendResult83;
			float2 panner11 = ( 1.0 * _Time.y * float2( 0.4,-0.6 ) + uv_TexCoord9);
			float2 uv_TexCoord10 = i.uv_texcoord * float2( 0.6,1.1 ) + appendResult83;
			float2 panner13 = ( 1.0 * _Time.y * float2( 0.3,-0.2 ) + uv_TexCoord10);
			float temp_output_16_0 = saturate( ( tex2D( Fire_Noise_Mask, ( panner11 * float2( 0.7,0.7 ) ) ).r * tex2D( Fire_Noise_Mask, ( panner13 * float2( 0.3,0.3 ) ) ).r ) );
			o.Emission = ( SampleGradient( gradient20, temp_output_16_0 ) * _EmissiveIntensity ).rgb;
			float2 uv_TexCoord46 = i.uv_texcoord * float2( 0.7,1.4 );
			float2 panner48 = ( 1.0 * _Time.y * float2( -0.5,-0.3 ) + uv_TexCoord46);
			float2 uv_TexCoord44 = i.uv_texcoord * float2( 1,1.8 );
			float2 panner49 = ( 1.0 * _Time.y * float2( 0.7,-0.2 ) + uv_TexCoord44);
			float smoothstepResult36 = smoothstep( ( _BorderThreshold - _BorderSmoothness ) , _BorderThreshold , ( ( ( tex2D( Fire_Noise_Mask, panner48 ).g * tex2D( Fire_Noise_Mask, panner49 ).g ) + i.uv_texcoord.y ) * _BorderIntensity ));
			float smoothstepResult68 = smoothstep( 0.0 , ( 1.0 - _MaskSmoothness ) , tex2D( Fire_Noise_Mask, ( i.uv_texcoord + appendResult83 ) ).b);
			o.Alpha = ( temp_output_16_0 * ( 1.0 - saturate( smoothstepResult36 ) ) * smoothstepResult68 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
457;-992;1105;716;4747.323;401.8207;1.884753;True;False
Node;AmplifyShaderEditor.CommentaryNode;84;-5433.99,144.686;Inherit;False;1438.428;517.7562;Waves;10;73;76;75;79;81;74;78;80;82;83;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-5367.767,391.1345;Inherit;False;Property;_WaveSize;Wave Size;7;0;Create;True;0;0;False;0;3;10;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-5383.99,519.8586;Inherit;False;Property;_WaveSpeed;Wave Speed;9;0;Create;True;0;0;False;0;2;3;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;-5377.569,194.686;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-5096.204,292.6929;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;81;-5072.76,538.3326;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;54;-3861.022,1005.448;Inherit;False;1543.988;965.0635;MaskNoise;11;41;42;43;44;45;46;48;49;50;51;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-4834.943,387.1941;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;41;-3607.775,1057.205;Inherit;False;Constant;_Vector8;Vector 8;1;0;Create;True;0;0;False;0;0.7,1.4;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;42;-3570.863,1630.223;Inherit;False;Constant;_Vector9;Vector 9;1;0;Create;True;0;0;False;0;1,1.8;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;80;-4719.354,547.4424;Inherit;False;Property;_WaveAmount;Wave Amount;8;0;Create;True;0;0;False;0;0.01;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;78;-4648.425,393.7616;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-3379.273,1631.981;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;45;-3430.246,1222.538;Inherit;False;Constant;_Vector11;Vector 11;1;0;Create;True;0;0;False;0;-0.5,-0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;43;-3354.665,1809.511;Inherit;False;Constant;_Vector10;Vector 10;1;0;Create;True;0;0;False;0;0.7,-0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-4450.534,298.4647;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-3386.709,1056.8;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;25;-3865.421,-390.1542;Inherit;False;1558.57;965.0626;AnimatedFireNoise;13;7;8;9;10;14;12;11;13;4;1;15;91;92;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;83;-4162.561,333.7079;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;7;-3611,-338.3965;Inherit;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;0.8,0.9;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;8;-3574.088,234.6206;Inherit;False;Constant;_Vector1;Vector 1;1;0;Create;True;0;0;False;0;0.6,1.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;49;-3119.129,1660.104;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;5;-4277.353,784.6243;Inherit;True;Property;Fire_Noise_Mask;MasterNoises;1;0;Create;False;0;0;False;0;867ba9783543a334bb28620364b7274a;867ba9783543a334bb28620364b7274a;False;gray;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;48;-3122.643,1132.788;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;12;-3433.471,-173.0646;Inherit;False;Constant;_Vector2;Vector 2;1;0;Create;True;0;0;False;0;0.4,-0.6;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;14;-3357.89,413.9084;Inherit;False;Constant;_Vector3;Vector 3;1;0;Create;True;0;0;False;0;0.3,-0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-3382.498,236.3784;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-3391.286,-340.1542;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;51;-2897.446,1485.485;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;55;-2168.228,1046.637;Inherit;False;1590.91;890.4734;VerticalMasking;10;39;36;32;37;34;26;35;33;27;90;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;50;-2916.946,1204.685;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;11;-3125.868,-262.8146;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;13;-3122.354,264.5018;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-2080.629,1411.053;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2520.748,1198.485;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-3047.939,378.8185;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.3,0.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1888.721,1820.256;Inherit;False;Property;_BorderSmoothness;Border Smoothness;6;0;Create;True;0;0;False;0;2;1.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3056.612,-110.0148;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.7,0.7;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1886.093,1561.935;Inherit;False;Property;_BorderIntensity;Border Intensity;4;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1876.818,1676.53;Inherit;False;Property;_BorderThreshold;Border Threshold;5;0;Create;True;0;0;False;0;4;2.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-1659.402,1203.974;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;37;-1530.635,1768.536;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1407.571,1415.371;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-2900.671,89.88278;Inherit;True;Property;_Tiling2;Tiling2;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-2341.833,611.9194;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2920.171,-190.9173;Inherit;True;Property;_Tiling1;Tiling1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;36;-1173.799,1642.755;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-2068.019,576.4751;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2485.06,-156.7318;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-1738.008,550.2603;Inherit;False;Property;_MaskSmoothness;Mask Smoothness;3;0;Create;True;0;0;False;0;0.53;0.53;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;56;-1953.05,724.9359;Inherit;True;Property;_Mask;Mask;7;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;39;-876.9286,1609.544;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;89;-1508.11,569.8179;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;20;-882.9261,-262.3372;Inherit;False;0;5;2;0.3137255,0,0,0;1,0,0,0.07353323;0.9260406,0.4294769,0.06137696,0.6588235;0.8584906,0.8217357,0.1174351,0.923537;1,0.9558154,0.5707547,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SaturateNode;16;-1055.105,45.10187;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1633.554,438.3016;Inherit;False;Constant;_Min;Min;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;90;-783.3522,1462.888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;23;-573.5302,-175.2318;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-540.282,48.87787;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;2;0;Create;True;0;0;False;0;50;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;68;-1256.553,512.1765;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-200.947,-139.7135;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-521.2643,312.0711;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;18;0,-1;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;_Expedition/FX_Fire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;True;7;Custom;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;True;Cylindrical;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;74;0;73;2
WireConnection;74;1;75;0
WireConnection;81;0;79;0
WireConnection;76;0;74;0
WireConnection;76;1;81;0
WireConnection;78;0;76;0
WireConnection;44;0;42;0
WireConnection;82;0;78;0
WireConnection;82;1;80;0
WireConnection;82;2;73;2
WireConnection;46;0;41;0
WireConnection;83;0;82;0
WireConnection;49;0;44;0
WireConnection;49;2;43;0
WireConnection;48;0;46;0
WireConnection;48;2;45;0
WireConnection;10;0;8;0
WireConnection;10;1;83;0
WireConnection;9;0;7;0
WireConnection;9;1;83;0
WireConnection;51;0;5;0
WireConnection;51;1;49;0
WireConnection;50;0;5;0
WireConnection;50;1;48;0
WireConnection;11;0;9;0
WireConnection;11;2;12;0
WireConnection;13;0;10;0
WireConnection;13;2;14;0
WireConnection;52;0;50;2
WireConnection;52;1;51;2
WireConnection;92;0;13;0
WireConnection;91;0;11;0
WireConnection;26;0;52;0
WireConnection;26;1;27;2
WireConnection;37;0;34;0
WireConnection;37;1;35;0
WireConnection;32;0;26;0
WireConnection;32;1;33;0
WireConnection;4;0;5;0
WireConnection;4;1;92;0
WireConnection;1;0;5;0
WireConnection;1;1;91;0
WireConnection;36;0;32;0
WireConnection;36;1;37;0
WireConnection;36;2;34;0
WireConnection;86;0;85;0
WireConnection;86;1;83;0
WireConnection;15;0;1;1
WireConnection;15;1;4;1
WireConnection;56;0;5;0
WireConnection;56;1;86;0
WireConnection;39;0;36;0
WireConnection;89;0;69;0
WireConnection;16;0;15;0
WireConnection;90;0;39;0
WireConnection;23;0;20;0
WireConnection;23;1;16;0
WireConnection;68;0;56;3
WireConnection;68;1;65;0
WireConnection;68;2;89;0
WireConnection;70;0;23;0
WireConnection;70;1;71;0
WireConnection;38;0;16;0
WireConnection;38;1;90;0
WireConnection;38;2;68;0
WireConnection;18;2;70;0
WireConnection;18;9;38;0
ASEEND*/
//CHKSM=412B552C458F3EDACE810255A57A1710816F76E7