using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class RandomTimedSeedManager : MonoBehaviourPun, IPunObservable, ITickSystemTick
{
	public static RandomTimedSeedManager instance
	{
		[CompilerGenerated]
		get
		{
			return RandomTimedSeedManager.<instance>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			RandomTimedSeedManager.<instance>k__BackingField = value;
		}
	}

	public int seed
	{
		[CompilerGenerated]
		get
		{
			return this.<seed>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<seed>k__BackingField = value;
		}
	}

	public float currentSyncTime
	{
		[CompilerGenerated]
		get
		{
			return this.<currentSyncTime>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<currentSyncTime>k__BackingField = value;
		}
	}

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

	bool ITickSystemTick.TickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<ITickSystemTick.TickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<ITickSystemTick.TickRunning>k__BackingField = value;
		}
	}

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

	public RandomTimedSeedManager()
	{
	}

	[CompilerGenerated]
	[OnEnterPlay_SetNull]
	private static RandomTimedSeedManager <instance>k__BackingField;

	private List<Action> callbacksOnSeedChanged = new List<Action>();

	[CompilerGenerated]
	private int <seed>k__BackingField;

	private float idealSyncTime;

	[CompilerGenerated]
	private float <currentSyncTime>k__BackingField;

	private int cachedSeed;

	private const int SeedMin = -1000000;

	private const int SeedMax = -1000000;

	private const float MaxSyncTime = 1E+09f;

	[CompilerGenerated]
	private bool <ITickSystemTick.TickRunning>k__BackingField;
}
