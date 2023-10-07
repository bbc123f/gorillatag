using System;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x06000D9D RID: 3485 RVA: 0x0004FA50 File Offset: 0x0004DC50
	private void Awake()
	{
		if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
		{
			return;
		}
		float num = (float)(this.frequency * this.items.Length);
		if (this.spline.Loop || num == 1f)
		{
			num = 1f / num;
		}
		else
		{
			num = 1f / (num - 1f);
		}
		int num2 = 0;
		for (int i = 0; i < this.frequency; i++)
		{
			int j = 0;
			while (j < this.items.Length)
			{
				Transform transform = Object.Instantiate<Transform>(this.items[j]);
				Vector3 point = this.spline.GetPoint((float)num2 * num);
				transform.transform.localPosition = point;
				if (this.lookForward)
				{
					transform.transform.LookAt(point + this.spline.GetDirection((float)num2 * num));
				}
				transform.transform.parent = base.transform;
				j++;
				num2++;
			}
		}
	}

	// Token: 0x040010B0 RID: 4272
	public BezierSpline spline;

	// Token: 0x040010B1 RID: 4273
	public int frequency;

	// Token: 0x040010B2 RID: 4274
	public bool lookForward;

	// Token: 0x040010B3 RID: 4275
	public Transform[] items;
}
