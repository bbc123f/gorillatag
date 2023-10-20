using System;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200010D RID: 269
public class GeodeItem : TransferrableObject
{
	// Token: 0x0600069C RID: 1692 RVA: 0x000296B2 File Offset: 0x000278B2
	protected override void Awake()
	{
		base.Awake();
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x000296D3 File Offset: 0x000278D3
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.prevItemState = TransferrableObject.ItemStates.State0;
		this.InitToDefault();
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x000296EF File Offset: 0x000278EF
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00029704 File Offset: 0x00027904
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			return;
		}
		base.InHand();
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00029720 File Offset: 0x00027920
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		this.effectsHaveBeenPlayed = false;
		if (this.hasEffectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.geodeFullMesh.SetActive(true);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(false);
		}
		this.hitLastFrame = false;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00029788 File Offset: 0x00027988
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				this.OnItemStateChanged();
			}
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minHitVelocity)
		{
			return;
		}
		if (base.InHand())
		{
			int num = Physics.SphereCastNonAlloc(this.geodeFullMesh.transform.position, this.sphereRayRadius * Mathf.Abs(this.geodeFullMesh.transform.lossyScale.x), this.geodeFullMesh.transform.TransformDirection(Vector3.forward), this.collidersHit, this.rayCastMaxDistance, this.collisionLayerMask, QueryTriggerInteraction.Collide);
			this.hitLastFrame = (num > 0);
		}
		if (!this.hitLastFrame)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
		this.index = this.RandomPickCrackedGeode();
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00029893 File Offset: 0x00027A93
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.currentItemState = this.itemState;
		if (this.currentItemState != this.prevItemState)
		{
			this.OnItemStateChanged();
		}
		this.prevItemState = this.currentItemState;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x000298C8 File Offset: 0x00027AC8
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.InitToDefault();
			return;
		}
		this.geodeFullMesh.SetActive(false);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(i == this.index);
		}
		if (PhotonNetwork.InRoom && GorillaGameManager.instance != null && !this.effectsHaveBeenPlayed)
		{
			GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("PlayGeodeEffect", RpcTarget.All, new object[]
			{
				this.geodeFullMesh.transform.position
			});
			this.effectsHaveBeenPlayed = true;
		}
		if (!PhotonNetwork.InRoom && !this.effectsHaveBeenPlayed)
		{
			if (this.audioSource)
			{
				this.audioSource.Play();
			}
			this.effectsHaveBeenPlayed = true;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000299A7 File Offset: 0x00027BA7
	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, this.geodeCrackedMeshes.Length);
	}

	// Token: 0x040007FB RID: 2043
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x040007FC RID: 2044
	public LayerMask collisionLayerMask;

	// Token: 0x040007FD RID: 2045
	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040007FE RID: 2046
	public float cooldown = 5f;

	// Token: 0x040007FF RID: 2047
	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	// Token: 0x04000800 RID: 2048
	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	// Token: 0x04000801 RID: 2049
	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	// Token: 0x04000802 RID: 2050
	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	// Token: 0x04000803 RID: 2051
	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	// Token: 0x04000804 RID: 2052
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x04000805 RID: 2053
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x04000806 RID: 2054
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000807 RID: 2055
	private bool hasEffectsGameObject;

	// Token: 0x04000808 RID: 2056
	private bool effectsHaveBeenPlayed;

	// Token: 0x04000809 RID: 2057
	private RaycastHit hit;

	// Token: 0x0400080A RID: 2058
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x0400080B RID: 2059
	private TransferrableObject.ItemStates currentItemState;

	// Token: 0x0400080C RID: 2060
	private TransferrableObject.ItemStates prevItemState;

	// Token: 0x0400080D RID: 2061
	private int index;
}
