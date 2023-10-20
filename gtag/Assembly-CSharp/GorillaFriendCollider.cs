using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class GorillaFriendCollider : MonoBehaviour
{
	// Token: 0x06000C5A RID: 3162 RVA: 0x0004B250 File Offset: 0x00049450
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

	// Token: 0x06000C5B RID: 3163 RVA: 0x0004B2B8 File Offset: 0x000494B8
	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0004B2D7 File Offset: 0x000494D7
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
						if (this.collidingRig != null && this.collidingRig.creator != null)
						{
							string userId = this.collidingRig.creator.UserId;
							this.AddUserID(userId);
						}
						else
						{
							if (this.otherColliderGO.GetComponent<Player>() == null)
							{
								goto IL_16F;
							}
							string userId = PhotonNetwork.LocalPlayer.UserId;
							this.AddUserID(userId);
						}
						this.overlapColliders[i] = null;
					}
					IL_16F:;
				}
				if (this.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
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

	// Token: 0x04000FC7 RID: 4039
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	// Token: 0x04000FC8 RID: 4040
	public CapsuleCollider thisCapsule;

	// Token: 0x04000FC9 RID: 4041
	public string[] myAllowedMapsToJoin;

	// Token: 0x04000FCA RID: 4042
	private readonly Collider[] overlapColliders = new Collider[20];

	// Token: 0x04000FCB RID: 4043
	private int tagAndBodyLayerMask;

	// Token: 0x04000FCC RID: 4044
	private float jiggleAmount;

	// Token: 0x04000FCD RID: 4045
	private Collider otherCollider;

	// Token: 0x04000FCE RID: 4046
	private GameObject otherColliderGO;

	// Token: 0x04000FCF RID: 4047
	private VRRig collidingRig;

	// Token: 0x04000FD0 RID: 4048
	private int collisions;
}
