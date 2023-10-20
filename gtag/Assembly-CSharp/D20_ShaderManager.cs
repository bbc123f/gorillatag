using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class D20_ShaderManager : MonoBehaviour
{
	// Token: 0x0600013A RID: 314 RVA: 0x0000AB88 File Offset: 0x00008D88
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000ABED File Offset: 0x00008DED
	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	// Token: 0x040001AD RID: 429
	private Rigidbody rb;

	// Token: 0x040001AE RID: 430
	private Vector3 lastPosition;

	// Token: 0x040001AF RID: 431
	public float updateInterval = 0.1f;

	// Token: 0x040001B0 RID: 432
	public Vector3 velocity;

	// Token: 0x040001B1 RID: 433
	private Material material;
}
