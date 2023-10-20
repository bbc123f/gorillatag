using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000198 RID: 408
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000A71 RID: 2673 RVA: 0x00041040 File Offset: 0x0003F240
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00041048 File Offset: 0x0003F248
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00041050 File Offset: 0x0003F250
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

	// Token: 0x06000A74 RID: 2676 RVA: 0x0004109B File Offset: 0x0003F29B
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x000410A3 File Offset: 0x0003F2A3
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x000410A8 File Offset: 0x0003F2A8
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

	// Token: 0x06000A77 RID: 2679 RVA: 0x00041109 File Offset: 0x0003F309
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0004110C File Offset: 0x0003F30C
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

	// Token: 0x06000A79 RID: 2681 RVA: 0x0004116D File Offset: 0x0003F36D
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0004116F File Offset: 0x0003F36F
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0004117C File Offset: 0x0003F37C
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000A7C RID: 2684 RVA: 0x00041189 File Offset: 0x0003F389
	int IReadOnlyCollection<!0>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x17000079 RID: 121
	T IReadOnlyList<!0>.this[int index]
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000A7E RID: 2686 RVA: 0x000411A4 File Offset: 0x0003F3A4
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x000411B0 File Offset: 0x0003F3B0
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

	// Token: 0x06000A80 RID: 2688 RVA: 0x000412A0 File Offset: 0x0003F4A0
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

	// Token: 0x06000A81 RID: 2689 RVA: 0x0004130C File Offset: 0x0003F50C
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

	// Token: 0x04000D2D RID: 3373
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04000D2E RID: 3374
	[SerializeField]
	private bool _networked;

	// Token: 0x04000D2F RID: 3375
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04000D30 RID: 3376
	private static GTSystem<T> gSingleton;

	// Token: 0x04000D31 RID: 3377
	private static bool gInitializing = false;

	// Token: 0x04000D32 RID: 3378
	private static bool gAppQuitting = false;

	// Token: 0x04000D33 RID: 3379
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
