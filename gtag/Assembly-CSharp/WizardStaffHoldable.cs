using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class WizardStaffHoldable : TransferrableObject
{
	// Token: 0x06000140 RID: 320 RVA: 0x0000AD40 File Offset: 0x00008F40
	protected override void Awake()
	{
		base.Awake();
		this.tipTargetLocalPosition = this.tipTransform.localPosition;
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000AD72 File Offset: 0x00008F72
	public override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000AD80 File Offset: 0x00008F80
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0000AD8E File Offset: 0x00008F8E
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		if (this.hasEffectsGameObject && this.effectsHaveBeenPlayed)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000ADC0 File Offset: 0x00008FC0
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand() || this.itemState == TransferrableObject.ItemStates.State1 || !GorillaParent.hasInstance || !this.hitLastFrame)
		{
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minSlamVelocity)
		{
			return;
		}
		Vector3 up = this.tipTransform.up;
		Vector3 up2 = Vector3.up;
		if (Vector3.Angle(up, up2) > this.minSlamAngle)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000AE44 File Offset: 0x00009044
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
			if (this.hasEffectsGameObject)
			{
				this.effectsGameObject.SetActive(false);
			}
			this.effectsHaveBeenPlayed = false;
		}
		if (base.InHand())
		{
			Vector3 position = base.transform.position;
			Vector3 end = base.transform.TransformPoint(this.tipTargetLocalPosition);
			RaycastHit raycastHit;
			if (Physics.Linecast(position, end, out raycastHit, this.tipCollisionLayerMask))
			{
				this.tipTransform.position = raycastHit.point;
				this.hitLastFrame = true;
			}
			else
			{
				this.tipTransform.localPosition = this.tipTargetLocalPosition;
				this.hitLastFrame = false;
			}
			if (this.itemState == TransferrableObject.ItemStates.State1 && this.hasEffectsGameObject && !this.effectsHaveBeenPlayed)
			{
				this.effectsGameObject.SetActive(true);
				this.effectsHaveBeenPlayed = true;
			}
		}
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000AF34 File Offset: 0x00009134
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.effectsHaveBeenPlayed)
		{
			this.cooldownRemaining = this.cooldown;
		}
	}

	// Token: 0x040001BE RID: 446
	[Tooltip("This GameObject will activate when the staff hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x040001BF RID: 447
	[Tooltip("The Transform of the staff's tip which will be used to determine if the staff is being slammed. Up axis (Y) should point along the length of the staff.")]
	public Transform tipTransform;

	// Token: 0x040001C0 RID: 448
	public float tipCollisionRadius = 0.05f;

	// Token: 0x040001C1 RID: 449
	public LayerMask tipCollisionLayerMask;

	// Token: 0x040001C2 RID: 450
	[Tooltip("Used to calculate velocity of the staff.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040001C3 RID: 451
	public float cooldown = 5f;

	// Token: 0x040001C4 RID: 452
	[Tooltip("The velocity of the staff's tip must be greater than this value to activate the effect.")]
	public float minSlamVelocity = 0.5f;

	// Token: 0x040001C5 RID: 453
	[Tooltip("The angle (in degrees) between the staff's tip and the ground must be less than this value to activate the effect.")]
	public float minSlamAngle = 5f;

	// Token: 0x040001C6 RID: 454
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x040001C7 RID: 455
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x040001C8 RID: 456
	private Vector3 tipTargetLocalPosition;

	// Token: 0x040001C9 RID: 457
	private bool hasEffectsGameObject;

	// Token: 0x040001CA RID: 458
	private bool effectsHaveBeenPlayed;
}
