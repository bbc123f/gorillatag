using System;
using System.Collections.Generic;

public class Map<T1, T2>
{
	public Map()
	{
		this.fwdDict = new Dictionary<T1, T2>();
		this.revDict = new Dictionary<T2, T1>();
	}

	public void Add(T1 t0, T2 t1)
	{
		this.fwdDict.Add(t0, t1);
		try
		{
			this.revDict.Add(t1, t0);
		}
		catch
		{
			this.fwdDict.Remove(t0);
		}
	}

	public void Remove(T1 t0)
	{
		T2 key;
		if (this.fwdDict.TryGetValue(t0, out key))
		{
			this.revDict.Remove(key);
			this.fwdDict.Remove(t0);
		}
	}

	public void Remove(T2 t1)
	{
		T1 key;
		if (this.revDict.TryGetValue(t1, out key))
		{
			this.fwdDict.Remove(key);
			this.revDict.Remove(t1);
		}
	}

	public T2 Lookup(T1 t0)
	{
		return this.fwdDict[t0];
	}

	public T1 Lookup(T2 t1)
	{
		return this.revDict[t1];
	}

	public Dictionary<T1, T2> fwdDict;

	public Dictionary<T2, T1> revDict;
}
