using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

public class FriendshipCharm : HoldableObject
{
	private void Awake()
	{
		this.parent = base.transform.parent;
	}

	private void LateUpdate()
	{
		if (!this.isBroken && (this.lineStart.transform.position - this.lineEnd.transform.position).IsLongerThan(this.breakBraceletLength * Player.Instance.scale))
		{
			this.DestroyBracelet();
		}
	}

	public override void OnEnable()
	{
		this.interactionPoint.enabled = true;
		this.meshRenderer.enabled = true;
		if (this.rb)
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
		}
		this.isBroken = false;
		this.UpdatePosition();
		base.OnEnable();
	}

	private void DestroyBracelet()
	{
		this.interactionPoint.enabled = false;
		this.rb.isKinematic = false;
		this.rb.useGravity = true;
		this.isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		base.transform.SetParent(flag ? this.leftHandHoldAnchor : this.rightHandHoldAnchor);
		base.transform.localPosition = Vector3.zero;
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool forLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
		this.UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	private void UpdatePosition()
	{
		base.transform.SetParent(this.parent);
		base.transform.localPosition = this.releasePosition.localPosition;
		base.transform.localRotation = this.releasePosition.localRotation;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!this.isBroken)
		{
			return;
		}
		if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
		{
			return;
		}
		this.meshRenderer.enabled = false;
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.UpdatePosition();
	}

	public FriendshipCharm()
	{
	}

	[SerializeField]
	private InteractionPoint interactionPoint;

	[SerializeField]
	private Transform rightHandHoldAnchor;

	[SerializeField]
	private Transform leftHandHoldAnchor;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Transform lineStart;

	[SerializeField]
	private Transform lineEnd;

	[SerializeField]
	private Transform releasePosition;

	[SerializeField]
	private float breakBraceletLength;

	[SerializeField]
	private LayerMask breakItemLayerMask;

	private Transform parent;

	private bool isBroken;
}
