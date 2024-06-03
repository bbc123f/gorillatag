using System;
using UnityEngine;

public static class FusionScalableIMGUI
{
	private static void InitializedGUIStyles(GUISkin baseSkin)
	{
		FusionScalableIMGUI._scalableSkin = ((baseSkin == null) ? GUI.skin : baseSkin);
		if (baseSkin == null)
		{
			FusionScalableIMGUI._scalableSkin = GUI.skin;
			FusionScalableIMGUI._scalableSkin.button.alignment = TextAnchor.MiddleCenter;
			FusionScalableIMGUI._scalableSkin.label.alignment = TextAnchor.MiddleCenter;
			FusionScalableIMGUI._scalableSkin.textField.alignment = TextAnchor.MiddleCenter;
			FusionScalableIMGUI._scalableSkin.button.normal.background = FusionScalableIMGUI._scalableSkin.box.normal.background;
			FusionScalableIMGUI._scalableSkin.button.hover.background = FusionScalableIMGUI._scalableSkin.window.normal.background;
			FusionScalableIMGUI._scalableSkin.button.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
			FusionScalableIMGUI._scalableSkin.button.hover.textColor = new Color(1f, 1f, 1f);
			FusionScalableIMGUI._scalableSkin.button.active.textColor = new Color(1f, 1f, 1f);
			FusionScalableIMGUI._scalableSkin.button.border = new RectOffset(6, 6, 6, 6);
			FusionScalableIMGUI._scalableSkin.window.border = new RectOffset(8, 8, 8, 10);
			return;
		}
		FusionScalableIMGUI._scalableSkin = baseSkin;
	}

	public static GUISkin GetScaledSkin(GUISkin baseSkin, out float height, out float width, out int padding, out int margin, out float boxLeft)
	{
		if (FusionScalableIMGUI._scalableSkin == null)
		{
			FusionScalableIMGUI.InitializedGUIStyles(baseSkin);
		}
		ValueTuple<float, float, int, int, float> valueTuple = FusionScalableIMGUI.ScaleGuiSkinToScreenHeight();
		height = valueTuple.Item1;
		width = valueTuple.Item2;
		padding = valueTuple.Item3;
		margin = valueTuple.Item4;
		boxLeft = valueTuple.Item5;
		return FusionScalableIMGUI._scalableSkin;
	}

	public static ValueTuple<float, float, int, int, float> ScaleGuiSkinToScreenHeight()
	{
		int height = Screen.height;
		int width = Screen.width;
		bool flag = (float)(Screen.height / Screen.width) > 1.8888888f;
		float num = (float)Screen.height * 0.08f;
		float num2 = Math.Min((float)Screen.width * 0.9f, (float)Screen.height * 0.6f);
		int num3 = (int)(num / 4f);
		int num4 = (int)(num / 8f);
		float item = ((float)Screen.width - num2) * 0.5f;
		int fontSize = (int)(flag ? ((num2 - (float)(num3 * 2)) * 0.07f) : (num * 0.4f));
		RectOffset margin = new RectOffset(0, 0, num4, num4);
		FusionScalableIMGUI._scalableSkin.button.fontSize = fontSize;
		FusionScalableIMGUI._scalableSkin.button.margin = margin;
		FusionScalableIMGUI._scalableSkin.label.fontSize = fontSize;
		FusionScalableIMGUI._scalableSkin.label.padding = new RectOffset(num3, num3, num3, num3);
		FusionScalableIMGUI._scalableSkin.textField.fontSize = fontSize;
		FusionScalableIMGUI._scalableSkin.window.padding = new RectOffset(num3, num3, num3, num3);
		FusionScalableIMGUI._scalableSkin.window.margin = new RectOffset(num4, num4, num4, num4);
		return new ValueTuple<float, float, int, int, float>(num, num2, num3, num4, item);
	}

	private static GUISkin _scalableSkin;
}
