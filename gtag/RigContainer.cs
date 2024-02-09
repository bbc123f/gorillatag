using System;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
internal class RigContainer : MonoBehaviour
{
	public bool Initialized { get; private set; }

	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	public PhotonVoiceView Voice
	{
		get
		{
			return this.photonVoiceView;
		}
		set
		{
			if (value == this.photonVoiceView)
			{
				return;
			}
			if (this.photonVoiceView != null)
			{
				this.photonVoiceView.SpeakerInUse.enabled = false;
			}
			this.photonVoiceView = value;
			this.RefreshVoiceChat();
		}
	}

	public PhotonView photonView
	{
		get
		{
			return this.vrrig.photonView;
		}
	}

	public bool Muted
	{
		get
		{
			return !this.enableVoice;
		}
		set
		{
			this.enableVoice = !value;
			this.RefreshVoiceChat();
		}
	}

	public Player Creator
	{
		get
		{
			return this.vrrig.creator;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creator != null && !this.vrrig.creator.IsInactive))
			{
				return;
			}
			this.vrrig.creator = value;
		}
	}

	public bool ForceMute
	{
		get
		{
			return this.forceMute;
		}
		set
		{
			this.forceMute = value;
			this.RefreshVoiceChat();
		}
	}

	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = PhotonNetwork.LocalPlayer;
		}
	}

	private void OnDisable()
	{
		this.Initialized = false;
		this.enableVoice = true;
		this.photonVoiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
		this.forceMute = false;
	}

	public void InitializeNetwork(PhotonView photonView, PhotonVoiceView voiceView)
	{
		if (!photonView || !voiceView || GTAppState.isQuitting)
		{
			return;
		}
		if (this.vrrig.photonView)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", this.Creator.UserId, this.Creator.NickName);
			if (this.vrrig.photonView.IsMine)
			{
				PhotonNetwork.Destroy(this.vrrig.photonView);
			}
			else
			{
				this.vrrig.photonView.gameObject.SetActive(false);
			}
		}
		this.vrrig.photonView = photonView;
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && PhotonNetwork.IsMasterClient)
			{
				object obj;
				bool flag = photonView.Owner.CustomProperties.TryGetValue("didTutorial", out obj) && !(bool)obj;
				GorillaGameManager.instance.NewVRRig(photonView.Owner, photonView.ViewID, flag);
			}
			photonView.RPC("RequestMaterialColor", photonView.Owner, new object[]
			{
				PhotonNetwork.LocalPlayer,
				false
			});
			this.Initialized = true;
		}
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	public void RefreshVoiceChat()
	{
		if (this.photonVoiceView == null)
		{
			return;
		}
		this.photonVoiceView.SpeakerInUse.enabled = !this.forceMute && this.enableVoice && GorillaComputer.instance.voiceChatOn == "TRUE";
	}

	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!PhotonNetwork.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (VRRigCache.Instance.TryGetVrrig(player, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

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
}
