using System;
using UnityEngine;

public static class UberShader
{
	[RuntimeInitializeOnLoadMethod]
	private static void InitializeOnLoad()
	{
		UberShader.kReferenceShader = Shader.Find("GorillaTag/UberShader");
		UberShader.kReferenceMaterial = new Material(UberShader.kReferenceShader);
		UberShader.kProperties = UberShaderProperty.GetAllProperties();
	}

	public static UberShaderProperty TransparencyMode
	{
		get
		{
			return UberShader.kProperties[0];
		}
	}

	public static UberShaderProperty Cutoff
	{
		get
		{
			return UberShader.kProperties[1];
		}
	}

	public static UberShaderProperty ColorSource
	{
		get
		{
			return UberShader.kProperties[2];
		}
	}

	public static UberShaderProperty BaseColor
	{
		get
		{
			return UberShader.kProperties[3];
		}
	}

	public static UberShaderProperty GChannelColor
	{
		get
		{
			return UberShader.kProperties[4];
		}
	}

	public static UberShaderProperty BChannelColor
	{
		get
		{
			return UberShader.kProperties[5];
		}
	}

	public static UberShaderProperty AChannelColor
	{
		get
		{
			return UberShader.kProperties[6];
		}
	}

	public static UberShaderProperty BaseMap
	{
		get
		{
			return UberShader.kProperties[7];
		}
	}

	public static UberShaderProperty BaseMap_WH
	{
		get
		{
			return UberShader.kProperties[8];
		}
	}

	public static UberShaderProperty UVSource
	{
		get
		{
			return UberShader.kProperties[9];
		}
	}

	public static UberShaderProperty MaskMapToggle
	{
		get
		{
			return UberShader.kProperties[10];
		}
	}

	public static UberShaderProperty MaskMap
	{
		get
		{
			return UberShader.kProperties[11];
		}
	}

	public static UberShaderProperty MaskMap_WH
	{
		get
		{
			return UberShader.kProperties[12];
		}
	}

	public static UberShaderProperty LavaLampToggle
	{
		get
		{
			return UberShader.kProperties[13];
		}
	}

	public static UberShaderProperty GradientMapToggle
	{
		get
		{
			return UberShader.kProperties[14];
		}
	}

	public static UberShaderProperty GradientMap
	{
		get
		{
			return UberShader.kProperties[15];
		}
	}

	public static UberShaderProperty DoTextureRotation
	{
		get
		{
			return UberShader.kProperties[16];
		}
	}

	public static UberShaderProperty RotateAngle
	{
		get
		{
			return UberShader.kProperties[17];
		}
	}

	public static UberShaderProperty RotateAnim
	{
		get
		{
			return UberShader.kProperties[18];
		}
	}

	public static UberShaderProperty UseWaveWarp
	{
		get
		{
			return UberShader.kProperties[19];
		}
	}

	public static UberShaderProperty WaveAmplitude
	{
		get
		{
			return UberShader.kProperties[20];
		}
	}

	public static UberShaderProperty WaveFrequency
	{
		get
		{
			return UberShader.kProperties[21];
		}
	}

	public static UberShaderProperty WaveScale
	{
		get
		{
			return UberShader.kProperties[22];
		}
	}

	public static UberShaderProperty WaveTimeScale
	{
		get
		{
			return UberShader.kProperties[23];
		}
	}

	public static UberShaderProperty UseWeatherMap
	{
		get
		{
			return UberShader.kProperties[24];
		}
	}

	public static UberShaderProperty WeatherMap
	{
		get
		{
			return UberShader.kProperties[25];
		}
	}

	public static UberShaderProperty WeatherMapDissolveEdgeSize
	{
		get
		{
			return UberShader.kProperties[26];
		}
	}

	public static UberShaderProperty ReflectToggle
	{
		get
		{
			return UberShader.kProperties[27];
		}
	}

	public static UberShaderProperty ReflectBoxProjectToggle
	{
		get
		{
			return UberShader.kProperties[28];
		}
	}

	public static UberShaderProperty ReflectBoxCubePos
	{
		get
		{
			return UberShader.kProperties[29];
		}
	}

	public static UberShaderProperty ReflectBoxSize
	{
		get
		{
			return UberShader.kProperties[30];
		}
	}

	public static UberShaderProperty ReflectBoxRotation
	{
		get
		{
			return UberShader.kProperties[31];
		}
	}

