using System;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class BlinkingLight : MonoBehaviour
{
	// Token: 0x0600084B RID: 2123 RVA: 0x000338A7 File Offset: 0x00031AA7
	private void Awake()
	{
		this.nextChange = Random.Range(0f, this.maxTime);
		this.meshRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x000338CC File Offset: 0x00031ACC
	private void Update()
	{
		if (Time.time > this.nextChange)
		{
			this.nextChange = Time.time + Random.Range(this.minTime, this.maxTime);
			if (this.materialArray.Length != 0)
			{
				this.meshRenderer.material = this.materialArray[Random.Range(0, this.materialArray.Length)];
			}
		}
	}

	// Token: 0x04000A39 RID: 2617
	public Material[] materialArray;

	// Token: 0x04000A3A RID: 2618
	public float minTime;

	// Token: 0x04000A3B RID: 2619
	public float maxTime;

	// Token: 0x04000A3C RID: 2620
	private float nextChange;

	// Token: 0x04000A3D RID: 2621
	private MeshRenderer meshRenderer;
}
