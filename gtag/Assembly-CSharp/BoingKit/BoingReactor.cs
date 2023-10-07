using System;

namespace BoingKit
{
	// Token: 0x0200036B RID: 875
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x060019AF RID: 6575 RVA: 0x0008DEC6 File Offset: 0x0008C0C6
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x0008DECE File Offset: 0x0008C0CE
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x0008DED6 File Offset: 0x0008C0D6
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
