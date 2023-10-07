using System;

namespace GTMathUtil
{
	// Token: 0x020002A7 RID: 679
	internal class CriticalSpringDamper
	{
		// Token: 0x060011AF RID: 4527 RVA: 0x00064910 File Offset: 0x00062B10
		private static float halflife_to_damping(float halflife, float eps = 1E-05f)
		{
			return 2.7725887f / (halflife + eps);
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0006491B File Offset: 0x00062B1B
		private static float fast_negexp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00064940 File Offset: 0x00062B40
		public float Update(float dt)
		{
			float num = CriticalSpringDamper.halflife_to_damping(this.halfLife, 1E-05f) / 2f;
			float num2 = this.x - this.xGoal;
			float num3 = this.curVel + num2 * num;
			float num4 = CriticalSpringDamper.fast_negexp(num * dt);
			this.x = num4 * (num2 + num3 * dt) + this.xGoal;
			this.curVel = num4 * (this.curVel - num3 * num * dt);
			return this.x;
		}

		// Token: 0x04001465 RID: 5221
		public float x;

		// Token: 0x04001466 RID: 5222
		public float xGoal;

		// Token: 0x04001467 RID: 5223
		public float halfLife = 0.1f;

		// Token: 0x04001468 RID: 5224
		private float curVel;
	}
}
