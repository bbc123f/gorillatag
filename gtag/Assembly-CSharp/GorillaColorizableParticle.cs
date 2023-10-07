using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x06000929 RID: 2345 RVA: 0x00037944 File Offset: 0x00035B44
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2 = new Color(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x04000B2C RID: 2860
	public ParticleSystem particleSystem;

	// Token: 0x04000B2D RID: 2861
	public float gradientColorPower = 2f;

	// Token: 0x04000B2E RID: 2862
	public bool useLinearColor = true;
}
