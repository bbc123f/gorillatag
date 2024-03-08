﻿using System;
using System.Collections;
using System.Collections.Generic;
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
		this.tagAndBodyLayerMask = LayerMask.GetMask(new string[] { "Gorilla Tag Collider" }) | LayerMask.GetMask(new string[] { "Gorilla Body Collider" });
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
							string text = NetworkSystem.Instance.LocalPlayer.UserId;
							this.AddUserID(text);
						}
						else
						{
							string text = this.collidingRig.creatorWrapped.UserId;
							this.AddUserID(text);
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
}
