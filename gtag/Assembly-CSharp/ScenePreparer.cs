using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
[DefaultExecutionOrder(-9999)]
public class ScenePreparer : MonoBehaviour
{
	// Token: 0x06000236 RID: 566 RVA: 0x0000F1F8 File Offset: 0x0000D3F8
	protected void Awake()
	{
		bool flag = false;
		GameObject[] array = this.betaEnableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = this.betaDisableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x040002DE RID: 734
	public OVRManager ovrManager;

	// Token: 0x040002DF RID: 735
	public GameObject[] betaDisableObjects;

	// Token: 0x040002E0 RID: 736
	public GameObject[] betaEnableObjects;
}
