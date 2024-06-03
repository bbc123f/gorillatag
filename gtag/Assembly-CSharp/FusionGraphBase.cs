using System;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.UI;

[ScriptHelp(BackColor = EditorHeaderBackColor.Olive)]
public abstract class FusionGraphBase : Fusion.Behaviour, IFusionStatsView
{
	public Simulation.Statistics.StatSourceTypes StateSourceType
	{
		get
		{
			return this._statSourceType;
		}
		set
		{
			this._statSourceType = value;
			this.TryConnect();
		}
	}

	public int StatId
	{
		get
		{
			return this._statId;
		}
		set
		{
			this._statId = value;
			this.TryConnect();
		}
	}

	public IStatsBuffer StatsBuffer
	{
		get
		{
			if (this._statsBuffer == null)
			{
				this.TryConnect();
			}
			return this._statsBuffer;
		}
	}

	public bool IsOverlay
	{
		get
		{
			return this._isOverlay;
		}
		set
		{
			if (this._isOverlay != value)
			{
				this._isOverlay = value;
				this.CalculateLayout();
				this._layoutDirty = true;
			}
		}
	}

	protected virtual Color BackColor
	{
		get
		{
			if (this._statSourceType == Simulation.Statistics.StatSourceTypes.Simulation)
			{
				return this._fusionStats.SimDataBackColor;
			}
			if (this._statSourceType == Simulation.Statistics.StatSourceTypes.NetConnection)
			{
				return this._fusionStats.NetDataBackColor;
			}
			return this._fusionStats.ObjDataBackColor;
		}
	}

	protected Type CastToStatType
	{
		get
		{
			if (this._statSourceType == Simulation.Statistics.StatSourceTypes.Simulation)
			{
				return typeof(Simulation.Statistics.SimStats);
			}
			if (this._statSourceType != Simulation.Statistics.StatSourceTypes.NetConnection)
			{
				return typeof(Simulation.Statistics.ObjStats);
			}
			return typeof(Simulation.Statistics.NetStats);
		}
	}

	protected FusionStats LocateParentFusionStats()
	{
		if (this._fusionStats == null)
		{
			this._fusionStats = base.GetComponentInParent<FusionStats>();
		}
		return this._fusionStats;
	}

	public virtual void Initialize()
	{
	}

	public virtual void CyclePer()
	{
		Simulation.Statistics.StatsPer perFlags = this.StatSourceInfo.PerFlags;
		switch (this.CurrentPer)
		{
		case Simulation.Statistics.StatsPer.Individual:
			if ((perFlags & Simulation.Statistics.StatsPer.Tick) == Simulation.Statistics.StatsPer.Tick)
			{
				this.CurrentPer = Simulation.Statistics.StatsPer.Tick;
				return;
			}
			if ((perFlags & Simulation.Statistics.StatsPer.Second) == Simulation.Statistics.StatsPer.Second)
			{
				this.CurrentPer = Simulation.Statistics.StatsPer.Second;
			}
			return;
		case Simulation.Statistics.StatsPer.Tick:
			if ((perFlags & Simulation.Statistics.StatsPer.Second) == Simulation.Statistics.StatsPer.Second)
			{
				this.CurrentPer = Simulation.Statistics.StatsPer.Second;
				return;
			}
			if ((perFlags & Simulation.Statistics.StatsPer.Individual) == Simulation.Statistics.StatsPer.Individual)
			{
				this.CurrentPer = Simulation.Statistics.StatsPer.Individual;
			}
			return;
		case Simulation.Statistics.StatsPer.Individual | Simulation.Statistics.StatsPer.Tick:
			return;
		case Simulation.Statistics.StatsPer.Second:
			if ((perFlags & Simulation.Statistics.StatsPer.Individual) == Simulation.Statistics.StatsPer.Individual)
			{
				this.CurrentPer = Simulation.Statistics.StatsPer.Individual;
				return;
			}
			if ((perFlags & Simulation.Statistics.StatsPer.Tick) == Simulation.Statistics.StatsPer.Tick)
			{
				this.CurrentPer = Simulation.Statistics.StatsPer.Tick;
			}
			return;
		default:
			return;
		}
	}

	public abstract void CalculateLayout();

	public abstract void Refresh();

