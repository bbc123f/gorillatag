using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
internal class RigContainer : MonoBehaviour
{
	public bool Initialized
	{
		[CompilerGenerated]
		get
		{
			return this.<Initialized>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<Initialized>k__BackingField = value;
		}
	}

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

	public bool GetIsPlayerAutoMuted()
	{
		return this.bPlayerAutoMuted;
	}

	public void UpdateAutomuteLevel(string autoMuteLevel)
	{
		if (autoMuteLevel.Equals("LOW", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 1;
		}
		else if (autoMuteLevel.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 0;
		}
		else if (autoMuteLevel.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 2;
		}
		else
		{
			this.playerChatQuality = 2;
		}
		Debug.Log(string.Concat(new string[]
		{
			"Updating Automute Level for Player ",
			this.Creator.UserId,
			" Nick ",
			this.Creator.NickName,
			" Level ",
			autoMuteLevel,
			" Chat quality is now ",
			this.playerChatQuality.ToString()
		}));
		this.RefreshVoiceChat();
	}

	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = PhotonNetwork.LocalPlayer;
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnMultiPlayerStarted;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
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

	private void OnReturnedToSinglePlayer()
	{
		MonoBehaviour.print("!! OnReturnedToSinglePlayer");
		if (this.Rig.isOfflineVRRig)
		{
			RigContainer.CancelAutomuteRequest();
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
		if (!this.vrrig.OwningNetPlayer.IsLocal)
		{
			photonView.RPC("RequestMaterialColor", photonView.Owner, new object[] { PhotonNetwork.LocalPlayer });
		}
		this.Initialized = true;
		if (!this.vrrig.isOfflineVRRig)
		{
			base.StartCoroutine(RigContainer.QueueAutomute(this.Creator));
		}
	}

	private static IEnumerator QueueAutomute(Player player)
	{
		RigContainer.playersToCheckAutomute.Add(player);
		MonoBehaviour.print(string.Format("!! QueueAutomute Player: {0}, Is Already Queued: {1}", player.UserId, RigContainer.automuteQueued));
		if (!RigContainer.automuteQueued)
		{
			MonoBehaviour.print("!!1 Begin Automute Queue");
			RigContainer.automuteQueued = true;
			yield return new WaitForSecondsRealtime(1f);
			MonoBehaviour.print("!!2 Automute Queue Delay Complete");
			while (RigContainer.waitingForAutomuteCallback)
			{
				yield return null;
			}
			MonoBehaviour.print("!!3 Automute Queue Wait Callback Complete");
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
		}
		yield break;
	}

	private static void RequestAutomuteSettings()
	{
		if (RigContainer.playersToCheckAutomute.Count == 0)
		{
			return;
		}
		RigContainer.waitingForAutomuteCallback = true;
		RigContainer.playersToCheckAutomute.RemoveAll((Player player) => player == null);
		RigContainer.requestedAutomutePlayers = new List<Player>(RigContainer.playersToCheckAutomute);
		RigContainer.playersToCheckAutomute.Clear();
		string[] array = RigContainer.requestedAutomutePlayers.Select((Player x) => x.UserId).ToArray<string>();
		foreach (Player player2 in RigContainer.requestedAutomutePlayers)
		{
			MonoBehaviour.print("!! Request UserId: " + player2.UserId);
		}
		ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
		executeFunctionRequest.Entity = new EntityKey
		{
			Id = PlayFabSettings.staticPlayer.EntityId,
			Type = PlayFabSettings.staticPlayer.EntityType
		};
		executeFunctionRequest.FunctionName = "ShouldUserAutomutePlayer";
		executeFunctionRequest.FunctionParameter = string.Join(",", array);
		PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			MonoBehaviour.print(string.Format("!!4 Execution Complete Results: {0}", result));
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				MonoBehaviour.print("!! Received results, key: " + keyValuePair.Key + ", value: " + keyValuePair.Value);
			}
			if (dictionary == null)
			{
				using (List<Player>.Enumerator enumerator3 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Player player3 = enumerator3.Current;
						if (player3 != null)
						{
							MonoBehaviour.print("!! ERROR  UserId: " + player3.UserId + ", Score: ERROR");
							RigContainer.ReceiveAutomuteSettings(player3, "none");
						}
					}
					goto IL_15B;
				}
			}
			foreach (Player player4 in RigContainer.requestedAutomutePlayers)
			{
				if (player4 != null)
				{
					string text;
					if (dictionary.TryGetValue(player4.UserId, out text))
					{
						MonoBehaviour.print("!! Score received; UserId: " + player4.UserId + ", Score: " + text);
						RigContainer.ReceiveAutomuteSettings(player4, text);
					}
					else
					{
						MonoBehaviour.print("!! No score returned; UserId: " + player4.UserId);
						RigContainer.ReceiveAutomuteSettings(player4, "none");
					}
				}
			}
			IL_15B:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, delegate(PlayFabError error)
		{
			Debug.LogError("Return message State = -1 " + error.ErrorMessage);
			MonoBehaviour.print(string.Format("!!4 Execution Error Results: {0}", error));
			foreach (Player player5 in RigContainer.requestedAutomutePlayers)
			{
				MonoBehaviour.print("!! ERROR UserId: " + player5.UserId + ", Score: ERROR");
				RigContainer.ReceiveAutomuteSettings(player5, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, null, null);
	}

	private static void CancelAutomuteRequest()
	{
		RigContainer.playersToCheckAutomute.Clear();
		RigContainer.automuteQueued = false;
		if (RigContainer.requestedAutomutePlayers != null)
		{
			RigContainer.requestedAutomutePlayers.Clear();
		}
		RigContainer.waitingForAutomuteCallback = false;
	}

	private static void ReceiveAutomuteSettings(Player player, string score)
	{
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer != null)
		{
			rigContainer.UpdateAutomuteLevel(score);
		}
	}

	private void ProcessAutomute()
	{
		int @int = PlayerPrefs.GetInt("autoMute", 1);
		this.bPlayerAutoMuted = !this.hasManualMute && this.playerChatQuality < @int;
		Debug.Log(string.Concat(new string[]
		{
			"Auto mute for Player ",
			this.Creator.NickName,
			" UserID=",
			this.Creator.UserId,
			" Has Manual Setting=",
			this.hasManualMute.ToString(),
			" AutoMuted=",
			this.bPlayerAutoMuted.ToString(),
			" Chat Quality=",
			this.playerChatQuality.ToString(),
			" AM Level=",
			@int.ToString()
		}));
	}

	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.ProcessAutomute();
		this.Voice.SpeakerInUse.enabled = !this.forceMute && this.enableVoice && !this.bPlayerAutoMuted && GorillaComputer.instance.voiceChatOn == "TRUE";
		this.replacementVoiceSource.mute = this.forceMute || !this.enableVoice || this.bPlayerAutoMuted || GorillaComputer.instance.voiceChatOn == "OFF";
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

	public RigContainer()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static RigContainer()
	{
	}

	[CompilerGenerated]
	private bool <Initialized>k__BackingField;

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

	public bool hasManualMute;

	private bool bPlayerAutoMuted;

	public int playerChatQuality = 2;

	private static List<Player> playersToCheckAutomute = new List<Player>();

	private static bool automuteQueued = false;

	private static List<Player> requestedAutomutePlayers;

	private static bool waitingForAutomuteCallback = false;

	private static RigContainer staticTempRC;

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal bool <RequestAutomuteSettings>b__52_0(Player player)
		{
			return player == null;
		}

		internal string <RequestAutomuteSettings>b__52_1(Player x)
		{
			return x.UserId;
		}

		internal void <RequestAutomuteSettings>b__52_2(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			MonoBehaviour.print(string.Format("!!4 Execution Complete Results: {0}", result));
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				MonoBehaviour.print("!! Received results, key: " + keyValuePair.Key + ", value: " + keyValuePair.Value);
			}
			if (dictionary == null)
			{
				using (List<Player>.Enumerator enumerator2 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Player player = enumerator2.Current;
						if (player != null)
						{
							MonoBehaviour.print("!! ERROR  UserId: " + player.UserId + ", Score: ERROR");
							RigContainer.ReceiveAutomuteSettings(player, "none");
						}
					}
					goto IL_15B;
				}
			}
			foreach (Player player2 in RigContainer.requestedAutomutePlayers)
			{
				if (player2 != null)
				{
					string text;
					if (dictionary.TryGetValue(player2.UserId, out text))
					{
						MonoBehaviour.print("!! Score received; UserId: " + player2.UserId + ", Score: " + text);
						RigContainer.ReceiveAutomuteSettings(player2, text);
					}
					else
					{
						MonoBehaviour.print("!! No score returned; UserId: " + player2.UserId);
						RigContainer.ReceiveAutomuteSettings(player2, "none");
					}
				}
			}
			IL_15B:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}

		internal void <RequestAutomuteSettings>b__52_3(PlayFabError error)
		{
			Debug.LogError("Return message State = -1 " + error.ErrorMessage);
			MonoBehaviour.print(string.Format("!!4 Execution Error Results: {0}", error));
			foreach (Player player in RigContainer.requestedAutomutePlayers)
			{
				MonoBehaviour.print("!! ERROR UserId: " + player.UserId + ", Score: ERROR");
				RigContainer.ReceiveAutomuteSettings(player, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}

		public static readonly RigContainer.<>c <>9 = new RigContainer.<>c();

		public static Predicate<Player> <>9__52_0;

		public static Func<Player, string> <>9__52_1;

		public static Action<ExecuteFunctionResult> <>9__52_2;

		public static Action<PlayFabError> <>9__52_3;
	}

	[CompilerGenerated]
	private sealed class <QueueAutomute>d__49 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <QueueAutomute>d__49(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			switch (this.<>1__state)
			{
			case 0:
				this.<>1__state = -1;
				RigContainer.playersToCheckAutomute.Add(player);
				MonoBehaviour.print(string.Format("!! QueueAutomute Player: {0}, Is Already Queued: {1}", player.UserId, RigContainer.automuteQueued));
				if (!RigContainer.automuteQueued)
				{
					MonoBehaviour.print("!!1 Begin Automute Queue");
					RigContainer.automuteQueued = true;
					this.<>2__current = new WaitForSecondsRealtime(1f);
					this.<>1__state = 1;
					return true;
				}
				return false;
			case 1:
				this.<>1__state = -1;
				MonoBehaviour.print("!!2 Automute Queue Delay Complete");
				break;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			if (RigContainer.waitingForAutomuteCallback)
			{
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			}
			MonoBehaviour.print("!!3 Automute Queue Wait Callback Complete");
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
			return false;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public Player player;
	}
}
