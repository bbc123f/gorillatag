using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x0600092D RID: 2349 RVA: 0x00037844 File Offset: 0x00035A44
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2 = new Color(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x04000B30 RID: 2864
	public ParticleSystem particleSystem;

	// Token: 0x04000B31 RID: 2865
	public float gradientColorPower = 2f;

	// Token: 0x04000B32 RID: 2866
	public bool useLinearColor = true;
}
