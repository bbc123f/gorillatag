using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001EC RID: 492
[Serializable]
public class CallLimiter
{
	// Token: 0x06000CC6 RID: 3270 RVA: 0x0004C3C6 File Offset: 0x0004A5C6
	public CallLimiter()
	{
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0004C3D0 File Offset: 0x0004A5D0
	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		this.callTimeHistory = new float[historyLength];
		this.callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.timeCooldown = coolDown;
		this.maxLatency = latencyMax;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0004C41D File Offset: 0x0004A61D
	public bool CheckCallServerTime(double time)
	{
		return Mathf.Abs((float)(PhotonNetwork.Time - time)) <= this.maxLatency && this.CheckCallTime((float)time);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x0004C440 File Offset: 0x0004A640
	public virtual bool CheckCallTime(float time)
	{
		if (this.callTimeHistory[this.oldTimeIndex] > time)
		{
			this.blockCall = true;
			this.blockStartTime = time;
			return false;
		}
		this.callTimeHistory[this.oldTimeIndex] = time + this.timeCooldown;
		int num = this.oldTimeIndex + 1;
		this.oldTimeIndex = num;
		this.oldTimeIndex = num % this.callHistoryLength;
		this.blockCall = false;
		return true;
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x0004C4A8 File Offset: 0x0004A6A8
	public virtual void Reset()
	{
		if (this.callTimeHistory == null)
		{
			return;
		}
		for (int i = 0; i < this.callHistoryLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.oldTimeIndex = 0;
		this.blockStartTime = 0f;
		this.blockCall = false;
	}

	// Token: 0x0400100F RID: 4111
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x04001010 RID: 4112
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x04001011 RID: 4113
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x04001012 RID: 4114
	[SerializeField]
	protected float maxLatency;

	// Token: 0x04001013 RID: 4115
	private int oldTimeIndex;

	// Token: 0x04001014 RID: 4116
	protected bool blockCall;

	// Token: 0x04001015 RID: 4117
	protected float blockStartTime;
}
