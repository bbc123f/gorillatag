using System;
using System.Collections.Generic;
using System.Reflection;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ScriptHelp(BackColor = EditorHeaderBackColor.Olive)]
[ExecuteAlways]
public class FusionStats : Fusion.Behaviour
{
	internal static FusionStats CreateInternal(NetworkRunner runner = null, FusionStats.DefaultLayouts layout = FusionStats.DefaultLayouts.Left, Simulation.Statistics.NetStatFlags? netStatsMask = null, Simulation.Statistics.SimStatFlags? simStatsMask = null)
	{
		return FusionStats.Create(null, runner, new FusionStats.DefaultLayouts?(layout), new FusionStats.DefaultLayouts?(layout), netStatsMask, simStatsMask);
	}

	public static FusionStats Create(Transform parent = null, NetworkRunner runner = null, FusionStats.DefaultLayouts? screenLayout = null, FusionStats.DefaultLayouts? objectLayout = null, Simulation.Statistics.NetStatFlags? netStatsMask = null, Simulation.Statistics.SimStatFlags? simStatsMask = null)
	{
		GameObject gameObject = new GameObject("FusionStats " + (runner ? runner.name : "null"));
		if (parent)
		{
			gameObject.transform.SetParent(parent);
		}
		FusionStats fusionStats = gameObject.AddComponent<FusionStats>();
		fusionStats.ResetInternal(null, netStatsMask, simStatsMask, objectLayout, screenLayout);
		fusionStats.Runner = runner;
		if (runner != null)
		{
			fusionStats.AutoDestroy = true;
		}
		return fusionStats;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatics()
	{
		FusionStats._statsForRunnerLookup.Clear();
		FusionStats._activeGuids.Clear();
		FusionStats._newInputSystemFound = null;
	}

	public static Simulation.Statistics.NetStatFlags DefaultNetStatsMask
	{
		get
		{
			return Simulation.Statistics.NetStatFlags.RoundTripTime | Simulation.Statistics.NetStatFlags.SentPacketSizes | Simulation.Statistics.NetStatFlags.ReceivedPacketSizes;
		}
	}

	private bool ShowColorControls
	{
		get
		{
			return !Application.isPlaying && this._modifyColors;
		}
	}

	private bool IsNotPlaying
	{
		get
		{
			return !Application.isPlaying;
		}
	}

	public FusionStats.StatCanvasTypes CanvasType
	{
		get
		{
			return this._canvasType;
		}
		set
		{
			this._canvasType = value;
			this.DirtyLayout(2);
		}
	}

	public bool ShowButtonLabels
	{
		get
		{
			return this._showButtonLabels;
		}
		set
		{
			this._showButtonLabels = value;
			this.DirtyLayout(1);
		}
	}

	public int MaxHeaderHeight
	{
		get
		{
			return this._maxHeaderHeight;
		}
		set
		{
			this._maxHeaderHeight = value;
			this.DirtyLayout(1);
		}
	}

	public Rect GameObjectRect
	{
		get
		{
			return this._gameObjectRect;
		}
		set
		{
			this._gameObjectRect = value;
			this.DirtyLayout(1);
		}
	}

	public Rect OverlayRect
	{
		get
		{
			return this._overlayRect;
		}
		set
		{
			this._overlayRect = value;
			this.DirtyLayout(1);
		}
	}

	public FusionGraph.Layouts DefaultLayout
	{
		get
		{
			return this._defaultLayout;
		}
		set
		{
			this._defaultLayout = value;
			this.DirtyLayout(1);
		}
	}

	public bool NoTextOverlap
	{
		get
		{
			return this._noTextOverlap;
		}
		set
		{
			this._noTextOverlap = value;
			this.DirtyLayout(1);
		}
	}

	public bool NoGraphShader
	{
		get
		{
			return this._noGraphShader;
		}
		set
		{
			this._noGraphShader = value;
			this.DirtyLayout(1);
		}
	}

	public int GraphMaxWidth
	{
		get
		{
			return this._graphMaxWidth;
		}
		set
		{
			this._graphMaxWidth = value;
			this.DirtyLayout(1);
		}
	}

	public bool EnableObjectStats
	{
		get
		{
			return this._enableObjectStats;
		}
		set
		{
			this._enableObjectStats = value;
			this.DirtyLayout(1);
		}
	}

	private bool ShowMissingNetObjWarning
	{
		get
		{
			return this._enableObjectStats && this.Object == null;
		}
	}

	public NetworkObject Object
	{
		get
		{
			if (this._object == null)
			{
				this._object = base.GetComponentInParent<NetworkObject>();
			}
			return this._object;
		}
	}

	public int ObjectTitleHeight
	{
		get
		{
			return this._objectTitleHeight;
		}
		set
		{
			this._objectTitleHeight = value;
			this.DirtyLayout(1);
		}
	}

	public int ObjectIdsHeight
	{
		get
		{
			return this._objectIdsHeight;
		}
		set
		{
			this._objectIdsHeight = value;
			this.DirtyLayout(1);
		}
	}

	public int ObjectMetersHeight
	{
		get
		{
			return this._objectMetersHeight;
		}
		set
		{
			this._objectIdsHeight = value;
			this.DirtyLayout(1);
		}
	}

	public NetworkRunner Runner
	{
		get
		{
			if (!Application.isPlaying)
			{
				return null;
			}
			if (this._runner)
			{
				if (!this._runner.IsShutdown)
				{
					return this._runner;
				}
				this.Runner = null;
			}
			if (this.Object)
			{
				NetworkRunner runner = this._object.Runner;
				if (runner && (!this.EnforceSingle || (runner.Mode & this.ConnectTo) != (SimulationModes)0))
				{
					this.Runner = runner;
					return this._runner;
				}
			}
			NetworkRunner networkRunner;
			FusionStatsUtilities.TryFindActiveRunner(this, out networkRunner, new SimulationModes?(this.ConnectTo));
			this.Runner = networkRunner;
			return networkRunner;
		}
		set
		{
			if (this._runner == value)
			{
				return;
			}
			this.DisassociateWithRunner(this._runner);
			this._runner = value;
			this.AssociateWithRunner(value);
			this.UpdateTitle();
		}
	}

	public Simulation.Statistics.ObjStatFlags IncludedObjectStats
	{
		get
		{
			return this._includedObjStats;
		}
		set
		{
			this._includedObjStats = value;
			this._activeDirty = true;
		}
	}

	public Simulation.Statistics.NetStatFlags IncludedNetStats
	{
		get
		{
			return this._includedNetStats;
		}
		set
		{
			this._includedNetStats = value;
			this._activeDirty = true;
		}
	}

	public Simulation.Statistics.SimStatFlags IncludedSimStats
	{
		get
		{
			return this._includedSimStats;
		}
		set
		{
			this._includedSimStats = value;
			this._activeDirty = true;
		}
	}

	public bool ModifyColors
	{
		get
		{
			return this._modifyColors;
		}
	}

	public Color FontColor
	{
		get
		{
			return this._fontColor;
		}
	}

	public Color GraphColorGood
	{
		get
		{
			return this._graphColorGood;
		}
	}

	public Color GraphColorWarn
	{
		get
		{
			return this._graphColorWarn;
		}
	}

	public Color GraphColorBad
	{
		get
		{
			return this._graphColorBad;
		}
	}

	public Color GraphColorFlag
	{
		get
		{
			return this._graphColorFlag;
		}
	}

	public Color SimDataBackColor
	{
		get
		{
			return this._simDataBackColor;
		}
	}

	public Color NetDataBackColor
	{
		get
		{
			return this._netDataBackColor;
		}
	}

	public Color ObjDataBackColor
	{
		get
		{
			return this._objDataBackColor;
		}
	}

	public Rect CurrentRect
	{
		get
		{
			if (this._canvasType != FusionStats.StatCanvasTypes.GameObject)
			{
				return this._overlayRect;
			}
			return this._gameObjectRect;
		}
	}

	private void UpdateTitle()
	{
		string text = (this._runner ? this._runner.name : "Disconnected");
		if (this._titleText)
		{
			this._titleText.text = text;
		}
	}

	private Shader Shader
	{
		get
		{
			return Resources.Load<Shader>("FusionGraphShader");
		}
	}

	private void DirtyLayout(int minimumRefreshes = 1)
	{
		if (this._layoutDirty < minimumRefreshes)
		{
			this._layoutDirty = minimumRefreshes;
		}
	}

	private void ResetInternal(bool? enableObjectStats = null, Simulation.Statistics.NetStatFlags? netStatsMask = null, Simulation.Statistics.SimStatFlags? simStatsMask = null, FusionStats.DefaultLayouts? objectLayout = null, FusionStats.DefaultLayouts? screenLayout = null)
	{
		Canvas componentInChildren = base.GetComponentInChildren<Canvas>();
		if (componentInChildren)
		{
			UnityEngine.Object.DestroyImmediate(componentInChildren.gameObject);
		}
		FusionStatsBillboard fusionStatsBillboard;
		if (!base.TryGetComponent<FusionStatsBillboard>(out fusionStatsBillboard))
		{
			base.gameObject.AddComponent<FusionStatsBillboard>().UpdateLookAt();
		}
		bool flag = base.GetComponentInParent<NetworkObject>();
		if (enableObjectStats.GetValueOrDefault() || (enableObjectStats.GetValueOrDefault(true) && flag))
		{
			this.EnableObjectStats = true;
			this._includedObjStats = Simulation.Statistics.ObjStatFlags.Buffer;
			this._includedSimStats = simStatsMask.GetValueOrDefault();
			this._includedNetStats = netStatsMask.GetValueOrDefault();
			this._canvasType = FusionStats.StatCanvasTypes.GameObject;
			this.EnforceSingle = false;
			this.GraphColumnCount = 1;
		}
		else
		{
			this.GraphColumnCount = 0;
			if (base.transform.parent)
			{
				this._canvasType = FusionStats.StatCanvasTypes.GameObject;
				this.EnforceSingle = false;
			}
			else
			{
				this._canvasType = FusionStats.StatCanvasTypes.Overlay;
				this.EnforceSingle = true;
			}
			this._includedSimStats = simStatsMask.GetValueOrDefault(Simulation.Statistics.SimStatFlags.ForwardSimCount | Simulation.Statistics.SimStatFlags.ResimCount | Simulation.Statistics.SimStatFlags.PacketSize);
			this._includedNetStats = netStatsMask.GetValueOrDefault(Simulation.Statistics.NetStatFlags.RoundTripTime | Simulation.Statistics.NetStatFlags.SentPacketSizes | Simulation.Statistics.NetStatFlags.ReceivedPacketSizes);
		}
		this.ApplyDefaultLayout(objectLayout.GetValueOrDefault(flag ? FusionStats.DefaultLayouts.UpperRight : FusionStats.DefaultLayouts.Full), new FusionStats.StatCanvasTypes?(FusionStats.StatCanvasTypes.GameObject));
		this.ApplyDefaultLayout(screenLayout.GetValueOrDefault(FusionStats.DefaultLayouts.Right), new FusionStats.StatCanvasTypes?(FusionStats.StatCanvasTypes.Overlay));
		this.Guid = System.Guid.NewGuid().ToString().Substring(0, 13);
		this.GenerateGraphs();
	}

	private void Awake()
	{
		if (this._guidesRT)
		{
			UnityEngine.Object.Destroy(this._guidesRT.gameObject);
		}
		if (Application.isPlaying)
		{
			this._foundViews = new List<IFusionStatsView>();
			base.GetComponentsInChildren<IFusionStatsView>(true, this._foundViews);
		}
		if (this.Guid == "")
		{
			this.Guid = System.Guid.NewGuid().ToString().Substring(0, 13);
		}
		if (this.EnforceSingle && this.Guid != null)
		{
			if (FusionStats._activeGuids.ContainsKey(this.Guid))
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			FusionStats._activeGuids.Add(this.Guid, this);
		}
		if (this.EnforceSingle && base.transform.parent == null && this._canvasType == FusionStats.StatCanvasTypes.Overlay)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			this.Initialize();
			this._activeDirty = true;
			this._layoutDirty = 2;
		}
	}

