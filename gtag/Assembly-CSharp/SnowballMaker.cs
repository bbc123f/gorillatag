using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class SnowballMaker : MonoBehaviour
{
	// Token: 0x06000110 RID: 272 RVA: 0x00009CA8 File Offset: 0x00007EA8
	protected void Awake()
	{
		List<SnowballThrowable> list = new List<SnowballThrowable>(this.snowballs.Length);
		int num = 0;
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
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
			Debug.LogError(string.Format("Found {0} null references in snowballs array.", num), this);
		}
		this.snowballs = list.ToArray();
		for (int j = 0; j < this.snowballs.Length; j++)
		{
			this.snowballs[j].throwableMakerIndex = j;
		}
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00009D44 File Offset: 0x00007F44
	protected void LateUpdate()
	{
		bool flag = false;
		SnowballThrowable[] array = this.snowballs;
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
		int num = this.isLeftHand ? instance.leftHandMaterialTouchIndex : instance.rightHandMaterialTouchIndex;
		if (num == 0)
		{
			return;
		}
		EquipmentInteractor instance2 = EquipmentInteractor.instance;
		bool flag2 = this.isLeftHand ? instance2.isLeftGrabbing : instance2.isRightGrabbing;
		bool flag3 = (this.isLeftHand ? instance2.leftHandHeldEquipment : instance2.rightHandHeldEquipment) != null;
		if (!flag2 || flag3)
		{
			return;
		}
		Transform transform = base.transform;
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			Transform transform2 = snowballThrowable.transform;
			if (snowballThrowable.matDataIndexes.Contains(num))
			{
				snowballThrowable.EnableSnowballLocal(true);
				snowballThrowable.velocityEstimator = this.velocityEstimator;
				transform2.position = transform.position;
				transform2.rotation = transform.rotation;
				return;
			}
		}
	}

	// Token: 0x04000173 RID: 371
	public bool isLeftHand;

	// Token: 0x04000174 RID: 372
	[RequiredListLength(1, 999, PrefabKind = PrefabKind.InstanceInScene)]
	public SnowballThrowable[] snowballs;

	// Token: 0x04000175 RID: 373
	public GorillaVelocityEstimator velocityEstimator;
}
