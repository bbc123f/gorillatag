using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020002A1 RID: 673
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x06001188 RID: 4488 RVA: 0x00062FF7 File Offset: 0x000611F7
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x04001430 RID: 5168
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x04001431 RID: 5169
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x04001432 RID: 5170
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
