using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000197 RID: 407
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x17000075 RID: 117
	// (get) Token: 0x06000A6C RID: 2668 RVA: 0x00040F08 File Offset: 0x0003F108
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00040F10 File Offset: 0x0003F110
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00040F18 File Offset: 0x0003F118
	protected virtual void Tick()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this._instances.Count; i++)
		{
			T t = this._instances[i];
			if (t)
			{
				this.OnTick(deltaTime, t);
			}
		}
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00040F63 File Offset: 0x0003F163
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00040F6B File Offset: 0x0003F16B
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00040F70 File Offset: 0x0003F170
	private bool RegisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError("[" + base.GetType().Name + "::Register] Instance is null.", null);
			return false;
		}
		if (this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnRegister(instance);
		return true;
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00040FD1 File Offset: 0x0003F1D1
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00040FD4 File Offset: 0x0003F1D4
	private bool UnregisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError("[" + base.GetType().Name + "::Unregister] Instance is null.", null);
			return false;
		}
		if (!this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnUnregister(instance);
		return true;
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x00041035 File Offset: 0x0003F235
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00041037 File Offset: 0x0003F237
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00041044 File Offset: 0x0003F244
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x06000A77 RID: 2679 RVA: 0x00041051 File Offset: 0x0003F251
	int IReadOnlyCollection<!0>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x17000077 RID: 119
	T IReadOnlyList<!0>.this[int index]
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000A79 RID: 2681 RVA: 0x0004106C File Offset: 0x0003F26C
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00041078 File Offset: 0x0003F278
	protected static void SetSingleton(GTSystem<T> system)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (GTSystem<T>.gSingleton != null && GTSystem<T>.gSingleton != system)
		{
			Object.Destroy(system);
			GTDev.LogWarning("Singleton of type " + GTSystem<T>.gSingleton.GetType().Name + " already exists.", null);
			return;
		}
		GTSystem<T>.gSingleton = system;
		if (!GTSystem<T>.gInitializing)
		{
			return;
		}
		GTSystem<T>.gSingleton._instances.Clear();
		T[] collection = (from x in GTSystem<T>.gQueueRegister
		where x != null
		select x).ToArray<T>();
		GTSystem<T>.gSingleton._instances.AddRange(collection);
		GTSystem<T>.gQueueRegister.Clear();
		PhotonView component = GTSystem<T>.gSingleton.GetComponent<PhotonView>();
		if (component != null)
		{
			GTSystem<T>.gSingleton._photonView = component;
			GTSystem<T>.gSingleton._networked = true;
		}
		GTSystem<T>.gInitializing = false;
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00041168 File Offset: 0x0003F368
	public static void Register(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		GTSystem<T>.gSingleton.RegisterInstance(instance);
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x000411D4 File Offset: 0x0003F3D4
	public static void Unregister(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		GTSystem<T>.gSingleton.UnregisterInstance(instance);
	}

	// Token: 0x04000D29 RID: 3369
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04000D2A RID: 3370
	[SerializeField]
	private bool _networked;

	// Token: 0x04000D2B RID: 3371
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04000D2C RID: 3372
	private static GTSystem<T> gSingleton;

	// Token: 0x04000D2D RID: 3373
	private static bool gInitializing = false;

	// Token: 0x04000D2E RID: 3374
	private static bool gAppQuitting = false;

	// Token: 0x04000D2F RID: 3375
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
