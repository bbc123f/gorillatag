using System;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x06000DCC RID: 3532 RVA: 0x00050522 File Offset: 0x0004E722
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00050550 File Offset: 0x0004E750
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

	// Token: 0x06000DCE RID: 3534 RVA: 0x000505F4 File Offset: 0x0004E7F4
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x040010C1 RID: 4289
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x040010C2 RID: 4290
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x040010C3 RID: 4291
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x040010C4 RID: 4292
	private UIMatchRotation.State state;

	// Token: 0x0200047D RID: 1149
	private enum State
	{
		// Token: 0x04001EAE RID: 7854
		Ready,
		// Token: 0x04001EAF RID: 7855
		Rotating
	}
}
