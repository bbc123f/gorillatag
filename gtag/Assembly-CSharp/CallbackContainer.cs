using System;
using System.Collections.Generic;

internal class CallbackContainer<T>
{
	public virtual void Add(T inCallback)
	{
		if (this.runningUpdate)
		{
			this.addQ.Enqueue(inCallback);
			return;
		}
		this.callbacks.Add(inCallback);
	}

	public virtual void Remove(T inCallback)
	{
		if (this.runningUpdate)
		{
			this.removeQ.Enqueue(inCallback);
			return;
		}
		this.callbacks.Remove(inCallback);
	}

	public void SetRunUpdateCallback(CallbackContainer<T>.runCallbacksDelegate callback)
	{
		this.RunCallBack = callback;
	}

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

	protected bool runningUpdate;

	protected Queue<T> addQ = new Queue<T>(20);

	protected Queue<T> removeQ = new Queue<T>(20);

	protected HashSet<T> callbacks = new HashSet<T>();

	protected CallbackContainer<T>.runCallbacksDelegate RunCallBack = delegate(T t)
	{
	};

	public delegate void runCallbacksDelegate(T callback);
}
