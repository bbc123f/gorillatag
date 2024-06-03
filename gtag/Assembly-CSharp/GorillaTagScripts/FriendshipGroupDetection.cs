using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTagScripts
{
	public class FriendshipGroupDetection : MonoBehaviourPunCallbacks
	{
		public static FriendshipGroupDetection Instance
		{
			[CompilerGenerated]
			get
			{
				return FriendshipGroupDetection.<Instance>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				FriendshipGroupDetection.<Instance>k__BackingField = value;
			}
		}

		public List<Color> myBeadColors
		{
			[CompilerGenerated]
			get
			{
				return this.<myBeadColors>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<myBeadColors>k__BackingField = value;
			}
		} = new List<Color>();

		public Color myBraceletColor
		{
			[CompilerGenerated]
			get
			{
				return this.<myBraceletColor>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<myBraceletColor>k__BackingField = value;
			}
		}

		public int MyBraceletSelfIndex
		{
			[CompilerGenerated]
			get
			{
				return this.<MyBraceletSelfIndex>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<MyBraceletSelfIndex>k__BackingField = value;
			}
		}

		public List<string> PartyMemberIDs
		{
			get
			{
				return this.myPartyMemberIDs;
			}
		}

		public bool IsInParty
		{
			get
			{
				return this.myPartyMemberIDs != null;
			}
		}

		public GroupJoinZone partyZone
		{
			[CompilerGenerated]
			get
			{
				return this.<partyZone>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<partyZone>k__BackingField = value;
			}
		}

		private void Awake()
		{
			FriendshipGroupDetection.Instance = this;
			if (this.friendshipBubble)
			{
				this.particleSystem = this.friendshipBubble.GetComponent<ParticleSystem>();
				this.audioSource = this.friendshipBubble.GetComponent<AudioSource>();
			}
		}

		public void AddGroupZoneCallback(Action<GroupJoinZone> callback)
		{
			this.groupZoneCallbacks.Add(callback);
		}

		public void RemoveGroupZoneCallback(Action<GroupJoinZone> callback)
		{
			this.groupZoneCallbacks.Remove(callback);
		}

		public bool IsInMyGroup(string userID)
		{
			return this.myPartyMemberIDs != null && this.myPartyMemberIDs.Contains(userID);
		}

		public bool DidJoinLeftHanded
		{
			[CompilerGenerated]
			get
			{
				return this.<DidJoinLeftHanded>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<DidJoinLeftHanded>k__BackingField = value;
			}
		}

		private void Update()
		{
			List<int> list = this.playersInProvisionalGroup;
			List<int> list2 = this.playersInProvisionalGroup;
			List<int> list3 = this.tempIntList;
			this.tempIntList = list2;
			this.playersInProvisionalGroup = list3;
			Vector3 position;
			this.UpdateProvisionalGroup(out position);
			if (this.playersInProvisionalGroup.Count > 0)
			{
				this.friendshipBubble.transform.position = position;
			}
			bool flag = false;
			if (list.Count == this.playersInProvisionalGroup.Count)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != this.playersInProvisionalGroup[i])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.groupCreateAfterTimestamp = Time.time + this.groupTime;
				this.amFirstProvisionalPlayer = (this.playersInProvisionalGroup.Count > 0 && this.playersInProvisionalGroup[0] == PhotonNetwork.LocalPlayer.ActorNumber);
				if (this.playersInProvisionalGroup.Count > 0 && !this.amFirstProvisionalPlayer)
				{
					List<int> list4 = this.tempIntList;
					list4.Clear();
					Player targetPlayer = null;
					foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
					{
						if (vrrig.creator.ActorNumber == this.playersInProvisionalGroup[0])
						{
							targetPlayer = vrrig.creator;
							if (vrrig.IsLocalPartyMember)
							{
								list4.Clear();
								break;
							}
						}
						else if (vrrig.IsLocalPartyMember)
						{
							list4.Add(vrrig.creator.ActorNumber);
						}
					}
					if (list4.Count > 0)
					{
						base.photonView.RPC("NotifyPartyMerging", targetPlayer, new object[]
						{
							list4.ToArray()
						});
					}
					else
					{
						base.photonView.RPC("NotifyNoPartyToMerge", targetPlayer, Array.Empty<object>());
					}
				}
				if (this.playersInProvisionalGroup.Count == 0)
				{
					if (Time.time > this.suppressPartyCreationUntilTimestamp)
					{
						this.audioSource.Stop();
						this.audioSource.PlayOneShot(this.fistBumpInterruptedAudio);
					}
					this.particleSystem.Stop();
				}
				else
				{
					this.audioSource.time = 0f;
					this.audioSource.Play();
					this.particleSystem.Play();
				}
			}
			else if (this.playersInProvisionalGroup.Count > 0 && Time.time > this.groupCreateAfterTimestamp && this.amFirstProvisionalPlayer)
			{
				List<int> list5 = this.tempIntList;
				list5.Clear();
				list5.AddRange(this.playersInProvisionalGroup);
				int num = 0;
				if (this.IsInParty)
				{
					foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
					{
						if (vrrig2.IsLocalPartyMember)
						{
							list5.Add(vrrig2.creator.ActorNumber);
							num++;
						}
					}
				}
				int num2 = 0;
				foreach (int key in this.playersInProvisionalGroup)
				{
					int[] collection;
					if (this.partyMergeIDs.TryGetValue(key, out collection))
					{
						list5.AddRange(collection);
						num2++;
					}
				}
				list5.Sort();
				int[] memberIDs = list5.Distinct<int>().ToArray<int>();
				this.myBraceletColor = GTColor.RandomHSV(this.braceletRandomColorHSVRanges);
				this.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(this.myBraceletColor), memberIDs);
				this.groupCreateAfterTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			}
			if (this.myPartyMemberIDs != null)
			{
				this.UpdateWarningSigns();
			}
		}

		private void UpdateProvisionalGroup(out Vector3 midpoint)
		{
			this.playersInProvisionalGroup.Clear();
			bool willJoinLeftHanded;
			VRMap makingFist = GorillaTagger.Instance.offlineVRRig.GetMakingFist(this.debug, out willJoinLeftHanded);
			if (makingFist == null || !PhotonNetwork.InRoom || GorillaParent.instance.vrrigs.Count == 0 || Time.time < this.suppressPartyCreationUntilTimestamp)
			{
				midpoint = Vector3.zero;
				return;
			}
			this.WillJoinLeftHanded = willJoinLeftHanded;
			this.playersToPropagateFrom.Clear();
			this.provisionalGroupUsingLeftHands.Clear();
			this.playersMakingFists.Clear();
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			int num = -1;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				bool isLeftHand;
				VRMap makingFist2 = vrrig.GetMakingFist(this.debug, out isLeftHand);
				if (makingFist2 != null)
				{
					FriendshipGroupDetection.PlayerFist item = new FriendshipGroupDetection.PlayerFist
					{
						actorNumber = vrrig.creator.ActorNumber,
						position = makingFist2.rigTarget.position,
						isLeftHand = isLeftHand
					};
					if (vrrig.isOfflineVRRig)
					{
						num = this.playersMakingFists.Count;
					}
					this.playersMakingFists.Add(item);
				}
			}
			if (this.playersMakingFists.Count <= 1)
			{
				midpoint = Vector3.zero;
				return;
			}
			this.playersToPropagateFrom.Enqueue(this.playersMakingFists[num]);
			this.playersInProvisionalGroup.Add(actorNumber);
			midpoint = makingFist.rigTarget.position;
			int num2 = 1 << num;
			FriendshipGroupDetection.PlayerFist playerFist;
			while (this.playersToPropagateFrom.TryDequeue(out playerFist))
			{
				for (int i = 0; i < this.playersMakingFists.Count; i++)
				{
					if ((num2 & 1 << i) == 0)
					{
						FriendshipGroupDetection.PlayerFist playerFist2 = this.playersMakingFists[i];
						if ((playerFist.position - playerFist2.position).IsShorterThan(this.detectionRadius))
						{
							int index = ~this.playersInProvisionalGroup.BinarySearch(playerFist2.actorNumber);
							num2 |= 1 << i;
							this.playersInProvisionalGroup.Insert(index, playerFist2.actorNumber);
							if (playerFist2.isLeftHand)
							{
								this.provisionalGroupUsingLeftHands.Add(playerFist2.actorNumber);
							}
							this.playersToPropagateFrom.Enqueue(playerFist2);
							midpoint += playerFist2.position;
						}
					}
				}
			}
			if (this.playersInProvisionalGroup.Count == 1)
			{
				this.playersInProvisionalGroup.Clear();
			}
			if (this.playersInProvisionalGroup.Count > 0)
			{
				midpoint /= (float)this.playersInProvisionalGroup.Count;
			}
		}

		private void UpdateWarningSigns()
		{
			ZoneEntity zoneEntity = GorillaTagger.Instance.offlineVRRig.zoneEntity;
			GTZone currentRoomZone = PhotonNetworkController.Instance.CurrentRoomZone;
			GroupJoinZone groupJoinZone = (GroupJoinZone)0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						groupJoinZone |= vrrig.zoneEntity.GroupZone;
					}
				}
			}
			if (groupJoinZone != this.partyZone)
			{
				this.debugStr.Clear();
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					if (vrrig2.IsLocalPartyMember && !vrrig2.isOfflineVRRig)
					{
						this.debugStr.Append(string.Format("{0} in {1};", vrrig2.playerNameVisible, vrrig2.zoneEntity.GroupZone));
					}
				}
				this.partyZone = groupJoinZone;
				foreach (Action<GroupJoinZone> action in this.groupZoneCallbacks)
				{
					action(this.partyZone);
				}
			}
		}

		[PunRPC]
		private void NotifyNoPartyToMerge(PhotonMessageInfo info)
		{
			this.partyMergeIDs.Remove(info.Sender.ActorNumber);
		}

		[PunRPC]
		private void NotifyPartyMerging(int[] memberIDs, PhotonMessageInfo info)
		{
			if (memberIDs.Length > 10)
			{
				return;
			}
			this.partyMergeIDs[info.Sender.ActorNumber] = memberIDs;
		}

		public void SendAboutToGroupJoin()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					base.photonView.RPC("PartyMemberIsAboutToGroupJoin", vrrig.creator, Array.Empty<object>());
				}
			}
		}

		[PunRPC]
		private void PartyMemberIsAboutToGroupJoin(PhotonMessageInfo info)
		{
			float time = Time.time;
			float num = this.aboutToGroupJoin_CooldownUntilTimestamp;
			if (info.Sender.ActorNumber < PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.aboutToGroupJoin_CooldownUntilTimestamp = Time.time + 5f;
				if (this.myPartyMembersHash.Contains(info.Sender.UserId))
				{
					PhotonNetworkController.Instance.DeferJoining(2f);
				}
			}
		}

		private void SendPartyFormedRPC(short braceletColor, int[] memberIDs)
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (this.playersInProvisionalGroup.BinarySearch(vrrig.creator.ActorNumber) >= 0)
				{
					base.photonView.RPC("PartyFormedSuccessfully", vrrig.Creator, new object[]
					{
						braceletColor,
						memberIDs
					});
				}
			}
		}

		[PunRPC]
		private void PartyFormedSuccessfully(short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			if (memberIDs.Length > 10 || !memberIDs.Contains(info.Sender.ActorNumber) || !this.playersInProvisionalGroup.Contains(info.Sender.ActorNumber))
			{
				return;
			}
			if (this.IsInParty)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						base.photonView.RPC("AddPartyMembers", vrrig.Creator, new object[]
						{
							braceletColor,
							memberIDs
						});
					}
				}
			}
			this.suppressPartyCreationUntilTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			this.DidJoinLeftHanded = this.WillJoinLeftHanded;
			this.SetNewParty(braceletColor, memberIDs);
		}

		[PunRPC]
		private void AddPartyMembers(short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			if (memberIDs.Length > 10 || !this.IsInParty || !this.myPartyMembersHash.Contains(info.Sender.UserId))
			{
				return;
			}
			this.SetNewParty(braceletColor, memberIDs);
		}

		private void SetNewParty(short braceletColor, int[] memberIDs)
		{
			this.myPartyMemberIDs = new List<string>();
			FriendshipGroupDetection.userIdLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				FriendshipGroupDetection.userIdLookup.Add(vrrig.creator.ActorNumber, vrrig.creator.UserId);
			}
			foreach (int key in memberIDs)
			{
				string item;
				if (FriendshipGroupDetection.userIdLookup.TryGetValue(key, out item))
				{
					this.myPartyMemberIDs.Add(item);
				}
			}
			this.myBraceletColor = FriendshipGroupDetection.UnpackColor(braceletColor);
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
			this.OnPartyMembershipChanged();
		}

		public void LeaveParty()
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					base.photonView.RPC("PlayerLeftParty", vrrig.creator, Array.Empty<object>());
				}
			}
			this.myPartyMemberIDs = null;
			this.OnPartyMembershipChanged();
			PhotonNetworkController.Instance.ClearDeferredJoin();
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}

		[PunRPC]
		private void PlayerLeftParty(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(info.Sender.UserId))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		public void SendVerifyPartyMember(Player player)
		{
			base.photonView.RPC("VerifyPartyMember", player, Array.Empty<object>());
		}

		[PunRPC]
		private void VerifyPartyMember(PhotonMessageInfo info)
		{
			if (this.myPartyMemberIDs == null || !this.myPartyMemberIDs.Contains(info.Sender.UserId))
			{
				base.photonView.RPC("PlayerLeftParty", info.Sender, Array.Empty<object>());
			}
		}

		private void OnPartyMembershipChanged()
		{
			this.myPartyMembersHash.Clear();
			if (this.myPartyMemberIDs != null)
			{
				foreach (string item in this.myPartyMemberIDs)
				{
					this.myPartyMembersHash.Add(item);
				}
			}
			this.myBeadColors.Clear();
			FriendshipGroupDetection.tempColorLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ClearPartyMemberStatus();
				if (vrrig.IsLocalPartyMember)
				{
					FriendshipGroupDetection.tempColorLookup.Add(vrrig.creator.UserId, vrrig.playerColor);
				}
			}
			this.MyBraceletSelfIndex = 0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (string text in this.myPartyMemberIDs)
				{
					Color item2;
					if (FriendshipGroupDetection.tempColorLookup.TryGetValue(text, out item2))
					{
						if (text == PhotonNetwork.LocalPlayer.UserId)
						{
							this.MyBraceletSelfIndex = this.myBeadColors.Count;
						}
						this.myBeadColors.Add(item2);
					}
				}
			}
			this.myBeadColors.Add(this.myBraceletColor);
			GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
			this.UpdateWarningSigns();
		}

		public bool IsPartyWithinCollider(GorillaFriendCollider friendCollider)
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig && !friendCollider.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		public static short PackColor(Color col)
		{
			return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
		}

		public static Color UnpackColor(short data)
		{
			return new Color
			{
				r = (float)(data % 10) / 9f,
				g = (float)(data / 10 % 10) / 9f,
				b = (float)(data / 100 % 10) / 9f
			};
		}

		public FriendshipGroupDetection()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FriendshipGroupDetection()
		{
		}

		[CompilerGenerated]
		private static FriendshipGroupDetection <Instance>k__BackingField;

		[SerializeField]
		private float detectionRadius = 0.5f;

		[SerializeField]
		private float groupTime = 5f;

		[SerializeField]
		private float cooldownAfterCreatingGroup = 5f;

		[SerializeField]
		private float hapticStrength = 1.5f;

		[SerializeField]
		private float hapticDuration = 2f;

		public bool debug;

		public double offset = 0.5;

		private List<string> myPartyMemberIDs;

		private HashSet<string> myPartyMembersHash = new HashSet<string>();

		[CompilerGenerated]
		private List<Color> <myBeadColors>k__BackingField;

		[CompilerGenerated]
		private Color <myBraceletColor>k__BackingField;

		[CompilerGenerated]
		private int <MyBraceletSelfIndex>k__BackingField;

		[CompilerGenerated]
		private GroupJoinZone <partyZone>k__BackingField;

		private List<Action<GroupJoinZone>> groupZoneCallbacks = new List<Action<GroupJoinZone>>();

		[SerializeField]
		private GTColor.HSVRanges braceletRandomColorHSVRanges;

		public GameObject friendshipBubble;

		public AudioClip fistBumpInterruptedAudio;

		private ParticleSystem particleSystem;

		private AudioSource audioSource;

		private Queue<FriendshipGroupDetection.PlayerFist> playersToPropagateFrom = new Queue<FriendshipGroupDetection.PlayerFist>();

		private List<int> playersInProvisionalGroup = new List<int>();

		private List<int> provisionalGroupUsingLeftHands = new List<int>();

		private List<int> tempIntList = new List<int>();

		private bool amFirstProvisionalPlayer;

		private Dictionary<int, int[]> partyMergeIDs = new Dictionary<int, int[]>();

		private float groupCreateAfterTimestamp;

		private float suppressPartyCreationUntilTimestamp;

		[CompilerGenerated]
		private bool <DidJoinLeftHanded>k__BackingField;

		private bool WillJoinLeftHanded;

		private List<FriendshipGroupDetection.PlayerFist> playersMakingFists = new List<FriendshipGroupDetection.PlayerFist>();

		private StringBuilder debugStr = new StringBuilder();

		private float aboutToGroupJoin_CooldownUntilTimestamp;

		private static Dictionary<int, string> userIdLookup = new Dictionary<int, string>();

		private static Dictionary<string, Color> tempColorLookup = new Dictionary<string, Color>();

		private struct PlayerFist
		{
			public int actorNumber;

			public Vector3 position;

			public bool isLeftHand;
		}
	}
}
