using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x02000164 RID: 356
[RequireComponent(typeof(PhotonVoiceView))]
internal class VRRigSerializer : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback, IFXContextParems<HandTapArgs>
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060008CA RID: 2250 RVA: 0x000359CC File Offset: 0x00033BCC
	FXSystemSettings IFXContextParems<HandTapArgs>.settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x000359DC File Offset: 0x00033BDC
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

	// Token: 0x060008CC RID: 2252 RVA: 0x00035AB0 File Offset: 0x00033CB0
	protected override void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
		this.rigContainer.InitializeNetwork(this.photonView, this.voiceView);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.photonView.AddCallbackTarget(this);
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00035B10 File Offset: 0x00033D10
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

	// Token: 0x060008CE RID: 2254 RVA: 0x00035BDD File Offset: 0x00033DDD
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.CleanUp(true);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00035BE6 File Offset: 0x00033DE6
	private void OnDisable()
	{
		this.CleanUp(false);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00035BEF File Offset: 0x00033DEF
	private void OnDestroy()
	{
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x00035C27 File Offset: 0x00033E27
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

	// Token: 0x060008D2 RID: 2258 RVA: 0x00035C40 File Offset: 0x00033E40
	[PunRPC]
	public void SetTaggedTime(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetTaggedTime(info);
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00035C53 File Offset: 0x00033E53
	[PunRPC]
	public void SetSlowedTime(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetSlowedTime(info);
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00035C66 File Offset: 0x00033E66
	[PunRPC]
	public void SetJoinTaggedTime(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetJoinTaggedTime(info);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x00035C79 File Offset: 0x00033E79
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

	// Token: 0x060008D6 RID: 2262 RVA: 0x00035C8E File Offset: 0x00033E8E
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

	// Token: 0x060008D7 RID: 2263 RVA: 0x00035CA1 File Offset: 0x00033EA1
	[PunRPC]
	public void PlayTagSound(int soundIndex, float soundVolume, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayTagSound(soundIndex, soundVolume, info);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00035CB6 File Offset: 0x00033EB6
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

	// Token: 0x060008D9 RID: 2265 RVA: 0x00035CCB File Offset: 0x00033ECB
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

	// Token: 0x060008DA RID: 2266 RVA: 0x00035CE4 File Offset: 0x00033EE4
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

	// Token: 0x060008DB RID: 2267 RVA: 0x00035D78 File Offset: 0x00033F78
	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00035D97 File Offset: 0x00033F97
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

	// Token: 0x060008DD RID: 2269 RVA: 0x00035DAB File Offset: 0x00033FAB
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

	// Token: 0x060008DE RID: 2270 RVA: 0x00035DC0 File Offset: 0x00033FC0
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

	// Token: 0x060008DF RID: 2271 RVA: 0x00035DDD File Offset: 0x00033FDD
	[PunRPC]
	public void PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(hitPosition, info);
	}

	// Token: 0x04000AEA RID: 2794
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x04000AEB RID: 2795
	public Transform networkSpeaker;

	// Token: 0x04000AEC RID: 2796
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04000AED RID: 2797
	private RigContainer rigContainer;

	// Token: 0x04000AEE RID: 2798
	private HandTapArgs handTapArgs = new HandTapArgs();
}