	public static UberShaderProperty ReflectMatcapToggle
	{
		get
		{
			return UberShader.kProperties[32];
		}
	}

	public static UberShaderProperty ReflectMatcapPerspToggle
	{
		get
		{
			return UberShader.kProperties[33];
		}
	}

	public static UberShaderProperty ReflectNormalToggle
	{
		get
		{
			return UberShader.kProperties[34];
		}
	}

	public static UberShaderProperty ReflectTex
	{
		get
		{
			return UberShader.kProperties[35];
		}
	}

	public static UberShaderProperty ReflectNormalTex
	{
		get
		{
			return UberShader.kProperties[36];
		}
	}

	public static UberShaderProperty ReflectAlbedoTint
	{
		get
		{
			return UberShader.kProperties[37];
		}
	}

	public static UberShaderProperty ReflectTint
	{
		get
		{
			return UberShader.kProperties[38];
		}
	}

	public static UberShaderProperty ReflectOpacity
	{
		get
		{
			return UberShader.kProperties[39];
		}
	}

	public static UberShaderProperty ReflectExposure
	{
		get
		{
			return UberShader.kProperties[40];
		}
	}

	public static UberShaderProperty ReflectOffset
	{
		get
		{
			return UberShader.kProperties[41];
		}
	}

	public static UberShaderProperty ReflectScale
	{
		get
		{
			return UberShader.kProperties[42];
		}
	}

	public static UberShaderProperty ReflectRotate
	{
		get
		{
			return UberShader.kProperties[43];
		}
	}

	public static UberShaderProperty ParallaxToggle
	{
		get
		{
			return UberShader.kProperties[44];
		}
	}

	public static UberShaderProperty ParallaxAAToggle
	{
		get
		{
			return UberShader.kProperties[45];
		}
	}

	public static UberShaderProperty ParallaxAABias
	{
		get
		{
			return UberShader.kProperties[46];
		}
	}

	public static UberShaderProperty DepthMap
	{
		get
		{
			return UberShader.kProperties[47];
		}
	}

	public static UberShaderProperty ParallaxAmplitude
	{
		get
		{
			return UberShader.kProperties[48];
		}
	}

	public static UberShaderProperty ParallaxSamplesMinMax
	{
		get
		{
			return UberShader.kProperties[49];
		}
	}

	public static UberShaderProperty UvShiftToggle
	{
		get
		{
			return UberShader.kProperties[50];
		}
	}

	public static UberShaderProperty UvShiftSteps
	{
		get
		{
			return UberShader.kProperties[51];
		}
	}

	public static UberShaderProperty UvShiftRate
	{
		get
		{
			return UberShader.kProperties[52];
		}
	}

	public static UberShaderProperty UvShiftOffset
	{
		get
		{
			return UberShader.kProperties[53];
		}
	}

	public static UberShaderProperty UseCrystalEffect
	{
		get
		{
			return UberShader.kProperties[54];
		}
	}

	public static UberShaderProperty CrystalPower
	{
		get
		{
			return UberShader.kProperties[55];
		}
	}

	public static UberShaderProperty CrystalRimColor
	{
		get
		{
			return UberShader.kProperties[56];
		}
	}

	public static UberShaderProperty LiquidVolume
	{
		get
		{
			return UberShader.kProperties[57];
		}
	}

	public static UberShaderProperty LiquidFill
	{
		get
		{
			return UberShader.kProperties[58];
		}
	}

	public static UberShaderProperty LiquidFillNormal
	{
		get
		{
			return UberShader.kProperties[59];
		}
	}

	public static UberShaderProperty LiquidSurfaceColor
	{
		get
		{
			return UberShader.kProperties[60];
		}
	}

	public static UberShaderProperty LiquidSwayX
	{
		get
		{
			return UberShader.kProperties[61];
		}
	}

	public static UberShaderProperty LiquidSwayY
	{
		get
		{
			return UberShader.kProperties[62];
		}
	}

	public static UberShaderProperty LiquidContainer
	{
		get
		{
			return UberShader.kProperties[63];
		}
	}

	public static UberShaderProperty LiquidPlanePosition
	{
		get
		{
			return UberShader.kProperties[64];
		}
	}

	public static UberShaderProperty LiquidPlaneNormal
	{
		get
		{
			return UberShader.kProperties[65];
		}
	}

	public static UberShaderProperty VertexFlapToggle
	{
		get
		{
			return UberShader.kProperties[66];
		}
	}

