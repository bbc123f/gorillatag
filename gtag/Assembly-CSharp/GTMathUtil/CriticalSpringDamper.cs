using System;

namespace GTMathUtil
{
	// Token: 0x020002A9 RID: 681
	internal class CriticalSpringDamper
	{
		// Token: 0x060011B6 RID: 4534 RVA: 0x00064D78 File Offset: 0x00062F78
		private static float halflife_to_damping(float halflife, float eps = 1E-05f)
		{
			return 2.7725887f / (halflife + eps);
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00064D83 File Offset: 0x00062F83
		private static float fast_negexp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00064DA8 File Offset: 0x00062FA8
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

		// Token: 0x04001472 RID: 5234
		public float x;

		// Token: 0x04001473 RID: 5235
		public float xGoal;

		// Token: 0x04001474 RID: 5236
		public float halfLife = 0.1f;

		// Token: 0x04001475 RID: 5237
		private float curVel;
	}
}
