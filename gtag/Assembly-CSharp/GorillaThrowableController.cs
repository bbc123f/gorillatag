using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020001B7 RID: 439
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x06000B34 RID: 2868 RVA: 0x000450EF File Offset: 0x000432EF
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaThrowable"
		});
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0004510C File Offset: 0x0004330C
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

	// Token: 0x06000B36 RID: 2870 RVA: 0x00045258 File Offset: 0x00043458
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

	// Token: 0x06000B37 RID: 2871 RVA: 0x000452D4 File Offset: 0x000434D4
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

	// Token: 0x06000B38 RID: 2872 RVA: 0x00045350 File Offset: 0x00043550
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

	// Token: 0x06000B39 RID: 2873 RVA: 0x00045439 File Offset: 0x00043639
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x04000E8B RID: 3723
	public Transform leftHandController;

	// Token: 0x04000E8C RID: 3724
	public Transform rightHandController;

	// Token: 0x04000E8D RID: 3725
	public bool leftHandIsGrabbing;

	// Token: 0x04000E8E RID: 3726
	public bool rightHandIsGrabbing;

	// Token: 0x04000E8F RID: 3727
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x04000E90 RID: 3728
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x04000E91 RID: 3729
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x04000E92 RID: 3730
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x04000E93 RID: 3731
	public float handRadius = 0.05f;

	// Token: 0x04000E94 RID: 3732
	private InputDevice rightDevice;

	// Token: 0x04000E95 RID: 3733
	private InputDevice leftDevice;

	// Token: 0x04000E96 RID: 3734
	private InputDevice inputDevice;

	// Token: 0x04000E97 RID: 3735
	private float triggerValue;

	// Token: 0x04000E98 RID: 3736
	private bool boolVar;

	// Token: 0x04000E99 RID: 3737
	private Collider[] colliders = new Collider[10];

	// Token: 0x04000E9A RID: 3738
	private Collider minCollider;

	// Token: 0x04000E9B RID: 3739
	private Collider returnCollider;

	// Token: 0x04000E9C RID: 3740
	private float magnitude;

	// Token: 0x04000E9D RID: 3741
	public bool testCanGrab;

	// Token: 0x04000E9E RID: 3742
	private int gorillaThrowableLayerMask;
}
