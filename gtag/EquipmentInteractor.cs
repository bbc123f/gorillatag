using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

public class EquipmentInteractor : MonoBehaviour
{
	public static volatile EquipmentInteractor instance;

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

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			hasInstance = true;
		}
		else if (instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		leftHandSet = false;
		rightHandSet = false;
		autoGrabLeft = true;
		autoGrabRight = true;
		gorillaInteractableLayerMask = LayerMask.GetMask("GorillaInteractable");
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			hasInstance = false;
			instance = null;
		}
	}

	public void ReleaseRightHand()
	{
		if (rightHandHeldEquipment != null)
		{
			rightHandHeldEquipment.OnRelease(null, rightHand);
		}
		if (leftHandHeldEquipment != null)
		{
			leftHandHeldEquipment.OnRelease(null, rightHand);
		}
		autoGrabRight = true;
	}

	public void ReleaseLeftHand()
	{
		if (rightHandHeldEquipment != null)
		{
			rightHandHeldEquipment.OnRelease(null, leftHand);
		}
		if (leftHandHeldEquipment != null)
		{
			leftHandHeldEquipment.OnRelease(null, leftHand);
		}
		autoGrabLeft = true;
	}

	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return leftHandHeldEquipment;
		}
		return rightHandHeldEquipment;
	}

	private void LateUpdate()
	{
		CheckInputValue(isLeftHand: true);
		isLeftGrabbing = (wasLeftGrabPressed && grabValue > grabThreshold - grabHysteresis) || (!wasLeftGrabPressed && grabValue > grabThreshold + grabHysteresis);
		if ((bool)leftClimber && leftClimber.isClimbing)
		{
			isLeftGrabbing = false;
		}
		CheckInputValue(isLeftHand: false);
		isRightGrabbing = (wasRightGrabPressed && grabValue > grabThreshold - grabHysteresis) || (!wasRightGrabPressed && grabValue > grabThreshold + grabHysteresis);
		if ((bool)rightClimber && rightClimber.isClimbing)
		{
			isRightGrabbing = false;
		}
		FireHandInteractions(leftHand, isLeftHand: true);
		FireHandInteractions(rightHand, isLeftHand: false);
		if (!isRightGrabbing && wasRightGrabPressed)
		{
			ReleaseRightHand();
		}
		if (!isLeftGrabbing && wasLeftGrabPressed)
		{
			ReleaseLeftHand();
		}
		wasLeftGrabPressed = isLeftGrabbing;
		wasRightGrabPressed = isRightGrabbing;
	}

	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand)
	{
		if (isLeftHand)
		{
			justGrabbed = (isLeftGrabbing && !wasLeftGrabPressed) || (isLeftGrabbing && autoGrabLeft);
			justReleased = leftHandHeldEquipment != null && !isLeftGrabbing && wasLeftGrabPressed;
		}
		else
		{
			justGrabbed = (isRightGrabbing && !wasRightGrabPressed) || (isRightGrabbing && autoGrabRight);
			justReleased = rightHandHeldEquipment != null && !isRightGrabbing && wasRightGrabPressed;
		}
		foreach (InteractionPoint item in isLeftHand ? overlapInteractionPointsLeft : overlapInteractionPointsRight)
		{
			bool num = (isLeftHand ? (leftHandHeldEquipment != null) : (rightHandHeldEquipment != null));
			bool flag = (isLeftHand ? disableLeftGrab : disableRightGrab);
			if (!num && !flag && item != null)
			{
				if (justGrabbed)
				{
					item.parentTransferrableObject.OnGrab(item, interactingHand);
				}
				else
				{
					item.parentTransferrableObject.OnHover(item, interactingHand);
				}
			}
			if (!justReleased)
			{
				continue;
			}
			tempZone = item.GetComponent<DropZone>();
			if (!(tempZone != null))
			{
				continue;
			}
			if (interactingHand == leftHand)
			{
				if (leftHandHeldEquipment != null)
				{
					leftHandHeldEquipment.OnRelease(tempZone, interactingHand);
				}
			}
			else if (rightHandHeldEquipment != null)
			{
				rightHandHeldEquipment.OnRelease(tempZone, interactingHand);
			}
		}
	}

	public void UpdateHandEquipment(HoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment == rightHandHeldEquipment)
			{
				rightHandHeldEquipment = null;
			}
			if (leftHandHeldEquipment != null)
			{
				leftHandHeldEquipment.DropItemCleanup();
			}
			leftHandHeldEquipment = newEquipment;
			autoGrabLeft = false;
		}
		else
		{
			if (newEquipment == leftHandHeldEquipment)
			{
				leftHandHeldEquipment = null;
			}
			if (rightHandHeldEquipment != null)
			{
				rightHandHeldEquipment.DropItemCleanup();
			}
			rightHandHeldEquipment = newEquipment;
			autoGrabRight = false;
		}
	}

	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			grabValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			tempValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		}
		else
		{
			grabValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
			tempValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		}
		grabValue = Mathf.Max(grabValue, tempValue);
	}

	public void ForceDropEquipment(HoldableObject equipment)
	{
		if (rightHandHeldEquipment == equipment)
		{
			rightHandHeldEquipment = null;
		}
		if (leftHandHeldEquipment == equipment)
		{
			leftHandHeldEquipment = null;
		}
	}
}