	public static UberShaderProperty VertexFlapAxis
	{
		get
		{
			return UberShader.kProperties[67];
		}
	}

	public static UberShaderProperty VertexFlapDegreesMinMax
	{
		get
		{
			return UberShader.kProperties[68];
		}
	}

	public static UberShaderProperty VertexFlapSpeed
	{
		get
		{
			return UberShader.kProperties[69];
		}
	}

	public static UberShaderProperty VertexFlapPhaseOffset
	{
		get
		{
			return UberShader.kProperties[70];
		}
	}

	public static UberShaderProperty VertexWaveToggle
	{
		get
		{
			return UberShader.kProperties[71];
		}
	}

	public static UberShaderProperty VertexWaveParams
	{
		get
		{
			return UberShader.kProperties[72];
		}
	}

	public static UberShaderProperty VertexWaveStartOffset
	{
		get
		{
			return UberShader.kProperties[73];
		}
	}

	public static UberShaderProperty VertexWavePhaseOffset
	{
		get
		{
			return UberShader.kProperties[74];
		}
	}

	public static UberShaderProperty VertexRotateToggle
	{
		get
		{
			return UberShader.kProperties[75];
		}
	}

	public static UberShaderProperty VertexRotateAngles
	{
		get
		{
			return UberShader.kProperties[76];
		}
	}

	public static UberShaderProperty UseEyeTracking
	{
		get
		{
			return UberShader.kProperties[77];
		}
	}

	public static UberShaderProperty EyeTileOffsetUV
	{
		get
		{
			return UberShader.kProperties[78];
		}
	}

	public static UberShaderProperty EyeOverrideUV
	{
		get
		{
			return UberShader.kProperties[79];
		}
	}

	public static UberShaderProperty EyeOverrideUVTransform
	{
		get
		{
			return UberShader.kProperties[80];
		}
	}

	public static UberShaderProperty UseMouthFlap
	{
		get
		{
			return UberShader.kProperties[81];
		}
	}

	public static UberShaderProperty MouthMap
	{
		get
		{
			return UberShader.kProperties[82];
		}
	}

	public static UberShaderProperty MouthMap_Atlas
	{
		get
		{
			return UberShader.kProperties[83];
		}
	}

	public static UberShaderProperty MouthMap_AtlasSlice
	{
		get
		{
			return UberShader.kProperties[84];
		}
	}

	public static UberShaderProperty UseVertexColor
	{
		get
		{
			return UberShader.kProperties[85];
		}
	}

	public static UberShaderProperty WaterEffect
	{
		get
		{
			return UberShader.kProperties[86];
		}
	}

	public static UberShaderProperty HeightBasedWaterEffect
	{
		get
		{
			return UberShader.kProperties[87];
		}
	}

	public static UberShaderProperty UseDayNightLightmap
	{
		get
		{
			return UberShader.kProperties[88];
		}
	}

	public static UberShaderProperty UseSpecular
	{
		get
		{
			return UberShader.kProperties[89];
		}
	}

	public static UberShaderProperty UseSpecularAlphaChannel
	{
		get
		{
			return UberShader.kProperties[90];
		}
	}

	public static UberShaderProperty Smoothness
	{
		get
		{
			return UberShader.kProperties[91];
		}
	}

	public static UberShaderProperty EmissionToggle
	{
		get
		{
			return UberShader.kProperties[92];
		}
	}

	public static UberShaderProperty EmissionColor
	{
		get
		{
			return UberShader.kProperties[93];
		}
	}

	public static UberShaderProperty EmissionMap
	{
		get
		{
			return UberShader.kProperties[94];
		}
	}

	public static UberShaderProperty EmissionMaskByBaseMapAlpha
	{
		get
		{
			return UberShader.kProperties[95];
		}
	}

	public static UberShaderProperty EmissionUVScrollSpeed
	{
		get
		{
			return UberShader.kProperties[96];
		}
	}

	public static UberShaderProperty EmissionDissolveProgress
	{
		get
		{
			return UberShader.kProperties[97];
		}
	}

	public static UberShaderProperty EmissionDissolveEdgeSize
	{
		get
		{
			return UberShader.kProperties[98];
		}
	}

	public static UberShaderProperty EmissionUseUVWaveWarp
	{
		get
		{
			return UberShader.kProperties[99];
		}
	}

	public static UberShaderProperty Cull
	{
		get
		{
			return UberShader.kProperties[100];
		}
	}

