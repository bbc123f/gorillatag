using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class GorillaVelocityEstimator : MonoBehaviour
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000106 RID: 262 RVA: 0x000099FA File Offset: 0x00007BFA
	// (set) Token: 0x06000107 RID: 263 RVA: 0x00009A02 File Offset: 0x00007C02
	public Vector3 linearVelocity { get; private set; }

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000108 RID: 264 RVA: 0x00009A0B File Offset: 0x00007C0B
	// (set) Token: 0x06000109 RID: 265 RVA: 0x00009A13 File Offset: 0x00007C13
	public Vector3 angularVelocity { get; private set; }

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600010A RID: 266 RVA: 0x00009A1C File Offset: 0x00007C1C
	// (set) Token: 0x0600010B RID: 267 RVA: 0x00009A24 File Offset: 0x00007C24
	public Vector3 handPos { get; private set; }

	// Token: 0x0600010C RID: 268 RVA: 0x00009A2D File Offset: 0x00007C2D
	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00009A40 File Offset: 0x00007C40
	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00009A9C File Offset: 0x00007C9C
	protected void LateUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 b = Player.Instance.currentVelocity;
		if (this.useGlobalSpace)
		{
			b = Vector3.zero;
		}
		Vector3 linear = (position - this.lastPos) / Time.deltaTime - b;
		Quaternion rotation = base.transform.rotation;
		Vector3 vector = (rotation * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		if (vector.y > 180f)
		{
			vector.y -= 360f;
		}
		if (vector.z > 180f)
		{
			vector.z -= 360f;
		}
		vector *= 0.017453292f / Time.fixedDeltaTime;
		this.history[this.currentFrame % this.numFrames] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = linear,
			angular = vector
		};
		this.linearVelocity = this.history[0].linear;
		this.angularVelocity = this.history[0].angular;
		for (int i = 0; i < this.numFrames; i++)
		{
			this.linearVelocity += this.history[i].linear;
			this.angularVelocity += this.history[i].angular;
		}
		this.linearVelocity /= (float)this.numFrames;
		this.angularVelocity /= (float)this.numFrames;
		this.handPos = position;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = position;
		this.lastRotation = rotation;
	}

	// Token: 0x04000169 RID: 361
	private int numFrames = 8;

	// Token: 0x0400016D RID: 365
	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	// Token: 0x0400016E RID: 366
	private int currentFrame;

	// Token: 0x0400016F RID: 367
	private Vector3 lastPos;

	// Token: 0x04000170 RID: 368
	private Quaternion lastRotation;

	// Token: 0x04000171 RID: 369
	private Vector3 lastRotationVec;

	// Token: 0x04000172 RID: 370
	public bool useGlobalSpace;

	// Token: 0x0200038E RID: 910
	public struct VelocityHistorySample
	{
		// Token: 0x04001B17 RID: 6935
		public Vector3 linear;

		// Token: 0x04001B18 RID: 6936
		public Vector3 angular;
	}
}