	protected virtual bool TryConnect()
	{
		this.StatSourceInfo = Simulation.Statistics.GetDescription(this._statSourceType, this._statId);
		if (this.WarnThreshold == 0f && this.ErrorThreshold == 0f)
		{
			this.WarnThreshold = this.StatSourceInfo.WarnThreshold;
			this.ErrorThreshold = this.StatSourceInfo.ErrorThreshold;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (this._fusionStats == null)
		{
			this._fusionStats = base.GetComponentInParent<FusionStats>();
		}
		FusionStats fusionStats = this._fusionStats;
		NetworkRunner networkRunner = (fusionStats != null) ? fusionStats.Runner : null;
		Simulation.Statistics statistics;
		if (networkRunner == null)
		{
			statistics = null;
		}
		else
		{
			Simulation simulation = networkRunner.Simulation;
			statistics = ((simulation != null) ? simulation.Stats : null);
		}
		Simulation.Statistics statistics2 = statistics;
		switch (this._statSourceType)
		{
		case Simulation.Statistics.StatSourceTypes.Simulation:
			this._statsBuffer = ((statistics2 != null) ? statistics2.GetStatBuffer((Simulation.Statistics.SimStats)this._statId) : null);
			break;
		case Simulation.Statistics.StatSourceTypes.NetworkObject:
			if (this._statId >= 2)
			{
				this.StatId = 0;
			}
			if (this._fusionStats.Object == null)
			{
				this._statsBuffer = null;
			}
			else
			{
				this._statsBuffer = ((statistics2 != null) ? statistics2.GetObjectBuffer(this._fusionStats.Object.Id, (Simulation.Statistics.ObjStats)this._statId, true) : null);
			}
			break;
		case Simulation.Statistics.StatSourceTypes.NetConnection:
			if (networkRunner == null)
			{
				this._statsBuffer = null;
			}
			else
			{
				this._statsBuffer = ((statistics2 != null) ? statistics2.GetStatBuffer((Simulation.Statistics.NetStats)this._statId, networkRunner) : null);
			}
			break;
		default:
			this._statsBuffer = null;
			break;
		}
		if (this.BackImage)
		{
			this.BackImage.color = this.BackColor;
		}
		if (this.LabelTitle)
		{
			this.CheckIfValidIncurrentMode(networkRunner);
			this.ApplyTitleText();
		}
		if ((this.StatSourceInfo.PerFlags & (Simulation.Statistics.StatsPer)this.StatsPerDefault) != (Simulation.Statistics.StatsPer)0)
		{
			this.CurrentPer = (Simulation.Statistics.StatsPer)this.StatsPerDefault;
		}
		else
		{
			this.CurrentPer = this.StatSourceInfo.PerDefault;
		}
		return this._statsBuffer != null;
	}

	protected void ApplyTitleText()
	{
		Simulation.Statistics.StatSourceInfo statSourceInfo = this.StatSourceInfo;
		if (statSourceInfo.LongName == null)
		{
			return;
		}
		if (statSourceInfo.InvalidReason != null)
		{
			this.LabelTitle.text = statSourceInfo.InvalidReason;
			this.BackImage.gameObject.SetActive(false);
			this.LabelTitle.color = this._fusionStats.FontColor * new Color(1f, 1f, 1f, 0.2f);
			return;
		}
		if (this.LabelTitle.rectTransform.rect.width < 100f)
		{
			this.LabelTitle.text = (statSourceInfo.ShortName ?? statSourceInfo.LongName);
		}
		else
		{
			this.LabelTitle.text = statSourceInfo.LongName;
		}
		this.BackImage.gameObject.SetActive(true);
	}

	protected void CheckIfValidIncurrentMode(NetworkRunner runner)
	{
		if (!runner)
		{
			return;
		}
		Simulation.Statistics.StatFlags flags = this.StatSourceInfo.Flags;
		if ((flags & Simulation.Statistics.StatFlags.ValidForBuildType) == (Simulation.Statistics.StatFlags)0)
		{
			this.StatSourceInfo.InvalidReason = "DEBUG DLL ONLY";
			return;
		}
		NetworkObject networkObject;
		if (this._statSourceType != Simulation.Statistics.StatSourceTypes.NetworkObject)
		{
			networkObject = null;
		}
		else
		{
			FusionStats fusionStats = this._fusionStats;
			networkObject = ((fusionStats != null) ? fusionStats.Object : null);
		}
		NetworkObject networkObject2 = networkObject;
		if (networkObject2 && (flags & Simulation.Statistics.StatFlags.ValidOnStateAuthority) == (Simulation.Statistics.StatFlags)0 && networkObject2.HasStateAuthority)
		{
			this.StatSourceInfo.InvalidReason = "NON STATE AUTH ONLY";
			return;
		}
		if (runner)
		{
			if ((flags & Simulation.Statistics.StatFlags.ValidOnServer) == (Simulation.Statistics.StatFlags)0 && !runner.IsClient)
			{
				this.StatSourceInfo.InvalidReason = "CLIENT ONLY";
				return;
			}
			if ((flags & Simulation.Statistics.StatFlags.ValidWithDeltaSnapshot) == (Simulation.Statistics.StatFlags)0 && runner.Config.Simulation.ReplicationMode == SimulationConfig.StateReplicationModes.DeltaSnapshots)
			{
				this.StatSourceInfo.InvalidReason = "EC MODE ONLY";
				return;
			}
		}
	}

	protected FusionGraphBase()
	{
	}

	bool IFusionStatsView.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	Transform IFusionStatsView.get_transform()
	{
		return base.transform;
	}

	protected const int PAD = 10;

	protected const int MRGN = 6;

	protected const int MAX_FONT_SIZE_WITH_GRAPH = 24;

	[SerializeField]
	[HideInInspector]
	protected Text LabelTitle;

	[SerializeField]
	[HideInInspector]
	protected Image BackImage;

	[InlineHelp]
	[SerializeField]
	protected Simulation.Statistics.StatSourceTypes _statSourceType;

	[InlineHelp]
	[SerializeField]
	[CastEnum("CastToStatType")]
	protected int _statId;

	[InlineHelp]
	public FusionGraphBase.StatsPer StatsPerDefault;

	[InlineHelp]
	public float WarnThreshold;

	[InlineHelp]
	public float ErrorThreshold;

	protected IStatsBuffer _statsBuffer;

	protected bool _isOverlay;

	protected FusionStats _fusionStats;

	protected bool _layoutDirty = true;

	protected Simulation.Statistics.StatsPer CurrentPer;

	public Simulation.Statistics.StatSourceInfo StatSourceInfo;

	[SerializeField]
	[HideInInspector]
	private Simulation.Statistics.StatSourceTypes _prevStatSourceType;

	[SerializeField]
	[HideInInspector]
	private int _prevStatId;

	public enum StatsPer
	{
		Default,
		Individual,
		Tick,
		Second = 4
	}
}
