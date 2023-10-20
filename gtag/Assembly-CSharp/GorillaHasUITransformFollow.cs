using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x06000965 RID: 2405 RVA: 0x00038A68 File Offset: 0x00036C68
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00038AA4 File Offset: 0x00036CA4
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00038AD4 File Offset: 0x00036CD4
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00038B04 File Offset: 0x00036D04
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x04000B7D RID: 2941
	public GorillaUITransformFollow[] transformFollowers;
}
