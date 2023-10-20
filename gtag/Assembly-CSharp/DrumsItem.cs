using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class DrumsItem : MonoBehaviour
{
	// Token: 0x06000689 RID: 1673 RVA: 0x00028DB0 File Offset: 0x00026FB0
	private void Start()
	{
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		for (int i = 0; i < this.collidersForThisDrum.Length; i++)
		{
			this.collidersForThisDrumList.Add(this.collidersForThisDrum[i]);
		}
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x00028E23 File Offset: 0x00027023
	private void LateUpdate()
	{
		this.CheckHandHit(ref this.leftHandIn, ref this.leftHandIndicator, true);
		this.CheckHandHit(ref this.rightHandIn, ref this.rightHandIndicator, false);
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00028E4C File Offset: 0x0002704C
	private void CheckHandHit(ref bool handIn, ref GorillaTriggerColliderHandIndicator handIndicator, bool isLeftHand)
	{
		this.spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (this.spherecastSweep.magnitude < 0.0001f)
		{
			this.spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = this.nullHit;
		}
		this.collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, this.sphereRadius, this.spherecastSweep.normalized, this.collidersHit, this.spherecastSweep.magnitude, this.drumsTouchable, QueryTriggerInteraction.Collide);
		this.drumHit = false;
		if (this.collidersHitCount > 0)
		{
			this.hitList.Clear();
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.collidersHit[j].collider != null && this.collidersForThisDrumList.Contains(this.collidersHit[j].collider) && this.collidersHit[j].collider.gameObject.activeSelf)
				{
					this.hitList.Add(this.collidersHit[j]);
				}
			}
			this.hitList.Sort(new Comparison<RaycastHit>(this.RayCastHitCompare));
			int k = 0;
			while (k < this.hitList.Count)
			{
				this.tempDrum = this.hitList[k].collider.GetComponent<Drum>();
				if (this.tempDrum != null)
				{
					this.drumHit = true;
					if (!handIn && !this.tempDrum.disabler)
					{
						this.DrumHit(this.tempDrum, isLeftHand, handIndicator.currentVelocity.magnitude);
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
		}
		if (!this.drumHit & handIn)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration);
		}
		handIn = this.drumHit;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00029067 File Offset: 0x00027267
	private int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00029090 File Offset: 0x00027290
	public void DrumHit(Drum tempDrumInner, bool isLeftHand, float hitVelocity)
	{
		if (isLeftHand)
		{
			if (this.leftHandIn)
			{
				return;
			}
			this.leftHandIn = true;
		}
		else
		{
			if (this.rightHandIn)
			{
				return;
			}
			this.rightHandIn = true;
		}
		this.volToPlay = Mathf.Max(Mathf.Min(1f, hitVelocity / this.maxDrumVolumeVelocity) * this.maxDrumVolume, this.minDrumVolume);
		if (PhotonNetwork.InRoom)
		{
			if (!this.myRig.isOfflineVRRig)
			{
				PhotonView photonView = this.myRig.photonView;
				if (photonView != null)
				{
					photonView.RPC("PlayDrum", RpcTarget.Others, new object[]
					{
						tempDrumInner.myIndex + this.onlineOffset,
						this.volToPlay
					});
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.RPC("PlayDrum", RpcTarget.Others, new object[]
				{
					tempDrumInner.myIndex + this.onlineOffset,
					this.volToPlay
				});
			}
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
		this.drumsAS[tempDrumInner.myIndex].volume = this.maxDrumVolume;
		this.drumsAS[tempDrumInner.myIndex].PlayOneShot(this.drumsAS[tempDrumInner.myIndex].clip, this.volToPlay);
	}

	// Token: 0x040007D5 RID: 2005
	[Tooltip("Array of colliders for this specific drum.")]
	public Collider[] collidersForThisDrum;

	// Token: 0x040007D6 RID: 2006
	private List<Collider> collidersForThisDrumList = new List<Collider>();

	// Token: 0x040007D7 RID: 2007
	[Tooltip("AudioSources where each index must match the index given to the corresponding Drum component.")]
	public AudioSource[] drumsAS;

	// Token: 0x040007D8 RID: 2008
	[Tooltip("Max volume a drum can reach.")]
	public float maxDrumVolume = 0.2f;

	// Token: 0x040007D9 RID: 2009
	[Tooltip("Min volume a drum can reach.")]
	public float minDrumVolume = 0.05f;

	// Token: 0x040007DA RID: 2010
	[Tooltip("Multiplies against actual velocity before capping by min & maxDrumVolume values.")]
	public float maxDrumVolumeVelocity = 1f;

	// Token: 0x040007DB RID: 2011
	private bool rightHandIn;

	// Token: 0x040007DC RID: 2012
	private bool leftHandIn;

	// Token: 0x040007DD RID: 2013
	private float volToPlay;

	// Token: 0x040007DE RID: 2014
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x040007DF RID: 2015
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x040007E0 RID: 2016
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x040007E1 RID: 2017
	private Collider[] actualColliders = new Collider[20];

	// Token: 0x040007E2 RID: 2018
	public LayerMask drumsTouchable;

	// Token: 0x040007E3 RID: 2019
	private float sphereRadius;

	// Token: 0x040007E4 RID: 2020
	private Vector3 spherecastSweep;

	// Token: 0x040007E5 RID: 2021
	private int collidersHitCount;

	// Token: 0x040007E6 RID: 2022
	private List<RaycastHit> hitList = new List<RaycastHit>();

	// Token: 0x040007E7 RID: 2023
	private Drum tempDrum;

	// Token: 0x040007E8 RID: 2024
	private bool drumHit;

	// Token: 0x040007E9 RID: 2025
	private RaycastHit nullHit;

	// Token: 0x040007EA RID: 2026
	public int onlineOffset;

	// Token: 0x040007EB RID: 2027
	[Tooltip("VRRig object of the player, used to determine if it is an offline rig.")]
	public VRRig myRig;
}
