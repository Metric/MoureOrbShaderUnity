// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
Copyright(c) 2015 Konstantinos Mourelas

zlib Licence

This software is provided 'as-is', without any express or implied
warranty.In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions :

1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software.If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.

2. Altered source versions must be plainly marked as such, and must not be
misrepresented as being the original software.

3. This notice may not be removed or altered from any source
distribution.
*/

/*
 * Modified by metric / arcanistry
 * 1. Updated to use proper floats for full precision of textures and colors etc.
 *   - This is important for VR headsets especially when it comes to colors / textures.
 * 2. _LineCol is now properly applied. It was being multiplied in the original source
 *   - rather than added. The _LineCol alpha is used as the add power for the _LineCol.rgb: 0 alpha = nothing; 1 alpha = pure white.
 * 3. Added _Shape which acts as an overall shape mask.
 *	- Since, with this material on the UI element, you cannot specify an actual sprite to use as a mask.
 *	- With this you can specify the overall general shape, rather than just a single sphere, rounded square, or square.
 */

Shader "Unlit/MoureOrb"
{
	Properties
	{
		_BgTex01("Background Texture 01", 2D) = "white" {}
		_Tex01Col("Texture 01 Color Multiplier", Color) = (1,1,1,1)
		_Tex02Col("Texture 02 Color Multiplier", Color) = (1,1,1,1)
		_Tex03Col("Texture 03 Color Multiplier", Color) = (1,1,1,1)
		_Shape("Shape Mask", 2D) = "white" {}
		_LineCol("Line Color", Color) = (0.8, 0, 0, 1)


		_Params01("Tex01Mult, Tex02Mult, Tex03Mult, LineMult", Vector) = (10,10,10,10)
		_Params02("WiggleAmount, LineWidth, Spherical, Scale", Vector) = (0.05,0.02,1,0.9)
		_Params03("PanSpeed01x, PanSpeed01y, PanSpeed02x, PanSpeed02y", Vector) = (-0.08, -0.12, -0.1, -0.08)
		_Params04("PanSpeed03x, PanSpeed03y", Vector) = (-0.06, -0.02, 0, 0)
		_Value("Value", Range(0.,1)) = 0.5
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		ColorMask RGB

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			#pragma multi_compile DUMMY PIXELSNAP_ON
			
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * float4(1,1,1,1);
				#ifdef PIXELSNAP_ON
								OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _BgTex01;
			sampler2D _Shape;

			float4 _Tex01Col;
			float4 _Tex02Col;
			float4 _Tex03Col;
			float4 _Params01;
			float4 _Params02;
			float4 _Params03;
			float4 _Params04;
			

			float4 _LineCol;

			static const float PI = 3.14159265f;
			float _Value;

			//Barrel Distortion Function used by permission of Chman. Originally found on his plugin Colorful FX "https://www.assetstore.unity3d.com/en/#!/content/44845" 
			float2 barrelDistortion(float2 coord, float spherical, float scale)
			{
				float2 h = coord.xy - float2(0.5, 0.5);
				float r2 = dot(h, h);
				float f = 1.0 + r2 * (spherical * sqrt(r2));
				return f * scale * h + 0.5;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				//After everything important i have included a commented return function.
				//You can uncomment them one by one to check what the shader does at each step


				//Save the uvs in the uv variable
				float2 uv = IN.texcoord;
				//Apply Barrel Uv Distortion on the uv. In our case we actually aply pure spherical and not barrel distortion
				uv = barrelDistortion(uv, _Params02.z, _Params02.w);

				//Animate 3 Background Layers Uvs
				float2 PanUV_01 = uv + frac(_Time.y * float2(_Params03.x, _Params03.y));
				float2 PanUV_02 = uv + frac(_Time.y * float2(_Params03.z, _Params03.w));
				float2 PanUV_03 = uv + frac(_Time.y * float2(_Params04.x, _Params04.y));

				//Create a local copy of the uvs and distort them on the y axis.
				//This will create the wavy look on the top. For some reason i call this effect Wiggle
				float2 uvWiggle = uv;				
				uvWiggle.y = uv.y + (2*tex2D(_BgTex01, PanUV_02).a-1)*_Params02.x;

				//Create the Value Vertical Mask based on the Wiggled uvs 
				float ValueMaskMoved = 32 * (1 - (length(uvWiggle.y + 0.0) + clamp((1.0 - _Value), 0.0, 1.))) + 0.1;
				ValueMaskMoved = clamp(ValueMaskMoved, 0., 1.);
				//return ValueMaskMoved;

				//Create another copy of the Value Mask but moved down a bit according to the LineWidth Parameter
				float MinusMask = 32 * (1 - (length(uvWiggle.y + _Params02.y) + clamp(((1 + _Params02.y) - _Value), _Params02.y, 1.))) + 0.1;
				MinusMask = clamp(MinusMask, 0., 1.);
				//return MinusMask;

				//Create the Line Mask simply by substracting the Minus Mask from the ValueMask
				//We will use the Line Mask later to emphasize the line on the top of the gradient
				float LineMask = ValueMaskMoved - MinusMask;
				LineMask = clamp(pow(LineMask,2), 0., 1.);
				//return LineMask;

				
				//Create Color from the Bg Channels Blending
				//First add the first two channels Red and Green. Before adding them together each one of them is multiplied by a color and a float value
				//Each of the channels uses one of the animated PanUvs we created earlier
				float4 col = (_Params01.x * tex2D(_BgTex01, PanUV_01).r*_Tex01Col) + (_Params01.y * tex2D(_BgTex01, PanUV_02).g * _Tex02Col);
				//Multiply the result of the addition with the third channel
				col *= _Params01.z * tex2D(_BgTex01, PanUV_03).b * _Tex03Col;
				//Multiply the result with the LineMask and the LineColor
				col.rgb += _Params01.w * (col.rgb + _LineCol.rgb * _LineCol.a) * tex2D(_BgTex01, PanUV_02).a * LineMask;
				//For additional emphasis add another 5% of the pure LineMask above
				col += 0.05*LineMask;

				//Remap the original uvs(not the Wiggled ones) to -1 1 from 0,1 and keep them in the new variable uv2
				//We will use them to create the overal circular mask below
				float2 uv2 = 2 * uv - 1;

				//Circular Mask on col alpha channel
				col.a = (1 - floor(length(uv2) + 0.05))*ValueMaskMoved;
				//return col.a;
				
				//Return the clamped result of our float4 color, we are done :)
				return clamp(col,0,1) * tex2D(_Shape, IN.texcoord);

			}

			ENDCG
		}
	}

	CustomEditor "MoureOrbEditor"
}
