using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class GameModeSerializer : GorillaSerializerMasterOnly, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	public GorillaGameManager GameModeInstance
	{
		get
		{
			return this.gameModeInstance;
		}
	}

	protected override bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		GorillaNot.IncrementRPCCall(info, "OnInstantiateSetup");
		GameModeSerializer activeNetworkHandler = GameMode.ActiveNetworkHandler;
		if (info.Sender != null && info.Sender.InRoom())
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				GorillaNot.instance.SendReport("trying to inappropriately create game managers", info.Sender.UserId, info.Sender.NickName);
				return false;
			}
			if (!this.photonView.IsRoomView)
			{
				GorillaNot.instance.SendReport("creating game manager as player object", info.Sender.UserId, info.Sender.NickName);
				return false;
			}
			if (activeNetworkHandler.IsNotNull() && activeNetworkHandler != this)
			{
				GorillaNot.instance.SendReport("trying to create multiple game managers", info.Sender.UserId, info.Sender.NickName);
				return false;
			}
		}
		else if ((activeNetworkHandler.IsNotNull() && activeNetworkHandler != this) || !this.photonView.IsRoomView)
		{
			return false;
		}
		object[] instantiationData = info.photonView.InstantiationData;
		if (instantiationData != null && instantiationData.Length >= 1)
		{
			object obj = instantiationData[0];
			if (obj is int)
			{
				int num = (int)obj;
				this.gameModeKey = (GameModeType)num;
				this.gameModeInstance = GameMode.GetGameModeInstance(this.gameModeKey);
				if (this.gameModeInstance.IsNull() || !this.gameModeInstance.ValidGameMode())
				{
					string str = "gamemode invalid null? ";
					GorillaGameManager gorillaGameManager = this.gameModeInstance;
					Debug.LogError(str + ((gorillaGameManager != null) ? gorillaGameManager.ToString() : null));
					return false;
				}
				this.serializeTarget = this.gameModeInstance;
				base.transform.parent = VRRigCache.Instance.NetworkParent;
				return true;
			}
		}
		Debug.LogError("missing instantiation data");
		return false;
	}

	protected override void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
		this.photonView.AddCallbackTarget(this);
		GameMode.SetupGameModeRemote(this);
	}

	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		GameMode.RemoveNetworkLink(this);
	}

	[PunRPC]
	private void ReportTagRPC(Player taggedPlayer, PhotonMessageInfo info)
	{
		this.gameModeInstance.ReportTag(taggedPlayer, info.Sender);
	}

	[PunRPC]
	private void ReportHitRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		InfectionLavaController instance = InfectionLavaController.Instance;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) && instance != null && instance.LavaCurrentlyActivated && (instance.SurfaceCenter - rigContainer.Rig.syncPos).sqrMagnitude < 2500f && instance.LavaPlane.GetDistanceToPoint(rigContainer.Rig.syncPos) < 5f)
		{
			this.GameModeInstance.HitPlayer(info.Sender);
		}
	}

	public GameModeSerializer()
	{
	}

	private GameModeType gameModeKey;

	private GorillaGameManager gameModeInstance;
}
