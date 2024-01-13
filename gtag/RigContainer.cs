using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
internal class RigContainer : MonoBehaviour
{
	[SerializeField]
	private VRRig vrrig;

	[SerializeField]
	private VRRigReliableState reliableState;

	[SerializeField]
	private Transform speakerHead;

	private PhotonVoiceView photonVoiceView;

	private bool enableVoice = true;

	private bool forceMute;

	private static RigContainer staticTempRC;

	public bool Initialized { get; private set; }

	public VRRig Rig => vrrig;

	public VRRigReliableState ReliableState => reliableState;

	public Transform SpeakerHead => speakerHead;

	public PhotonVoiceView Voice
	{
		get
		{
			return photonVoiceView;
		}
		set
		{
			if (!(value == photonVoiceView))
			{
				if (photonVoiceView != null)
				{
					photonVoiceView.SpeakerInUse.enabled = false;
				}
				photonVoiceView = value;
				RefreshVoiceChat();
			}
		}
	}

	public bool Muted
	{
		get
		{
			return !enableVoice;
		}
		set
		{
			enableVoice = !value;
			RefreshVoiceChat();
		}
	}

	public Player Creator
	{
		get
		{
			return vrrig.creator;
		}
		set
		{
			if (!vrrig.isOfflineVRRig && (vrrig.creator == null || vrrig.creator.IsInactive))
			{
				vrrig.creator = value;
			}
		}
	}

	public bool ForceMute
	{
		get
		{
			return forceMute;
		}
		set
		{
			forceMute = value;
			RefreshVoiceChat();
		}
	}

	private void Start()
	{
		if (Rig.isOfflineVRRig)
		{
			vrrig.creator = PhotonNetwork.LocalPlayer;
		}
	}

	private void OnDisable()
	{
		Initialized = false;
		enableVoice = true;
		photonVoiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		vrrig.syncPos = base.gameObject.transform.position;
		vrrig.syncRotation = base.gameObject.transform.rotation;
		forceMute = false;
	}

	public void InitializeNetwork(PhotonView photonView, PhotonVoiceView voiceView)
	{
		if (!photonView || !voiceView)
		{
			return;
		}
		if ((bool)vrrig.photonView)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", Creator.UserId, Creator.NickName);
			if (vrrig.photonView.IsMine)
			{
				PhotonNetwork.Destroy(vrrig.photonView);
			}
			else
			{
				vrrig.photonView.gameObject.SetActive(value: false);
			}
		}
		vrrig.photonView = photonView;
		if (!Initialized)
		{
			vrrig.NetInitialize();
			if (GorillaGameManager.instance != null)
			{
				photonView.RPC("RequestCosmetics", photonView.Owner);
				if (GorillaGameManager.instance.GetComponent<PhotonView>().IsMine)
				{
					object value;
					bool didTutorial = photonView.Owner.CustomProperties.TryGetValue("didTutorial", out value) && !(bool)value;
					GorillaGameManager.instance.NewVRRig(photonView.Owner, photonView.ViewID, didTutorial);
				}
			}
			photonView.RPC("RequestMaterialColor", photonView.Owner, PhotonNetwork.LocalPlayer, false);
			Initialized = true;
		}
		Voice = voiceView;
		vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	public void RefreshVoiceChat()
	{
		if (!(photonVoiceView == null))
		{
			photonVoiceView.SpeakerInUse.enabled = !forceMute && enableVoice && GorillaComputer.instance.voiceChatOn == "TRUE";
		}
	}

	public static void RefreshAllRigVoices()
	{
		staticTempRC = null;
		if (!PhotonNetwork.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Player targetPlayer in playerListOthers)
		{
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out staticTempRC))
			{
				staticTempRC.RefreshVoiceChat();
			}
		}
	}
}
