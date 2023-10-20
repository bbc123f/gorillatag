using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class LerpScale : LerpComponent
{
	// Token: 0x06000BDC RID: 3036 RVA: 0x000497CC File Offset: 0x000479CC
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x04000F60 RID: 3936
	[Space]
	public Transform target;

	// Token: 0x04000F61 RID: 3937
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x04000F62 RID: 3938
	public Vector3 end = Vector3.one;

	// Token: 0x04000F63 RID: 3939
	public Vector3 current;

	// Token: 0x04000F64 RID: 3940
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
