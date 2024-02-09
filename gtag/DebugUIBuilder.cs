using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIBuilder : MonoBehaviour
{
	public void Awake()
	{
		DebugUIBuilder.instance = this;
		this.menuOffset = base.transform.position;
		base.gameObject.SetActive(false);
		this.rig = Object.FindObjectOfType<OVRCameraRig>();
		for (int i = 0; i < this.toEnable.Count; i++)
		{
			this.toEnable[i].SetActive(false);
		}
		this.insertPositions = new Vector2[this.targetContentPanels.Length];
		for (int j = 0; j < this.insertPositions.Length; j++)
		{
			this.insertPositions[j].x = this.marginH;
			this.insertPositions[j].y = -this.marginV;
		}
		this.insertedElements = new List<RectTransform>[this.targetContentPanels.Length];
		for (int k = 0; k < this.insertedElements.Length; k++)
		{
			this.insertedElements[k] = new List<RectTransform>();
		}
		if (this.uiHelpersToInstantiate)
		{
			Object.Instantiate<GameObject>(this.uiHelpersToInstantiate);
		}
		this.lp = Object.FindObjectOfType<LaserPointer>();
		if (!this.lp)
		{
			Debug.LogError("Debug UI requires use of a LaserPointer and will not function without it. Add one to your scene, or assign the UIHelpers prefab to the DebugUIBuilder in the inspector.");
			return;
		}
		this.lp.laserBeamBehavior = this.laserBeamBehavior;
		if (!this.toEnable.Contains(this.lp.gameObject))
		{
			this.toEnable.Add(this.lp.gameObject);
		}
		base.GetComponent<OVRRaycaster>().pointer = this.lp.gameObject;
		this.lp.gameObject.SetActive(false);
	}

	public void Show()
	{
		this.Relayout();
		base.gameObject.SetActive(true);
		base.transform.position = this.rig.transform.TransformPoint(this.menuOffset);
		Vector3 eulerAngles = this.rig.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
		if (this.reEnable == null || this.reEnable.Length < this.toDisable.Count)
		{
			this.reEnable = new bool[this.toDisable.Count];
		}
		this.reEnable.Initialize();
		int num = this.toDisable.Count;
		for (int i = 0; i < num; i++)
		{
			if (this.toDisable[i])
			{
				this.reEnable[i] = this.toDisable[i].activeSelf;
				this.toDisable[i].SetActive(false);
			}
		}
		num = this.toEnable.Count;
		for (int j = 0; j < num; j++)
		{
			this.toEnable[j].SetActive(true);
		}
		int num2 = this.targetContentPanels.Length;
		for (int k = 0; k < num2; k++)
		{
			this.targetContentPanels[k].gameObject.SetActive(this.insertedElements[k].Count > 0);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		for (int i = 0; i < this.reEnable.Length; i++)
		{
			if (this.toDisable[i] && this.reEnable[i])
			{
				this.toDisable[i].SetActive(true);
			}
		}
		int count = this.toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			this.toEnable[j].SetActive(false);
		}
	}

	private void StackedRelayout()
	{
		for (int i = 0; i < this.targetContentPanels.Length; i++)
		{
			RectTransform component = this.targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = this.insertedElements[i];
			int count = list.Count;
			float num = this.marginH;
			float num2 = -this.marginV;
			float num3 = 0f;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				rectTransform.anchoredPosition = new Vector2(num, num2);
				if (this.isHorizontal)
				{
					num += rectTransform.rect.width + this.elementSpacing;
				}
				else
				{
					num2 -= rectTransform.rect.height + this.elementSpacing;
				}
				num3 = Mathf.Max(rectTransform.rect.width + 2f * this.marginH, num3);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num2 + this.marginV);
		}
	}

	private void PanelCentricRelayout()
	{
		if (!this.isHorizontal)
		{
			Debug.Log("Error:Panel Centeric relayout is implemented only for horizontal panels");
			return;
		}
		for (int i = 0; i < this.targetContentPanels.Length; i++)
		{
			RectTransform component = this.targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = this.insertedElements[i];
			int count = list.Count;
			float num = this.marginH;
			float num2 = -this.marginV;
			float num3 = num;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				num3 += rectTransform.rect.width + this.elementSpacing;
			}
			num3 -= this.elementSpacing;
			num3 += this.marginH;
			float num4 = num3;
			num = -0.5f * num4;
			num2 = -this.marginV;
			for (int k = 0; k < count; k++)
			{
				RectTransform rectTransform2 = list[k];
				if (k == 0)
				{
					num += this.marginH;
				}
				num += 0.5f * rectTransform2.rect.width;
				rectTransform2.anchoredPosition = new Vector2(num, num2);
				num += rectTransform2.rect.width * 0.5f + this.elementSpacing;
				num3 = Mathf.Max(rectTransform2.rect.width + 2f * this.marginH, num3);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num2 + this.marginV);
		}
	}

	private void Relayout()
	{
		if (this.usePanelCentricRelayout)
		{
			this.PanelCentricRelayout();
			return;
		}
		this.StackedRelayout();
	}

	private void AddRect(RectTransform r, int targetCanvas)
	{
		if (targetCanvas > this.targetContentPanels.Length)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Attempted to add debug panel to canvas ",
				targetCanvas.ToString(),
				", but only ",
				this.targetContentPanels.Length.ToString(),
				" panels were provided. Fix in the inspector or pass a lower value for target canvas."
			}));
			return;
		}
		r.transform.SetParent(this.targetContentPanels[targetCanvas], false);
		this.insertedElements[targetCanvas].Add(r);
		if (base.gameObject.activeInHierarchy)
		{
			this.Relayout();
		}
	}

	public RectTransform AddButton(string label, DebugUIBuilder.OnClick handler = null, int buttonIndex = -1, int targetCanvas = 0, bool highResolutionText = false)
	{
		RectTransform rectTransform;
		if (buttonIndex == -1)
		{
			rectTransform = Object.Instantiate<RectTransform>(this.buttonPrefab).GetComponent<RectTransform>();
		}
		else
		{
			rectTransform = Object.Instantiate<RectTransform>(this.additionalButtonPrefab[buttonIndex]).GetComponent<RectTransform>();
		}
		Button componentInChildren = rectTransform.GetComponentInChildren<Button>();
		if (handler != null)
		{
			componentInChildren.onClick.AddListener(delegate
			{
				handler();
			});
		}
		if (highResolutionText)
		{
			((TextMeshProUGUI)rectTransform.GetComponentsInChildren(typeof(TextMeshProUGUI), true)[0]).text = label;
		}
		else
		{
			((Text)rectTransform.GetComponentsInChildren(typeof(Text), true)[0]).text = label;
		}
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	public RectTransform AddLabel(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.labelPrefab).GetComponent<RectTransform>();
		component.GetComponent<Text>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	public RectTransform AddSlider(string label, float min, float max, DebugUIBuilder.OnSlider onValueChanged, bool wholeNumbersOnly = false, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.sliderPrefab);
		Slider componentInChildren = rectTransform.GetComponentInChildren<Slider>();
		componentInChildren.minValue = min;
		componentInChildren.maxValue = max;
		componentInChildren.onValueChanged.AddListener(delegate(float f)
		{
			onValueChanged(f);
		});
		componentInChildren.wholeNumbers = wholeNumbersOnly;
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	public RectTransform AddDivider(int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.dividerPrefab);
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.onValueChanged.AddListener(delegate
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, bool defaultValue, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.isOn = defaultValue;
		t.onValueChanged.AddListener(delegate
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	public RectTransform AddRadio(string label, string group, DebugUIBuilder.OnToggleValueChange handler, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.radioPrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle tb = rectTransform.GetComponentInChildren<Toggle>();
		if (group == null)
		{
			group = "default";
		}
		bool flag = false;
		ToggleGroup toggleGroup;
		if (!this.radioGroups.ContainsKey(group))
		{
			toggleGroup = tb.gameObject.AddComponent<ToggleGroup>();
			this.radioGroups[group] = toggleGroup;
			flag = true;
		}
		else
		{
			toggleGroup = this.radioGroups[group];
		}
		tb.group = toggleGroup;
		tb.isOn = flag;
		tb.onValueChanged.AddListener(delegate
		{
			handler(tb);
		});
		return rectTransform;
	}

	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	public void ToggleLaserPointer(bool isOn)
	{
		if (this.lp)
		{
			if (isOn)
			{
				this.lp.enabled = true;
				return;
			}
			this.lp.enabled = false;
		}
	}

	public const int DEBUG_PANE_CENTER = 0;

	public const int DEBUG_PANE_RIGHT = 1;

	public const int DEBUG_PANE_LEFT = 2;

	[SerializeField]
	private RectTransform buttonPrefab;

	[SerializeField]
	private RectTransform[] additionalButtonPrefab;

	[SerializeField]
	private RectTransform labelPrefab;

	[SerializeField]
	private RectTransform sliderPrefab;

	[SerializeField]
	private RectTransform dividerPrefab;

	[SerializeField]
	private RectTransform togglePrefab;

	[SerializeField]
	private RectTransform radioPrefab;

	[SerializeField]
	private RectTransform textPrefab;

	[SerializeField]
	private GameObject uiHelpersToInstantiate;

	[SerializeField]
	private Transform[] targetContentPanels;

	private bool[] reEnable;

	[SerializeField]
	private List<GameObject> toEnable;

	[SerializeField]
	private List<GameObject> toDisable;

	public static DebugUIBuilder instance;

	public float elementSpacing = 16f;

	public float marginH = 16f;

	public float marginV = 16f;

	private Vector2[] insertPositions;

	private List<RectTransform>[] insertedElements;

	private Vector3 menuOffset;

	private OVRCameraRig rig;

	private Dictionary<string, ToggleGroup> radioGroups = new Dictionary<string, ToggleGroup>();

	private LaserPointer lp;

	private LineRenderer lr;

	public LaserPointer.LaserBeamBehavior laserBeamBehavior;

	public bool isHorizontal;

	public bool usePanelCentricRelayout;

	public delegate void OnClick();

	public delegate void OnToggleValueChange(Toggle t);

	public delegate void OnSlider(float f);

	public delegate bool ActiveUpdate();
}
