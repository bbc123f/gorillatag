using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class GorillaRenderVolume : MonoBehaviour
{
	// Token: 0x06000196 RID: 406 RVA: 0x0000C1F4 File Offset: 0x0000A3F4
	protected void Awake()
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		this.overlapResults = new Collider[this.colliders.Length];
		this.layerMask = 1 << LayerMask.NameToLayer("Camera Scene Trigger");
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000C248 File Offset: 0x0000A448
	protected void LateUpdate()
	{
		Transform transform = Camera.main.transform;
		Vector3 position = transform.position;
		float radius = 0.1f * transform.lossyScale.x;
		int num = Physics.OverlapSphereNonAlloc(position, radius, this.overlapResults, this.layerMask, QueryTriggerInteraction.Collide);
		if (this.isInside == num > 0)
		{
			return;
		}
		this.isInside = !this.isInside;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isInside;
		}
	}

	// Token: 0x0400027A RID: 634
	[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
	public Renderer[] renderers;

	// Token: 0x0400027B RID: 635
	public Collider[] colliders;

	// Token: 0x0400027C RID: 636
	private Collider[] overlapResults;

	// Token: 0x0400027D RID: 637
	private int layerMask;

	// Token: 0x0400027E RID: 638
	private bool isInside;

	// Token: 0x0400027F RID: 639
	private const float cameraRadius = 0.1f;
}
