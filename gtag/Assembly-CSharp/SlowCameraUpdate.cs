using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x06000ACA RID: 2762 RVA: 0x000425C7 File Offset: 0x000407C7
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x000425F2 File Offset: 0x000407F2
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00042601 File Offset: 0x00040801
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00042609 File Offset: 0x00040809
	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	// Token: 0x04000D92 RID: 3474
	private Camera myCamera;

	// Token: 0x04000D93 RID: 3475
	private float frameRate;

	// Token: 0x04000D94 RID: 3476
	private float timeToNextFrame;
}
