using System;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x06000D1F RID: 3359 RVA: 0x0004D474 File Offset: 0x0004B674
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x0004D48B File Offset: 0x0004B68B
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x0004D4A1 File Offset: 0x0004B6A1
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x0004D4B8 File Offset: 0x0004B6B8
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

	// Token: 0x0400104D RID: 4173
	public Transform start;

	// Token: 0x0400104E RID: 4174
	public Transform end;

	// Token: 0x0400104F RID: 4175
	public Rigidbody rigidbody;

	// Token: 0x04001050 RID: 4176
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x04001051 RID: 4177
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x04001052 RID: 4178
	public float period = 4f;

	// Token: 0x02000471 RID: 1137
	public enum UpdateType
	{
		// Token: 0x04001E84 RID: 7812
		Update,
		// Token: 0x04001E85 RID: 7813
		LateUpdate,
		// Token: 0x04001E86 RID: 7814
		FixedUpdate
	}

	// Token: 0x02000472 RID: 1138
	public enum MoveType
	{
		// Token: 0x04001E88 RID: 7816
		TransformPosition,
		// Token: 0x04001E89 RID: 7817
		RigidbodyMovePosition
	}
}
