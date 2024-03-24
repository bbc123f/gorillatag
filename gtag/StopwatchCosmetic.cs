using System;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

public class StopwatchCosmetic : TransferrableObject
{
	public bool isActivating
	{
		get
		{
			return this._isActivating;
		}
	}

	public float activeTimeElapsed
	{
		get
		{
			return this._activeTimeElapsed;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (StopwatchCosmetic.gWatchToggleRPC == null)
		{
			StopwatchCosmetic.gWatchToggleRPC = new PhotonEvent(StaticHash.Combine("StopwatchCosmetic", "WatchToggle"));
		}
		if (StopwatchCosmetic.gWatchResetRPC == null)
		{
			StopwatchCosmetic.gWatchResetRPC = new PhotonEvent(StaticHash.Combine("StopwatchCosmetic", "WatchReset"));
		}
		this._watchToggle = new Action<int, int, object[]>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[]>(this.OnWatchReset);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		int num;
		if (!this.FetchMyViewID(out num))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = num.GetStaticHash();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	private void OnWatchToggle(int sender, int target, object[] args)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int num = (int)args[2];
		this._watchFace.SetMillisElapsed(num, true);
		this._watchFace.WatchToggle();
	}

	private void OnWatchReset(int sender, int target, object[] args)
	{
		if (this._photonID == -1)
		{
			return;
		}
		int num = (int)args[0];
		int photonID = this._photonID;
	}

	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		Player creator = (this.targetRigSet ? this.targetRig : (this.myOnlineRig ?? this.myRig)).creator;
		if (creator == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(creator, out rigContainer))
		{
			return false;
		}
		if (rigContainer.Rig.photonView == null)
		{
			return false;
		}
		viewID = rigContainer.Rig.photonView.ViewID;
		return true;
	}

	public bool PollActivated()
	{
		if (!this._activated)
		{
			return false;
		}
		this._activated = false;
		return true;
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this._isActivating)
		{
			this._activeTimeElapsed += Time.deltaTime;
		}
		if (this._isActivating && this._activeTimeElapsed > 1f)
		{
			this._isActivating = false;
			this._watchFace.WatchReset(true);
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[] { this._photonID });
		}
	}

	public override void OnActivate()
	{
		if (!this.CanActivate())
		{
			return;
		}
		base.OnActivate();
		if (this.IsMyItem())
		{
			this._activeTimeElapsed = 0f;
			this._isActivating = true;
		}
	}

	public override void OnDeactivate()
	{
		if (!this.CanDeactivate())
		{
			return;
		}
		base.OnDeactivate();
		if (!this.IsMyItem())
		{
			return;
		}
		this._isActivating = false;
		this._activated = true;
		this._watchFace.WatchToggle();
		StopwatchCosmetic.gWatchToggleRPC.RaiseOthers(new object[]
		{
			this._photonID,
			this._watchFace.watchActive,
			this._watchFace.millisElapsed
		});
		this._activated = false;
	}

	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	public StopwatchCosmetic()
	{
	}

	[SerializeField]
	private StopwatchFace _watchFace;

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	private bool _isActivating;

	[DebugReadOnly]
	[NonSerialized]
	private float _activeTimeElapsed;

	[DebugReadOnly]
	[NonSerialized]
	private bool _activated;

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	private int _photonID = -1;

	private static PhotonEvent gWatchToggleRPC;

	private static PhotonEvent gWatchResetRPC;

	private Action<int, int, object[]> _watchToggle;

	private Action<int, int, object[]> _watchReset;

	[DebugOption]
	public bool disableActivation;

	[DebugOption]
	public bool disableDeactivation;
}
