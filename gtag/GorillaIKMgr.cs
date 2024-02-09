using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class GorillaIKMgr : MonoBehaviour
{
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.cachedInput = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.cachedOutput = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.firstFrame = true;
	}

	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.cachedOutput.Dispose();
	}

	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
	}

	public void DeregisterIK(GorillaIK ik)
	{
		this.ikList.Remove(ik);
		this.actualListSz -= 2;
	}

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

	public void LateUpdate()
	{
		this.CopyInput();
		GorillaIKMgr.IKJob ikjob = new GorillaIKMgr.IKJob
		{
			input = this.cachedInput,
			output = this.cachedOutput
		};
		this.jobHandle = ikjob.Schedule(this.actualListSz, 1, default(JobHandle));
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

	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	private const int MaxSize = 20;

	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	private NativeArray<GorillaIKMgr.IKInput> cachedInput;

	private NativeArray<GorillaIKMgr.IKOutput> cachedOutput;

	private int actualListSz;

	private JobHandle jobHandle;

	private bool firstFrame = true;

	private struct IKInput
	{
		public Vector3 handPos;

		public Vector3 upperArmPos;

		public Quaternion upperArmRot;

		public Quaternion upperArmLocalRot;

		public Vector3 lowerArmPos;

		public Quaternion lowerArmRot;

		public Quaternion lowerArmLocalRot;

		public Quaternion initRotLower;

		public Quaternion initRotUpper;

		public Vector3 targetPos;

		public float eps;
	}

	private struct IKOutput
	{
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
		}

		public Quaternion upperArmLocalRot;

		public Quaternion lowerArmLocalRot;
	}

	private struct IKJob : IJobParallelFor
	{
		public void Execute(int i)
		{
			float eps = this.input[i].eps;
			float magnitude = (this.input[i].upperArmPos - this.input[i].lowerArmPos).magnitude;
			float magnitude2 = (this.input[i].lowerArmPos - this.input[i].handPos).magnitude;
			float num = magnitude + magnitude2 - eps;
			float num2 = Mathf.Clamp((this.input[i].targetPos - this.input[i].upperArmPos).magnitude, eps, num);
			float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((this.input[i].handPos - this.input[i].upperArmPos).normalized, (this.input[i].lowerArmPos - this.input[i].upperArmPos).normalized), -1f, 1f));
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((this.input[i].upperArmPos - this.input[i].lowerArmPos).normalized, (this.input[i].handPos - this.input[i].lowerArmPos).normalized), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((this.input[i].handPos - this.input[i].upperArmPos).normalized, (this.input[i].targetPos - this.input[i].upperArmPos).normalized), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num2 * num2) / (-2f * magnitude * num2), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((num2 * num2 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized = Vector3.Cross(this.input[i].handPos - this.input[i].upperArmPos, this.input[i].lowerArmPos - this.input[i].upperArmPos).normalized;
			Vector3 normalized2 = Vector3.Cross(this.input[i].handPos - this.input[i].upperArmPos, this.input[i].targetPos - this.input[i].upperArmPos).normalized;
			Quaternion quaternion = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(this.input[i].upperArmRot) * normalized);
			Quaternion quaternion2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(this.input[i].lowerArmRot) * normalized);
			Quaternion quaternion3 = Quaternion.AngleAxis(num5 * 57.29578f, Quaternion.Inverse(this.input[i].upperArmRot) * normalized2);
			Quaternion quaternion4 = this.input[i].initRotUpper * quaternion3 * quaternion;
			Quaternion quaternion5 = this.input[i].initRotLower * quaternion2;
			this.output[i] = new GorillaIKMgr.IKOutput(quaternion4, quaternion5);
		}

		[ReadOnly]
		public NativeArray<GorillaIKMgr.IKInput> input;

		public NativeArray<GorillaIKMgr.IKOutput> output;
	}
}
