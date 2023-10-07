using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000996 RID: 2454 RVA: 0x0003AA70 File Offset: 0x00038C70
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0003AA77 File Offset: 0x00038C77
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.cachedInput = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.cachedOutput = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.firstFrame = true;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0003AAA4 File Offset: 0x00038CA4
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.cachedOutput.Dispose();
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0003AAC7 File Offset: 0x00038CC7
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0003AAE3 File Offset: 0x00038CE3
	public void DeregisterIK(GorillaIK ik)
	{
		this.ikList.Remove(ik);
		this.actualListSz -= 2;
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x0003AB00 File Offset: 0x00038D00
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i += 2)
		{
			this.ikList[i / 2].leftUpperArm.localRotation = this.ikList[i / 2].initialUpperLeft;
			this.ikList[i / 2].leftLowerArm.localRotation = this.ikList[i / 2].initialLowerLeft;
			this.ikList[i / 2].rightUpperArm.localRotation = this.ikList[i / 2].initialUpperRight;
			this.ikList[i / 2].rightLowerArm.localRotation = this.ikList[i / 2].initialLowerRight;
			this.cachedInput[i] = new GorillaIKMgr.IKInput
			{
				handPos = this.ikList[i / 2].leftHand.position,
				upperArmPos = this.ikList[i / 2].leftUpperArm.position,
				upperArmRot = this.ikList[i / 2].leftUpperArm.rotation,
				upperArmLocalRot = this.ikList[i / 2].leftUpperArm.localRotation,
				lowerArmPos = this.ikList[i / 2].leftLowerArm.position,
				lowerArmRot = this.ikList[i / 2].leftLowerArm.rotation,
				lowerArmLocalRot = this.ikList[i / 2].leftLowerArm.localRotation,
				initRotLower = this.ikList[i / 2].initialLowerLeft,
				initRotUpper = this.ikList[i / 2].initialUpperLeft,
				targetPos = this.ikList[i / 2].targetLeft.position,
				eps = this.ikList[i / 2].eps
			};
			this.cachedInput[i + 1] = new GorillaIKMgr.IKInput
			{
				handPos = this.ikList[i / 2].rightHand.position,
				upperArmPos = this.ikList[i / 2].rightUpperArm.position,
				upperArmRot = this.ikList[i / 2].rightUpperArm.rotation,
				upperArmLocalRot = this.ikList[i / 2].rightUpperArm.localRotation,
				lowerArmPos = this.ikList[i / 2].rightLowerArm.position,
				lowerArmRot = this.ikList[i / 2].rightLowerArm.rotation,
				lowerArmLocalRot = this.ikList[i / 2].rightLowerArm.localRotation,
				initRotLower = this.ikList[i / 2].initialLowerRight,
				initRotUpper = this.ikList[i / 2].initialUpperRight,
				targetPos = this.ikList[i / 2].targetRight.position,
				eps = this.ikList[i / 2].eps
			};
		}
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0003AE88 File Offset: 0x00039088
	private void CopyOutput()
	{
		for (int i = 0; i < this.ikList.Count; i++)
		{
			this.ikList[i].leftUpperArm.localRotation = this.cachedOutput[i * 2].upperArmLocalRot;
			this.ikList[i].leftLowerArm.localRotation = this.cachedOutput[i * 2].lowerArmLocalRot;
			this.ikList[i].rightUpperArm.localRotation = this.cachedOutput[i * 2 + 1].upperArmLocalRot;
			this.ikList[i].rightLowerArm.localRotation = this.cachedOutput[i * 2 + 1].lowerArmLocalRot;
		}
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0003AF5C File Offset: 0x0003915C
	public void LateUpdate()
	{
		this.CopyInput();
		GorillaIKMgr.IKJob jobData = new GorillaIKMgr.IKJob
		{
			input = this.cachedInput,
			output = this.cachedOutput
		};
		this.jobHandle = jobData.Schedule(this.actualListSz, 1, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		foreach (GorillaIK gorillaIK in this.ikList)
		{
			gorillaIK.headBone.rotation = gorillaIK.targetHead.rotation;
			gorillaIK.leftHand.rotation = gorillaIK.targetLeft.rotation;
			gorillaIK.rightHand.rotation = gorillaIK.targetRight.rotation;
		}
		this.firstFrame = false;
	}

	// Token: 0x04000BC9 RID: 3017
	private static GorillaIKMgr _instance;

	// Token: 0x04000BCA RID: 3018
	private const int MaxSize = 20;

	// Token: 0x04000BCB RID: 3019
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04000BCC RID: 3020
	private NativeArray<GorillaIKMgr.IKInput> cachedInput;

	// Token: 0x04000BCD RID: 3021
	private NativeArray<GorillaIKMgr.IKOutput> cachedOutput;

	// Token: 0x04000BCE RID: 3022
	private int actualListSz;

	// Token: 0x04000BCF RID: 3023
	private JobHandle jobHandle;

	// Token: 0x04000BD0 RID: 3024
	private bool firstFrame = true;

	// Token: 0x02000427 RID: 1063
	private struct IKInput
	{
		// Token: 0x04001D30 RID: 7472
		public Vector3 handPos;

		// Token: 0x04001D31 RID: 7473
		public Vector3 upperArmPos;

		// Token: 0x04001D32 RID: 7474
		public Quaternion upperArmRot;

		// Token: 0x04001D33 RID: 7475
		public Quaternion upperArmLocalRot;

		// Token: 0x04001D34 RID: 7476
		public Vector3 lowerArmPos;

		// Token: 0x04001D35 RID: 7477
		public Quaternion lowerArmRot;

		// Token: 0x04001D36 RID: 7478
		public Quaternion lowerArmLocalRot;

		// Token: 0x04001D37 RID: 7479
		public Quaternion initRotLower;

		// Token: 0x04001D38 RID: 7480
		public Quaternion initRotUpper;

		// Token: 0x04001D39 RID: 7481
		public Vector3 targetPos;

		// Token: 0x04001D3A RID: 7482
		public float eps;
	}

	// Token: 0x02000428 RID: 1064
	private struct IKOutput
	{
		// Token: 0x06001C6F RID: 7279 RVA: 0x00097B0D File Offset: 0x00095D0D
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
		}

		// Token: 0x04001D3B RID: 7483
		public Quaternion upperArmLocalRot;

		// Token: 0x04001D3C RID: 7484
		public Quaternion lowerArmLocalRot;
	}

	// Token: 0x02000429 RID: 1065
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x06001C70 RID: 7280 RVA: 0x00097B20 File Offset: 0x00095D20
		public void Execute(int i)
		{
			float eps = this.input[i].eps;
			float magnitude = (this.input[i].upperArmPos - this.input[i].lowerArmPos).magnitude;
			float magnitude2 = (this.input[i].lowerArmPos - this.input[i].handPos).magnitude;
			float max = magnitude + magnitude2 - eps;
			float num = Mathf.Clamp((this.input[i].targetPos - this.input[i].upperArmPos).magnitude, eps, max);
			float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((this.input[i].handPos - this.input[i].upperArmPos).normalized, (this.input[i].lowerArmPos - this.input[i].upperArmPos).normalized), -1f, 1f));
			float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((this.input[i].upperArmPos - this.input[i].lowerArmPos).normalized, (this.input[i].handPos - this.input[i].lowerArmPos).normalized), -1f, 1f));
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((this.input[i].handPos - this.input[i].upperArmPos).normalized, (this.input[i].targetPos - this.input[i].upperArmPos).normalized), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num * num) / (-2f * magnitude * num), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp((num * num - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized = Vector3.Cross(this.input[i].handPos - this.input[i].upperArmPos, this.input[i].lowerArmPos - this.input[i].upperArmPos).normalized;
			Vector3 normalized2 = Vector3.Cross(this.input[i].handPos - this.input[i].upperArmPos, this.input[i].targetPos - this.input[i].upperArmPos).normalized;
			Quaternion rhs = Quaternion.AngleAxis((num5 - num2) * 57.29578f, Quaternion.Inverse(this.input[i].upperArmRot) * normalized);
			Quaternion rhs2 = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(this.input[i].lowerArmRot) * normalized);
			Quaternion rhs3 = Quaternion.AngleAxis(num4 * 57.29578f, Quaternion.Inverse(this.input[i].upperArmRot) * normalized2);
			Quaternion upperArmLocalRot_ = this.input[i].initRotUpper * rhs3 * rhs;
			Quaternion lowerArmLocalRot_ = this.input[i].initRotLower * rhs2;
			this.output[i] = new GorillaIKMgr.IKOutput(upperArmLocalRot_, lowerArmLocalRot_);
		}

		// Token: 0x04001D3D RID: 7485
		[ReadOnly]
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x04001D3E RID: 7486
		public NativeArray<GorillaIKMgr.IKOutput> output;
	}
}