	public static UberShaderProperty StencilReference
	{
		get
		{
			return UberShader.kProperties[101];
		}
	}

	public static UberShaderProperty StencilComparison
	{
		get
		{
			return UberShader.kProperties[102];
		}
	}

	public static UberShaderProperty StencilPassFront
	{
		get
		{
			return UberShader.kProperties[103];
		}
	}

	public static UberShaderProperty USE_DEFORM_MAP
	{
		get
		{
			return UberShader.kProperties[104];
		}
	}

	public static UberShaderProperty DeformMap
	{
		get
		{
			return UberShader.kProperties[105];
		}
	}

	public static UberShaderProperty DeformMapIntensity
	{
		get
		{
			return UberShader.kProperties[106];
		}
	}

	public static UberShaderProperty DeformMapMaskByVertColorRAmount
	{
		get
		{
			return UberShader.kProperties[107];
		}
	}

	public static UberShaderProperty DeformMapScrollSpeed
	{
		get
		{
			return UberShader.kProperties[108];
		}
	}

	public static UberShaderProperty DeformMapUV0Influence
	{
		get
		{
			return UberShader.kProperties[109];
		}
	}

	public static UberShaderProperty DeformMapObjectSpaceOffsetsU
	{
		get
		{
			return UberShader.kProperties[110];
		}
	}

	public static UberShaderProperty DeformMapObjectSpaceOffsetsV
	{
		get
		{
			return UberShader.kProperties[111];
		}
	}

	public static UberShaderProperty DeformMapWorldSpaceOffsetsU
	{
		get
		{
			return UberShader.kProperties[112];
		}
	}

	public static UberShaderProperty DeformMapWorldSpaceOffsetsV
	{
		get
		{
			return UberShader.kProperties[113];
		}
	}

	public static UberShaderProperty RotateOnYAxisBySinTime
	{
		get
		{
			return UberShader.kProperties[114];
		}
	}

	public static UberShaderProperty USE_TEX_ARRAY_ATLAS
	{
		get
		{
			return UberShader.kProperties[115];
		}
	}

	public static UberShaderProperty BaseMap_Atlas
	{
		get
		{
			return UberShader.kProperties[116];
		}
	}

	public static UberShaderProperty BaseMap_AtlasSlice
	{
		get
		{
			return UberShader.kProperties[117];
		}
	}

	public static UberShaderProperty EmissionMap_Atlas
	{
		get
		{
			return UberShader.kProperties[118];
		}
	}

	public static UberShaderProperty EmissionMap_AtlasSlice
	{
		get
		{
			return UberShader.kProperties[119];
		}
	}

	public static UberShaderProperty DeformMap_Atlas
	{
		get
		{
			return UberShader.kProperties[120];
		}
	}

	public static UberShaderProperty DeformMap_AtlasSlice
	{
		get
		{
			return UberShader.kProperties[121];
		}
	}

	public static UberShaderProperty SrcBlend
	{
		get
		{
			return UberShader.kProperties[122];
		}
	}

	public static UberShaderProperty DstBlend
	{
		get
		{
			return UberShader.kProperties[123];
		}
	}

	public static UberShaderProperty SrcBlendAlpha
	{
		get
		{
			return UberShader.kProperties[124];
		}
	}

	public static UberShaderProperty DstBlendAlpha
	{
		get
		{
			return UberShader.kProperties[125];
		}
	}

	public static UberShaderProperty ZWrite
	{
		get
		{
			return UberShader.kProperties[126];
		}
	}

	public static UberShaderProperty AlphaToMask
	{
		get
		{
			return UberShader.kProperties[127];
		}
	}

	public static UberShaderProperty Surface
	{
		get
		{
			return UberShader.kProperties[129];
		}
	}

	public static UberShaderProperty Metallic
	{
		get
		{
			return UberShader.kProperties[130];
		}
	}

	public static UberShaderProperty SpecColor
	{
		get
		{
			return UberShader.kProperties[131];
		}
	}

	public static UberShaderProperty DayNightLightmapArray
	{
		get
		{
			return UberShader.kProperties[132];
		}
	}

	public static UberShaderProperty DayNightLightmapArray_AtlasSlice
	{
		get
		{
			return UberShader.kProperties[133];
		}
	}

	public static UberShaderProperty SingleLightmap
	{
		get
		{
			return UberShader.kProperties[134];
		}
	}

	public static Material kReferenceMaterial;

	public static Shader kReferenceShader;

	private static UberShaderProperty[] kProperties;
}
