using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIBuilder : MonoBehaviour
{
	public delegate void OnClick();

	public delegate void OnToggleValueChange(Toggle t);

	public delegate void OnSlider(float f);

	public delegate bool ActiveUpdate();

	public const int DEBUG_PANE_CENTER = 0;

	public const int DEBUG_PANE_RIGHT = 1;

	public const int DEBUG_PANE_LEFT = 2;

	[SerializeField]
	private RectTransform buttonPrefab;

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
	private GameObject uiHelpersToInstantiate;

	[SerializeField]
	private Transform[] targetContentPanels;

	private bool[] reEnable;

	[SerializeField]
	private List<GameObject> toEnable;

	[SerializeField]
	private List<GameObject> toDisable;

	public static DebugUIBuilder instance;

	private const float elementSpacing = 16f;

	private const float marginH = 16f;

	private const float marginV = 16f;

	private Vector2[] insertPositions;

	private List<RectTransform>[] insertedElements;

	private Vector3 menuOffset;

	private OVRCameraRig rig;

	private Dictionary<string, ToggleGroup> radioGroups = new Dictionary<string, ToggleGroup>();

	private LaserPointer lp;

	private LineRenderer lr;

	public LaserPointer.LaserBeamBehavior laserBeamBehavior;

	public void Awake()
	{
		instance = this;
		menuOffset = base.transform.position;
		base.gameObject.SetActive(value: false);
		rig = Object.FindObjectOfType<OVRCameraRig>();
		for (int i = 0; i < toEnable.Count; i++)
		{
			toEnable[i].SetActive(value: false);
		}
		insertPositions = new Vector2[targetContentPanels.Length];
		for (int j = 0; j < insertPositions.Length; j++)
		{
			insertPositions[j].x = 16f;
			insertPositions[j].y = -16f;
		}
		insertedElements = new List<RectTransform>[targetContentPanels.Length];
		for (int k = 0; k < insertedElements.Length; k++)
		{
			insertedElements[k] = new List<RectTransform>();
		}
		if ((bool)uiHelpersToInstantiate)
		{
			Object.Instantiate(uiHelpersToInstantiate);
		}
		lp = Object.FindObjectOfType<LaserPointer>();
		if (!lp)
		{
			Debug.LogError("Debug UI requires use of a LaserPointer and will not function without it. Add one to your scene, or assign the UIHelpers prefab to the DebugUIBuilder in the inspector.");
			return;
		}
		lp.laserBeamBehavior = laserBeamBehavior;
		if (!toEnable.Contains(lp.gameObject))
		{
			toEnable.Add(lp.gameObject);
		}
		GetComponent<OVRRaycaster>().pointer = lp.gameObject;
		lp.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		Relayout();
		base.gameObject.SetActive(value: true);
		base.transform.position = rig.transform.TransformPoint(menuOffset);
		Vector3 eulerAngles = rig.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
		if (reEnable == null || reEnable.Length < toDisable.Count)
		{
			reEnable = new bool[toDisable.Count];
		}
		reEnable.Initialize();
		int count = toDisable.Count;
		for (int i = 0; i < count; i++)
		{
			if ((bool)toDisable[i])
			{
				reEnable[i] = toDisable[i].activeSelf;
				toDisable[i].SetActive(value: false);
			}
		}
		count = toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			toEnable[j].SetActive(value: true);
		}
		int num = targetContentPanels.Length;
		for (int k = 0; k < num; k++)
		{
			targetContentPanels[k].gameObject.SetActive(insertedElements[k].Count > 0);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
		for (int i = 0; i < reEnable.Length; i++)
		{
			if ((bool)toDisable[i] && reEnable[i])
			{
				toDisable[i].SetActive(value: true);
			}
		}
		int count = toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			toEnable[j].SetActive(value: false);
		}
	}

	private void Relayout()
	{
		for (int i = 0; i < targetContentPanels.Length; i++)
		{
			RectTransform component = targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = insertedElements[i];
			int count = list.Count;
			float x = 16f;
			float num = -16f;
			float num2 = 0f;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				rectTransform.anchoredPosition = new Vector2(x, num);
				num -= rectTransform.rect.height + 16f;
				num2 = Mathf.Max(rectTransform.rect.width + 32f, num2);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num2);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f - num + 16f);
		}
	}

	private void AddRect(RectTransform r, int targetCanvas)
	{
		if (targetCanvas > targetContentPanels.Length)
		{
			Debug.LogError("Attempted to add debug panel to canvas " + targetCanvas + ", but only " + targetContentPanels.Length + " panels were provided. Fix in the inspector or pass a lower value for target canvas.");
		}
		else
		{
			r.transform.SetParent(targetContentPanels[targetCanvas], worldPositionStays: false);
			insertedElements[targetCanvas].Add(r);
			if (base.gameObject.activeInHierarchy)
			{
				Relayout();
			}
		}
	}

	public RectTransform AddButton(string label, OnClick handler, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate(buttonPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<Button>().onClick.AddListener(delegate
		{
			handler();
		});
		((Text)component.GetComponentsInChildren(typeof(Text), includeInactive: true)[0]).text = label;
		AddRect(component, targetCanvas);
		return component;
	}

	public RectTransform AddLabel(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate(labelPrefab).GetComponent<RectTransform>();
		component.GetComponent<Text>().text = label;
		AddRect(component, targetCanvas);
		return component;
	}

	public RectTransform AddSlider(string label, float min, float max, OnSlider onValueChanged, bool wholeNumbersOnly = false, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate(sliderPrefab);
		Slider componentInChildren = rectTransform.GetComponentInChildren<Slider>();
		componentInChildren.minValue = min;
		componentInChildren.maxValue = max;
		componentInChildren.onValueChanged.AddListener(delegate(float f)
		{
			onValueChanged(f);
		});
		componentInChildren.wholeNumbers = wholeNumbersOnly;
		AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	public RectTransform AddDivider(int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate(dividerPrefab);
		AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	public RectTransform AddToggle(string label, OnToggleValueChange onValueChanged, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate(togglePrefab);
		AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.onValueChanged.AddListener(delegate
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	public RectTransform AddToggle(string label, OnToggleValueChange onValueChanged, bool defaultValue, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate(togglePrefab);
		AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.isOn = defaultValue;
		t.onValueChanged.AddListener(delegate
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	public RectTransform AddRadio(string label, string group, OnToggleValueChange handler, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate(radioPrefab);
		AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle tb = rectTransform.GetComponentInChildren<Toggle>();
		if (group == null)
		{
			group = "default";
		}
		ToggleGroup toggleGroup = null;
		bool isOn = false;
		if (!radioGroups.ContainsKey(group))
		{
			toggleGroup = tb.gameObject.AddComponent<ToggleGroup>();
			radioGroups[group] = toggleGroup;
			isOn = true;
		}
		else
		{
			toggleGroup = radioGroups[group];
		}
		tb.group = toggleGroup;
		tb.isOn = isOn;
		tb.onValueChanged.AddListener(delegate
		{
			handler(tb);
		});
		return rectTransform;
	}

	public void ToggleLaserPointer(bool isOn)
	{
		if ((bool)lp)
		{
			if (isOn)
			{
				lp.enabled = true;
			}
			else
			{
				lp.enabled = false;
			}
		}
	}
}
