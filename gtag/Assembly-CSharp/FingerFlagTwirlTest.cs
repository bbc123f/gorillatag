using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class FingerFlagTwirlTest : MonoBehaviour
{
	// Token: 0x060000E3 RID: 227 RVA: 0x00008E28 File Offset: 0x00007028
	protected void FixedUpdate()
	{
		this.animTimes += Time.deltaTime * this.rotAnimDurations;
		this.animTimes.x = this.animTimes.x % 1f;
		this.animTimes.y = this.animTimes.y % 1f;
		this.animTimes.z = this.animTimes.z % 1f;
		base.transform.localRotation = Quaternion.Euler(this.rotXAnimCurve.Evaluate(this.animTimes.x) * this.rotAnimAmplitudes.x, this.rotYAnimCurve.Evaluate(this.animTimes.y) * this.rotAnimAmplitudes.y, this.rotZAnimCurve.Evaluate(this.animTimes.z) * this.rotAnimAmplitudes.z);
	}

	// Token: 0x0400012F RID: 303
	public Vector3 rotAnimDurations = new Vector3(0.2f, 0.1f, 0.5f);

	// Token: 0x04000130 RID: 304
	public Vector3 rotAnimAmplitudes = Vector3.one * 360f;

	// Token: 0x04000131 RID: 305
	public AnimationCurve rotXAnimCurve;

	// Token: 0x04000132 RID: 306
	public AnimationCurve rotYAnimCurve;

	// Token: 0x04000133 RID: 307
	public AnimationCurve rotZAnimCurve;

	// Token: 0x04000134 RID: 308
	private Vector3 animTimes = Vector3.zero;
}
