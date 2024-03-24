using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WatchableGenericSO<T> : ScriptableObject
{
	[DebugReadOnly]
	private T _value
	{
		[CompilerGenerated]
		get
		{
			return this.<_value>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<_value>k__BackingField = value;
		}
	}

	public T Value
	{
		get
		{
			this.EnsureInitialized();
			return this._value;
		}
		set
		{
			this.EnsureInitialized();
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<T>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			T value = this._value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	public void RemoveCallback(Action<T> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	public WatchableGenericSO()
	{
	}

	public T InitialValue;

	[CompilerGenerated]
	private T <_value>k__BackingField;

	private EnterPlayID enterPlayID;

	private List<Action<T>> callbacks;
}
