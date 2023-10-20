using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x06000DA3 RID: 3491 RVA: 0x0004FCB0 File Offset: 0x0004DEB0
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

	// Token: 0x040010B5 RID: 4277
	public BezierSpline spline;

	// Token: 0x040010B6 RID: 4278
	public int frequency;

	// Token: 0x040010B7 RID: 4279
	public bool lookForward;

	// Token: 0x040010B8 RID: 4280
	public Transform[] items;
}
