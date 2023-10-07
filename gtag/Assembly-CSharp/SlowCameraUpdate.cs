using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x06000AC5 RID: 2757 RVA: 0x0004248F File Offset: 0x0004068F
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x000424BA File Offset: 0x000406BA
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x000424C9 File Offset: 0x000406C9
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x000424D1 File Offset: 0x000406D1
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

	// Token: 0x04000D8E RID: 3470
	private Camera myCamera;

	// Token: 0x04000D8F RID: 3471
	private float frameRate;

	// Token: 0x04000D90 RID: 3472
	private float timeToNextFrame;
}
