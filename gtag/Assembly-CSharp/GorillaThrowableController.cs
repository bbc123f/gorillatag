using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020001B6 RID: 438
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x06000B2E RID: 2862 RVA: 0x00044E87 File Offset: 0x00043087
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaThrowable"
		});
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00044EA4 File Offset: 0x000430A4
	private void LateUpdate()
	{
		if (this.testCanGrab)
		{
			this.testCanGrab = false;
			this.CanGrabAnObject(this.rightHandController, out this.returnCollider);
			Debug.Log(this.returnCollider.gameObject, this.returnCollider.gameObject);
		}
		if (this.leftHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.LeftHand))
			{
				if (this.leftHandGrabbedObject != null)
				{
					this.leftHandGrabbedObject.ThrowThisThingo();
					this.leftHandGrabbedObject = null;
				}
				this.leftHandIsGrabbing = false;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.LeftHand))
		{
			this.leftHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.leftHandController, out this.returnCollider))
			{
				this.leftHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.leftHandGrabbedObject.Grabbed(this.leftHandController);
			}
		}
		if (this.rightHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.RightHand))
			{
				if (this.rightHandGrabbedObject != null)
				{
					this.rightHandGrabbedObject.ThrowThisThingo();
					this.rightHandGrabbedObject = null;
				}
				this.rightHandIsGrabbing = false;
				return;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.RightHand))
		{
			this.rightHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.rightHandController, out this.returnCollider))
			{
				this.rightHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.rightHandGrabbedObject.Grabbed(this.rightHandController);
			}
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00044FF0 File Offset: 0x000431F0
	private bool CheckIfHandHasReleased(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue < 0.75f)
		{
			this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
			if (this.triggerValue < 0.75f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0004506C File Offset: 0x0004326C
	private bool CheckIfHandHasGrabbed(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue > 0.75f)
		{
			return true;
		}
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		return this.triggerValue > 0.75f;
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000450E8 File Offset: 0x000432E8
	private bool CanGrabAnObject(Transform handTransform, out Collider returnCollider)
	{
		this.magnitude = 100f;
		returnCollider = null;
		Debug.Log("trying:");
		if (Physics.OverlapSphereNonAlloc(handTransform.position, this.handRadius, this.colliders, this.gorillaThrowableLayerMask) > 0)
		{
			Debug.Log("found something!");
			this.minCollider = this.colliders[0];
			foreach (Collider collider in this.colliders)
			{
				if (collider != null)
				{
					Debug.Log("found this", collider);
					if ((collider.transform.position - handTransform.position).magnitude < this.magnitude)
					{
						this.minCollider = collider;
						this.magnitude = (collider.transform.position - handTransform.position).magnitude;
					}
				}
			}
			returnCollider = this.minCollider;
			return true;
		}
		return false;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000451D1 File Offset: 0x000433D1
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x04000E87 RID: 3719
	public Transform leftHandController;

	// Token: 0x04000E88 RID: 3720
	public Transform rightHandController;

	// Token: 0x04000E89 RID: 3721
	public bool leftHandIsGrabbing;

	// Token: 0x04000E8A RID: 3722
	public bool rightHandIsGrabbing;

	// Token: 0x04000E8B RID: 3723
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x04000E8C RID: 3724
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x04000E8D RID: 3725
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x04000E8E RID: 3726
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x04000E8F RID: 3727
	public float handRadius = 0.05f;

	// Token: 0x04000E90 RID: 3728
	private InputDevice rightDevice;

	// Token: 0x04000E91 RID: 3729
	private InputDevice leftDevice;

	// Token: 0x04000E92 RID: 3730
	private InputDevice inputDevice;

	// Token: 0x04000E93 RID: 3731
	private float triggerValue;

	// Token: 0x04000E94 RID: 3732
	private bool boolVar;

	// Token: 0x04000E95 RID: 3733
	private Collider[] colliders = new Collider[10];

	// Token: 0x04000E96 RID: 3734
	private Collider minCollider;

	// Token: 0x04000E97 RID: 3735
	private Collider returnCollider;

	// Token: 0x04000E98 RID: 3736
	private float magnitude;

	// Token: 0x04000E99 RID: 3737
	public bool testCanGrab;

	// Token: 0x04000E9A RID: 3738
	private int gorillaThrowableLayerMask;
}
