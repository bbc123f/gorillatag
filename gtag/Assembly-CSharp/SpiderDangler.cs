using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class SpiderDangler : MonoBehaviour
{
	// Token: 0x060000F9 RID: 249 RVA: 0x00009460 File Offset: 0x00007660
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.ropeSegLen = magnitude / 6f;
		this.ropeSegs = new SpiderDangler.RopeSegment[6];
		for (int i = 0; i < 6; i++)
		{
			this.ropeSegs[i] = new SpiderDangler.RopeSegment(position);
			position.y -= this.ropeSegLen;
		}
	}

	// Token: 0x060000FA RID: 250 RVA: 0x000094E7 File Offset: 0x000076E7
	protected void FixedUpdate()
	{
		this.Simulate();
	}

	// Token: 0x060000FB RID: 251 RVA: 0x000094F0 File Offset: 0x000076F0
	protected void LateUpdate()
	{
		this.DrawRope();
		Vector3 normalized = (this.ropeSegs[this.ropeSegs.Length - 2].pos - this.ropeSegs[this.ropeSegs.Length - 1].pos).normalized;
		this.endTransform.position = this.ropeSegs[this.ropeSegs.Length - 1].pos;
		this.endTransform.up = normalized;
		Vector4 vector = this.spinSpeeds * Time.time;
		vector = new Vector4(Mathf.Sin(vector.x), Mathf.Sin(vector.y), Mathf.Sin(vector.z), Mathf.Sin(vector.w));
		vector.Scale(this.spinScales);
		this.endTransform.Rotate(Vector3.up, vector.x + vector.y + vector.z + vector.w);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x000095F4 File Offset: 0x000077F4
	private void Simulate()
	{
		this.ropeSegLenScaled = this.ropeSegLen * base.transform.lossyScale.x;
		Vector3 b = new Vector3(0f, -0.5f, 0f) * Time.fixedDeltaTime;
		for (int i = 1; i < 6; i++)
		{
			Vector3 a = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			SpiderDangler.RopeSegment[] array = this.ropeSegs;
			int num = i;
			array[num].pos = array[num].pos + a * 0.95f;
			SpiderDangler.RopeSegment[] array2 = this.ropeSegs;
			int num2 = i;
			array2[num2].pos = array2[num2].pos + b;
		}
		for (int j = 0; j < 8; j++)
		{
			this.ApplyConstraint();
		}
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000096FC File Offset: 0x000078FC
	private void ApplyConstraint()
	{
		this.ropeSegs[0].pos = base.transform.position;
		this.ApplyConstraintSegment(ref this.ropeSegs[0], ref this.ropeSegs[1], 0f, 1f);
		for (int i = 1; i < 5; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], 0.5f, 0.5f);
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00009784 File Offset: 0x00007984
	private void ApplyConstraintSegment(ref SpiderDangler.RopeSegment segA, ref SpiderDangler.RopeSegment segB, float dampenA, float dampenB)
	{
		float d = (segA.pos - segB.pos).magnitude - this.ropeSegLenScaled;
		Vector3 a = (segA.pos - segB.pos).normalized * d;
		segA.pos -= a * dampenA;
		segB.pos += a * dampenB;
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00009810 File Offset: 0x00007A10
	private void DrawRope()
	{
		Vector3[] array = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = this.ropeSegs[i].pos;
		}
		this.lineRenderer.positionCount = array.Length;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x04000155 RID: 341
	public Transform endTransform;

	// Token: 0x04000156 RID: 342
	public Vector4 spinSpeeds = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);

	// Token: 0x04000157 RID: 343
	public Vector4 spinScales = new Vector4(180f, 90f, 120f, 180f);

	// Token: 0x04000158 RID: 344
	private LineRenderer lineRenderer;

	// Token: 0x04000159 RID: 345
	private SpiderDangler.RopeSegment[] ropeSegs;

	// Token: 0x0400015A RID: 346
	private float ropeSegLen;

	// Token: 0x0400015B RID: 347
	private float ropeSegLenScaled;

	// Token: 0x0400015C RID: 348
	private const int kSegmentCount = 6;

	// Token: 0x0400015D RID: 349
	private const float kVelocityDamper = 0.95f;

	// Token: 0x0400015E RID: 350
	private const int kConstraintCalculationIterations = 8;

	// Token: 0x0200038D RID: 909
	public struct RopeSegment
	{
		// Token: 0x06001AC2 RID: 6850 RVA: 0x00094B13 File Offset: 0x00092D13
		public RopeSegment(Vector3 pos)
		{
			this.pos = pos;
			this.posOld = pos;
		}

		// Token: 0x04001B15 RID: 6933
		public Vector3 pos;

		// Token: 0x04001B16 RID: 6934
		public Vector3 posOld;
	}
}
