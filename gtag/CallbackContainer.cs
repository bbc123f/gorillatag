using System;
using System.Collections.Generic;
using UnityEngine;

internal class CallbackContainer<T> where T : ICallBack
{
	public virtual void Add(T inCallback)
	{
		this.callbackCount++;
		this.callbackList.Add(inCallback);
	}

	public virtual void Remove(T inCallback)
	{
		int num = this.callbackList.IndexOf(inCallback);
		if (num > -1)
		{
			if (num <= this.currentIndex)
			{
				this.currentIndex--;
			}
			this.callbackCount--;
			this.callbackList.RemoveAt(num);
		}
	}

	public virtual void TryRunCallbacks()
	{
		this.callbackCount = this.callbackList.Count;
		this.currentIndex = 0;
		while (this.currentIndex < this.callbackCount)
		{
			try
			{
				T t = this.callbackList[this.currentIndex];
				t.CallBack();
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			this.currentIndex++;
		}
	}

	public virtual void Clear()
	{
		this.callbackList.Clear();
	}

	public CallbackContainer()
	{
	}

	protected List<T> callbackList = new List<T>(100);

	protected int currentIndex = -1;

	protected int callbackCount = -1;
}
