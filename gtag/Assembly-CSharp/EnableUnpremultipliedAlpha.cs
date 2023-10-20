using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class EnableUnpremultipliedAlpha : MonoBehaviour
{
	// Token: 0x060003CC RID: 972 RVA: 0x0001779C File Offset: 0x0001599C
	private void Start()
	{
		OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
	}
}
