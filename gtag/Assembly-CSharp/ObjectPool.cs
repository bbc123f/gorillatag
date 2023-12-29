using System;
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
			this.pool.Push(Activator.CreateInstance<T>());
		}
	}

	public T Take()
	{
		T result;
		if (this.pool.Count < 1)
		{
			result = Activator.CreateInstance<T>();
		}
		else
		{
			result = this.pool.Pop();
		}
		result.OnTaken();
		return result;
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
