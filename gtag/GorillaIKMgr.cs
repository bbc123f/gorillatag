using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class GorillaIKMgr : MonoBehaviour
{
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
		public Quaternion upperArmLocalRot;

		public Quaternion lowerArmLocalRot;

		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			upperArmLocalRot = upperArmLocalRot_;
			lowerArmLocalRot = lowerArmLocalRot_;
		}
	}

	private struct IKJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<IKInput> input;

		public NativeArray<IKOutput> output;

		public void Execute(int i)
		{
			float eps = input[i].eps;
			float magnitude = (input[i].upperArmPos - input[i].lowerArmPos).magnitude;
			float magnitude2 = (input[i].lowerArmPos - input[i].handPos).magnitude;
			float max = magnitude + magnitude2 - eps;
			float num = Mathf.Clamp((input[i].targetPos - input[i].upperArmPos).magnitude, eps, max);
			float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((input[i].handPos - input[i].upperArmPos).normalized, (input[i].lowerArmPos - input[i].upperArmPos).normalized), -1f, 1f));
			float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((input[i].upperArmPos - input[i].lowerArmPos).normalized, (input[i].handPos - input[i].lowerArmPos).normalized), -1f, 1f));
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((input[i].handPos - input[i].upperArmPos).normalized, (input[i].targetPos - input[i].upperArmPos).normalized), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num * num) / (-2f * magnitude * num), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp((num * num - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized = Vector3.Cross(input[i].handPos - input[i].upperArmPos, input[i].lowerArmPos - input[i].upperArmPos).normalized;
			Vector3 normalized2 = Vector3.Cross(input[i].handPos - input[i].upperArmPos, input[i].targetPos - input[i].upperArmPos).normalized;
			Quaternion quaternion = Quaternion.AngleAxis((num5 - num2) * 57.29578f, Quaternion.Inverse(input[i].upperArmRot) * normalized);
			Quaternion quaternion2 = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(input[i].lowerArmRot) * normalized);
			Quaternion quaternion3 = Quaternion.AngleAxis(num4 * 57.29578f, Quaternion.Inverse(input[i].upperArmRot) * normalized2);
			Quaternion upperArmLocalRot_ = input[i].initRotUpper * quaternion3 * quaternion;
			Quaternion lowerArmLocalRot_ = input[i].initRotLower * quaternion2;
			output[i] = new IKOutput(upperArmLocalRot_, lowerArmLocalRot_);
		}
	}

	private static GorillaIKMgr _instance;

	private const int MaxSize = 20;

	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	private NativeArray<IKInput> cachedInput;

	private NativeArray<IKOutput> cachedOutput;

	private int actualListSz;

	private JobHandle jobHandle;

	private bool firstFrame = true;

	public static GorillaIKMgr Instance => _instance;

	private void Awake()
	{
		_instance = this;
		cachedInput = new NativeArray<IKInput>(40, Allocator.Persistent);
		cachedOutput = new NativeArray<IKOutput>(40, Allocator.Persistent);
		firstFrame = true;
	}

	private void OnDestroy()
	{
		jobHandle.Complete();
		cachedInput.Dispose();
		cachedOutput.Dispose();
	}

	public void RegisterIK(GorillaIK ik)
	{
		ikList.Add(ik);
		actualListSz += 2;
	}

	public void DeregisterIK(GorillaIK ik)
	{
		ikList.Remove(ik);
		actualListSz -= 2;
	}

	private void CopyInput()
	{
		for (int i = 0; i < actualListSz; i += 2)
		{
			ikList[i / 2].leftUpperArm.localRotation = ikList[i / 2].initialUpperLeft;
			ikList[i / 2].leftLowerArm.localRotation = ikList[i / 2].initialLowerLeft;
			ikList[i / 2].rightUpperArm.localRotation = ikList[i / 2].initialUpperRight;
			ikList[i / 2].rightLowerArm.localRotation = ikList[i / 2].initialLowerRight;
			cachedInput[i] = new IKInput
			{
				handPos = ikList[i / 2].leftHand.position,
				upperArmPos = ikList[i / 2].leftUpperArm.position,
				upperArmRot = ikList[i / 2].leftUpperArm.rotation,
				upperArmLocalRot = ikList[i / 2].leftUpperArm.localRotation,
				lowerArmPos = ikList[i / 2].leftLowerArm.position,
				lowerArmRot = ikList[i / 2].leftLowerArm.rotation,
				lowerArmLocalRot = ikList[i / 2].leftLowerArm.localRotation,
				initRotLower = ikList[i / 2].initialLowerLeft,
				initRotUpper = ikList[i / 2].initialUpperLeft,
				targetPos = ikList[i / 2].targetLeft.position,
				eps = ikList[i / 2].eps
			};
			cachedInput[i + 1] = new IKInput
			{
				handPos = ikList[i / 2].rightHand.position,
				upperArmPos = ikList[i / 2].rightUpperArm.position,
				upperArmRot = ikList[i / 2].rightUpperArm.rotation,
				upperArmLocalRot = ikList[i / 2].rightUpperArm.localRotation,
				lowerArmPos = ikList[i / 2].rightLowerArm.position,
				lowerArmRot = ikList[i / 2].rightLowerArm.rotation,
				lowerArmLocalRot = ikList[i / 2].rightLowerArm.localRotation,
				initRotLower = ikList[i / 2].initialLowerRight,
				initRotUpper = ikList[i / 2].initialUpperRight,
				targetPos = ikList[i / 2].targetRight.position,
				eps = ikList[i / 2].eps
			};
		}
	}

	private void CopyOutput()
	{
		for (int i = 0; i < ikList.Count; i++)
		{
			ikList[i].leftUpperArm.localRotation = cachedOutput[i * 2].upperArmLocalRot;
			ikList[i].leftLowerArm.localRotation = cachedOutput[i * 2].lowerArmLocalRot;
			ikList[i].rightUpperArm.localRotation = cachedOutput[i * 2 + 1].upperArmLocalRot;
			ikList[i].rightLowerArm.localRotation = cachedOutput[i * 2 + 1].lowerArmLocalRot;
		}
	}

	public void LateUpdate()
	{
		CopyInput();
		IKJob iKJob = default(IKJob);
		iKJob.input = cachedInput;
		iKJob.output = cachedOutput;
		IKJob jobData = iKJob;
		jobHandle = jobData.Schedule(actualListSz, 1);
		jobHandle.Complete();
		CopyOutput();
		foreach (GorillaIK ik in ikList)
		{
			ik.headBone.rotation = ik.targetHead.rotation;
			ik.leftHand.rotation = ik.targetLeft.rotation;
			ik.rightHand.rotation = ik.targetRight.rotation;
		}
		firstFrame = false;
	}
}
