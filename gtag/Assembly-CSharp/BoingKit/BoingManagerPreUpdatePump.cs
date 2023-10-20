using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200036C RID: 876
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x060019B3 RID: 6579 RVA: 0x0008E34F File Offset: 0x0008C54F
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x0008E357 File Offset: 0x0008C557
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x0008E35F File Offset: 0x0008C55F
		private void TryPump()
		{
			if (this.m_lastPumpedFrame >= Time.frameCount)
			{
				return;
			}
			if (this.m_lastPumpedFrame >= 0)
			{
				this.DoPump();
			}
			this.m_lastPumpedFrame = Time.frameCount;
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0008E389 File Offset: 0x0008C589
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x04001A5E RID: 6750
		private int m_lastPumpedFrame = -1;
	}
}
