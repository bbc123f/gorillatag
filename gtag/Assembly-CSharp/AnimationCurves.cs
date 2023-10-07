using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public static class AnimationCurves
{
	// Token: 0x06000B96 RID: 2966 RVA: 0x00047968 File Offset: 0x00045B68
	static AnimationCurves()
	{
		Dictionary<AnimationCurves.EaseType, AnimationCurve> dictionary = new Dictionary<AnimationCurves.EaseType, AnimationCurve>();
		dictionary[AnimationCurves.EaseType.EaseInQuad] = AnimationCurves.EaseInQuad;
		dictionary[AnimationCurves.EaseType.EaseOutQuad] = AnimationCurves.EaseOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInOutQuad] = AnimationCurves.EaseInOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInCubic] = AnimationCurves.EaseInCubic;
		dictionary[AnimationCurves.EaseType.EaseOutCubic] = AnimationCurves.EaseOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInOutCubic] = AnimationCurves.EaseInOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInQuart] = AnimationCurves.EaseInQuart;
		dictionary[AnimationCurves.EaseType.EaseOutQuart] = AnimationCurves.EaseOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInOutQuart] = AnimationCurves.EaseInOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInQuint] = AnimationCurves.EaseInQuint;
		dictionary[AnimationCurves.EaseType.EaseOutQuint] = AnimationCurves.EaseOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInOutQuint] = AnimationCurves.EaseInOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInSine] = AnimationCurves.EaseInSine;
		dictionary[AnimationCurves.EaseType.EaseOutSine] = AnimationCurves.EaseOutSine;
		dictionary[AnimationCurves.EaseType.EaseInOutSine] = AnimationCurves.EaseInOutSine;
		dictionary[AnimationCurves.EaseType.EaseInExpo] = AnimationCurves.EaseInExpo;
		dictionary[AnimationCurves.EaseType.EaseOutExpo] = AnimationCurves.EaseOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInOutExpo] = AnimationCurves.EaseInOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInCirc] = AnimationCurves.EaseInCirc;
		dictionary[AnimationCurves.EaseType.EaseOutCirc] = AnimationCurves.EaseOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInOutCirc] = AnimationCurves.EaseInOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInBounce] = AnimationCurves.EaseInBounce;
		dictionary[AnimationCurves.EaseType.EaseOutBounce] = AnimationCurves.EaseOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInOutBounce] = AnimationCurves.EaseInOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInBack] = AnimationCurves.EaseInBack;
		dictionary[AnimationCurves.EaseType.EaseOutBack] = AnimationCurves.EaseOutBack;
		dictionary[AnimationCurves.EaseType.EaseInOutBack] = AnimationCurves.EaseInOutBack;
		dictionary[AnimationCurves.EaseType.EaseInElastic] = AnimationCurves.EaseInElastic;
		dictionary[AnimationCurves.EaseType.EaseOutElastic] = AnimationCurves.EaseOutElastic;
		dictionary[AnimationCurves.EaseType.EaseInOutElastic] = AnimationCurves.EaseInOutElastic;
		dictionary[AnimationCurves.EaseType.Spring] = AnimationCurves.Spring;
		dictionary[AnimationCurves.EaseType.Linear] = AnimationCurves.Linear;
		dictionary[AnimationCurves.EaseType.Step] = AnimationCurves.Step;
		AnimationCurves.gEaseTypeToCurve = dictionary;
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00048E9A File Offset: 0x0004709A
	public static AnimationCurve GetCurveForEase(AnimationCurves.EaseType ease)
	{
		return AnimationCurves.gEaseTypeToCurve[ease];
	}

	// Token: 0x04000F26 RID: 3878
	public static readonly AnimationCurve EaseInQuad = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
		new Keyframe(1f, 1f, 2.000003f, 0f, 0.333333f, 0f)
	});

	// Token: 0x04000F27 RID: 3879
	public static readonly AnimationCurve EaseOutQuad = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 2.000003f, 0f, 0.333333f),
		new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
	});

	// Token: 0x04000F28 RID: 3880
	public static readonly AnimationCurve EaseInOutQuad = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
		new Keyframe(0.5f, 0.5f, 1.999994f, 1.999994f, 0.333334f, 0.333334f),
		new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
	});

	// Token: 0x04000F29 RID: 3881
	public static readonly AnimationCurve EaseInCubic = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
		new Keyframe(1f, 1f, 3.000003f, 0f, 0.333333f, 0f)
	});

	// Token: 0x04000F2A RID: 3882
	public static readonly AnimationCurve EaseOutCubic = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 3.000003f, 0f, 0.333333f),
		new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
	});

	// Token: 0x04000F2B RID: 3883
	public static readonly AnimationCurve EaseInOutCubic = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
		new Keyframe(0.5f, 0.5f, 2.999994f, 2.999994f, 0.333334f, 0.333334f),
		new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
	});

	// Token: 0x04000F2C RID: 3884
	public static readonly AnimationCurve EaseInQuart = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.0139424f, 0f, 0.434789f),
		new Keyframe(1f, 1f, 3.985819f, 0f, 0.269099f, 0f)
	});

	// Token: 0x04000F2D RID: 3885
	public static readonly AnimationCurve EaseOutQuart = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 3.985823f, 0f, 0.269099f),
		new Keyframe(1f, 1f, 0.01394233f, 0f, 0.434789f, 0f)
	});

	// Token: 0x04000F2E RID: 3886
	public static readonly AnimationCurve EaseInOutQuart = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.01394243f, 0f, 0.434788f),
		new Keyframe(0.5f, 0.5f, 3.985842f, 3.985834f, 0.269098f, 0.269098f),
		new Keyframe(1f, 1f, 0.0139425f, 0f, 0.434788f, 0f)
	});

	// Token: 0x04000F2F RID: 3887
	public static readonly AnimationCurve EaseInQuint = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.02411811f, 0f, 0.519568f),
		new Keyframe(1f, 1f, 4.951815f, 0f, 0.225963f, 0f)
	});

	// Token: 0x04000F30 RID: 3888
	public static readonly AnimationCurve EaseOutQuint = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 4.953289f, 0f, 0.225963f),
		new Keyframe(1f, 1f, 0.02414908f, 0f, 0.518901f, 0f)
	});

	// Token: 0x04000F31 RID: 3889
	public static readonly AnimationCurve EaseInOutQuint = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.02412004f, 0f, 0.519568f),
		new Keyframe(0.5f, 0.5f, 4.951789f, 4.953269f, 0.225964f, 0.225964f),
		new Keyframe(1f, 1f, 0.02415099f, 0f, 0.5189019f, 0f)
	});

	// Token: 0x04000F32 RID: 3890
	public static readonly AnimationCurve EaseInSine = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, -0.001208493f, 0f, 0.36078f),
		new Keyframe(1f, 1f, 1.572508f, 0f, 0.326514f, 0f)
	});

	// Token: 0x04000F33 RID: 3891
	public static readonly AnimationCurve EaseOutSine = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1.573552f, 0f, 0.330931f),
		new Keyframe(1f, 1f, -0.0009282457f, 0f, 0.358689f, 0f)
	});

	// Token: 0x04000F34 RID: 3892
	public static readonly AnimationCurve EaseInOutSine = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, -0.001202949f, 0f, 0.36078f),
		new Keyframe(0.5f, 0.5f, 1.572508f, 1.573372f, 0.326514f, 0.33093f),
		new Keyframe(1f, 1f, -0.0009312395f, 0f, 0.358688f, 0f)
	});

	// Token: 0x04000F35 RID: 3893
	public static readonly AnimationCurve EaseInExpo = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.03124388f, 0f, 0.636963f),
		new Keyframe(1f, 1f, 6.815432f, 0f, 0.155667f, 0f)
	});

	// Token: 0x04000F36 RID: 3894
	public static readonly AnimationCurve EaseOutExpo = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 6.815433f, 0f, 0.155667f),
		new Keyframe(1f, 1f, 0.03124354f, 0f, 0.636963f, 0f)
	});

	// Token: 0x04000F37 RID: 3895
	public static readonly AnimationCurve EaseInOutExpo = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.03124509f, 0f, 0.636964f),
		new Keyframe(0.5f, 0.5f, 6.815477f, 6.815476f, 0.155666f, 0.155666f),
		new Keyframe(1f, 1f, 0.03124377f, 0f, 0.636964f, 0f)
	});

	// Token: 0x04000F38 RID: 3896
	public static readonly AnimationCurve EaseInCirc = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.002162338f, 0f, 0.55403f),
		new Keyframe(1f, 1f, 459.267f, 0f, 0.001197994f, 0f)
	});

	// Token: 0x04000F39 RID: 3897
	public static readonly AnimationCurve EaseOutCirc = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 461.7679f, 0f, 0.001198f),
		new Keyframe(1f, 1f, 0.00216235f, 0f, 0.554024f, 0f)
	});

	// Token: 0x04000F3A RID: 3898
	public static readonly AnimationCurve EaseInOutCirc = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.002162353f, 0f, 0.554026f),
		new Keyframe(0.5f, 0.5f, 461.7703f, 461.7474f, 0.001197994f, 0.001198053f),
		new Keyframe(1f, 1f, 0.00216245f, 0f, 0.554026f, 0f)
	});

	// Token: 0x04000F3B RID: 3899
	public static readonly AnimationCurve EaseInBounce = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.6874897f, 0f, 0.3333663f),
		new Keyframe(0.0909f, 0f, -0.687694f, 1.374792f, 0.3332673f, 0.3334159f),
		new Keyframe(0.2727f, 0f, -1.375608f, 2.749388f, 0.3332179f, 0.3333489f),
		new Keyframe(0.6364f, 0f, -2.749183f, 5.501642f, 0.3333737f, 0.3332673f),
		new Keyframe(1f, 1f, 0f, 0f, 0.3333663f, 0f)
	});

	// Token: 0x04000F3C RID: 3900
	public static readonly AnimationCurve EaseOutBounce = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.3333663f),
		new Keyframe(0.3636f, 1f, 5.501643f, -2.749183f, 0.3332673f, 0.3333737f),
		new Keyframe(0.7273f, 1f, 2.749366f, -1.375609f, 0.3333516f, 0.3332178f),
		new Keyframe(0.9091f, 1f, 1.374792f, -0.6877043f, 0.3334158f, 0.3332673f),
		new Keyframe(1f, 1f, 0.6875f, 0f, 0.3333663f, 0f)
	});

	// Token: 0x04000F3D RID: 3901
	public static readonly AnimationCurve EaseInOutBounce = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.6875001f, 0f, 0.333011f),
		new Keyframe(0.0455f, 0f, -0.6854643f, 1.377057f, 0.334f, 0.3328713f),
		new Keyframe(0.1364f, 0f, -1.373381f, 2.751643f, 0.3337624f, 0.3331683f),
		new Keyframe(0.3182f, 0f, -2.749192f, 5.501634f, 0.3334654f, 0.3332673f),
		new Keyframe(0.5f, 0.5f, 0f, 0f, 0.3333663f, 0.3333663f),
		new Keyframe(0.6818f, 1f, 5.501634f, -2.749191f, 0.3332673f, 0.3334653f),
		new Keyframe(0.8636f, 1f, 2.751642f, -1.37338f, 0.3331683f, 0.3319367f),
		new Keyframe(0.955f, 1f, 1.354673f, -0.7087823f, 0.3365205f, 0.3266002f),
		new Keyframe(1f, 1f, 0.6875f, 0f, 0.3367105f, 0f)
	});

	// Token: 0x04000F3E RID: 3902
	public static readonly AnimationCurve EaseInBack = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
		new Keyframe(1f, 1f, 4.701583f, 0f, 0.333333f, 0f)
	});

	// Token: 0x04000F3F RID: 3903
	public static readonly AnimationCurve EaseOutBack = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 4.701584f, 0f, 0.333333f),
		new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
	});

	// Token: 0x04000F40 RID: 3904
	public static readonly AnimationCurve EaseInOutBack = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
		new Keyframe(0.5f, 0.5f, 5.594898f, 5.594899f, 0.333334f, 0.333334f),
		new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
	});

	// Token: 0x04000F41 RID: 3905
	public static readonly AnimationCurve EaseInElastic = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.0143284f, 0f, 1f),
		new Keyframe(0.175f, 0f, 0f, -0.06879552f, 0.008331452f, 0.8916667f),
		new Keyframe(0.475f, 0f, -0.4081632f, -0.5503653f, 0.4083333f, 0.8666668f),
		new Keyframe(0.775f, 0f, -3.26241f, -4.402922f, 0.3916665f, 0.5916666f),
		new Keyframe(1f, 1f, 12.51956f, 0f, 0.5916666f, 0f)
	});

	// Token: 0x04000F42 RID: 3906
	public static readonly AnimationCurve EaseOutElastic = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 12.51956f, 0f, 0.5916667f),
		new Keyframe(0.225f, 1f, -4.402922f, -3.262408f, 0.5916666f, 0.3916667f),
		new Keyframe(0.525f, 1f, -0.5503654f, -0.4081634f, 0.8666667f, 0.4083333f),
		new Keyframe(0.825f, 1f, -0.06879558f, 0f, 0.8916666f, 0.008331367f),
		new Keyframe(1f, 1f, 0.01432861f, 0f, 1f, 0f)
	});

	// Token: 0x04000F43 RID: 3907
	public static readonly AnimationCurve EaseInOutElastic = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0.01433143f, 0f, 1f),
		new Keyframe(0.0875f, 0f, 0f, -0.06879253f, 0.008331452f, 0.8916667f),
		new Keyframe(0.2375f, 0f, -0.4081632f, -0.5503692f, 0.4083333f, 0.8666668f),
		new Keyframe(0.3875f, 0f, -3.262419f, -4.402895f, 0.3916665f, 0.5916712f),
		new Keyframe(0.5f, 0.5f, 12.51967f, 12.51958f, 0.5916621f, 0.5916664f),
		new Keyframe(0.6125f, 1f, -4.402927f, -3.262402f, 0.5916669f, 0.3916666f),
		new Keyframe(0.7625f, 1f, -0.5503691f, -0.4081627f, 0.8666668f, 0.4083335f),
		new Keyframe(0.9125f, 1f, -0.06879289f, 0f, 0.8916666f, 0.008331029f),
		new Keyframe(1f, 1f, 0.01432828f, 0f, 1f, 0f)
	});

	// Token: 0x04000F44 RID: 3908
	public static readonly AnimationCurve Spring = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 3.582263f, 0f, 0.2385296f),
		new Keyframe(0.336583f, 0.828268f, 1.767519f, 1.767491f, 0.4374225f, 0.2215123f),
		new Keyframe(0.550666f, 1.079651f, 0.3095257f, 0.3095275f, 0.4695607f, 0.4154884f),
		new Keyframe(0.779498f, 0.974607f, -0.2321364f, -0.2321428f, 0.3585643f, 0.3623514f),
		new Keyframe(0.897999f, 1.003668f, 0.2797853f, 0.2797431f, 0.3331026f, 0.3306926f),
		new Keyframe(1f, 1f, -0.2023914f, 0f, 0.3296829f, 0f)
	});

	// Token: 0x04000F45 RID: 3909
	public static readonly AnimationCurve Linear = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1f, 0f, 0f),
		new Keyframe(1f, 1f, 1f, 0f, 0f, 0f)
	});

	// Token: 0x04000F46 RID: 3910
	public static readonly AnimationCurve Step = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.5f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.5f, 1f, 0f, 0f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
	});

	// Token: 0x04000F47 RID: 3911
	private static Dictionary<AnimationCurves.EaseType, AnimationCurve> gEaseTypeToCurve;

	// Token: 0x02000458 RID: 1112
	public enum EaseType
	{
		// Token: 0x04001DF7 RID: 7671
		EaseInQuad = 1,
		// Token: 0x04001DF8 RID: 7672
		EaseOutQuad,
		// Token: 0x04001DF9 RID: 7673
		EaseInOutQuad,
		// Token: 0x04001DFA RID: 7674
		EaseInCubic,
		// Token: 0x04001DFB RID: 7675
		EaseOutCubic,
		// Token: 0x04001DFC RID: 7676
		EaseInOutCubic,
		// Token: 0x04001DFD RID: 7677
		EaseInQuart,
		// Token: 0x04001DFE RID: 7678
		EaseOutQuart,
		// Token: 0x04001DFF RID: 7679
		EaseInOutQuart,
		// Token: 0x04001E00 RID: 7680
		EaseInQuint,
		// Token: 0x04001E01 RID: 7681
		EaseOutQuint,
		// Token: 0x04001E02 RID: 7682
		EaseInOutQuint,
		// Token: 0x04001E03 RID: 7683
		EaseInSine,
		// Token: 0x04001E04 RID: 7684
		EaseOutSine,
		// Token: 0x04001E05 RID: 7685
		EaseInOutSine,
		// Token: 0x04001E06 RID: 7686
		EaseInExpo,
		// Token: 0x04001E07 RID: 7687
		EaseOutExpo,
		// Token: 0x04001E08 RID: 7688
		EaseInOutExpo,
		// Token: 0x04001E09 RID: 7689
		EaseInCirc,
		// Token: 0x04001E0A RID: 7690
		EaseOutCirc,
		// Token: 0x04001E0B RID: 7691
		EaseInOutCirc,
		// Token: 0x04001E0C RID: 7692
		EaseInBounce,
		// Token: 0x04001E0D RID: 7693
		EaseOutBounce,
		// Token: 0x04001E0E RID: 7694
		EaseInOutBounce,
		// Token: 0x04001E0F RID: 7695
		EaseInBack,
		// Token: 0x04001E10 RID: 7696
		EaseOutBack,
		// Token: 0x04001E11 RID: 7697
		EaseInOutBack,
		// Token: 0x04001E12 RID: 7698
		EaseInElastic,
		// Token: 0x04001E13 RID: 7699
		EaseOutElastic,
		// Token: 0x04001E14 RID: 7700
		EaseInOutElastic,
		// Token: 0x04001E15 RID: 7701
		Spring,
		// Token: 0x04001E16 RID: 7702
		Linear,
		// Token: 0x04001E17 RID: 7703
		Step
	}
}
