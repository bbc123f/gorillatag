using System;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x06000D25 RID: 3365 RVA: 0x0004D6D4 File Offset: 0x0004B8D4
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x0004D6EB File Offset: 0x0004B8EB
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x0004D701 File Offset: 0x0004B901
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x0004D718 File Offset: 0x0004B918
	private void UpdatePosition(float time)
	{
		float t = Mathf.Sin(time * 2f * 3.1415927f * this.period) * 0.5f + 0.5f;
		Vector3 position = Vector3.Lerp(this.start.position, this.end.position, t);
		if (this.moveType == KinematicTestMotion.MoveType.TransformPosition)
		{
			base.transform.position = position;
			return;
		}
		if (this.moveType == KinematicTestMotion.MoveType.RigidbodyMovePosition)
		{
			this.rigidbody.MovePosition(position);
		}
	}

	// Token: 0x04001052 RID: 4178
	public Transform start;

	// Token: 0x04001053 RID: 4179
	public Transform end;

	// Token: 0x04001054 RID: 4180
	public Rigidbody rigidbody;

	// Token: 0x04001055 RID: 4181
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x04001056 RID: 4182
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x04001057 RID: 4183
	public float period = 4f;

	// Token: 0x02000473 RID: 1139
	public enum UpdateType
	{
		// Token: 0x04001E91 RID: 7825
		Update,
		// Token: 0x04001E92 RID: 7826
		LateUpdate,
		// Token: 0x04001E93 RID: 7827
		FixedUpdate
	}

	// Token: 0x02000474 RID: 1140
	public enum MoveType
	{
		// Token: 0x04001E95 RID: 7829
		TransformPosition,
		// Token: 0x04001E96 RID: 7830
		RigidbodyMovePosition
	}
}
