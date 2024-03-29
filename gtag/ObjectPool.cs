﻿using System;
using System.Collections.Generic;

public class ObjectPool<T> where T : ObjectPoolEvents, new()
{
	private ObjectPool()
	{
	}

	public ObjectPool(int amount)
	{
		this.maxInstances = amount;
		for (int i = 0; i < this.maxInstances; i++)
		{
			this.pool.Push(new T());
		}
	}

	public T Take()
	{
		T t;
		if (this.pool.Count < 1)
		{
			t = new T();
		}
		else
		{
			t = this.pool.Pop();
		}
		t.OnTaken();
		return t;
	}

	public void Return(T instance)
	{
		instance.OnReturned();
		if (this.pool.Count == this.maxInstances)
		{
			return;
		}
		this.pool.Push(instance);
	}

	private Stack<T> pool = new Stack<T>(100);

	public int maxInstances = 500;
}
