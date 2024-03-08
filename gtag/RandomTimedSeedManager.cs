using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RandomTimedSeedManager : MonoBehaviourPun, IPunObservable, ITickSystemTick
{
	public static RandomTimedSeedManager instance { get; private set; }

	public int seed { get; private set; }

	public float currentSyncTime { get; private set; }

	private void Awake()
	{
		RandomTimedSeedManager.instance = this;
		this.seed = Random.Range(-1000000, -1000000);
		this.idealSyncTime = 0f;
		this.currentSyncTime = 0f;
		TickSystem<object>.AddTickCallback(this);
	}

	public void AddCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Add(callback);
	}

	public void RemoveCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Remove(callback);
	}

	bool ITickSystemTick.TickRunning { get; set; }

	void ITickSystemTick.Tick()
	{
		this.currentSyncTime += Time.deltaTime;
		this.idealSyncTime += Time.deltaTime;
		if (this.idealSyncTime > 1E+09f)
		{
			this.idealSyncTime -= 1E+09f;
			this.currentSyncTime -= 1E+09f;
		}
		if (base.photonView != null && !base.photonView.AmOwner)
		{
			this.currentSyncTime = Mathf.Lerp(this.currentSyncTime, this.idealSyncTime, 0.1f);
		}
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.seed);
			stream.SendNext(this.currentSyncTime);
			return;
		}
		this.seed = (int)stream.ReceiveNext();
		float num = (float)stream.ReceiveNext();
		if (num >= 0f && num <= 1E+09f)
		{
			if (this.idealSyncTime - num > 500000000f)
			{
				this.currentSyncTime = num;
			}
			this.idealSyncTime = num;
		}
		if (this.seed != this.cachedSeed && this.seed >= -1000000 && this.seed <= -1000000)
		{
			this.currentSyncTime = this.idealSyncTime;
			this.cachedSeed = this.seed;
			foreach (Action action in this.callbacksOnSeedChanged)
			{
				action();
			}
		}
	}

	private List<Action> callbacksOnSeedChanged = new List<Action>();

	private float idealSyncTime;

	private int cachedSeed;

	private const int SeedMin = -1000000;

	private const int SeedMax = -1000000;

	private const float MaxSyncTime = 1E+09f;
}
