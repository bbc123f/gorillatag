using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029F RID: 671
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x06001181 RID: 4481 RVA: 0x00062B8F File Offset: 0x00060D8F
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x04001423 RID: 5155
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x04001424 RID: 5156
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x04001425 RID: 5157
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
