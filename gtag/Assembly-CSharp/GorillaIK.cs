using System;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class GorillaIK : MonoBehaviour
{
	// Token: 0x06000995 RID: 2453 RVA: 0x0003A640 File Offset: 0x00038840
	private void Awake()
	{
		if (Application.isPlaying && !this.testInEditor)
		{
			this.dU = (this.leftUpperArm.position - this.leftLowerArm.position).magnitude;
			this.dL = (this.leftLowerArm.position - this.leftHand.position).magnitude;
			this.dMax = this.dU + this.dL - this.eps;
			this.initialUpperLeft = this.leftUpperArm.localRotation;
			this.initialLowerLeft = this.leftLowerArm.localRotation;
			this.initialUpperRight = this.rightUpperArm.localRotation;
			this.initialLowerRight = this.rightLowerArm.localRotation;
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0003A712 File Offset: 0x00038912
	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0003A71F File Offset: 0x0003891F
	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0003A72C File Offset: 0x0003892C
	private void ArmIK(ref Transform upperArm, ref Transform lowerArm, ref Transform hand, Quaternion initRotUpper, Quaternion initRotLower, Transform target)
	{
		upperArm.localRotation = initRotUpper;
		lowerArm.localRotation = initRotLower;
		float num = Mathf.Clamp((target.position - upperArm.position).magnitude, this.eps, this.dMax);
		float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (lowerArm.position - upperArm.position).normalized), -1f, 1f));
		float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((upperArm.position - lowerArm.position).normalized, (hand.position - lowerArm.position).normalized), -1f, 1f));
		float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (target.position - upperArm.position).normalized), -1f, 1f));
		float num5 = Mathf.Acos(Mathf.Clamp((this.dL * this.dL - this.dU * this.dU - num * num) / (-2f * this.dU * num), -1f, 1f));
		float num6 = Mathf.Acos(Mathf.Clamp((num * num - this.dU * this.dU - this.dL * this.dL) / (-2f * this.dU * this.dL), -1f, 1f));
		Vector3 normalized = Vector3.Cross(hand.position - upperArm.position, lowerArm.position - upperArm.position).normalized;
		Vector3 normalized2 = Vector3.Cross(hand.position - upperArm.position, target.position - upperArm.position).normalized;
		Quaternion rhs = Quaternion.AngleAxis((num5 - num2) * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized);
		Quaternion rhs2 = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(lowerArm.rotation) * normalized);
		Quaternion rhs3 = Quaternion.AngleAxis(num4 * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized2);
		this.newRotationUpper = upperArm.localRotation * rhs3 * rhs;
		this.newRotationLower = lowerArm.localRotation * rhs2;
		upperArm.localRotation = this.newRotationUpper;
		lowerArm.localRotation = this.newRotationLower;
		hand.rotation = target.rotation;
	}

	// Token: 0x04000BB3 RID: 2995
	public Transform headBone;

	// Token: 0x04000BB4 RID: 2996
	public Transform leftUpperArm;

	// Token: 0x04000BB5 RID: 2997
	public Transform leftLowerArm;

	// Token: 0x04000BB6 RID: 2998
	public Transform leftHand;

	// Token: 0x04000BB7 RID: 2999
	public Transform rightUpperArm;

	// Token: 0x04000BB8 RID: 3000
	public Transform rightLowerArm;

	// Token: 0x04000BB9 RID: 3001
	public Transform rightHand;

	// Token: 0x04000BBA RID: 3002
	public Transform targetLeft;

	// Token: 0x04000BBB RID: 3003
	public Transform targetRight;

	// Token: 0x04000BBC RID: 3004
	public Transform targetHead;

	// Token: 0x04000BBD RID: 3005
	public Quaternion initialUpperLeft;

	// Token: 0x04000BBE RID: 3006
	public Quaternion initialLowerLeft;

	// Token: 0x04000BBF RID: 3007
	public Quaternion initialUpperRight;

	// Token: 0x04000BC0 RID: 3008
	public Quaternion initialLowerRight;

	// Token: 0x04000BC1 RID: 3009
	public Quaternion newRotationUpper;

	// Token: 0x04000BC2 RID: 3010
	public Quaternion newRotationLower;

	// Token: 0x04000BC3 RID: 3011
	public float dU;

	// Token: 0x04000BC4 RID: 3012
	public float dL;

	// Token: 0x04000BC5 RID: 3013
	public float dMax;

	// Token: 0x04000BC6 RID: 3014
	public bool testInEditor;

	// Token: 0x04000BC7 RID: 3015
	public bool reset;

	// Token: 0x04000BC8 RID: 3016
	public bool testDefineRot;

	// Token: 0x04000BC9 RID: 3017
	public bool moveOnce;

	// Token: 0x04000BCA RID: 3018
	public float eps;

	// Token: 0x04000BCB RID: 3019
	public float upperArmAngle;

	// Token: 0x04000BCC RID: 3020
	public float elbowAngle;
}
