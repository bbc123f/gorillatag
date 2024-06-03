using System;
using GorillaTagScripts;
using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		private void Start()
		{
			if (this.primaryTriggerForMyZone == null)
			{
				this.primaryTriggerForMyZone = this;
			}
			PhotonNetworkController.Instance.RegisterJoinTrigger(this);
			if (!this.didRegisterForCallbacks && this.ui != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZone>(this.OnGroupPositionsChanged));
			}
		}

		public void RegisterUI(JoinTriggerUI ui)
		{
			this.ui = ui;
			if (!this.didRegisterForCallbacks && FriendshipGroupDetection.Instance != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZone>(this.OnGroupPositionsChanged));
			}
			this.UpdateUI();
		}

		public void UnregisterUI(JoinTriggerUI ui)
		{
			this.ui = null;
		}

		private void OnDestroy()
		{
			if (this.didRegisterForCallbacks)
			{
				FriendshipGroupDetection.Instance.RemoveGroupZoneCallback(new Action<GroupJoinZone>(this.OnGroupPositionsChanged));
			}
		}

		private void OnGroupPositionsChanged(GroupJoinZone groupZone)
		{
			this.UpdateUI();
		}

		public void UpdateUI()
		{
			if (this.ui == null)
			{
				return;
			}
			if (GorillaScoreboardTotalUpdater.instance.offlineTextErrorString != null)
			{
				this.ui.SetState(JoinTriggerVisualState.ConnectionError, new Func<string>(this.GetCurrentRoomName), new Func<string>(this.GetRoomName));
				return;
			}
			if (PhotonNetworkController.Instance.currentJoinTrigger == this.primaryTriggerForMyZone)
			{
				this.ui.SetState(JoinTriggerVisualState.AlreadyInRoom, new Func<string>(this.GetCurrentRoomName), new Func<string>(this.GetRoomName));
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.ui.SetState(JoinTriggerVisualState.InPrivateRoom, new Func<string>(this.GetCurrentRoomName), new Func<string>(this.GetRoomName));
				return;
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				this.ui.SetState(this.CanPartyJoin() ? JoinTriggerVisualState.LeaveRoomAndPartyJoin : JoinTriggerVisualState.AbandonPartyAndSoloJoin, new Func<string>(this.GetCurrentRoomName), new Func<string>(this.GetRoomName));
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				this.ui.SetState(JoinTriggerVisualState.NotConnectedSoloJoin, new Func<string>(this.GetCurrentRoomName), new Func<string>(this.GetRoomName));
				return;
			}
			this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndSoloJoin, new Func<string>(this.GetCurrentRoomName), new Func<string>(this.GetRoomName));
		}

		private string GetCurrentRoomName()
		{
			return PhotonNetworkController.Instance.currentJoinTrigger.gameModeName.ToUpper();
		}

		private string GetRoomName()
		{
			return this.gameModeName.ToUpper();
		}

		public bool CanPartyJoin()
		{
			return this.CanPartyJoin(FriendshipGroupDetection.Instance.partyZone);
		}

		public bool CanPartyJoin(GroupJoinZone zone)
		{
			return (this.groupJoinRequiredZones & zone) == zone;
		}

		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			if (NetworkSystem.Instance.groupJoinInProgress)
			{
				return;
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (this.ignoredIfInParty)
				{
					return;
				}
				if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting || NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					return;
				}
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.GameModeString.Contains(this.gameModeName))
					{
						Debug.Log("JoinTrigger: Ignoring party join/leave because " + this.gameModeName + " is already the game mode");
						return;
					}
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						Debug.Log("JoinTrigger: Ignoring party join/leave because we're in a private room");
						return;
					}
				}
				if (this.CanPartyJoin())
				{
					Debug.Log(string.Format("JoinTrigger: Attempting party join in 1 second! <{0}> accepts <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
					PhotonNetworkController.Instance.DeferJoining(1f);
					FriendshipGroupDetection.Instance.SendAboutToGroupJoin();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.JoinWithParty);
					return;
				}
				Debug.Log(string.Format("JoinTrigger: LeaveGroup: Leaving party and will solo join, wanted <{0}> but got <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
				FriendshipGroupDetection.Instance.LeaveParty();
				PhotonNetworkController.Instance.DeferJoining(1f);
			}
			else
			{
				Debug.Log("JoinTrigger: Solo join (not in a group)");
				PhotonNetworkController.Instance.ClearDeferredJoin();
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo);
		}

		public GorillaNetworkJoinTrigger()
		{
		}

		public GameObject[] makeSureThisIsDisabled;

		public GameObject[] makeSureThisIsEnabled;

		public GTZone zone;

		public GroupJoinZone groupJoinRequiredZones;

		public string gameModeName;

		public string componentTypeToAdd;

		public GameObject componentTarget;

		public GorillaFriendCollider myCollider;

		public GorillaNetworkJoinTrigger primaryTriggerForMyZone;

		public bool ignoredIfInParty;

		private JoinTriggerUI ui;

		private bool didRegisterForCallbacks;
	}
}
