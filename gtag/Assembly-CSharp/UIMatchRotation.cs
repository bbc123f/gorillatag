using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x06000DD2 RID: 3538 RVA: 0x00050782 File Offset: 0x0004E982
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x000507B0 File Offset: 0x0004E9B0
	private void Update()
	{
		Vector3 lhs = this.x0z(base.transform.forward);
		Vector3 vector = this.x0z(this.referenceTransform.forward);
		float num = Vector3.Dot(lhs, vector);
		UIMatchRotation.State state = this.state;
		if (state != UIMatchRotation.State.Ready)
		{
			if (state != UIMatchRotation.State.Rotating)
			{
				return;
			}
			base.transform.forward = Vector3.Lerp(base.transform.forward, vector, Time.deltaTime * this.lerpSpeed);
			if (Vector3.Dot(base.transform.forward, vector) > 0.995f)
			{
				this.state = UIMatchRotation.State.Ready;
			}
		}
		else if (num < 1f - this.threshold)
		{
			this.state = UIMatchRotation.State.Rotating;
			return;
		}
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00050854 File Offset: 0x0004EA54
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x040010C6 RID: 4294
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x040010C7 RID: 4295
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x040010C8 RID: 4296
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x040010C9 RID: 4297
	private UIMatchRotation.State state;

	// Token: 0x0200047F RID: 1151
	private enum State
	{
		// Token: 0x04001EBB RID: 7867
		Ready,
		// Token: 0x04001EBC RID: 7868
		Rotating
	}
}
