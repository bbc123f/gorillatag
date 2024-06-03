using System;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Fusion;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(35)]
internal class VRRigSerializer : GorillaWrappedSerializer<InputStruct>, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	[Networked]
	[NetworkedWeaved(0, 17)]
	public unsafe NetworkString<_16> nickName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 0) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(17, 17)]
	public unsafe NetworkString<_16> defaultName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 17);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 17) = value;
		}
	}

	[Networked]
	[NetworkedWeaved(34, 1)]
	public bool tutorialComplete
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 34);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 34, value);
		}
	}

	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (this.photonView.IsRoomView)
		{
			if (player != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player.UserId, player.NickName);
			}
			return false;
		}
		if (NetworkSystem.Instance.IsObjectRoomObject(base.gameObject))
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
			if (player2 != null)
			{
				Debug.LogWarning("creating rigs as room objects " + player2.UserId + " " + player2.NickName);
				GorillaNot.instance.SendReport("creating rigs as room objects", player2.UserId, player2.NickName);
			}
			return false;
		}
		if (((PunNetPlayer)player).playerRef != this.photonView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", player.UserId, player.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		this.rigContainer.InitializeNetwork(this.photonView, this.voiceView, this);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.photonView.AddCallbackTarget(this);
		NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
	}

	protected override void OnFailedSpawn()
	{
	}

	protected override void OnBeforeDespawn()
	{
		this.CleanUp(true);
	}

	private void CleanUp(bool netDestroy)
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		this.successfullInstantiate = false;
		if (this.vrrig != null)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.vrrig.isOfflineVRRig)
				{
					this.vrrig.ChangeMaterialLocal(0);
				}
			}
			else
			{
				if (this.vrrig.isOfflineVRRig)
				{
					NetworkSystem.Instance.NetDestroy(base.gameObject);
				}
				if (this.vrrig.photonView == this.photonView)
				{
					this.vrrig.photonView = null;
				}
				if (this.vrrig.rigSerializer == this)
				{
					this.vrrig.rigSerializer = null;
				}
			}
		}
		if (this.networkSpeaker != null)
		{
			if (netDestroy)
			{
				this.networkSpeaker.SetParent(base.transform, false);
			}
			else
			{
				this.networkSpeaker.SetParent(null);
			}
			this.networkSpeaker.gameObject.SetActive(false);
		}
		this.vrrig = null;
	}

	private void OnDisable()
	{
		this.CleanUp(false);
	}

	private void OnDestroy()
	{
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			UnityEngine.Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPC_InitializeNoobMaterial(float red, float green, float blue, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (this.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = this.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 1) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void VRRigSerializer::RPC_InitializeNoobMaterial(System.Single,System.Single,System.Single,Fusion.RpcInfo)", this.Object, 1);
				}
				else
				{
					if (this.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						num += 4;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(this.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(this.Object.Id, this.ObjectIndex, 1), data);
						ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, red);
						num2 += 4;
						ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, green);
						num2 += 4;
						ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, blue);
						num2 += 4;
						ptr->Offset = num2 * 8;
						this.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						info = RpcInfo.FromLocal(this.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, new PhotonMessageInfoWrapped(info));
	}

	[PunRPC]
	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, new PhotonMessageInfoWrapped(info));
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RequestMaterialColor(int askingPlayerID, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (this.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = this.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						if (this.Runner.HasAnyActiveConnections())
						{
							int num = 8;
							num += 4;
							SimulationMessage* ptr = SimulationMessage.Allocate(this.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(this.Object.Id, this.ObjectIndex, 2), data);
							*(int*)(data + num2) = askingPlayerID;
							num2 += 4;
							ptr->Offset = num2 * 8;
							this.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(this.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_12;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void VRRigSerializer::RPC_RequestMaterialColor(System.Int32,Fusion.RpcInfo)", this.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayerID, new PhotonMessageInfoWrapped(info));
	}

	[PunRPC]
	public void RequestMaterialColor(Player askingPlayer, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayer.ActorNumber, new PhotonMessageInfoWrapped(info));
	}

	[PunRPC]
	public void RequestCosmetics(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestCosmetics(info);
	}

	[PunRPC]
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayDrum(drumIndex, drumVolume, info);
	}

	[PunRPC]
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All, InvokeLocal = false)]
	public unsafe void RPC_PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, RpcInfo info = default(RpcInfo))
	{
		if (this.InvokeRpc)
		{
			this.InvokeRpc = false;
			GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "RPC_PlayHandTap");
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Max(tapVolume, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, default(PhotonMessageInfo));
			return;
		}
		NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
		if (this.Runner.Stage != SimulationStages.Resimulate)
		{
			int localAuthorityMask = this.Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void VRRigSerializer::RPC_PlayHandTap(System.Int32,System.Boolean,System.Single,Fusion.RpcInfo)", this.Object, 1);
			}
			else if (this.Runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += 4;
				num += 4;
				SimulationMessage* ptr = SimulationMessage.Allocate(this.Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(this.Object.Id, this.ObjectIndex, 3), data);
				*(int*)(data + num2) = soundIndex;
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isLeftHand);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, tapVolume);
				num2 += 4;
				ptr->Offset = num2 * 8;
				this.Runner.SendRpc(ptr);
			}
		}
	}

	[PunRPC]
	public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		if (info.Sender == this.photonView.Owner && float.IsFinite(tapVolume))
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Clamp(tapVolume, 0f, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, info);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent hand tap", info.Sender.UserId, info.Sender.NickName);
	}

	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	[PunRPC]
	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmetics(currentItems, info);
	}

	[PunRPC]
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithTryon(currentItems, tryOnItems, info);
	}

	[PunRPC]
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	[PunRPC]
	public void PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "PlayGeodeEffect");
		if (info.Sender == this.photonView.Owner && hitPosition.IsValid())
		{
			this.geoSoundArg.position = hitPosition;
			FXSystem.PlayFX<GeoSoundArg>(FXType.PlayHandTap, this, this.geoSoundArg, info);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
	}

	void IFXContextParems<GeoSoundArg>.OnPlayFX(GeoSoundArg parems)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(parems.position);
	}

	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.EnableNonCosmeticHandItemRPC(enable, isLeftHand, info);
	}

	public VRRigSerializer()
	{
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.nickName = this._nickName;
		this.defaultName = this._defaultName;
		this.tutorialComplete = this._tutorialComplete;
	}

	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._nickName = this.nickName;
		this._defaultName = this.defaultName;
		this._tutorialComplete = this.tutorialComplete;
	}

	[NetworkRpcWeavedInvoker(1, 1, 7)]
	[Preserve]
	protected unsafe static void RPC_InitializeNoobMaterial@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float red = num2;
		float num3 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float green = num3;
		float num4 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float blue = num4;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((VRRigSerializer)behaviour).RPC_InitializeNoobMaterial(red, green, blue, info);
	}

	[NetworkRpcWeavedInvoker(2, 7, 1)]
	[Preserve]
	protected unsafe static void RPC_RequestMaterialColor@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int askingPlayerID = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((VRRigSerializer)behaviour).RPC_RequestMaterialColor(askingPlayerID, info);
	}

	[NetworkRpcWeavedInvoker(3, 1, 7)]
	[Preserve]
	protected unsafe static void RPC_PlayHandTap@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int soundIndex = num2;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isLeftHand = flag;
		float num3 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float tapVolume = num3;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((VRRigSerializer)behaviour).RPC_PlayHandTap(soundIndex, isLeftHand, tapVolume, info);
	}

	[SerializeField]
	[DefaultForProperty("nickName", 0, 17)]
	private NetworkString<_16> _nickName;

	[SerializeField]
	[DefaultForProperty("defaultName", 17, 17)]
	private NetworkString<_16> _defaultName;

	[SerializeField]
	[DefaultForProperty("tutorialComplete", 34, 1)]
	private bool _tutorialComplete;

	[SerializeField]
	private PhotonVoiceView voiceView;

	[SerializeField]
	private VoiceNetworkObject fusionVoiceView;

	public Transform networkSpeaker;

	[SerializeField]
	private VRRig vrrig;

	private RigContainer rigContainer;

	private HandTapArgs handTapArgs = new HandTapArgs();

	private GeoSoundArg geoSoundArg = new GeoSoundArg();

	new static Changed<VRRigSerializer> $IL2CPP_CHANGED;

	new static ChangedDelegate<VRRigSerializer> $IL2CPP_CHANGED_DELEGATE;

	new static NetworkBehaviourCallbacks<VRRigSerializer> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;
}
