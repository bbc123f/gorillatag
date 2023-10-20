using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600099A RID: 2458 RVA: 0x0003AA28 File Offset: 0x00038C28
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x0003AA2F File Offset: 0x00038C2F
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.cachedInput = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.cachedOutput = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.firstFrame = true;
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0003AA5C File Offset: 0x00038C5C
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.cachedOutput.Dispose();
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0003AA7F File Offset: 0x00038C7F
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0003AA9B File Offset: 0x00038C9B
	public void DeregisterIK(GorillaIK ik)
	{
		this.ikList.Remove(ik);
		this.actualListSz -= 2;
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0003AAB8 File Offset: 0x00038CB8
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

	// Token: 0x060009A0 RID: 2464 RVA: 0x0003AE40 File Offset: 0x00039040
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

	// Token: 0x060009A1 RID: 2465 RVA: 0x0003AF14 File Offset: 0x00039114
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

	// Token: 0x04000BCD RID: 3021
	private static GorillaIKMgr _instance;

	// Token: 0x04000BCE RID: 3022
	private const int MaxSize = 20;

	// Token: 0x04000BCF RID: 3023
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04000BD0 RID: 3024
	private NativeArray<GorillaIKMgr.IKInput> cachedInput;

	// Token: 0x04000BD1 RID: 3025
	private NativeArray<GorillaIKMgr.IKOutput> cachedOutput;

	// Token: 0x04000BD2 RID: 3026
	private int actualListSz;

	// Token: 0x04000BD3 RID: 3027
	private JobHandle jobHandle;

	// Token: 0x04000BD4 RID: 3028
	private bool firstFrame = true;

	// Token: 0x02000429 RID: 1065
	private struct IKInput
	{
		// Token: 0x04001D3D RID: 7485
		public Vector3 handPos;

		// Token: 0x04001D3E RID: 7486
		public Vector3 upperArmPos;

		// Token: 0x04001D3F RID: 7487
		public Quaternion upperArmRot;

		// Token: 0x04001D40 RID: 7488
		public Quaternion upperArmLocalRot;

		// Token: 0x04001D41 RID: 7489
		public Vector3 lowerArmPos;

		// Token: 0x04001D42 RID: 7490
		public Quaternion lowerArmRot;

		// Token: 0x04001D43 RID: 7491
		public Quaternion lowerArmLocalRot;

		// Token: 0x04001D44 RID: 7492
		public Quaternion initRotLower;

		// Token: 0x04001D45 RID: 7493
		public Quaternion initRotUpper;

		// Token: 0x04001D46 RID: 7494
		public Vector3 targetPos;

		// Token: 0x04001D47 RID: 7495
		public float eps;
	}

	// Token: 0x0200042A RID: 1066
	private struct IKOutput
	{
		// Token: 0x06001C78 RID: 7288 RVA: 0x00097FF5 File Offset: 0x000961F5
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
		}

		// Token: 0x04001D48 RID: 7496
		public Quaternion upperArmLocalRot;

		// Token: 0x04001D49 RID: 7497
		public Quaternion lowerArmLocalRot;
	}

	// Token: 0x0200042B RID: 1067
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x06001C79 RID: 7289 RVA: 0x00098008 File Offset: 0x00096208
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

		// Token: 0x04001D4A RID: 7498
		[ReadOnly]
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x04001D4B RID: 7499
		public NativeArray<GorillaIKMgr.IKOutput> output;
	}
}
