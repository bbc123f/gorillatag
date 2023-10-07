using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001EB RID: 491
[Serializable]
public class CallLimiter
{
	// Token: 0x06000CC0 RID: 3264 RVA: 0x0004C15E File Offset: 0x0004A35E
	public CallLimiter()
	{
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0004C168 File Offset: 0x0004A368
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

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0004C1B5 File Offset: 0x0004A3B5
	public bool CheckCallServerTime(double time)
	{
		return Mathf.Abs((float)(PhotonNetwork.Time - time)) <= this.maxLatency && this.CheckCallTime((float)time);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0004C1D8 File Offset: 0x0004A3D8
	public virtual bool CheckCallTime(float time)
	{
		Debug.Log("old time index " + this.oldTimeIndex.ToString());
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

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0004C25C File Offset: 0x0004A45C
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

	// Token: 0x0400100B RID: 4107
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x0400100C RID: 4108
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x0400100D RID: 4109
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x0400100E RID: 4110
	[SerializeField]
	protected float maxLatency;

	// Token: 0x0400100F RID: 4111
	private int oldTimeIndex;

	// Token: 0x04001010 RID: 4112
	protected bool blockCall;

	// Token: 0x04001011 RID: 4113
	protected float blockStartTime;
}
