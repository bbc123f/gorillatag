using System;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class CallLimiter
{
	public CallLimiter()
	{
	}

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

	public bool CheckCallServerTime(double time)
	{
		return Mathf.Abs((float)(PhotonNetwork.Time - time)) <= this.maxLatency && this.CheckCallTime((float)time);
	}

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

	[SerializeField]
	protected float[] callTimeHistory;

	[SerializeField]
	protected int callHistoryLength;

	[SerializeField]
	protected float timeCooldown;

	[SerializeField]
	protected float maxLatency;

	private int oldTimeIndex;

	protected bool blockCall;

	protected float blockStartTime;
}
