using System;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x02000160 RID: 352
[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
internal class RigContainer : MonoBehaviour
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060008A0 RID: 2208 RVA: 0x00035117 File Offset: 0x00033317
	// (set) Token: 0x060008A1 RID: 2209 RVA: 0x0003511F File Offset: 0x0003331F
	public bool Initialized { get; private set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060008A2 RID: 2210 RVA: 0x00035128 File Offset: 0x00033328
	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00035130 File Offset: 0x00033330
	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060008A4 RID: 2212 RVA: 0x00035138 File Offset: 0x00033338
	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060008A5 RID: 2213 RVA: 0x00035140 File Offset: 0x00033340
	// (set) Token: 0x060008A6 RID: 2214 RVA: 0x00035148 File Offset: 0x00033348
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

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00035185 File Offset: 0x00033385
	public PhotonView photonView
	{
		get
		{
			return this.vrrig.photonView;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060008A8 RID: 2216 RVA: 0x00035192 File Offset: 0x00033392
	// (set) Token: 0x060008A9 RID: 2217 RVA: 0x0003519D File Offset: 0x0003339D
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

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060008AA RID: 2218 RVA: 0x000351AF File Offset: 0x000333AF
	// (set) Token: 0x060008AB RID: 2219 RVA: 0x000351BC File Offset: 0x000333BC
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

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060008AC RID: 2220 RVA: 0x000351F7 File Offset: 0x000333F7
	// (set) Token: 0x060008AD RID: 2221 RVA: 0x000351FF File Offset: 0x000333FF
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

	// Token: 0x060008AE RID: 2222 RVA: 0x0003520E File Offset: 0x0003340E
	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = PhotonNetwork.LocalPlayer;
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00035230 File Offset: 0x00033430
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

	// Token: 0x060008B0 RID: 2224 RVA: 0x000352BC File Offset: 0x000334BC
	public void InitializeNetwork(PhotonView photonView, PhotonVoiceView voiceView)
	{
		if (!photonView || !voiceView)
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
			if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<PhotonView>().IsMine)
			{
				object obj;
				bool didTutorial = photonView.Owner.CustomProperties.TryGetValue("didTutorial", out obj) && !(bool)obj;
				GorillaGameManager.instance.NewVRRig(photonView.Owner, photonView.ViewID, didTutorial);
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

	// Token: 0x060008B1 RID: 2225 RVA: 0x0003541C File Offset: 0x0003361C
	public void RefreshVoiceChat()
	{
		if (this.photonVoiceView == null)
		{
			return;
		}
		this.photonVoiceView.SpeakerInUse.enabled = (!this.forceMute && this.enableVoice && GorillaComputer.instance.voiceChatOn == "TRUE");
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00035474 File Offset: 0x00033674
	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!PhotonNetwork.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (Player targetPlayer in PhotonNetwork.PlayerListOthers)
		{
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

	// Token: 0x04000AD8 RID: 2776
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04000AD9 RID: 2777
	[SerializeField]
	private VRRigReliableState reliableState;

	// Token: 0x04000ADA RID: 2778
	[SerializeField]
	private Transform speakerHead;

	// Token: 0x04000ADB RID: 2779
	private PhotonVoiceView photonVoiceView;

	// Token: 0x04000ADC RID: 2780
	private bool enableVoice = true;

	// Token: 0x04000ADD RID: 2781
	private bool forceMute;

	// Token: 0x04000ADE RID: 2782
	private static RigContainer staticTempRC;
}
