using System;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class CallLimiter
{
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

	public CallLimiter()
	{
	}

	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		callTimeHistory = new float[historyLength];
		callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			callTimeHistory[i] = float.MinValue;
		}
		timeCooldown = coolDown;
		maxLatency = latencyMax;
	}

	public bool CheckCallServerTime(double time)
	{
		if (Mathf.Abs((float)(PhotonNetwork.Time - time)) > maxLatency)
		{
			return false;
		}
		return CheckCallTime((float)time);
	}

	public virtual bool CheckCallTime(float time)
	{
		Debug.Log("old time index " + oldTimeIndex);
		if (callTimeHistory[oldTimeIndex] > time)
		{
			blockCall = true;
			blockStartTime = time;
			return false;
		}
		callTimeHistory[oldTimeIndex] = time + timeCooldown;
		oldTimeIndex = ++oldTimeIndex % callHistoryLength;
		blockCall = false;
		return true;
	}

	public virtual void reset()
	{
		if (callTimeHistory != null)
		{
			for (int i = 0; i < callHistoryLength; i++)
			{
				callTimeHistory[i] = float.MinValue;
			}
			oldTimeIndex = 0;
			blockStartTime = 0f;
			blockCall = false;
		}
	}
}
