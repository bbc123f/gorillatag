using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

[RequireComponent(typeof(PhotonVoiceView))]
internal class VRRigSerializer : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	[SerializeField]
	private PhotonVoiceView voiceView;

	public Transform networkSpeaker;

	[SerializeField]
	private VRRig vrrig;

	private RigContainer rigContainer;

	protected override bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (photonView.IsRoomView)
		{
			if (info.Sender != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", info.Sender.UserId, info.Sender.NickName);
			}
			return false;
		}
		if (info.Sender != photonView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", info.Sender.UserId, info.Sender.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			vrrig = rigContainer.Rig;
			return true;
		}
		return false;
	}

	protected override void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
		rigContainer.InitializeNetwork(photonView, voiceView);
		networkSpeaker.SetParent(rigContainer.SpeakerHead, worldPositionStays: false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, worldPositionStays: true);
		photonView.AddCallbackTarget(this);
	}

	private void CleanUp(bool netDestroy)
	{
		if (!successfullInstantiate)
		{
			return;
		}
		successfullInstantiate = false;
		if (vrrig != null)
		{
			if (!PhotonNetwork.InRoom)
			{
				if (vrrig.isOfflineVRRig)
				{
					vrrig.ChangeMaterialLocal(0);
				}
			}
			else
			{
				if (vrrig.isOfflineVRRig)
				{
					PhotonNetwork.Destroy(photonView);
				}
				if ((object)vrrig.photonView == photonView)
				{
					vrrig.photonView = null;
				}
			}
		}
		if (networkSpeaker != null)
		{
			if (netDestroy)
			{
				networkSpeaker.SetParent(base.transform, worldPositionStays: false);
			}
			else
			{
				networkSpeaker.SetParent(null);
			}
			networkSpeaker.gameObject.SetActive(value: false);
		}
		vrrig = null;
	}

	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		CleanUp(netDestroy: true);
	}

	private void OnDisable()
	{
		CleanUp(netDestroy: false);
	}

	private void OnDestroy()
	{
		if (networkSpeaker != null && networkSpeaker.parent != base.transform)
		{
			UnityEngine.Object.Destroy(networkSpeaker.gameObject);
		}
	}

	[PunRPC]
	public void InitializeNoobMaterial(float red, float green, float blue, bool leftHanded, PhotonMessageInfo info)
	{
		vrrig?.InitializeNoobMaterial(red, green, blue, leftHanded, info);
	}

	[PunRPC]
	public void SetTaggedTime(PhotonMessageInfo info)
	{
		vrrig?.SetTaggedTime(info);
	}

	[PunRPC]
	public void SetSlowedTime(PhotonMessageInfo info)
	{
		vrrig?.SetSlowedTime(info);
	}

	[PunRPC]
	public void SetJoinTaggedTime(PhotonMessageInfo info)
	{
		vrrig?.SetJoinTaggedTime(info);
	}

	[PunRPC]
	public void RequestMaterialColor(Player askingPlayer, bool noneBool, PhotonMessageInfo info)
	{
		vrrig?.RequestMaterialColor(askingPlayer, noneBool, info);
	}

	[PunRPC]
	public void RequestCosmetics(PhotonMessageInfo info)
	{
		vrrig?.RequestCosmetics(info);
	}

	[PunRPC]
	public void PlayTagSound(int soundIndex, float soundVolume, PhotonMessageInfo info)
	{
		vrrig?.PlayTagSound(soundIndex, soundVolume, info);
	}

	[PunRPC]
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		vrrig?.PlayDrum(drumIndex, drumVolume, info);
	}

	[PunRPC]
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		vrrig?.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	[PunRPC]
	public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info)
	{
		vrrig?.PlayHandTap(soundIndex, isLeftHand, tapVolume, info);
	}

	[PunRPC]
	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
		vrrig?.UpdateCosmetics(currentItems, info);
	}

	[PunRPC]
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
		vrrig?.UpdateCosmeticsWithTryon(currentItems, tryOnItems, info);
	}

	[PunRPC]
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		vrrig?.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	[PunRPC]
	public void PlaySlamEffects(Vector3 slamPosition, Quaternion slamRotation, PhotonMessageInfo info)
	{
		vrrig?.PlaySlamEffects(slamPosition, slamRotation, info);
	}

	[PunRPC]
	public void PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		vrrig?.PlayGeodeEffect(hitPosition, info);
	}
}
