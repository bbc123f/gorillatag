using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

public class EquipmentInteractor : MonoBehaviour
{
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
		this.gorillaInteractableLayerMask = LayerMask.GetMask(new string[] { "GorillaInteractable" });
	}

	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

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

	public void ForceStopClimbing()
	{
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.leftHandHeldEquipment;
		}
		return this.rightHandHeldEquipment;
	}

	private void LateUpdate()
	{
		this.CheckInputValue(true);
		this.isLeftGrabbing = (this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis);
		if (this.leftClimber && this.leftClimber.isClimbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = (this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis);
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

	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = (this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft);
			this.justReleased = this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed;
		}
		else
		{
			this.justGrabbed = (this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight);
			this.justReleased = this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed;
		}
		foreach (InteractionPoint interactionPoint in (isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight))
		{
			bool flag = (isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null));
			bool flag2 = (isLeftHand ? this.disableLeftGrab : this.disableRightGrab);
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

	[OnEnterPlay_SetNull]
	public static volatile EquipmentInteractor instance;

	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	public HoldableObject leftHandHeldEquipment;

	public HoldableObject rightHandHeldEquipment;

	public Transform leftHandTransform;

	public Transform rightHandTransform;

	public Transform chestTransform;

	public Transform leftArmTransform;

	public Transform rightArmTransform;

	public GameObject rightHand;

	public GameObject leftHand;

	private bool leftHandSet;

	private bool rightHandSet;

	public InputDevice leftHandDevice;

	public InputDevice rightHandDevice;

	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	private int gorillaInteractableLayerMask;

	public float grabRadius;

	public float grabThreshold = 0.7f;

	public float grabHysteresis = 0.05f;

	public bool wasLeftGrabPressed;

	public bool wasRightGrabPressed;

	public bool isLeftGrabbing;

	public bool isRightGrabbing;

	public bool justReleased;

	public bool justGrabbed;

	public bool disableLeftGrab;

	public bool disableRightGrab;

	public bool autoGrabLeft;

	public bool autoGrabRight;

	private float grabValue;

	private float tempValue;

	private InteractionPoint tempPoint;

	private DropZone tempZone;

	[SerializeField]
	private GorillaHandClimber leftClimber;

	[SerializeField]
	private GorillaHandClimber rightClimber;
}
