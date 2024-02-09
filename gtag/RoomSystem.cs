using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class RoomSystem : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
	internal static void DeserializeLaunchProjectile(object[] projectileData, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		float num = (float)projectileData[5];
		float num2 = (float)projectileData[6];
		float num3 = (float)projectileData[7];
		float num4 = (float)projectileData[8];
		Vector3 vector = (Vector3)projectileData[0];
		Vector3 vector2 = (Vector3)projectileData[1];
		if (!(vector).IsValid() || !(vector2).IsValid() || !float.IsFinite(num) || !float.IsFinite(num2) || !float.IsFinite(num3) || !float.IsFinite(num4))
		{
			GorillaNot.instance.SendReport("invalid projectile state", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		RoomSystem.ProjectileSource projectileSource = (RoomSystem.ProjectileSource)projectileData[2];
		int num5 = (int)projectileData[3];
		bool flag = (bool)projectileData[4];
		VRRig rig = rigContainer.Rig;
		if (rig.isOfflineVRRig || rig.CheckDistance(vector, 4f))
		{
			RoomSystem.launchProjectile.targetRig = rig;
			RoomSystem.launchProjectile.position = vector;
			RoomSystem.launchProjectile.velocity = vector2;
			RoomSystem.launchProjectile.overridecolour = flag;
			RoomSystem.launchProjectile.colour = new Color(num, num2, num3, num4);
			RoomSystem.launchProjectile.projectileCount = num5;
			RoomSystem.launchProjectile.projectileSource = projectileSource;
			RoomSystem.launchProjectile.messageInfo = info;
			FXSystem.PlayFXForRig(FXType.Projectile, RoomSystem.launchProjectile, info);
		}
	}

	internal static void SendLaunchProjectile(Vector3 position, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCount, bool randomColour, float r, float g, float b, float a)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		RoomSystem.projectileSendData[0] = position;
		RoomSystem.projectileSendData[1] = velocity;
		RoomSystem.projectileSendData[2] = projectileSource;
		RoomSystem.projectileSendData[3] = projectileCount;
		RoomSystem.projectileSendData[4] = randomColour;
		RoomSystem.projectileSendData[5] = r;
		RoomSystem.projectileSendData[6] = g;
		RoomSystem.projectileSendData[7] = b;
		RoomSystem.projectileSendData[8] = a;
		byte b2 = 0;
		object obj = RoomSystem.projectileSendData;
		RoomSystem.SendEvent(b2, obj, RoomSystem.reoOthers, RoomSystem.soUnreliable);
	}

	internal static void ImpactEffect(VRRig targetRig, Vector3 position, float r, float g, float b, float a, int projectileCount, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		RoomSystem.impactEffect.targetRig = targetRig;
		RoomSystem.impactEffect.position = position;
		RoomSystem.impactEffect.colour = new Color(r, g, b, a);
		RoomSystem.impactEffect.projectileCount = projectileCount;
		FXSystem.PlayFXForRig(FXType.Impact, RoomSystem.impactEffect, info);
	}

	internal static void DeserializeImpactEffect(object[] impactData, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		float num = (float)impactData[1];
		float num2 = (float)impactData[2];
		float num3 = (float)impactData[3];
		float num4 = (float)impactData[4];
		Vector3 vector = (Vector3)impactData[0];
		if (!(vector).IsValid() || !float.IsFinite(num) || !float.IsFinite(num2) || !float.IsFinite(num3) || !float.IsFinite(num4))
		{
			GorillaNot.instance.SendReport("invalid impact state", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		int num5 = (int)impactData[5];
		RoomSystem.ImpactEffect(rigContainer.Rig, vector, num, num2, num3, num4, num5, info);
	}

	internal static void SendImpactEffect(Vector3 position, float r, float g, float b, float a, int projectileCount)
	{
		RoomSystem.ImpactEffect(VRRigCache.Instance.localRig.Rig, position, r, g, b, a, projectileCount, default(PhotonMessageInfo));
		if (RoomSystem.joinedRoom)
		{
			RoomSystem.impactSendData[0] = position;
			RoomSystem.impactSendData[1] = r;
			RoomSystem.impactSendData[2] = g;
			RoomSystem.impactSendData[3] = b;
			RoomSystem.impactSendData[4] = a;
			RoomSystem.impactSendData[5] = projectileCount;
			byte b2 = 1;
			object obj = RoomSystem.impactSendData;
			RoomSystem.SendEvent(b2, obj, RoomSystem.reoOthers, RoomSystem.soUnreliable);
		}
	}

	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
		PhotonNetwork.AddCallbackTarget(this);
		PhotonNetwork.NetworkingClient.EventReceived += RoomSystem.OnEvent;
		RoomSystem.playerImpactEffectPrefab = this.roomSettings.PlayerImpactEffect;
		RoomSystem.callbackInstance = this;
	}

	private void Start()
	{
		List<PhotonView> list = new List<PhotonView>(20);
		foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
		{
			if (photonView.IsRoomView)
			{
				list.Add(photonView);
			}
		}
		RoomSystem.sceneViews = list.ToArray();
	}

	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		RoomSystem.joinedRoom = true;
		foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
		{
			RoomSystem.playersInRoom.Add(player);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(RoomSystem.playersInRoom);
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
		{
			string text = obj as string;
			if (text != null)
			{
				RoomSystem.roomGameMode = text;
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			for (int i = 0; i < this.prefabsToInstantiateByPath.Length; i++)
			{
				this.prefabsInstantiated.Add(PhotonNetwork.InstantiateRoomObject(this.prefabsToInstantiateByPath[i], Vector3.zero, Quaternion.identity, 0, null));
			}
		}
		try
		{
			Action joinedRoomEvent = RoomSystem.JoinedRoomEvent;
			if (joinedRoomEvent != null)
			{
				joinedRoomEvent();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		RoomSystem.playersInRoom.Add(newPlayer);
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
		try
		{
			Action<Player> playerJoinedEvent = RoomSystem.PlayerJoinedEvent;
			if (playerJoinedEvent != null)
			{
				playerJoinedEvent(newPlayer);
			}
			Action playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	void IMatchmakingCallbacks.OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.joinedRoom = false;
		RoomSystem.playersInRoom.Clear();
		RoomSystem.roomGameMode = "";
		PlayerCosmeticsSystem.StaticReset();
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		for (int i = 0; i < RoomSystem.sceneViews.Length; i++)
		{
			RoomSystem.sceneViews[i].ControllerActorNr = actorNumber;
			RoomSystem.sceneViews[i].OwnerActorNr = actorNumber;
		}
		this.roomSettings.StatusEffectLimiter.Reset();
		this.roomSettings.SoundEffectLimiter.Reset();
		this.roomSettings.SoundEffectOtherLimiter.Reset();
		try
		{
			Action leftRoomEvent = RoomSystem.LeftRoomEvent;
			if (leftRoomEvent != null)
			{
				leftRoomEvent();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
		GC.Collect(0);
	}

	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
		RoomSystem.playersInRoom.Remove(otherPlayer);
		try
		{
			Action<Player> playerLeftEvent = RoomSystem.PlayerLeftEvent;
			if (playerLeftEvent != null)
			{
				playerLeftEvent(otherPlayer);
			}
			Action playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	public static List<Player> PlayersInRoom
	{
		get
		{
			return RoomSystem.playersInRoom;
		}
	}

	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	public static bool JoinedRoom
	{
		get
		{
			return PhotonNetwork.InRoom && RoomSystem.joinedRoom;
		}
	}

	public static bool AmITheHost
	{
		get
		{
			return PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom;
		}
	}

	static RoomSystem()
	{
		RoomSystem.StaticLoad();
	}

	[OnEnterPlay_Run]
	private static void StaticLoad()
	{
		RoomSystem.netEventCallbacks[0] = new Action<object[], PhotonMessageInfo>(RoomSystem.DeserializeLaunchProjectile);
		RoomSystem.netEventCallbacks[1] = new Action<object[], PhotonMessageInfo>(RoomSystem.DeserializeImpactEffect);
		RoomSystem.netEventCallbacks[4] = new Action<object[], PhotonMessageInfo>(RoomSystem.SearchForFriend);
		RoomSystem.netEventCallbacks[2] = new Action<object[], PhotonMessageInfo>(RoomSystem.DeserializeStatusEffect);
		RoomSystem.netEventCallbacks[3] = new Action<object[], PhotonMessageInfo>(RoomSystem.DeserializeSoundEffect);
		RoomSystem.netEventCallbacks[5] = new Action<object[], PhotonMessageInfo>(RoomSystem.DeserializeReportTouch);
		RoomSystem.soundEffectCallback = new Action<RoomSystem.SoundEffect, Player>(RoomSystem.OnPlaySoundEffect);
		RoomSystem.statusEffectCallback = new Action<RoomSystem.StatusEffects>(RoomSystem.OnStatusEffect);
	}

	internal static void SendEvent(in byte code, in object evData, in Player target, in SendOptions so)
	{
		RoomSystem.reoTarget.TargetActors[0] = target.ActorNumber;
		RoomSystem.SendEvent(code, evData, RoomSystem.reoTarget, so);
	}

	internal static void SendEvent(in byte code, in object evData, in RaiseEventOptions reo, in SendOptions so)
	{
		RoomSystem.sendEventData[0] = PhotonNetwork.ServerTimestamp;
		RoomSystem.sendEventData[1] = code;
		RoomSystem.sendEventData[2] = evData;
		PhotonNetwork.RaiseEvent(3, RoomSystem.sendEventData, reo, so);
	}

	private static void OnEvent(EventData data)
	{
		Player player;
		if (data.Code != 3 || !Utils.PlayerInRoom(data.Sender, out player))
		{
			return;
		}
		try
		{
			object[] array = (object[])data.CustomData;
			int num = (int)array[0];
			byte b = (byte)array[1];
			object[] array2 = null;
			if (array.Length > 2)
			{
				object obj = array[2];
				array2 = ((obj == null) ? null : ((object[])obj));
			}
			PhotonMessageInfo photonMessageInfo = new PhotonMessageInfo(player, num, null);
			Action<object[], PhotonMessageInfo> action;
			if (RoomSystem.netEventCallbacks.TryGetValue(b, out action))
			{
				action(array2, photonMessageInfo);
			}
		}
		catch
		{
		}
	}

	internal static void SearchForFriend(object[] shuffleData, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "JoinPubWithFriends");
		string text = (string)shuffleData[0];
		string text2 = (string)shuffleData[1];
		if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
		{
			PhotonNetworkController.Instance.AttemptToFollowFriendIntoPub(info.Sender.UserId, info.Sender.ActorNumber, text2, text);
			return;
		}
		GorillaNot.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
	}

	internal static void JoinPubWithFriends(GorillaFriendCollider friendCollider, string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (Player player in RoomSystem.playersInRoom)
		{
			if (friendCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
			{
				raiseEventOptions.TargetActors[0] = player.ActorNumber;
				byte b = 4;
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(b, obj, raiseEventOptions, RoomSystem.soUnreliable);
			}
		}
	}

	private static void DeserializeReportTouch(object[] data, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Player player = (Player)data[0];
		Player sender = info.Sender;
		Action<Player, Player> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(player, sender);
	}

	internal static void SendReportTouch(Player touchedPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			RoomSystem.reportTouchSendData[0] = touchedPlayer;
			byte b = 5;
			object obj = RoomSystem.reportTouchSendData;
			RoomSystem.SendEvent(b, obj, RoomSystem.reoMaster, RoomSystem.soUnreliable);
			return;
		}
		Action<Player, Player> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(touchedPlayer, PhotonNetwork.LocalPlayer);
	}

	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	private static void SetSlowedTime()
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
	}

	private static void SetTaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	private static void SetJoinedTaggedTime()
	{
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	private static void OnStatusEffect(RoomSystem.StatusEffects status)
	{
		switch (status)
		{
		case RoomSystem.StatusEffects.TaggedTime:
			RoomSystem.SetTaggedTime();
			return;
		case RoomSystem.StatusEffects.JoinedTaggedTime:
			RoomSystem.SetJoinedTaggedTime();
			return;
		case RoomSystem.StatusEffects.SetSlowedTime:
			RoomSystem.SetSlowedTime();
			return;
		default:
			return;
		}
	}

	private static void DeserializeStatusEffect(object[] data, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializeStatusEffect");
		if (!info.Sender.IsMasterClient)
		{
			GorillaNot.instance.SendReport("invalid status", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!RoomSystem.callbackInstance.roomSettings.StatusEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		RoomSystem.StatusEffects statusEffects = (RoomSystem.StatusEffects)data[0];
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(statusEffects);
	}

	internal static void SendStatusEffectAll(RoomSystem.StatusEffects status)
	{
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action != null)
		{
			action(status);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.statusSendData[0] = status;
		byte b = 2;
		object obj = RoomSystem.statusSendData;
		RoomSystem.SendEvent(b, obj, RoomSystem.reoOthers, RoomSystem.soUnreliable);
	}

	internal static void SendStatusEffectToPlayer(RoomSystem.StatusEffects status, Player target)
	{
		if (!target.IsLocal)
		{
			RoomSystem.statusSendData[0] = status;
			byte b = 2;
			object obj = RoomSystem.statusSendData;
			RoomSystem.SendEvent(b, obj, target, RoomSystem.soUnreliable);
			return;
		}
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(status);
	}

	internal static void PlaySoundEffect(int soundIndex, float soundVolume)
	{
		VRRigCache.Instance.localRig.Rig.PlayTagSoundLocal(soundIndex, soundVolume);
	}

	internal static void PlaySoundEffect(int soundIndex, float soundVolume, Player target)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(target, out rigContainer))
		{
			rigContainer.Rig.PlayTagSoundLocal(soundIndex, soundVolume);
		}
	}

	private static void OnPlaySoundEffect(RoomSystem.SoundEffect sound, Player target)
	{
		if (target.IsLocal)
		{
			RoomSystem.PlaySoundEffect(sound.id, sound.volume);
			return;
		}
		RoomSystem.PlaySoundEffect(sound.id, sound.volume, target);
	}

	private static void DeserializeSoundEffect(object[] data, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializeSoundEffect");
		if (!info.Sender.IsMasterClient)
		{
			GorillaNot.instance.SendReport("invalid sound effect", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		RoomSystem.SoundEffect soundEffect;
		soundEffect.id = (int)data[0];
		soundEffect.volume = (float)data[1];
		if (!float.IsFinite(soundEffect.volume))
		{
			return;
		}
		Player player;
		if (data.Length > 2)
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectOtherLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			player = (Player)data[2];
		}
		else
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			player = PhotonNetwork.LocalPlayer;
		}
		RoomSystem.soundEffectCallback(soundEffect, player);
	}

	internal static void SendSoundEffectAll(int soundIndex, float soundVolume)
	{
		RoomSystem.SendSoundEffectAll(new RoomSystem.SoundEffect(soundIndex, soundVolume));
	}

	internal static void SendSoundEffectAll(RoomSystem.SoundEffect sound)
	{
		Action<RoomSystem.SoundEffect, Player> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, PhotonNetwork.LocalPlayer);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.soundSendData[0] = sound.id;
		RoomSystem.soundSendData[1] = sound.volume;
		byte b = 3;
		object obj = RoomSystem.soundSendData;
		RoomSystem.SendEvent(b, obj, RoomSystem.reoOthers, RoomSystem.soUnreliable);
	}

	internal static void SendSoundEffectToPlayer(int soundIndex, float soundVolume, Player player)
	{
		RoomSystem.SendSoundEffectToPlayer(new RoomSystem.SoundEffect(soundIndex, soundVolume), player);
	}

	internal static void SendSoundEffectToPlayer(RoomSystem.SoundEffect sound, Player player)
	{
		if (player.IsLocal)
		{
			Action<RoomSystem.SoundEffect, Player> action = RoomSystem.soundEffectCallback;
			if (action == null)
			{
				return;
			}
			action(sound, player);
			return;
		}
		else
		{
			if (!RoomSystem.joinedRoom)
			{
				return;
			}
			RoomSystem.soundSendData[0] = sound.id;
			RoomSystem.soundSendData[1] = sound.volume;
			byte b = 3;
			object obj = RoomSystem.soundSendData;
			RoomSystem.SendEvent(b, obj, player, RoomSystem.soUnreliable);
			return;
		}
	}

	internal static void SendSoundEffectOnOther(int soundIndex, float soundvolume, Player target)
	{
		RoomSystem.SendSoundEffectOnOther(new RoomSystem.SoundEffect(soundIndex, soundvolume), target);
	}

	internal static void SendSoundEffectOnOther(RoomSystem.SoundEffect sound, Player target)
	{
		Action<RoomSystem.SoundEffect, Player> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, target);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.sendSoundDataOther[0] = sound.id;
		RoomSystem.sendSoundDataOther[1] = sound.volume;
		RoomSystem.sendSoundDataOther[2] = target;
		byte b = 3;
		object obj = RoomSystem.sendSoundDataOther;
		RoomSystem.SendEvent(b, obj, RoomSystem.reoOthers, RoomSystem.soUnreliable);
	}

	internal static void OnPlayerEffect(RoomSystem.PlayerEffect effect, Player target)
	{
	}

	private static void DeserializePlayerEffect(object[] data, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializePlayerEffect");
		if (!RoomSystem.callbackInstance.roomSettings.SoundEffectOtherLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		RoomSystem.PlayerEffect playerEffect = (RoomSystem.PlayerEffect)data[0];
		Player player = (Player)data[1];
		RoomSystem.OnPlayerEffect(playerEffect, player);
	}

	private static void SendPlayerEffect(RoomSystem.PlayerEffect effect, Player target)
	{
		RoomSystem.OnPlayerEffect(effect, target);
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.playerEffectData[0] = target;
		RoomSystem.playerEffectData[1] = effect;
	}

	private static RoomSystem.ImpactFxContainer impactEffect = new RoomSystem.ImpactFxContainer();

	private static RoomSystem.LaunchProjectileContainer launchProjectile = new RoomSystem.LaunchProjectileContainer();

	private static GameObject playerImpactEffectPrefab = null;

	private static readonly object[] projectileSendData = new object[9];

	private static readonly object[] impactSendData = new object[6];

	private static readonly List<int> hashValues = new List<int>(2);

	[SerializeField]
	private RoomSystemSettings roomSettings;

	[SerializeField]
	private string[] prefabsToInstantiateByPath;

	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	[OnEnterPlay_SetNull]
	private static RoomSystem callbackInstance;

	[OnEnterPlay_Clear]
	private static List<Player> playersInRoom = new List<Player>(10);

	[OnEnterPlay_Set("")]
	private static string roomGameMode = "";

	[OnEnterPlay_Set(false)]
	private static bool joinedRoom = false;

	[OnEnterPlay_SetNull]
	private static PhotonView[] sceneViews;

	[OnExitPlay_SetNull]
	public static Action LeftRoomEvent;

	[OnExitPlay_SetNull]
	public static Action JoinedRoomEvent;

	[OnExitPlay_SetNull]
	public static Action<Player> PlayerJoinedEvent;

	[OnExitPlay_SetNull]
	public static Action<Player> PlayerLeftEvent;

	[OnExitPlay_SetNull]
	public static Action PlayersChangedEvent;

	[OnExitPlay_Clear]
	internal static readonly Dictionary<byte, Action<object[], PhotonMessageInfo>> netEventCallbacks = new Dictionary<byte, Action<object[], PhotonMessageInfo>>(10);

	private static readonly object[] sendEventData = new object[3];

	private static readonly SendOptions soUnreliable = SendOptions.SendUnreliable;

	private static readonly RaiseEventOptions reoOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	private static readonly RaiseEventOptions reoMaster = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.MasterClient
	};

	private static readonly RaiseEventOptions reoTarget = new RaiseEventOptions
	{
		TargetActors = new int[1]
	};

	private static readonly object[] groupJoinSendData = new object[2];

	private static readonly object[] reportTouchSendData = new object[1];

	[OnExitPlay_SetNull]
	public static Action<Player, Player> playerTouchedCallback;

	private static object[] statusSendData = new object[1];

	public static Action<RoomSystem.StatusEffects> statusEffectCallback;

	private static object[] soundSendData = new object[2];

	private static object[] sendSoundDataOther = new object[3];

	public static Action<RoomSystem.SoundEffect, Player> soundEffectCallback;

	private static object[] playerEffectData = new object[2];

	private class ImpactFxContainer : IFXContext
	{
		public FXSystemSettings settings
		{
			get
			{
				return this.targetRig.fxSettings;
			}
		}

		public virtual void OnPlayFX()
		{
			this.targetRig.slingshot.DestroyProjectile(this.projectileCount, this.position);
			GameObject gameObject = ObjectPools.instance.Instantiate(RoomSystem.playerImpactEffectPrefab, this.position);
			gameObject.transform.localScale = Vector3.one * this.targetRig.scaleFactor;
			gameObject.GetComponent<GorillaColorizableBase>().SetColor(this.colour);
		}

		public VRRig targetRig;

		public Vector3 position;

		public Color colour;

		public int projectileCount;
	}

	private class LaunchProjectileContainer : RoomSystem.ImpactFxContainer
	{
		public override void OnPlayFX()
		{
			this.targetRig.slingshot.LaunchNetworkedProjectile(this.position, this.velocity, this.projectileSource, this.projectileCount, this.targetRig.scaleFactor, this.overridecolour, this.colour, this.messageInfo);
		}

		public Vector3 velocity;

		public RoomSystem.ProjectileSource projectileSource;

		public bool overridecolour;

		public PhotonMessageInfo messageInfo;
	}

	internal enum ProjectileSource
	{
		Slingshot,
		LeftHand,
		RightHand
	}

	private struct Events
	{
		public const byte PROJECTILE = 0;

		public const byte IMPACT = 1;

		public const byte STATUS_EFFECT = 2;

		public const byte SOUND_EFFECT = 3;

		public const byte GROUP_JOIN = 4;

		public const byte PLAYER_TOUCHED = 5;

		public const byte PLAYER_EFFECT = 6;
	}

	public enum StatusEffects
	{
		TaggedTime,
		JoinedTaggedTime,
		SetSlowedTime
	}

	public struct SoundEffect
	{
		public SoundEffect(int soundID, float soundVolume)
		{
			this.id = soundID;
			this.volume = soundVolume;
			this.volume = soundVolume;
		}

		public int id;

		public float volume;
	}

	internal enum PlayerEffect
	{

	}
}
