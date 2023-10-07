using System;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class LerpScale : LerpComponent
{
	// Token: 0x06000BD6 RID: 3030 RVA: 0x00049564 File Offset: 0x00047764
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x04000F5C RID: 3932
	[Space]
	public Transform target;

	// Token: 0x04000F5D RID: 3933
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x04000F5E RID: 3934
	public Vector3 end = Vector3.one;

	// Token: 0x04000F5F RID: 3935
	public Vector3 current;

	// Token: 0x04000F60 RID: 3936
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
