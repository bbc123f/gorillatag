using System;
using System.Collections.Generic;

// Token: 0x0200020D RID: 525
internal class CallbackContainer<T>
{
	// Token: 0x06000D32 RID: 3378 RVA: 0x0004D6E0 File Offset: 0x0004B8E0
	public virtual void Add(T inCallback)
	{
		if (this.runningUpdate)
		{
			this.addQ.Enqueue(inCallback);
			return;
		}
		this.callbacks.Add(inCallback);
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x0004D704 File Offset: 0x0004B904
	public virtual void Remove(T inCallback)
	{
		if (this.runningUpdate)
		{
			this.removeQ.Enqueue(inCallback);
			return;
		}
		this.callbacks.Remove(inCallback);
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x0004D728 File Offset: 0x0004B928
	public void SetRunUpdateCallback(CallbackContainer<T>.runCallbacksDelegate callback)
	{
		this.RunCallBack = callback;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x0004D734 File Offset: 0x0004B934
	public virtual void UpdateCallbacks()
	{
		while (this.addQ.Count > 0)
		{
			this.callbacks.Add(this.addQ.Dequeue());
		}
		while (this.removeQ.Count > 0)
		{
			this.callbacks.Remove(this.removeQ.Dequeue());
		}
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0004D790 File Offset: 0x0004B990
	public virtual void TryRunCallbacks()
	{
		this.runningUpdate = true;
		this.UpdateCallbacks();
		foreach (T callback in this.callbacks)
		{
			try
			{
				this.RunCallBack(callback);
			}
			catch (Exception ex)
			{
				GTDev.LogError(ex.ToString(), null);
			}
		}
		this.runningUpdate = false;
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x0004D818 File Offset: 0x0004BA18
	public virtual void TryRunCallbacks(CallbackContainer<T>.runCallbacksDelegate callbackDelegate)
	{
		this.runningUpdate = true;
		this.UpdateCallbacks();
		foreach (T callback in this.callbacks)
		{
			try
			{
				callbackDelegate(callback);
			}
			catch (Exception ex)
			{
				GTDev.LogError(ex.ToString(), null);
			}
		}
		this.runningUpdate = false;
	}

	// Token: 0x04001060 RID: 4192
	protected bool runningUpdate;

	// Token: 0x04001061 RID: 4193
	protected Queue<T> addQ = new Queue<T>(20);

	// Token: 0x04001062 RID: 4194
	protected Queue<T> removeQ = new Queue<T>(20);

	// Token: 0x04001063 RID: 4195
	protected HashSet<T> callbacks = new HashSet<T>();

	// Token: 0x04001064 RID: 4196
	protected CallbackContainer<T>.runCallbacksDelegate RunCallBack = delegate(T t)
	{
	};

	// Token: 0x02000476 RID: 1142
	// (Invoke) Token: 0x06001D42 RID: 7490
	public delegate void runCallbacksDelegate(T callback);
}
