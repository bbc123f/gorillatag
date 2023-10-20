using System;

namespace BoingKit
{
	// Token: 0x0200036D RID: 877
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x060019B8 RID: 6584 RVA: 0x0008E3AE File Offset: 0x0008C5AE
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0008E3B6 File Offset: 0x0008C5B6
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x0008E3BE File Offset: 0x0008C5BE
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
