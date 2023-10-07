using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x02000163 RID: 355
[RequireComponent(typeof(PhotonVoiceView))]
internal class VRRigSerializer : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x060008C8 RID: 2248 RVA: 0x00035B84 File Offset: 0x00033D84
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

	// Token: 0x060008C9 RID: 2249 RVA: 0x00035C58 File Offset: 0x00033E58
	protected override void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
		this.rigContainer.InitializeNetwork(this.photonView, this.voiceView);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.photonView.AddCallbackTarget(this);
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00035CB8 File Offset: 0x00033EB8
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

	// Token: 0x060008CB RID: 2251 RVA: 0x00035D85 File Offset: 0x00033F85
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.CleanUp(true);
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x00035D8E File Offset: 0x00033F8E
	private void OnDisable()
	{
		this.CleanUp(false);
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00035D97 File Offset: 0x00033F97
	private void OnDestroy()
	{
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00035DCF File Offset: 0x00033FCF
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

	// Token: 0x060008CF RID: 2255 RVA: 0x00035DE8 File Offset: 0x00033FE8
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

	// Token: 0x060008D0 RID: 2256 RVA: 0x00035DFB File Offset: 0x00033FFB
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

	// Token: 0x060008D1 RID: 2257 RVA: 0x00035E0E File Offset: 0x0003400E
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

	// Token: 0x060008D2 RID: 2258 RVA: 0x00035E21 File Offset: 0x00034021
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

	// Token: 0x060008D3 RID: 2259 RVA: 0x00035E36 File Offset: 0x00034036
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

	// Token: 0x060008D4 RID: 2260 RVA: 0x00035E49 File Offset: 0x00034049
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

	// Token: 0x060008D5 RID: 2261 RVA: 0x00035E5E File Offset: 0x0003405E
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

	// Token: 0x060008D6 RID: 2262 RVA: 0x00035E73 File Offset: 0x00034073
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

	// Token: 0x060008D7 RID: 2263 RVA: 0x00035E8A File Offset: 0x0003408A
	[PunRPC]
	public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayHandTap(soundIndex, isLeftHand, tapVolume, info);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00035EA1 File Offset: 0x000340A1
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

	// Token: 0x060008D9 RID: 2265 RVA: 0x00035EB5 File Offset: 0x000340B5
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

	// Token: 0x060008DA RID: 2266 RVA: 0x00035ECA File Offset: 0x000340CA
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

	// Token: 0x060008DB RID: 2267 RVA: 0x00035EE7 File Offset: 0x000340E7
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

	// Token: 0x04000AE7 RID: 2791
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x04000AE8 RID: 2792
	public Transform networkSpeaker;

	// Token: 0x04000AE9 RID: 2793
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04000AEA RID: 2794
	private RigContainer rigContainer;
}
