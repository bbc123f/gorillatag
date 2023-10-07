using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class LengthStiffnessComparison : MonoBehaviour
{
	// Token: 0x06000017 RID: 23 RVA: 0x00002503 File Offset: 0x00000703
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002510 File Offset: 0x00000710
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			foreach (Transform transform in array)
			{
				Vector3 position = transform.position;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 6f);
		foreach (Transform transform2 in array)
		{
			Vector3 position2 = transform2.position;
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
			transform2.rotation = Quaternion.AngleAxis(this.Tilt * (1f - num3), Vector3.right);
		}
	}

	// Token: 0x0400000B RID: 11
	public float Run = 11f;

	// Token: 0x0400000C RID: 12
	public float Tilt = 15f;

	// Token: 0x0400000D RID: 13
	public float Period = 3f;

	// Token: 0x0400000E RID: 14
	public float Rest = 3f;

	// Token: 0x0400000F RID: 15
	public Transform BonesA;

	// Token: 0x04000010 RID: 16
	public Transform BonesB;

	// Token: 0x04000011 RID: 17
	private float m_timer;
}
