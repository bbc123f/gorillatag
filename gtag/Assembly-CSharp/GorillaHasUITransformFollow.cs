using System;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x06000961 RID: 2401 RVA: 0x00038AB0 File Offset: 0x00036CB0
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00038AEC File Offset: 0x00036CEC
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00038B1C File Offset: 0x00036D1C
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00038B4C File Offset: 0x00036D4C
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x04000B79 RID: 2937
	public GorillaUITransformFollow[] transformFollowers;
}
