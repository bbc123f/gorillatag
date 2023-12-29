using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ColorUtils
{
	public static Color ComposeHDR(Color baseColor, float intensity)
	{
		intensity = Mathf.Clamp(intensity, -10f, 10f);
		Color color = baseColor;
		if (baseColor.maxColorComponent > 1f)
		{
			color = ColorUtils.DecomposeHDR(baseColor).Item1;
		}
		float b = Mathf.Pow(2f, intensity);
		if (QualitySettings.activeColorSpace == ColorSpace.Linear)
		{
			b = Mathf.GammaToLinearSpace(intensity);
		}
		color *= b;
		color.a = baseColor.a;
		return color;
	}

	[return: TupleElementNames(new string[]
	{
		"baseColor",
		"intensity"
	})]
	public static ValueTuple<Color, float> DecomposeHDR(Color hdrColor)
	{
		Color32 c = default(Color32);
		float item = 0f;
		float maxColorComponent = hdrColor.maxColorComponent;
		if (maxColorComponent == 0f || (maxColorComponent <= 1f && maxColorComponent >= 0.003921569f))
		{
			c.r = (byte)Mathf.RoundToInt(hdrColor.r * 255f);
			c.g = (byte)Mathf.RoundToInt(hdrColor.g * 255f);
			c.b = (byte)Mathf.RoundToInt(hdrColor.b * 255f);
		}
		else
		{
			float num = 191f / maxColorComponent;
			item = Mathf.Log(255f / num) / Mathf.Log(2f);
			c.r = Math.Min(191, (byte)Mathf.CeilToInt(num * hdrColor.r));
			c.g = Math.Min(191, (byte)Mathf.CeilToInt(num * hdrColor.g));
			c.b = Math.Min(191, (byte)Mathf.CeilToInt(num * hdrColor.b));
		}
		return new ValueTuple<Color, float>(c, item);
	}

	private const byte kMaxByteForOverexposedColor = 191;
}
