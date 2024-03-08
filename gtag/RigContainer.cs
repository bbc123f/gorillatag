using System;
using GorillaNetworking;
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

	public AudioSource ReplacementVoiceSource
	{
		get
		{
			return this.replacementVoiceSource;
		}
	}

	public PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
		set
		{
			if (value == this.voiceView)
			{
				return;
			}
			if (this.voiceView != null)
			{
				this.voiceView.SpeakerInUse.enabled = false;
			}
			this.voiceView = value;
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

	public NetPlayer CreatorWrapped
	{
		get
		{
			return this.vrrig.creatorWrapped;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creatorWrapped != null && !this.vrrig.creatorWrapped.InRoom))
			{
				return;
			}
			this.vrrig.creatorWrapped = value;
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
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnMultiPlayerStarted;
		}
		this.Rig.rigContainer = this;
	}

	private void OnMultiPlayerStarted()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creatorWrapped = NetworkSystem.Instance.GetLocalPlayer();
		}
	}

	private void OnDisable()
	{
		this.Initialized = false;
		this.enableVoice = true;
		this.voiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
		this.forceMute = false;
	}

	internal void InitializeNetwork(PhotonView photonView, PhotonVoiceView voiceView, VRRigSerializer vrRigSerializer)
	{
		if (!photonView || !voiceView)
		{
			return;
		}
		this.InitializeNetwork_Shared(photonView, vrRigSerializer);
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	private void InitializeNetwork_Shared(PhotonView photonView, VRRigSerializer vrRigSerializer)
	{
		if (this.vrrig.photonView)
		{
			Debug.Log("Skipping report photon view is not null onm this rig during Init Network");
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
		this.vrrig.rigSerializer = vrRigSerializer;
		this.vrrig.OwningNetPlayer = NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject));
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && NetworkSystem.Instance.IsMasterClient)
			{
				int owningPlayerID = NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject);
				bool playerTutorialCompletion = NetworkSystem.Instance.GetPlayerTutorialCompletion(owningPlayerID);
				GorillaGameManager.instance.NewVRRig(photonView.Owner, photonView.ViewID, playerTutorialCompletion);
			}
		}
		Debug.Log("Running MAT REQEST RPC");
		photonView.RPC("RequestMaterialColor", photonView.Owner, new object[] { PhotonNetwork.LocalPlayer });
		this.Initialized = true;
	}

	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.Voice.SpeakerInUse.enabled = !this.forceMute && this.enableVoice && GorillaComputer.instance.voiceChatOn == "TRUE";
		this.replacementVoiceSource.mute = this.forceMute || !this.enableVoice || GorillaComputer.instance.voiceChatOn == "OFF";
	}

	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!NetworkSystem.Instance.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out RigContainer.staticTempRC))
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

	[SerializeField]
	private AudioSource replacementVoiceSource;

	private PhotonVoiceView voiceView;

	private bool enableVoice = true;

	private bool forceMute;

	private static RigContainer staticTempRC;
}
