using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fusion.StatsInternal
{
	public static class FusionStatsUtilities
	{
		public static List<string> CachedTelemetryNames
		{
			get
			{
				if (FusionStatsUtilities._cachedGraphVisualizationNames == null)
				{
					Type typeFromHandle = typeof(FusionGraphVisualization);
					string[] names = Enum.GetNames(typeFromHandle);
					FusionStatsUtilities._cachedGraphVisualizationNames = new List<string>(names.Length);
					for (int i = 0; i < names.Length; i++)
					{
						string item;
						try
						{
							item = ((DescriptionAttribute)typeFromHandle.GetMember(names[i])[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
						}
						catch
						{
							item = names[i];
						}
						FusionStatsUtilities._cachedGraphVisualizationNames.Add(item);
					}
				}
				return FusionStatsUtilities._cachedGraphVisualizationNames;
			}
		}

		private static Texture2D MeterTexture
		{
			get
			{
				if (FusionStatsUtilities._meterTexture == null)
				{
					Texture2D texture2D = new Texture2D(512, 2);
					for (int i = 0; i < 512; i++)
					{
						for (int j = 0; j < 2; j++)
						{
							Color color = (i != 0 && i % 16 == 0) ? new Color(1f, 1f, 1f, 0.75f) : new Color(1f, 1f, 1f, 1f);
							texture2D.SetPixel(i, j, color);
						}
					}
					texture2D.Apply();
					return FusionStatsUtilities._meterTexture = texture2D;
				}
				return FusionStatsUtilities._meterTexture;
			}
		}

		public static Sprite MeterSprite
		{
			get
			{
				if (FusionStatsUtilities._meterSprite == null)
				{
					FusionStatsUtilities._meterSprite = Sprite.Create(FusionStatsUtilities.MeterTexture, new Rect(0f, 0f, 512f, 2f), default(Vector2));
				}
				return FusionStatsUtilities._meterSprite;
			}
		}

		private static Texture2D Circle32Texture
		{
			get
			{
				if (FusionStatsUtilities._circle32Texture == null)
				{
					Texture2D texture2D = new Texture2D(128, 128);
					for (int i = 0; i < 64; i++)
					{
						for (int j = 0; j < 64; j++)
						{
							double num = Math.Abs(Math.Sqrt((double)(i * i + j * j)));
							float a = (num > 64.0) ? 0f : ((num < 63.0) ? 1f : ((float)(64.0 - num)));
							Color color = new Color(1f, 1f, 1f, a);
							texture2D.SetPixel(64 + i, 64 + j, color);
							texture2D.SetPixel(63 - i, 64 + j, color);
							texture2D.SetPixel(64 + i, 63 - j, color);
							texture2D.SetPixel(63 - i, 63 - j, color);
						}
					}
					texture2D.Apply();
					return FusionStatsUtilities._circle32Texture = texture2D;
				}
				return FusionStatsUtilities._circle32Texture;
			}
		}

		public static Sprite CircleSprite
		{
			get
			{
				if (FusionStatsUtilities._circle32Sprite == null)
				{
					FusionStatsUtilities._circle32Sprite = Sprite.Create(FusionStatsUtilities.Circle32Texture, new Rect(0f, 0f, 128f, 128f), new Vector2(64f, 64f), 10f, 0U, SpriteMeshType.Tight, new Vector4(63f, 63f, 63f, 63f));
				}
				return FusionStatsUtilities._circle32Sprite;
			}
		}

		public static bool TryFindActiveRunner(FusionStats fusionStats, out NetworkRunner runner, SimulationModes? mode = null)
		{
			GameObject gameObject = fusionStats.gameObject;
			Scene scene = fusionStats.gameObject.scene;
			List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
			while (instancesEnumerator.MoveNext())
			{
				NetworkRunner networkRunner = instancesEnumerator.Current;
				if (networkRunner && networkRunner.IsRunning && (mode == null || (mode.Value & networkRunner.Mode) != (SimulationModes)0))
				{
					if (fusionStats.EnforceSingle)
					{
						runner = networkRunner;
						return true;
					}
					if (networkRunner.SimulationUnityScene == scene)
					{
						runner = networkRunner;
						return true;
					}
				}
			}
			runner = null;
			return false;
		}

		public static RectTransform CreateRectTransform(this Transform parent, string name, bool expand = false)
		{
			RectTransform rectTransform = new GameObject(name).AddComponent<RectTransform>();
			rectTransform.SetParent(parent);
			rectTransform.localPosition = default(Vector3);
			rectTransform.localScale = default(Vector3);
			rectTransform.localScale = new Vector3(1f, 1f, 1f);
			if (expand)
			{
				rectTransform.ExpandAnchor(null);
			}
			return rectTransform;
		}

		[Obsolete]
		internal static RectTransform CreateRectTransform(string name, Transform parent, bool expand = false)
		{
			RectTransform rectTransform = new GameObject(name).AddComponent<RectTransform>();
			rectTransform.SetParent(parent);
			rectTransform.localPosition = default(Vector3);
			rectTransform.localScale = default(Vector3);
			rectTransform.localScale = new Vector3(1f, 1f, 1f);
			if (expand)
			{
				rectTransform.ExpandAnchor(null);
			}
			return rectTransform;
		}

		public static Dropdown CreateDropdown(this RectTransform rt, float padding, Color fontColor)
		{
			RectTransform rectTransform = rt.CreateRectTransform("Dropdown", false).ExpandAnchor(new float?((float)-6));
			Image image = rectTransform.gameObject.AddComponent<Image>();
			Dropdown dropdown = rectTransform.gameObject.AddComponent<Dropdown>();
			image.color = new Color(0f, 0f, 0f, 0f);
			dropdown.image = image;
			RectTransform rectTransform2 = rectTransform.CreateRectTransform("Template", true).ExpandTopAnchor(null).SetOffsets(0f, 0f, -150f, 0f);
			RectTransform rectTransform3 = rectTransform2.CreateRectTransform("Content", false).ExpandTopAnchor(null).SetOffsets(0f, 0f, -150f, 0f).CreateRectTransform("Item", true).SetAnchors(0f, 1f, 1f, 1f).SetPivot(0.5f, 1f).SetSizeDelta(0f, 50f);
			Toggle toggle = rectTransform3.gameObject.AddComponent<Toggle>();
			toggle.colors = new ColorBlock
			{
				colorMultiplier = 1f,
				normalColor = new Color(0.2f, 0.2f, 0.2f, 1f),
				highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f),
				pressedColor = new Color(0.4f, 0.4f, 0.4f, 4f),
				selectedColor = new Color(0.25f, 0.25f, 0.25f, 1f)
			};
			Image targetGraphic = rectTransform3.CreateRectTransform("Item Background", true).gameObject.AddComponent<Image>();
			Image image2 = rectTransform3.CreateRectTransform("Item Checkmark", true).SetAnchors(0.05f, 0.1f, 0.1f, 0.9f).SetOffsets(0f, 0f, 0f, 0f).gameObject.AddComponent<Image>();
			image2.sprite = FusionStatsUtilities.CircleSprite;
			image2.preserveAspect = true;
			Text text = rectTransform3.CreateRectTransform("Item Label", true).SetAnchors(0.15f, 0.9f, 0.1f, 0.9f).SetOffsets(0f, 0f, 0f, 0f).AddText("Sample", TextAnchor.UpperLeft, fontColor);
			text.alignment = TextAnchor.MiddleLeft;
			text.resizeTextMaxSize = 24;
			toggle.targetGraphic = targetGraphic;
			toggle.graphic = image2;
			toggle.isOn = true;
			dropdown.template = rectTransform2;
			dropdown.itemText = text;
			rectTransform2.gameObject.SetActive(false);
			return dropdown;
		}

		public static Text AddText(this RectTransform rt, string label, TextAnchor anchor, Color FontColor)
		{
			Text text = rt.gameObject.AddComponent<Text>();
			text.text = label;
			text.color = FontColor;
			text.alignment = anchor;
			text.fontSize = 12;
			text.raycastTarget = false;
			text.resizeTextMinSize = 4;
			text.resizeTextMaxSize = 200;
			text.resizeTextForBestFit = true;
			return text;
		}

		internal static void MakeButton(this RectTransform parent, ref Button button, string iconText, string labelText, out Text icon, out Text text, UnityAction action)
		{
			RectTransform rectTransform = parent.CreateRectTransform(labelText, false);
			button = rectTransform.gameObject.AddComponent<Button>();
			RectTransform rectTransform2 = rectTransform.CreateRectTransform("Icon", true);
			rectTransform2.anchorMin = new Vector2(0f, 0.175f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(0f, 0f);
			rectTransform2.offsetMax = new Vector2(0f, 0f);
			icon = rectTransform2.gameObject.AddComponent<Text>();
			button.targetGraphic = icon;
			icon.text = iconText;
			icon.alignment = TextAnchor.MiddleCenter;
			icon.fontStyle = FontStyle.Bold;
			icon.fontSize = 100;
			icon.resizeTextMinSize = 0;
			icon.resizeTextMaxSize = 100;
			icon.alignByGeometry = true;
			icon.resizeTextForBestFit = true;
			RectTransform rectTransform3 = rectTransform.CreateRectTransform("Label", true);
			rectTransform3.anchorMin = new Vector2(0f, 0f);
			rectTransform3.anchorMax = new Vector2(1f, 0.175f);
			rectTransform3.pivot = new Vector2(0.5f, 0.0875f);
			rectTransform3.offsetMin = new Vector2(0f, 0f);
			rectTransform3.offsetMax = new Vector2(0f, 0f);
			text = rectTransform3.gameObject.AddComponent<Text>();
			text.color = Color.black;
			text.text = labelText;
			text.alignment = TextAnchor.MiddleCenter;
			text.fontStyle = FontStyle.Bold;
			text.fontSize = 0;
			text.resizeTextMinSize = 0;
			text.resizeTextMaxSize = 100;
			text.resizeTextForBestFit = true;
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			ColorBlock colors = button.colors;
			colors.normalColor = new Color(0f, 0f, 0f, 0.925f);
			colors.pressedColor = new Color(0.5f, 0.5f, 0.5f, 0.925f);
			colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.925f);
			colors.selectedColor = new Color(0f, 0f, 0f, 0.925f);
			button.colors = colors;
			button.onClick.AddListener(action);
		}

		public static RectTransform AddHorizontalLayoutGroup(this RectTransform rt, float spacing, int? rgtPad = null, int? lftPad = null, int? topPad = null, int? botPad = null)
		{
			HorizontalLayoutGroup horizontalLayoutGroup = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childControlWidth = true;
			horizontalLayoutGroup.spacing = spacing;
			horizontalLayoutGroup.padding = new RectOffset((rgtPad != null) ? rgtPad.Value : 0, (lftPad != null) ? lftPad.Value : 0, (topPad != null) ? topPad.Value : 0, (botPad != null) ? botPad.Value : 0);
			return rt;
		}

		public static RectTransform AddVerticalLayoutGroup(this RectTransform rt, float spacing, int? rgtPad = null, int? lftPad = null, int? topPad = null, int? botPad = null)
		{
			VerticalLayoutGroup verticalLayoutGroup = rt.gameObject.AddComponent<VerticalLayoutGroup>();
			verticalLayoutGroup.childControlHeight = true;
			verticalLayoutGroup.childControlWidth = true;
			verticalLayoutGroup.spacing = spacing;
			return rt;
		}

		public static GridLayoutGroup AddGridlLayoutGroup(this RectTransform rt, float spacing, int? rgtPad = null, int? lftPad = null, int? topPad = null, int? botPad = null)
		{
			GridLayoutGroup gridLayoutGroup = rt.gameObject.AddComponent<GridLayoutGroup>();
			gridLayoutGroup.spacing = new Vector2(spacing, spacing);
			return gridLayoutGroup;
		}

		public static RectTransform AddImage(this RectTransform rt, Color color)
		{
			Image image = rt.gameObject.AddComponent<Image>();
			image.color = color;
			image.raycastTarget = false;
			return rt;
		}

		public static RectTransform AddCircleSprite(this RectTransform rt, Color color)
		{
			Image image;
			rt.AddCircleSprite(color, out image);
			return rt;
		}

		public static RectTransform AddCircleSprite(this RectTransform rt, Color color, out Image image)
		{
			image = rt.gameObject.AddComponent<Image>();
			image.sprite = FusionStatsUtilities.CircleSprite;
			image.type = Image.Type.Sliced;
			image.pixelsPerUnitMultiplier = 100f;
			image.color = color;
			image.raycastTarget = false;
			return rt;
		}

		public static RectTransform ExpandAnchor(this RectTransform rt, float? padding = null)
		{
			rt.anchorMax = new Vector2(1f, 1f);
			rt.anchorMin = new Vector2(0f, 0f);
			rt.pivot = new Vector2(0.5f, 0.5f);
			if (padding != null)
			{
				rt.offsetMin = new Vector2(padding.Value, padding.Value);
				rt.offsetMax = new Vector2(-padding.Value, -padding.Value);
			}
			else
			{
				rt.sizeDelta = default(Vector2);
				rt.anchoredPosition = default(Vector2);
			}
			return rt;
		}

		public static RectTransform ExpandTopAnchor(this RectTransform rt, float? padding = null)
		{
			rt.anchorMax = new Vector2(1f, 1f);
			rt.anchorMin = new Vector2(0f, 1f);
			rt.pivot = new Vector2(0.5f, 1f);
			if (padding != null)
			{
				rt.offsetMin = new Vector2(padding.Value, padding.Value);
				rt.offsetMax = new Vector2(-padding.Value, -padding.Value);
			}
			else
			{
				rt.sizeDelta = default(Vector2);
				rt.anchoredPosition = default(Vector2);
			}
			return rt;
		}

		public static RectTransform ExpandMiddleLeft(this RectTransform rt)
		{
			rt.anchorMax = new Vector2(0f, 0.5f);
			rt.anchorMin = new Vector2(0f, 0.5f);
			rt.pivot = new Vector2(0f, 0.5f);
			return rt;
		}

		public static RectTransform SetSizeDelta(this RectTransform rt, float offsetX, float offsetY)
		{
			rt.sizeDelta = new Vector2(offsetX, offsetY);
			return rt;
		}

		public static RectTransform SetOffsets(this RectTransform rt, float minX, float maxX, float minY, float maxY)
		{
			rt.offsetMin = new Vector2(minX, minY);
			rt.offsetMax = new Vector2(maxX, maxY);
			return rt;
		}

		public static RectTransform SetPivot(this RectTransform rt, float pivotX, float pivotY)
		{
			rt.pivot = new Vector2(pivotX, pivotY);
			return rt;
		}

		public static RectTransform SetAnchors(this RectTransform rt, float minX, float maxX, float minY, float maxY)
		{
			rt.anchorMin = new Vector2(minX, minY);
			rt.anchorMax = new Vector2(maxX, maxY);
			return rt;
		}

		internal static RectTransform MakeGuides(this RectTransform parent)
		{
			Color color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
			RectTransform rectTransform = parent.CreateRectTransform("Guides", true);
			rectTransform.SetSiblingIndex(0);
			rectTransform.CreateRectTransform("Back", true).gameObject.AddComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
			RectTransform rectTransform2 = rectTransform.CreateRectTransform("Left", true);
			rectTransform2.anchorMin = new Vector2(-0.01f, 0f);
			rectTransform2.anchorMax = new Vector2(0f, 1f);
			rectTransform2.gameObject.AddComponent<Image>().color = color;
			RectTransform rectTransform3 = rectTransform.CreateRectTransform("Right", true);
			rectTransform3.anchorMin = new Vector2(1f, 0f);
			rectTransform3.anchorMax = new Vector2(1.01f, 1f);
			rectTransform3.gameObject.AddComponent<Image>().color = color;
			RectTransform rectTransform4 = rectTransform.CreateRectTransform("Top", true);
			rectTransform4.anchorMin = new Vector2(-0.01f, 1f);
			rectTransform4.anchorMax = new Vector2(1.01f, 1.01f);
			rectTransform4.gameObject.AddComponent<Image>().color = color;
			RectTransform rectTransform5 = rectTransform.CreateRectTransform("Bottom", true);
			rectTransform5.anchorMin = new Vector2(-0.01f, -0.01f);
			rectTransform5.anchorMax = new Vector2(1.01f, 0f);
			rectTransform5.gameObject.AddComponent<Image>().color = color;
			rectTransform.CreateRectTransform("Center", true).SetAnchors(0.495f, 0.505f, 0f, 1f).AddImage(color);
			rectTransform.CreateRectTransform("Middle", true).SetAnchors(0f, 1f, 0.495f, 0.505f).AddImage(color);
			return rectTransform;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FusionStatsUtilities()
		{
		}

		public const int PAD = 10;

		public const int MARGIN = 6;

		public const int FONT_SIZE = 12;

		public const int FONT_SIZE_MIN = 4;

		public const int FONT_SIZE_MAX = 200;

		private static List<string> _cachedGraphVisualizationNames;

		private const int METER_TEXTURE_WIDTH = 512;

		private static Texture2D _meterTexture;

		private static Sprite _meterSprite;

		private const int R = 64;

		private static Texture2D _circle32Texture;

		private static Sprite _circle32Sprite;

		public static Color DARK_GREEN = new Color(0f, 0.5f, 0f, 1f);

		public static Color DARK_BLUE = new Color(0f, 0f, 0.5f, 1f);

		public static Color DARK_RED = new Color(0.5f, 0f, 0f, 1f);

		public const float BTTN_LBL_NORM_HGHT = 0.175f;

		private const int BTTN_FONT_SIZE_MAX = 100;

		private const float BTTN_ALPHA = 0.925f;

		private const float GUIDE_MARGIN = 0.01f;

		private const float GUIDE_MARGIN_HALF = 0.005f;
	}
}
