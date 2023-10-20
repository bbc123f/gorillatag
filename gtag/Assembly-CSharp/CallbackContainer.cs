using System;
using System.Collections.Generic;

// Token: 0x0200020E RID: 526
internal class CallbackContainer<T>
{
	// Token: 0x06000D38 RID: 3384 RVA: 0x0004D940 File Offset: 0x0004BB40
	public virtual void Add(T inCallback)
	{
		if (this.runningUpdate)
		{
			this.addQ.Enqueue(inCallback);
			return;
		}
		this.callbacks.Add(inCallback);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0004D964 File Offset: 0x0004BB64
	public virtual void Remove(T inCallback)
	{
		if (this.runningUpdate)
		{
			this.removeQ.Enqueue(inCallback);
			return;
		}
		this.callbacks.Remove(inCallback);
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x0004D988 File Offset: 0x0004BB88
	public void SetRunUpdateCallback(CallbackContainer<T>.runCallbacksDelegate callback)
	{
		this.RunCallBack = callback;
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x0004D994 File Offset: 0x0004BB94
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

	// Token: 0x06000D3C RID: 3388 RVA: 0x0004D9F0 File Offset: 0x0004BBF0
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

	// Token: 0x06000D3D RID: 3389 RVA: 0x0004DA78 File Offset: 0x0004BC78
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

	// Token: 0x04001065 RID: 4197
	protected bool runningUpdate;

	// Token: 0x04001066 RID: 4198
	protected Queue<T> addQ = new Queue<T>(20);

	// Token: 0x04001067 RID: 4199
	protected Queue<T> removeQ = new Queue<T>(20);

	// Token: 0x04001068 RID: 4200
	protected HashSet<T> callbacks = new HashSet<T>();

	// Token: 0x04001069 RID: 4201
	protected CallbackContainer<T>.runCallbacksDelegate RunCallBack = delegate(T t)
	{
	};

	// Token: 0x02000478 RID: 1144
	// (Invoke) Token: 0x06001D4B RID: 7499
	public delegate void runCallbacksDelegate(T callback);
}
