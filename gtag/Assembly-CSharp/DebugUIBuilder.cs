using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200007D RID: 125
public class DebugUIBuilder : MonoBehaviour
{
	// Token: 0x06000273 RID: 627 RVA: 0x0001026C File Offset: 0x0000E46C
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

	// Token: 0x06000274 RID: 628 RVA: 0x000103FC File Offset: 0x0000E5FC
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
		int count = this.toDisable.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.toDisable[i])
			{
				this.reEnable[i] = this.toDisable[i].activeSelf;
				this.toDisable[i].SetActive(false);
			}
		}
		count = this.toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			this.toEnable[j].SetActive(true);
		}
		int num = this.targetContentPanels.Length;
		for (int k = 0; k < num; k++)
		{
			this.targetContentPanels[k].gameObject.SetActive(this.insertedElements[k].Count > 0);
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00010584 File Offset: 0x0000E784
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

	// Token: 0x06000276 RID: 630 RVA: 0x0001060C File Offset: 0x0000E80C
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

	// Token: 0x06000277 RID: 631 RVA: 0x0001071C File Offset: 0x0000E91C
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

	// Token: 0x06000278 RID: 632 RVA: 0x000108A7 File Offset: 0x0000EAA7
	private void Relayout()
	{
		if (this.usePanelCentricRelayout)
		{
			this.PanelCentricRelayout();
			return;
		}
		this.StackedRelayout();
	}

	// Token: 0x06000279 RID: 633 RVA: 0x000108C0 File Offset: 0x0000EAC0
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

	// Token: 0x0600027A RID: 634 RVA: 0x00010954 File Offset: 0x0000EB54
	public RectTransform AddButton(string label, DebugUIBuilder.OnClick handler = null, int buttonIndex = -1, int targetCanvas = 0, bool highResolutionText = false)
	{
		RectTransform component;
		if (buttonIndex == -1)
		{
			component = Object.Instantiate<RectTransform>(this.buttonPrefab).GetComponent<RectTransform>();
		}
		else
		{
			component = Object.Instantiate<RectTransform>(this.additionalButtonPrefab[buttonIndex]).GetComponent<RectTransform>();
		}
		Button componentInChildren = component.GetComponentInChildren<Button>();
		if (handler != null)
		{
			componentInChildren.onClick.AddListener(delegate()
			{
				handler();
			});
		}
		if (highResolutionText)
		{
			((TextMeshProUGUI)component.GetComponentsInChildren(typeof(TextMeshProUGUI), true)[0]).text = label;
		}
		else
		{
			((Text)component.GetComponentsInChildren(typeof(Text), true)[0]).text = label;
		}
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x0600027B RID: 635 RVA: 0x00010A0C File Offset: 0x0000EC0C
	public RectTransform AddLabel(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.labelPrefab).GetComponent<RectTransform>();
		component.GetComponent<Text>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00010A40 File Offset: 0x0000EC40
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

	// Token: 0x0600027D RID: 637 RVA: 0x00010AA4 File Offset: 0x0000ECA4
	public RectTransform AddDivider(int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.dividerPrefab);
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00010AC8 File Offset: 0x0000ECC8
	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.onValueChanged.AddListener(delegate(bool <p0>)
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00010B2C File Offset: 0x0000ED2C
	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, bool defaultValue, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.isOn = defaultValue;
		t.onValueChanged.AddListener(delegate(bool <p0>)
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00010B9C File Offset: 0x0000ED9C
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
		bool isOn = false;
		ToggleGroup toggleGroup;
		if (!this.radioGroups.ContainsKey(group))
		{
			toggleGroup = tb.gameObject.AddComponent<ToggleGroup>();
			this.radioGroups[group] = toggleGroup;
			isOn = true;
		}
		else
		{
			toggleGroup = this.radioGroups[group];
		}
		tb.group = toggleGroup;
		tb.isOn = isOn;
		tb.onValueChanged.AddListener(delegate(bool <p0>)
		{
			handler(tb);
		});
		return rectTransform;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00010C64 File Offset: 0x0000EE64
	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00010C97 File Offset: 0x0000EE97
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

	// Token: 0x0400033F RID: 831
	public const int DEBUG_PANE_CENTER = 0;

	// Token: 0x04000340 RID: 832
	public const int DEBUG_PANE_RIGHT = 1;

	// Token: 0x04000341 RID: 833
	public const int DEBUG_PANE_LEFT = 2;

	// Token: 0x04000342 RID: 834
	[SerializeField]
	private RectTransform buttonPrefab;

	// Token: 0x04000343 RID: 835
	[SerializeField]
	private RectTransform[] additionalButtonPrefab;

	// Token: 0x04000344 RID: 836
	[SerializeField]
	private RectTransform labelPrefab;

	// Token: 0x04000345 RID: 837
	[SerializeField]
	private RectTransform sliderPrefab;

	// Token: 0x04000346 RID: 838
	[SerializeField]
	private RectTransform dividerPrefab;

	// Token: 0x04000347 RID: 839
	[SerializeField]
	private RectTransform togglePrefab;

	// Token: 0x04000348 RID: 840
	[SerializeField]
	private RectTransform radioPrefab;

	// Token: 0x04000349 RID: 841
	[SerializeField]
	private RectTransform textPrefab;

	// Token: 0x0400034A RID: 842
	[SerializeField]
	private GameObject uiHelpersToInstantiate;

	// Token: 0x0400034B RID: 843
	[SerializeField]
	private Transform[] targetContentPanels;

	// Token: 0x0400034C RID: 844
	private bool[] reEnable;

	// Token: 0x0400034D RID: 845
	[SerializeField]
	private List<GameObject> toEnable;

	// Token: 0x0400034E RID: 846
	[SerializeField]
	private List<GameObject> toDisable;

	// Token: 0x0400034F RID: 847
	public static DebugUIBuilder instance;

	// Token: 0x04000350 RID: 848
	public float elementSpacing = 16f;

	// Token: 0x04000351 RID: 849
	public float marginH = 16f;

	// Token: 0x04000352 RID: 850
	public float marginV = 16f;

	// Token: 0x04000353 RID: 851
	private Vector2[] insertPositions;

	// Token: 0x04000354 RID: 852
	private List<RectTransform>[] insertedElements;

	// Token: 0x04000355 RID: 853
	private Vector3 menuOffset;

	// Token: 0x04000356 RID: 854
	private OVRCameraRig rig;

	// Token: 0x04000357 RID: 855
	private Dictionary<string, ToggleGroup> radioGroups = new Dictionary<string, ToggleGroup>();

	// Token: 0x04000358 RID: 856
	private LaserPointer lp;

	// Token: 0x04000359 RID: 857
	private LineRenderer lr;

	// Token: 0x0400035A RID: 858
	public LaserPointer.LaserBeamBehavior laserBeamBehavior;

	// Token: 0x0400035B RID: 859
	public bool isHorizontal;

	// Token: 0x0400035C RID: 860
	public bool usePanelCentricRelayout;

	// Token: 0x020003A6 RID: 934
	// (Invoke) Token: 0x06001AF1 RID: 6897
	public delegate void OnClick();

	// Token: 0x020003A7 RID: 935
	// (Invoke) Token: 0x06001AF5 RID: 6901
	public delegate void OnToggleValueChange(Toggle t);

	// Token: 0x020003A8 RID: 936
	// (Invoke) Token: 0x06001AF9 RID: 6905
	public delegate void OnSlider(float f);

	// Token: 0x020003A9 RID: 937
	// (Invoke) Token: 0x06001AFD RID: 6909
	public delegate bool ActiveUpdate();
}
