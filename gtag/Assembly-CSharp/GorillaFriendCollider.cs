using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

public class GorillaFriendCollider : MonoBehaviour
{
	public void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.jiggleAmount = Random.Range(0f, 1f);
		base.StartCoroutine(this.UpdatePlayersInSphere());
		this.tagAndBodyLayerMask = (LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		}));
	}

	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	private IEnumerator UpdatePlayersInSphere()
	{
		yield return new WaitForSeconds(1f + this.jiggleAmount);
		WaitForSeconds wait = new WaitForSeconds(1f);
		for (;;)
		{
			this.playerIDsCurrentlyTouching.Clear();
			this.collisions = Physics.OverlapSphereNonAlloc(base.transform.position, this.thisCapsule.radius, this.overlapColliders, this.tagAndBodyLayerMask);
			this.collisions = Mathf.Min(this.collisions, this.overlapColliders.Length);
			if (this.collisions > 0)
			{
				for (int i = 0; i < this.collisions; i++)
				{
					this.otherCollider = this.overlapColliders[i];
					if (!(this.otherCollider == null))
					{
						this.otherColliderGO = this.otherCollider.attachedRigidbody.gameObject;
						this.collidingRig = this.otherColliderGO.GetComponent<VRRig>();
						if (this.collidingRig == null || this.collidingRig.creatorWrapped == null || this.collidingRig.creatorWrapped.IsNull || string.IsNullOrEmpty(this.collidingRig.creatorWrapped.UserId))
						{
							if (this.otherColliderGO.GetComponent<Player>() == null || NetworkSystem.Instance.LocalPlayer == null)
							{
								goto IL_1A9;
							}
							string userId = NetworkSystem.Instance.LocalPlayer.UserId;
							this.AddUserID(userId);
						}
						else
						{
							string userId = this.collidingRig.creatorWrapped.UserId;
							this.AddUserID(userId);
						}
						this.overlapColliders[i] = null;
					}
					IL_1A9:;
				}
				if (NetworkSystem.Instance.LocalPlayer != null && this.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
				{
					GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
					GorillaComputer.instance.friendJoinCollider = this;
					GorillaComputer.instance.UpdateScreen();
				}
				this.otherCollider = null;
				this.otherColliderGO = null;
				this.collidingRig = null;
			}
			yield return wait;
		}
		yield break;
	}

	public GorillaFriendCollider()
	{
	}

	public List<string> playerIDsCurrentlyTouching = new List<string>();

	public CapsuleCollider thisCapsule;

	public string[] myAllowedMapsToJoin;

	private readonly Collider[] overlapColliders = new Collider[20];

	private int tagAndBodyLayerMask;

	private float jiggleAmount;

	private Collider otherCollider;

	private GameObject otherColliderGO;

	private VRRig collidingRig;

	private int collisions;

	[CompilerGenerated]
	private sealed class <UpdatePlayersInSphere>d__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdatePlayersInSphere>d__12(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			GorillaFriendCollider gorillaFriendCollider = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(1f + gorillaFriendCollider.jiggleAmount);
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				wait = new WaitForSeconds(1f);
				break;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			gorillaFriendCollider.playerIDsCurrentlyTouching.Clear();
			gorillaFriendCollider.collisions = Physics.OverlapSphereNonAlloc(gorillaFriendCollider.transform.position, gorillaFriendCollider.thisCapsule.radius, gorillaFriendCollider.overlapColliders, gorillaFriendCollider.tagAndBodyLayerMask);
			gorillaFriendCollider.collisions = Mathf.Min(gorillaFriendCollider.collisions, gorillaFriendCollider.overlapColliders.Length);
			if (gorillaFriendCollider.collisions > 0)
			{
				for (int i = 0; i < gorillaFriendCollider.collisions; i++)
				{
					gorillaFriendCollider.otherCollider = gorillaFriendCollider.overlapColliders[i];
					if (!(gorillaFriendCollider.otherCollider == null))
					{
						gorillaFriendCollider.otherColliderGO = gorillaFriendCollider.otherCollider.attachedRigidbody.gameObject;
						gorillaFriendCollider.collidingRig = gorillaFriendCollider.otherColliderGO.GetComponent<VRRig>();
						if (gorillaFriendCollider.collidingRig == null || gorillaFriendCollider.collidingRig.creatorWrapped == null || gorillaFriendCollider.collidingRig.creatorWrapped.IsNull || string.IsNullOrEmpty(gorillaFriendCollider.collidingRig.creatorWrapped.UserId))
						{
							if (gorillaFriendCollider.otherColliderGO.GetComponent<Player>() == null || NetworkSystem.Instance.LocalPlayer == null)
							{
								goto IL_1A9;
							}
							GorillaFriendCollider gorillaFriendCollider2 = gorillaFriendCollider;
							string userId = NetworkSystem.Instance.LocalPlayer.UserId;
							gorillaFriendCollider2.AddUserID(userId);
						}
						else
						{
							GorillaFriendCollider gorillaFriendCollider3 = gorillaFriendCollider;
							string userId = gorillaFriendCollider.collidingRig.creatorWrapped.UserId;
							gorillaFriendCollider3.AddUserID(userId);
						}
						gorillaFriendCollider.overlapColliders[i] = null;
					}
					IL_1A9:;
				}
				if (NetworkSystem.Instance.LocalPlayer != null && gorillaFriendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != gorillaFriendCollider)
				{
					GorillaComputer.instance.allowedMapsToJoin = gorillaFriendCollider.myAllowedMapsToJoin;
					GorillaComputer.instance.friendJoinCollider = gorillaFriendCollider;
					GorillaComputer.instance.UpdateScreen();
				}
				gorillaFriendCollider.otherCollider = null;
				gorillaFriendCollider.otherColliderGO = null;
				gorillaFriendCollider.collidingRig = null;
			}
			this.<>2__current = wait;
			this.<>1__state = 2;
			return true;
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

		public GorillaFriendCollider <>4__this;

		private WaitForSeconds <wait>5__2;
	}
}
