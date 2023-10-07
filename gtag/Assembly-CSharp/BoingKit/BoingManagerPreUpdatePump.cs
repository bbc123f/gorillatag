using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200036A RID: 874
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x060019AA RID: 6570 RVA: 0x0008DE67 File Offset: 0x0008C067
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x0008DE6F File Offset: 0x0008C06F
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x0008DE77 File Offset: 0x0008C077
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

		// Token: 0x060019AD RID: 6573 RVA: 0x0008DEA1 File Offset: 0x0008C0A1
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x04001A51 RID: 6737
		private int m_lastPumpedFrame = -1;
	}
}
