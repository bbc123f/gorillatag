using System.Collections.Generic;
using GorillaLocomotion;
using Sirenix.OdinInspector;
using UnityEngine;

public class SnowballMaker : MonoBehaviour
{
	public bool isLeftHand;

	[RequiredListLength(1, 999, PrefabKind = PrefabKind.InstanceInScene)]
	public SnowballThrowable[] snowballs;

	public GorillaVelocityEstimator velocityEstimator;

	protected void Awake()
	{
		List<SnowballThrowable> list = new List<SnowballThrowable>(snowballs.Length);
		int num = 0;
		SnowballThrowable[] array = snowballs;
		foreach (SnowballThrowable snowballThrowable in array)
		{
			if (snowballThrowable == null)
			{
				num++;
			}
			else
			{
				list.Add(snowballThrowable);
			}
		}
		if (num > 0)
		{
			Debug.LogError($"Found {num} null references in snowballs array.", this);
		}
		snowballs = list.ToArray();
		for (int j = 0; j < snowballs.Length; j++)
		{
			snowballs[j].throwableMakerIndex = j;
		}
	}

	protected void LateUpdate()
	{
		bool flag = false;
		SnowballThrowable[] array = snowballs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.activeSelf)
			{
				flag = true;
				break;
			}
		}
		if (flag || !Player.hasInstance || !EquipmentInteractor.hasInstance || !Player.hasInstance || !GorillaTagger.hasInstance || GorillaTagger.Instance.offlineVRRig == null)
		{
			return;
		}
		Player instance = Player.Instance;
		int num = (isLeftHand ? instance.leftHandMaterialTouchIndex : instance.rightHandMaterialTouchIndex);
		if (num == 0)
		{
			return;
		}
		EquipmentInteractor instance2 = EquipmentInteractor.instance;
		bool num2 = (isLeftHand ? instance2.isLeftGrabbing : instance2.isRightGrabbing);
		bool flag2 = (isLeftHand ? instance2.leftHandHeldEquipment : instance2.rightHandHeldEquipment) != null;
		if (!num2 || flag2)
		{
			return;
		}
		Transform transform = base.transform;
		array = snowballs;
		foreach (SnowballThrowable snowballThrowable in array)
		{
			Transform transform2 = snowballThrowable.transform;
			if (snowballThrowable.matDataIndexes.Contains(num))
			{
				snowballThrowable.EnableSnowballLocal(enable: true);
				snowballThrowable.velocityEstimator = velocityEstimator;
				transform2.position = transform.position;
				transform2.rotation = transform.rotation;
				break;
			}
		}
	}
}