	private void OnDestroy()
	{
		this.DisassociateWithRunner(this._runner);
		FusionStats fusionStats;
		if (this.Guid != null && FusionStats._activeGuids.TryGetValue(this.Guid, out fusionStats) && fusionStats == this)
		{
			FusionStats._activeGuids.Remove(this.Guid);
		}
	}

	[BehaviourButtonAction("Destroy Graphs", null, "_canvasRT", ConditionFlags = BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime)]
	private void DestroyGraphs()
	{
		if (this._canvasRT)
		{
			UnityEngine.Object.DestroyImmediate(this._canvasRT.gameObject);
		}
		this._canvasRT = null;
	}

	public static bool NewInputSystemFound
	{
		get
		{
			if (FusionStats._newInputSystemFound == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					Type[] types = assemblies[i].GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						if (types[j].Namespace == "UnityEngine.InputSystem")
						{
							FusionStats._newInputSystemFound = new bool?(true);
							return true;
						}
					}
				}
				FusionStats._newInputSystemFound = new bool?(false);
				return false;
			}
			return FusionStats._newInputSystemFound.Value;
		}
	}

	private void Initialize()
	{
		if (Application.isPlaying && !FusionStats.NewInputSystemFound && UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
		{
			GameObject gameObject = new GameObject("Event System");
			gameObject.AddComponent<EventSystem>();
			gameObject.AddComponent<StandaloneInputModule>();
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		if (!this._canvasRT)
		{
			this.GenerateGraphs();
		}
		if (this._canvasRT)
		{
			Button togglButton = this._togglButton;
			if (togglButton != null)
			{
				togglButton.onClick.RemoveListener(new UnityAction(this.Toggle));
			}
			Button canvsButton = this._canvsButton;
			if (canvsButton != null)
			{
				canvsButton.onClick.RemoveListener(new UnityAction(this.ToggleCanvasType));
			}
			Button clearButton = this._clearButton;
			if (clearButton != null)
			{
				clearButton.onClick.RemoveListener(new UnityAction(this.Clear));
			}
			Button pauseButton = this._pauseButton;
			if (pauseButton != null)
			{
				pauseButton.onClick.RemoveListener(new UnityAction(this.Pause));
			}
			Button closeButton = this._closeButton;
			if (closeButton != null)
			{
				closeButton.onClick.RemoveListener(new UnityAction(this.Close));
			}
			Button titleButton = this._titleButton;
			if (titleButton != null)
			{
				titleButton.onClick.RemoveListener(new UnityAction(this.PingSelectFusionStats));
			}
			Button objctButton = this._objctButton;
			if (objctButton != null)
			{
				objctButton.onClick.RemoveListener(new UnityAction(this.PingSelectObject));
			}
			Button togglButton2 = this._togglButton;
			if (togglButton2 != null)
			{
				togglButton2.onClick.AddListener(new UnityAction(this.Toggle));
			}
			Button canvsButton2 = this._canvsButton;
			if (canvsButton2 != null)
			{
				canvsButton2.onClick.AddListener(new UnityAction(this.ToggleCanvasType));
			}
			Button clearButton2 = this._clearButton;
			if (clearButton2 != null)
			{
				clearButton2.onClick.AddListener(new UnityAction(this.Clear));
			}
			Button pauseButton2 = this._pauseButton;
			if (pauseButton2 != null)
			{
				pauseButton2.onClick.AddListener(new UnityAction(this.Pause));
			}
			Button closeButton2 = this._closeButton;
			if (closeButton2 != null)
			{
				closeButton2.onClick.AddListener(new UnityAction(this.Close));
			}
			Button titleButton2 = this._titleButton;
			if (titleButton2 != null)
			{
				titleButton2.onClick.AddListener(new UnityAction(this.PingSelectFusionStats));
			}
			Button objctButton2 = this._objctButton;
			if (objctButton2 != null)
			{
				objctButton2.onClick.AddListener(new UnityAction(this.PingSelectObject));
			}
			base.GetComponentsInChildren<IFusionStatsView>(true, this._foundViews);
			foreach (IFusionStatsView fusionStatsView in this._foundViews)
			{
				fusionStatsView.Initialize();
			}
			this._layoutDirty = 1;
		}
	}

	private bool _graphsAreMissing
	{
		get
		{
			return this._canvasRT == null;
		}
	}

	[BehaviourButtonAction("Generate Graphs", null, "_graphsAreMissing", ConditionFlags = BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime)]
	private void GenerateGraphs()
	{
		Transform component = base.gameObject.GetComponent<Transform>();
		this._canvasRT = component.CreateRectTransform("Stats Canvas", false);
		this._canvas = this._canvasRT.gameObject.AddComponent<Canvas>();
		this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		if (this.Runner && this.Runner.IsRunning)
		{
			RunnerVisibilityNode.AddVisibilityNodes(this._canvasRT.gameObject, this.Runner);
		}
		CanvasScaler canvasScaler = this._canvasRT.gameObject.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1080f, 1080f);
		canvasScaler.matchWidthOrHeight = 0.4f;
		this._canvasRT.gameObject.AddComponent<GraphicRaycaster>();
		this._rootPanelRT = this._canvasRT.CreateRectTransform("Root Panel", false);
		this._headerRT = this._rootPanelRT.CreateRectTransform("Header Panel", false).AddCircleSprite(this.PanelColor);
		this._titleRT = this._headerRT.CreateRectTransform("Runner Title", false).SetAnchors(0f, 1f, 0.75f, 1f).SetOffsets(6f, -6f, 0f, -6f);
		this._titleButton = this._titleRT.gameObject.AddComponent<Button>();
		this._titleText = this._titleRT.AddText(this._runner ? this._runner.name : "Disconnected", TextAnchor.UpperCenter, this._fontColor);
		this._titleText.raycastTarget = true;
		this._buttonsRT = this._headerRT.CreateRectTransform("Buttons", false).SetAnchors(0f, 1f, 0f, 0.75f).SetOffsets(6f, -6f, 6f, 0f);
		HorizontalLayoutGroup horizontalLayoutGroup = this._buttonsRT.gameObject.AddComponent<HorizontalLayoutGroup>();
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childControlWidth = true;
		horizontalLayoutGroup.spacing = 6f;
		this._buttonsRT.MakeButton(ref this._togglButton, "▼", "HIDE", out this._togglIcon, out this._togglLabel, new UnityAction(this.Toggle));
		this._buttonsRT.MakeButton(ref this._canvsButton, "ﬦ", "CANVAS", out this._canvsIcon, out this._canvsLabel, new UnityAction(this.ToggleCanvasType));
		this._buttonsRT.MakeButton(ref this._pauseButton, "װ", "PAUSE", out this._pauseIcon, out this._pauseLabel, new UnityAction(this.Pause));
		this._buttonsRT.MakeButton(ref this._clearButton, "ᴓ", "CLEAR", out this._clearIcon, out this._clearLabel, new UnityAction(this.Clear));
		this._buttonsRT.MakeButton(ref this._closeButton, "x", "CLOSE", out this._closeIcon, out this._closeLabel, new UnityAction(this.Close));
		this._togglIcon.rectTransform.anchorMax = new Vector2(1f, 0.85f);
		this._statsPanelRT = this._rootPanelRT.CreateRectTransform("Stats Panel", false).AddCircleSprite(this.PanelColor);
		this._objectTitlePanelRT = this._statsPanelRT.CreateRectTransform("Object Name Panel", false).ExpandTopAnchor(new float?((float)6)).AddCircleSprite(this._objDataBackColor);
		this._objctButton = this._objectTitlePanelRT.gameObject.AddComponent<Button>();
		RectTransform rectTransform = this._objectTitlePanelRT.CreateRectTransform("Object Name", false).SetAnchors(0f, 1f, 0.15f, 0.85f).SetOffsets(10f, -10f, 0f, 0f);
		this._objectNameText = rectTransform.AddText("Object Name", TextAnchor.MiddleCenter, this._fontColor);
		this._objectNameText.alignByGeometry = false;
		this._objectNameText.raycastTarget = false;
		this._objectIdsGroupRT = FusionStatsObjectIds.Create(this._statsPanelRT, this);
		this._objectMetersPanelRT = this._statsPanelRT.CreateRectTransform("Object Meters Layout", false).ExpandTopAnchor(new float?((float)6)).AddVerticalLayoutGroup(6f, null, null, null, null);
		FusionStatsMeterBar.Create(this._objectMetersPanelRT, this, Simulation.Statistics.StatSourceTypes.NetworkObject, 0, 15f, 30f);
		FusionStatsMeterBar.Create(this._objectMetersPanelRT, this, Simulation.Statistics.StatSourceTypes.NetworkObject, 1, 3f, 6f);
		this._graphsLayoutRT = this._statsPanelRT.CreateRectTransform("Graphs Layout", false).ExpandAnchor(null).SetOffsets(6f, 0f, 0f, 0f);
		this._graphGridLayoutGroup = this._graphsLayoutRT.AddGridlLayoutGroup(6f, null, null, null, null);
		this._objGraphs = new FusionGraph[2];
		for (int i = 0; i < 2; i++)
		{
			if (this.InitializeAllGraphs || ((1 << i) & (int)this._includedObjStats) != 0)
			{
				this.CreateGraph(Simulation.Statistics.StatSourceTypes.NetworkObject, i, this._graphsLayoutRT);
			}
		}
		this._netGraphs = new FusionGraph[3];
		for (int j = 0; j < 3; j++)
		{
			if (this.InitializeAllGraphs || ((1 << j) & (int)this._includedNetStats) != 0)
			{
				this.CreateGraph(Simulation.Statistics.StatSourceTypes.NetConnection, j, this._graphsLayoutRT);
			}
		}
		this._simGraphs = new FusionGraph[16];
		for (int k = 0; k < 16; k++)
		{
			if (this.InitializeAllGraphs || ((1 << k) & (int)this._includedSimStats) != 0)
			{
				this.CreateGraph(Simulation.Statistics.StatSourceTypes.Simulation, k, this._graphsLayoutRT);
			}
		}
		this._activeDirty = true;
		this._layoutDirty = 2;
	}

	private void AssociateWithRunner(NetworkRunner runner)
	{
		if (runner != null)
		{
			List<FusionStats> list;
			if (!FusionStats._statsForRunnerLookup.TryGetValue(runner, out list))
			{
				FusionStats._statsForRunnerLookup.Add(runner, new List<FusionStats> { this });
				return;
			}
			list.Add(this);
		}
	}

	private void DisassociateWithRunner(NetworkRunner runner)
	{
		List<FusionStats> list;
		if (runner != null && FusionStats._statsForRunnerLookup.TryGetValue(runner, out list) && list.Contains(this))
		{
			list.Remove(this);
		}
	}

	private void Pause()
	{
		if (this._runner && this._runner.Simulation != null)
		{
			this._paused = !this._paused;
			string text = (this._paused ? "►" : "װ");
			string text2 = (this._paused ? "PLAY" : "PAUSE");
			this._pauseIcon.text = text;
			this._pauseLabel.text = text2;
			List<FusionStats> list;
			if (FusionStats._statsForRunnerLookup.TryGetValue(this._runner, out list))
			{
				bool flag = false;
				using (List<FusionStats>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current._paused)
						{
							flag = true;
							break;
						}
					}
				}
				this._runner.Simulation.Stats.Pause(!flag);
			}
		}
	}

	private void Toggle()
	{
		this._hidden = !this._hidden;
		this._togglIcon.text = (this._hidden ? "▲" : "▼");
		this._togglLabel.text = (this._hidden ? "SHOW" : "HIDE");
		this._statsPanelRT.gameObject.SetActive(!this._hidden);
		for (int i = 0; i < this._simGraphs.Length; i++)
		{
			if (this._simGraphs[i])
			{
				this._simGraphs[i].gameObject.SetActive(!this._hidden && ((1 << i) & (int)this._includedSimStats) != 0);
			}
		}
		for (int j = 0; j < this._objGraphs.Length; j++)
		{
			if (this._objGraphs[j])
			{
				this._objGraphs[j].gameObject.SetActive(!this._hidden && ((1 << j) & (int)this._includedObjStats) != 0);
			}
		}
		for (int k = 0; k < this._netGraphs.Length; k++)
		{
			if (this._netGraphs[k])
			{
				this._netGraphs[k].gameObject.SetActive(!this._hidden && ((1 << k) & (int)this._includedNetStats) != 0);
			}
		}
	}

	private void Clear()
	{
		if (this._runner && this._runner.Simulation != null)
		{
			this._runner.Simulation.Stats.Clear();
		}
		for (int i = 0; i < this._simGraphs.Length; i++)
		{
			if (this._simGraphs[i])
			{
				this._simGraphs[i].Clear();
			}
		}
		for (int j = 0; j < this._objGraphs.Length; j++)
		{
			if (this._objGraphs[j])
			{
				this._objGraphs[j].Clear();
			}
		}
		for (int k = 0; k < this._netGraphs.Length; k++)
		{
			if (this._netGraphs[k])
			{
				this._netGraphs[k].Clear();
			}
		}
	}

	private void ToggleCanvasType()
	{
		this._canvasType = ((this._canvasType == FusionStats.StatCanvasTypes.GameObject) ? FusionStats.StatCanvasTypes.Overlay : FusionStats.StatCanvasTypes.GameObject);
		this._layoutDirty = 3;
		this.CalculateLayout();
	}

	private void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void PingSelectObject()
	{
	}

	private void PingSelectFusionStats()
	{
	}

	private void LateUpdate()
	{
		NetworkRunner runner = this.Runner;
		bool flag = runner == null;
		if (this.AutoDestroy && flag)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (this._activeDirty)
		{
			this.ReapplyEnabled();
		}
		if (this._layoutDirty > 0)
		{
			this.CalculateLayout();
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (flag || runner.IsShutdown)
		{
			return;
		}
		if (this._paused)
		{
			return;
		}
		if (this.RedrawInterval > 0f)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble > this._delayDrawUntil)
			{
				this._currentDrawTime = timeAsDouble;
				while (this._delayDrawUntil <= timeAsDouble)
				{
					this._delayDrawUntil += (double)this.RedrawInterval;
				}
			}
			if (timeAsDouble != this._currentDrawTime)
			{
				return;
			}
		}
		if (this.EnableObjectStats)
		{
			this.RefreshObjectValues();
		}
		foreach (IFusionStatsView fusionStatsView in this._foundViews)
		{
			if (fusionStatsView != null && fusionStatsView.isActiveAndEnabled)
			{
				fusionStatsView.Refresh();
			}
		}
	}

	private void RefreshObjectValues()
	{
		NetworkObject @object = this.Object;
		if (@object == null)
		{
			return;
		}
		string name = @object.name;
		if (this._previousObjectTitle != name)
		{
			this._objectNameText.text = name;
			this._previousObjectTitle = name;
		}
	}

	public FusionGraph CreateGraph(Simulation.Statistics.StatSourceTypes type, int statId, RectTransform parentRT)
	{
		FusionGraph fusionGraph = FusionGraph.Create(this, type, statId, parentRT);
		if (type == Simulation.Statistics.StatSourceTypes.Simulation)
		{
			this._simGraphs[statId] = fusionGraph;
			if ((this._includedSimStats & (Simulation.Statistics.SimStatFlags)(1 << statId)) == (Simulation.Statistics.SimStatFlags)0)
			{
				fusionGraph.gameObject.SetActive(false);
			}
		}
		else if (type == Simulation.Statistics.StatSourceTypes.NetworkObject)
		{
			this._objGraphs[statId] = fusionGraph;
			if ((this._includedObjStats & (Simulation.Statistics.ObjStatFlags)(1 << statId)) == (Simulation.Statistics.ObjStatFlags)0)
			{
				fusionGraph.gameObject.SetActive(false);
			}
		}
		else
		{
			this._netGraphs[statId] = fusionGraph;
			if ((this._includedNetStats & (Simulation.Statistics.NetStatFlags)(1 << statId)) == (Simulation.Statistics.NetStatFlags)0)
			{
				fusionGraph.gameObject.SetActive(false);
			}
		}
		return fusionGraph;
	}

	private void ReapplyEnabled()
	{
		this._activeDirty = false;
		if (this._simGraphs == null || this._simGraphs.Length < 0)
		{
			return;
		}
		if (this._graphsLayoutRT == null)
		{
			return;
		}
		int i = 0;
		while (i < this._simGraphs.Length)
		{
			FusionGraph fusionGraph = this._simGraphs[i];
			bool flag = ((1 << i) & (int)this._includedSimStats) != 0;
			if (!(fusionGraph == null))
			{
				goto IL_6C;
			}
			if (flag)
			{
				fusionGraph = this.CreateGraph(Simulation.Statistics.StatSourceTypes.Simulation, i, this._graphsLayoutRT);
				this._simGraphs[i] = fusionGraph;
				goto IL_6C;
			}
			IL_78:
			i++;
			continue;
			IL_6C:
			fusionGraph.gameObject.SetActive(flag);
			goto IL_78;
		}
		int j = 0;
		while (j < this._objGraphs.Length)
		{
			FusionGraph fusionGraph2 = this._objGraphs[j];
			bool flag2 = this._enableObjectStats && ((1 << j) & (int)this._includedObjStats) != 0;
			if (!(fusionGraph2 == null))
			{
				goto IL_DA;
			}
			if (flag2)
			{
				fusionGraph2 = this.CreateGraph(Simulation.Statistics.StatSourceTypes.NetworkObject, j, this._graphsLayoutRT);
				this._objGraphs[j] = fusionGraph2;
				goto IL_DA;
			}
			IL_F8:
			j++;
			continue;
			IL_DA:
			if (this._objGraphs[j] != null)
			{
				fusionGraph2.gameObject.SetActive(flag2);
				goto IL_F8;
			}
			goto IL_F8;
		}
		int k = 0;
		while (k < this._netGraphs.Length)
		{
			FusionGraph fusionGraph3 = this._netGraphs[k];
			bool flag3 = ((1 << k) & (int)this._includedNetStats) != 0;
			if (!(fusionGraph3 == null))
			{
				goto IL_154;
			}
			if (flag3)
			{
				fusionGraph3 = this.CreateGraph(Simulation.Statistics.StatSourceTypes.NetConnection, k, this._graphsLayoutRT);
				this._netGraphs[k] = fusionGraph3;
				goto IL_154;
			}
			IL_173:
			k++;
			continue;
			IL_154:
			if (this._netGraphs[k] != null)
			{
				fusionGraph3.gameObject.SetActive(flag3);
				goto IL_173;
			}
			goto IL_173;
		}
	}

	private void CalculateLayout()
	{
		if (this._rootPanelRT == null || this._graphsLayoutRT == null)
		{
			return;
		}
		if (this._foundGraphs == null)
		{
			this._foundGraphs = new List<FusionGraph>(this._graphsLayoutRT.GetComponentsInChildren<FusionGraph>(false));
		}
		else
		{
			base.GetComponentsInChildren<FusionGraph>(false, this._foundGraphs);
		}
		float time = Time.time;
		if (this._lastLayoutUpdate < time)
		{
			this._layoutDirty--;
			this._lastLayoutUpdate = time;
		}
		if (this._layoutDirty <= 0)
		{
			bool enabled = this._canvas.enabled;
		}
		if (this._rootPanelRT)
		{
			float num = Math.Min((float)this._maxHeaderHeight, this._rootPanelRT.rect.width / 4f);
			if (this._canvasType == FusionStats.StatCanvasTypes.GameObject)
			{
				this._canvas.renderMode = RenderMode.WorldSpace;
				float num2 = this.CanvasScale / 1080f;
				this._canvasRT.localScale = new Vector3(num2, num2, num2);
				this._canvasRT.sizeDelta = new Vector2(1024f, 1024f);
				this._canvasRT.localPosition = new Vector3(0f, 0f, this.CanvasDistance);
				if (!this._canvasRT.GetComponent<FusionStatsBillboard>())
				{
					this._canvasRT.localRotation = default(Quaternion);
				}
			}
			else
			{
				this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			}
			this._objectTitlePanelRT.gameObject.SetActive(this._enableObjectStats);
			this._objectIdsGroupRT.gameObject.SetActive(this._enableObjectStats);
			this._objectMetersPanelRT.gameObject.SetActive(this._enableObjectStats);
			Vector2 vector;
			if (this._showButtonLabels)
			{
				vector = new Vector2(0f, 0.0875f);
			}
			else
			{
				vector = new Vector2(0f, 0f);
			}
			this._togglIcon.rectTransform.anchorMin = vector + new Vector2(0f, 0.15f);
			this._canvsIcon.rectTransform.anchorMin = vector;
			this._clearIcon.rectTransform.anchorMin = vector;
			this._pauseIcon.rectTransform.anchorMin = vector;
			this._closeIcon.rectTransform.anchorMin = vector;
			this._togglLabel.gameObject.SetActive(this._showButtonLabels);
			this._canvsLabel.gameObject.SetActive(this._showButtonLabels);
			this._clearLabel.gameObject.SetActive(this._showButtonLabels);
			this._pauseLabel.gameObject.SetActive(this._showButtonLabels);
			this._closeLabel.gameObject.SetActive(this._showButtonLabels);
			Rect currentRect = this.CurrentRect;
			this._rootPanelRT.anchorMax = new Vector2(currentRect.xMax, currentRect.yMax);
			this._rootPanelRT.anchorMin = new Vector2(currentRect.xMin, currentRect.yMin);
			this._rootPanelRT.sizeDelta = new Vector2(0f, 0f);
			this._rootPanelRT.pivot = new Vector2(0.5f, 0.5f);
			this._rootPanelRT.anchoredPosition3D = default(Vector3);
			this._headerRT.anchorMin = new Vector2(0f, 1f);
			this._headerRT.anchorMax = new Vector2(1f, 1f);
			this._headerRT.pivot = new Vector2(0.5f, 1f);
			this._headerRT.anchoredPosition3D = default(Vector3);
			this._headerRT.sizeDelta = new Vector2(0f, num);
			this._objectTitlePanelRT.offsetMax = new Vector2(-6f, -6f);
			this._objectTitlePanelRT.offsetMin = new Vector2(6f, (float)(-(float)this.ObjectTitleHeight));
			this._objectIdsGroupRT.offsetMax = new Vector2(-6f, (float)(-(float)(this.ObjectTitleHeight + 6)));
			this._objectIdsGroupRT.offsetMin = new Vector2(6f, (float)(-(float)(this.ObjectTitleHeight + this.ObjectIdsHeight)));
			this._objectMetersPanelRT.offsetMax = new Vector2(-6f, (float)(-(float)(this.ObjectTitleHeight + this.ObjectIdsHeight + 6)));
			this._objectMetersPanelRT.offsetMin = new Vector2(6f, (float)(-(float)(this.ObjectTitleHeight + this.ObjectIdsHeight + this.ObjectMetersHeight)));
			this._objectTitlePanelRT.gameObject.SetActive(this.EnableObjectStats && this.ObjectTitleHeight > 0);
			this._objectIdsGroupRT.gameObject.SetActive(this.EnableObjectStats && this.ObjectIdsHeight > 0);
			this._objectMetersPanelRT.gameObject.SetActive(this.EnableObjectStats && this.ObjectMetersHeight > 0);
			this._statsPanelRT.ExpandAnchor(null).SetOffsets(0f, 0f, 0f, -num);
			if (this._enableObjectStats && this._statsPanelRT.rect.height < (float)(this.ObjectTitleHeight + this.ObjectIdsHeight + this.ObjectMetersHeight))
			{
				this._statsPanelRT.offsetMin = new Vector2(0f, this._statsPanelRT.rect.height - (float)(this.ObjectTitleHeight + this.ObjectIdsHeight + this.ObjectMetersHeight + 6));
			}
			int num3 = ((this.GraphColumnCount > 0) ? this.GraphColumnCount : ((int)(this._graphsLayoutRT.rect.width / (float)(this._graphMaxWidth + 6))));
			if (num3 < 1)
			{
				num3 = 1;
			}
			int num4 = (int)Math.Ceiling((double)this._foundGraphs.Count / (double)num3);
			if (num4 < 1)
			{
				num4 = 1;
			}
			if (num4 == 1)
			{
				num3 = this._foundGraphs.Count;
			}
			this._graphGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			this._graphGridLayoutGroup.constraintCount = num3;
			float num5 = this._graphsLayoutRT.rect.width / (float)num3 - 6f;
			float num6 = this._graphsLayoutRT.rect.height / (float)num4 - 6f;
			this._graphGridLayoutGroup.cellSize = new Vector2(num5, num6);
			this._graphsLayoutRT.offsetMax = new Vector2(0f, (float)(this._enableObjectStats ? (-(float)(this.ObjectTitleHeight + this.ObjectIdsHeight + this.ObjectMetersHeight + 6)) : (-6)));
			if (this._foundViews == null)
			{
				this._foundViews = new List<IFusionStatsView>(base.GetComponentsInChildren<IFusionStatsView>(false));
			}
			else
			{
				base.GetComponentsInChildren<IFusionStatsView>(false, this._foundViews);
			}
			if (this._objGraphs != null)
			{
				foreach (FusionGraph fusionGraph in this._objGraphs)
				{
					if (fusionGraph)
					{
						fusionGraph.gameObject.SetActive((this._includedObjStats & (Simulation.Statistics.ObjStatFlags)(1 << fusionGraph.StatId)) != (Simulation.Statistics.ObjStatFlags)0 && this._enableObjectStats);
					}
				}
			}
			for (int j = 0; j < this._foundViews.Count; j++)
			{
				IFusionStatsView fusionStatsView = this._foundViews[j];
				if (fusionStatsView != null && fusionStatsView.isActiveAndEnabled)
				{
					fusionStatsView.CalculateLayout();
					fusionStatsView.transform.localRotation = default(Quaternion);
					fusionStatsView.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
	}

	private void ApplyDefaultLayout(FusionStats.DefaultLayouts defaults, FusionStats.StatCanvasTypes? applyForCanvasType = null)
	{
		bool flag = applyForCanvasType == null || applyForCanvasType.Value == FusionStats.StatCanvasTypes.GameObject;
		bool flag2 = applyForCanvasType == null || applyForCanvasType.Value == FusionStats.StatCanvasTypes.Overlay;
		if (defaults == FusionStats.DefaultLayouts.Custom)
		{
			return;
		}
		bool flag3 = Screen.height > Screen.width;
		Rect rect;
		Rect rect2;
		switch (defaults)
		{
		case FusionStats.DefaultLayouts.Left:
			rect = Rect.MinMaxRect(0f, 0f, 0.3f, 1f);
			rect2 = rect;
			break;
		case FusionStats.DefaultLayouts.Right:
			rect = Rect.MinMaxRect(0.7f, 0f, 1f, 1f);
			rect2 = rect;
			break;
		case FusionStats.DefaultLayouts.UpperLeft:
			rect = Rect.MinMaxRect(0f, 0.5f, 0.3f, 1f);
			rect2 = (flag3 ? Rect.MinMaxRect(0f, 0.7f, 0.3f, 1f) : rect);
			break;
		case FusionStats.DefaultLayouts.UpperRight:
			rect = Rect.MinMaxRect(0.7f, 0.5f, 1f, 1f);
			rect2 = (flag3 ? Rect.MinMaxRect(0.7f, 0.7f, 1f, 1f) : rect);
			break;
		case FusionStats.DefaultLayouts.Full:
			rect = Rect.MinMaxRect(0f, 0f, 1f, 1f);
			rect2 = rect;
			break;
		default:
			rect = Rect.MinMaxRect(0f, 0.5f, 0.3f, 1f);
			rect2 = rect;
			break;
		}
		if (flag)
		{
			this.GameObjectRect = rect;
		}
		if (flag2)
		{
			this.OverlayRect = rect2;
		}
		this._layoutDirty++;
	}

	public FusionStats()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static FusionStats()
	{
	}

	private static Dictionary<NetworkRunner, List<FusionStats>> _statsForRunnerLookup = new Dictionary<NetworkRunner, List<FusionStats>>();

	private static Dictionary<string, FusionStats> _activeGuids = new Dictionary<string, FusionStats>();

	public const Simulation.Statistics.SimStatFlags DefaultSimStatsMask = Simulation.Statistics.SimStatFlags.ForwardSimCount | Simulation.Statistics.SimStatFlags.ResimCount | Simulation.Statistics.SimStatFlags.PacketSize;

	private const int SCREEN_SCALE_W = 1080;

	private const int SCREEN_SCALE_H = 1080;

	private const float TEXT_MARGIN = 0.25f;

	private const float TITLE_HEIGHT = 20f;

	private const int MARGIN = 6;

	private const int PAD = 10;

	private const string PLAY_TEXT = "PLAY";

	private const string PAUS_TEXT = "PAUSE";

	private const string SHOW_TEXT = "SHOW";

	private const string HIDE_TEXT = "HIDE";

	private const string CLER_TEXT = "CLEAR";

	private const string CNVS_TEXT = "CANVAS";

	private const string CLSE_TEXT = "CLOSE";

	private const string PLAY_ICON = "►";

	private const string PAUS_ICON = "װ";

	private const string HIDE_ICON = "▼";

	private const string SHOW_ICON = "▲";

	private const string CLER_ICON = "ᴓ";

	private const string CNVS_ICON = "ﬦ";

	private const string CLSE_ICON = "x";

	[InlineHelp]
	[Unit(Units.Seconds, 1.0, 0.0, DecimalPlaces = 2)]
	[MultiPropertyDrawersFix]
	public float RedrawInterval = 0.1f;

	[Header("Layout")]
	[InlineHelp]
	[SerializeField]
	private FusionStats.StatCanvasTypes _canvasType;

	[InlineHelp]
	[SerializeField]
	private bool _showButtonLabels = true;

	[InlineHelp]
	[SerializeField]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _maxHeaderHeight = 70;

	[InlineHelp]
	[DrawIf("_canvasType", 1.0, Hide = true)]
	[Range(0f, 20f)]
	[MultiPropertyDrawersFix]
	public float CanvasScale = 5f;

	[InlineHelp]
	[DrawIf("_canvasType", 1.0, Hide = true)]
	[Range(-10f, 10f)]
	[MultiPropertyDrawersFix]
	public float CanvasDistance;

	[InlineHelp]
	[SerializeField]
	[DrawIf("CanvasType", 1.0, Hide = true)]
	[NormalizedRect(true, 1f)]
	[MultiPropertyDrawersFix]
	private Rect _gameObjectRect = new Rect(0f, 0f, 0.3f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("CanvasType", 0.0, Hide = true)]
	[NormalizedRect(true, 0f)]
	[MultiPropertyDrawersFix]
	private Rect _overlayRect = new Rect(0f, 0f, 0.3f, 1f);

	[Header("Fusion Graphs Layout")]
	[InlineHelp]
	[SerializeField]
	private FusionGraph.Layouts _defaultLayout;

	[InlineHelp]
	[SerializeField]
	private bool _noTextOverlap;

	[InlineHelp]
	[SerializeField]
	private bool _noGraphShader;

	[InlineHelp]
	[Range(0f, 16f)]
	[MultiPropertyDrawersFix]
	public int GraphColumnCount = 1;

	[InlineHelp]
	[SerializeField]
	[DrawIf("GraphColumnCount", 0.0)]
	[Range(30f, 1080f)]
	[MultiPropertyDrawersFix]
	private int _graphMaxWidth = 270;

	[Header("Network Object Stats")]
	[InlineHelp]
	[SerializeField]
	[WarnIf("ShowMissingNetObjWarning", "No NetworkObject found on this GameObject, nor parent. Object stats will be unavailable.")]
	private bool _enableObjectStats;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	private NetworkObject _object;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _objectTitleHeight = 48;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _objectIdsHeight = 60;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _objectMetersHeight = 90;

	[Header("Data")]
	[SerializeField]
	[InlineHelp]
	[EditorDisabled(false)]
	[MultiPropertyDrawersFix]
	private NetworkRunner _runner;

	[InlineHelp]
	public bool InitializeAllGraphs;

	[InlineHelp]
	[VersaMask(false, null)]
	[MultiPropertyDrawersFix]
	public SimulationModes ConnectTo = SimulationModes.Client;

	[InlineHelp]
	[SerializeField]
	[VersaMask(false, null)]
	[DrawIf("EnableObjectStats")]
	[MultiPropertyDrawersFix]
	private Simulation.Statistics.ObjStatFlags _includedObjStats;

	[InlineHelp]
	[SerializeField]
	[VersaMask(false, null)]
	[MultiPropertyDrawersFix]
	private Simulation.Statistics.NetStatFlags _includedNetStats;

	[InlineHelp]
	[SerializeField]
	[VersaMask(false, null)]
	[MultiPropertyDrawersFix]
	private Simulation.Statistics.SimStatFlags _includedSimStats;

	[Header("Life-Cycle")]
	[InlineHelp]
	[SerializeField]
	public bool AutoDestroy;

	[InlineHelp]
	[SerializeField]
	public bool EnforceSingle = true;

	[InlineHelp]
	[DrawIf("EnforceSingle")]
	[SerializeField]
	public string Guid;

	[Header("Customization")]
	[InlineHelp]
	[SerializeField]
	[DrawIf("IsNotPlaying", Hide = true)]
	[MultiPropertyDrawersFix]
	private bool _modifyColors;

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorGood = new Color(0.1f, 0.5f, 0.1f, 0.9f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorWarn = new Color(0.75f, 0.75f, 0.2f, 0.9f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorBad = new Color(0.9f, 0.2f, 0.2f, 0.9f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorFlag = new Color(0.8f, 0.75f, 0f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _fontColor = new Color(1f, 1f, 1f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color PanelColor = new Color(0.3f, 0.3f, 0.3f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _simDataBackColor = new Color(0.1f, 0.08f, 0.08f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _netDataBackColor = new Color(0.15f, 0.14f, 0.09f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _objDataBackColor = new Color(0f, 0.2f, 0.4f, 1f);

	[SerializeField]
	[HideInInspector]
	private FusionGraph[] _simGraphs;

	[SerializeField]
	[HideInInspector]
	private FusionGraph[] _objGraphs;

	[SerializeField]
	[HideInInspector]
	private FusionGraph[] _netGraphs;

	[NonSerialized]
	private List<IFusionStatsView> _foundViews;

	[NonSerialized]
	private List<FusionGraph> _foundGraphs;

	[SerializeField]
	[HideInInspector]
	private Text _titleText;

	[SerializeField]
	[HideInInspector]
	private Text _clearIcon;

	[SerializeField]
	[HideInInspector]
	private Text _pauseIcon;

	[SerializeField]
	[HideInInspector]
	private Text _togglIcon;

	[SerializeField]
	[HideInInspector]
	private Text _closeIcon;

	[SerializeField]
	[HideInInspector]
	private Text _canvsIcon;

	[SerializeField]
	[HideInInspector]
	private Text _clearLabel;

	[SerializeField]
	[HideInInspector]
	private Text _pauseLabel;

	[SerializeField]
	[HideInInspector]
	private Text _togglLabel;

	[SerializeField]
	[HideInInspector]
	private Text _closeLabel;

	[SerializeField]
	[HideInInspector]
	private Text _canvsLabel;

	[SerializeField]
	[HideInInspector]
	private Text _objectNameText;

	[SerializeField]
	[HideInInspector]
	private GridLayoutGroup _graphGridLayoutGroup;

	[SerializeField]
	[HideInInspector]
	private Canvas _canvas;

	[SerializeField]
	[HideInInspector]
	private RectTransform _canvasRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _rootPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _guidesRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _headerRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _statsPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _graphsLayoutRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _titleRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _buttonsRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _objectTitlePanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _objectIdsGroupRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _objectMetersPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _clientIdPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _authorityPanelRT;

	[SerializeField]
	[HideInInspector]
	private Button _titleButton;

	[SerializeField]
	[HideInInspector]
	private Button _objctButton;

	[SerializeField]
	[HideInInspector]
	private Button _clearButton;

	[SerializeField]
	[HideInInspector]
	private Button _togglButton;

	[SerializeField]
	[HideInInspector]
	private Button _pauseButton;

	[SerializeField]
	[HideInInspector]
	private Button _closeButton;

	[SerializeField]
	[HideInInspector]
	private Button _canvsButton;

	private Font _font;

	private bool _hidden;

	private bool _paused;

	private int _layoutDirty;

	private bool _activeDirty;

	private double _currentDrawTime;

	private double _delayDrawUntil;

	private static bool? _newInputSystemFound;

	private string _previousObjectTitle;

	private float _lastLayoutUpdate;

	public enum StatCanvasTypes
	{
		Overlay,
		GameObject
	}

	public enum DefaultLayouts
	{
		Custom,
		Left,
		Right,
		UpperLeft,
		UpperRight,
		Full
	}
}
