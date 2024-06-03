using System;
using GorillaTag;
using Photon.Pun;
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
		this._watchToggle = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchReset);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		int i;
		if (!this.FetchMyViewID(out i))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = i.GetStaticHash();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	private void OnWatchToggle(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creatorWrapped.ID)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchToggle");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int millis = (int)args[2];
		this._watchFace.SetMillisElapsed(millis, true);
		this._watchFace.WatchToggle();
	}

	private void OnWatchReset(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creatorWrapped.ID)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchReset");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		this._watchFace.WatchReset();
	}

	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		Player player = (this.myOnlineRig != null) ? this.myOnlineRig.creator : ((this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : PhotonNetwork.LocalPlayer) : null);
		if (player == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
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
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[]
			{
				this._photonID
			});
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

	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchToggle;

	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchReset;

	[DebugOption]
	public bool disableActivation;

	[DebugOption]
	public bool disableDeactivation;
}
