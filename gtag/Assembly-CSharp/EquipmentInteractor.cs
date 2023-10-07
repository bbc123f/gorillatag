using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000DC RID: 220
public class EquipmentInteractor : MonoBehaviour
{
	// Token: 0x060004F8 RID: 1272 RVA: 0x0001F9E0 File Offset: 0x0001DBE0
	private void Awake()
	{
		if (EquipmentInteractor.instance == null)
		{
			EquipmentInteractor.instance = this;
			EquipmentInteractor.hasInstance = true;
		}
		else if (EquipmentInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.leftHandSet = false;
		this.rightHandSet = false;
		this.autoGrabLeft = true;
		this.autoGrabRight = true;
		this.gorillaInteractableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaInteractable"
		});
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001FA5B File Offset: 0x0001DC5B
	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0001FA7C File Offset: 0x0001DC7C
	public void ReleaseRightHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		this.autoGrabRight = true;
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001FAD0 File Offset: 0x0001DCD0
	public void ReleaseLeftHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		this.autoGrabLeft = true;
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001FB24 File Offset: 0x0001DD24
	public void ForceStopClimbing()
	{
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001FB40 File Offset: 0x0001DD40
	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.leftHandHeldEquipment;
		}
		return this.rightHandHeldEquipment;
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001FB60 File Offset: 0x0001DD60
	private void LateUpdate()
	{
		this.CheckInputValue(true);
		this.isLeftGrabbing = ((this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.leftClimber && this.leftClimber.isClimbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = ((this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.rightClimber && this.rightClimber.isClimbing)
		{
			this.isRightGrabbing = false;
		}
		this.FireHandInteractions(this.leftHand, true);
		this.FireHandInteractions(this.rightHand, false);
		if (!this.isRightGrabbing && this.wasRightGrabPressed)
		{
			this.ReleaseRightHand();
		}
		if (!this.isLeftGrabbing && this.wasLeftGrabPressed)
		{
			this.ReleaseLeftHand();
		}
		this.wasLeftGrabPressed = this.isLeftGrabbing;
		this.wasRightGrabPressed = this.isRightGrabbing;
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001FCA8 File Offset: 0x0001DEA8
	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = ((this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft));
			this.justReleased = (this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed);
		}
		else
		{
			this.justGrabbed = ((this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight));
			this.justReleased = (this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed);
		}
		foreach (InteractionPoint interactionPoint in (isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight))
		{
			bool flag = isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null);
			bool flag2 = isLeftHand ? this.disableLeftGrab : this.disableRightGrab;
			if (!flag && !flag2 && interactionPoint != null)
			{
				if (this.justGrabbed)
				{
					interactionPoint.parentTransferrableObject.OnGrab(interactionPoint, interactingHand);
				}
				else
				{
					interactionPoint.parentTransferrableObject.OnHover(interactionPoint, interactingHand);
				}
			}
			if (this.justReleased)
			{
				this.tempZone = interactionPoint.GetComponent<DropZone>();
				if (this.tempZone != null)
				{
					if (interactingHand == this.leftHand)
					{
						if (this.leftHandHeldEquipment != null)
						{
							this.leftHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
						}
					}
					else if (this.rightHandHeldEquipment != null)
					{
						this.rightHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
					}
				}
			}
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001FE80 File Offset: 0x0001E080
	public void UpdateHandEquipment(HoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment == this.rightHandHeldEquipment)
			{
				this.rightHandHeldEquipment = null;
			}
			if (this.leftHandHeldEquipment != null)
			{
				this.leftHandHeldEquipment.DropItemCleanup();
			}
			this.leftHandHeldEquipment = newEquipment;
			this.autoGrabLeft = false;
			return;
		}
		if (newEquipment == this.leftHandHeldEquipment)
		{
			this.leftHandHeldEquipment = null;
		}
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.DropItemCleanup();
		}
		this.rightHandHeldEquipment = newEquipment;
		this.autoGrabRight = false;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001FF0C File Offset: 0x0001E10C
	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		}
		else
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		}
		this.grabValue = Mathf.Max(this.grabValue, this.tempValue);
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001FF65 File Offset: 0x0001E165
	public void ForceDropEquipment(HoldableObject equipment)
	{
		if (this.rightHandHeldEquipment == equipment)
		{
			this.rightHandHeldEquipment = null;
		}
		if (this.leftHandHeldEquipment == equipment)
		{
			this.leftHandHeldEquipment = null;
		}
	}

	// Token: 0x040005C7 RID: 1479
	public static volatile EquipmentInteractor instance;

	// Token: 0x040005C8 RID: 1480
	public static bool hasInstance;

	// Token: 0x040005C9 RID: 1481
	public HoldableObject leftHandHeldEquipment;

	// Token: 0x040005CA RID: 1482
	public HoldableObject rightHandHeldEquipment;

	// Token: 0x040005CB RID: 1483
	public Transform leftHandTransform;

	// Token: 0x040005CC RID: 1484
	public Transform rightHandTransform;

	// Token: 0x040005CD RID: 1485
	public Transform chestTransform;

	// Token: 0x040005CE RID: 1486
	public Transform leftArmTransform;

	// Token: 0x040005CF RID: 1487
	public Transform rightArmTransform;

	// Token: 0x040005D0 RID: 1488
	public GameObject rightHand;

	// Token: 0x040005D1 RID: 1489
	public GameObject leftHand;

	// Token: 0x040005D2 RID: 1490
	private bool leftHandSet;

	// Token: 0x040005D3 RID: 1491
	private bool rightHandSet;

	// Token: 0x040005D4 RID: 1492
	public InputDevice leftHandDevice;

	// Token: 0x040005D5 RID: 1493
	public InputDevice rightHandDevice;

	// Token: 0x040005D6 RID: 1494
	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	// Token: 0x040005D7 RID: 1495
	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	// Token: 0x040005D8 RID: 1496
	private int gorillaInteractableLayerMask;

	// Token: 0x040005D9 RID: 1497
	public float grabRadius;

	// Token: 0x040005DA RID: 1498
	public float grabThreshold = 0.7f;

	// Token: 0x040005DB RID: 1499
	public float grabHysteresis = 0.05f;

	// Token: 0x040005DC RID: 1500
	public bool wasLeftGrabPressed;

	// Token: 0x040005DD RID: 1501
	public bool wasRightGrabPressed;

	// Token: 0x040005DE RID: 1502
	public bool isLeftGrabbing;

	// Token: 0x040005DF RID: 1503
	public bool isRightGrabbing;

	// Token: 0x040005E0 RID: 1504
	public bool justReleased;

	// Token: 0x040005E1 RID: 1505
	public bool justGrabbed;

	// Token: 0x040005E2 RID: 1506
	public bool disableLeftGrab;

	// Token: 0x040005E3 RID: 1507
	public bool disableRightGrab;

	// Token: 0x040005E4 RID: 1508
	public bool autoGrabLeft;

	// Token: 0x040005E5 RID: 1509
	public bool autoGrabRight;

	// Token: 0x040005E6 RID: 1510
	private float grabValue;

	// Token: 0x040005E7 RID: 1511
	private float tempValue;

	// Token: 0x040005E8 RID: 1512
	private InteractionPoint tempPoint;

	// Token: 0x040005E9 RID: 1513
	private DropZone tempZone;

	// Token: 0x040005EA RID: 1514
	[SerializeField]
	private GorillaHandClimber leftClimber;

	// Token: 0x040005EB RID: 1515
	[SerializeField]
	private GorillaHandClimber rightClimber;
}
