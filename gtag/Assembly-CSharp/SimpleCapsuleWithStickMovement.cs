using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	// Token: 0x1400000E RID: 14
	// (add) Token: 0x060002D2 RID: 722 RVA: 0x000120BC File Offset: 0x000102BC
	// (remove) Token: 0x060002D3 RID: 723 RVA: 0x000120F4 File Offset: 0x000102F4
	public event Action CameraUpdated;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x060002D4 RID: 724 RVA: 0x0001212C File Offset: 0x0001032C
	// (remove) Token: 0x060002D5 RID: 725 RVA: 0x00012164 File Offset: 0x00010364
	public event Action PreCharacterMove;

	// Token: 0x060002D6 RID: 726 RVA: 0x00012199 File Offset: 0x00010399
	private void Awake()
	{
		this._rigidbody = base.GetComponent<Rigidbody>();
		if (this.CameraRig == null)
		{
			this.CameraRig = base.GetComponentInChildren<OVRCameraRig>();
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x000121C1 File Offset: 0x000103C1
	private void Start()
	{
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x000121C4 File Offset: 0x000103C4
	private void FixedUpdate()
	{
		if (this.CameraUpdated != null)
		{
			this.CameraUpdated();
		}
		if (this.PreCharacterMove != null)
		{
			this.PreCharacterMove();
		}
		if (this.HMDRotatesPlayer)
		{
			this.RotatePlayerToHMD();
		}
		if (this.EnableLinearMovement)
		{
			this.StickMovement();
		}
		if (this.EnableRotation)
		{
			this.SnapTurn();
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00012224 File Offset: 0x00010424
	private void RotatePlayerToHMD()
	{
		Transform trackingSpace = this.CameraRig.trackingSpace;
		Transform centerEyeAnchor = this.CameraRig.centerEyeAnchor;
		Vector3 position = trackingSpace.position;
		Quaternion rotation = trackingSpace.rotation;
		base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
		trackingSpace.position = position;
		trackingSpace.rotation = rotation;
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00012290 File Offset: 0x00010490
	private void StickMovement()
	{
		Vector3 eulerAngles = this.CameraRig.centerEyeAnchor.rotation.eulerAngles;
		eulerAngles.z = (eulerAngles.x = 0f);
		Quaternion rotation = Quaternion.Euler(eulerAngles);
		Vector3 a = Vector3.zero;
		Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active);
		a += rotation * (vector.x * Vector3.right);
		a += rotation * (vector.y * Vector3.forward);
		this._rigidbody.MovePosition(this._rigidbody.position + a * this.Speed * Time.fixedDeltaTime);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00012354 File Offset: 0x00010554
	private void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, -this.RotationAngle);
				return;
			}
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, this.RotationAngle);
				return;
			}
		}
		else
		{
			this.ReadyToSnapTurn = true;
		}
	}

	// Token: 0x04000397 RID: 919
	public bool EnableLinearMovement = true;

	// Token: 0x04000398 RID: 920
	public bool EnableRotation = true;

	// Token: 0x04000399 RID: 921
	public bool HMDRotatesPlayer = true;

	// Token: 0x0400039A RID: 922
	public bool RotationEitherThumbstick;

	// Token: 0x0400039B RID: 923
	public float RotationAngle = 45f;

	// Token: 0x0400039C RID: 924
	public float Speed;

	// Token: 0x0400039D RID: 925
	public OVRCameraRig CameraRig;

	// Token: 0x0400039E RID: 926
	private bool ReadyToSnapTurn;

	// Token: 0x0400039F RID: 927
	private Rigidbody _rigidbody;
}
