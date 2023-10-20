using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CC RID: 716
	public class PauseOnInputLoss : MonoBehaviour
	{
		// Token: 0x06001349 RID: 4937 RVA: 0x0006F9BF File Offset: 0x0006DBBF
		private void Start()
		{
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0006F9E3 File Offset: 0x0006DBE3
		private void OnInputFocusLost()
		{
			Time.timeScale = 0f;
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x0006F9EF File Offset: 0x0006DBEF
		private void OnInputFocusAcquired()
		{
			Time.timeScale = 1f;
		}
	}
}
