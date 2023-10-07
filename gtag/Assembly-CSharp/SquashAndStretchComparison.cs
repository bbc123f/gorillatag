using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class SquashAndStretchComparison : MonoBehaviour
{
	// Token: 0x0600001D RID: 29 RVA: 0x00002940 File Offset: 0x00000B40
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002950 File Offset: 0x00000B50
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
		float t = 1f - Mathf.Pow(1f - num2, 6f);
		foreach (Transform transform2 in array)
		{
			Vector3 position2 = transform2.position;
			position2.z = Mathf.Lerp(-num, num, t);
			transform2.position = position2;
		}
	}

	// Token: 0x0400001B RID: 27
	public float Run = 11f;

	// Token: 0x0400001C RID: 28
	public float Period = 3f;

	// Token: 0x0400001D RID: 29
	public float Rest = 3f;

	// Token: 0x0400001E RID: 30
	public Transform BonesA;

	// Token: 0x0400001F RID: 31
	public Transform BonesB;

	// Token: 0x04000020 RID: 32
	private float m_timer;
}
