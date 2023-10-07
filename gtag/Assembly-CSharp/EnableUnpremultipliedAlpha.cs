using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class EnableUnpremultipliedAlpha : MonoBehaviour
{
	// Token: 0x060003CC RID: 972 RVA: 0x000179C0 File Offset: 0x00015BC0
	private void Start()
	{
		OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
	}
}
