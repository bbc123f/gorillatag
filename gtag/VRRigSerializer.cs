using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

[RequireComponent(typeof(PhotonVoiceView))]
internal class VRRigSerializer : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	protected override bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (this.photonView.IsRoomView)
		{
			if (info.Sender != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", info.Sender.UserId, info.Sender.NickName);
			}
			return false;
		}
		if (info.Sender != this.photonView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", info.Sender.UserId, info.Sender.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	protected override void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
		this.rigContainer.InitializeNetwork(this.photonView, this.voiceView);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.photonView.AddCallbackTarget(this);
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
			if (!PhotonNetwork.InRoom)
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
					PhotonNetwork.Destroy(this.photonView);
				}
				if (this.vrrig.photonView == this.photonView)
				{
					this.vrrig.photonView = null;
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

	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.CleanUp(true);
	}

	private void OnDisable()
	{
		this.CleanUp(false);
	}

	private void OnDestroy()
	{
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	[PunRPC]
	public void InitializeNoobMaterial(float red, float green, float blue, bool leftHanded, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, leftHanded, info);
	}

	[PunRPC]
	public void RequestMaterialColor(Player askingPlayer, bool noneBool, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayer, noneBool, info);
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

	[PunRPC]
	public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "PlayHandTap");
		if (info.Sender == this.photonView.Owner)
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Max(tapVolume, 0.1f);
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
		if (info.Sender == this.photonView.Owner)
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

	[SerializeField]
	private PhotonVoiceView voiceView;

	public Transform networkSpeaker;

	[SerializeField]
	private VRRig vrrig;

	private RigContainer rigContainer;

	private HandTapArgs handTapArgs = new HandTapArgs();

	private GeoSoundArg geoSoundArg = new GeoSoundArg();
}
