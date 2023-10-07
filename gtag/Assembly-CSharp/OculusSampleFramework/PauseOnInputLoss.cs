using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CA RID: 714
	public class PauseOnInputLoss : MonoBehaviour
	{
		// Token: 0x06001342 RID: 4930 RVA: 0x0006F4F3 File Offset: 0x0006D6F3
		private void Start()
		{
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x0006F517 File Offset: 0x0006D717
		private void OnInputFocusLost()
		{
			Time.timeScale = 0f;
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x0006F523 File Offset: 0x0006D723
		private void OnInputFocusAcquired()
		{
			Time.timeScale = 1f;
		}
	}
}
