using System;

[Serializable]
public struct StateHash
{
	public bool Changed()
	{
		if (this.former == this.newest)
		{
			return false;
		}
		this.former = this.newest;
		return true;
	}

	public void Poll<T0>(T0 v0)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T0>(v0);
	}

	public void Poll<T1, T2>(T1 v1, T2 v2)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2>(v1, v2);
	}

	public void Poll<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2, T3>(v1, v2, v3);
	}

	public void Poll<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2, T3, T4>(v1, v2, v3, v4);
	}

	public void Poll<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2, T3, T4, T5>(v1, v2, v3, v4, v5);
	}

	public void Poll<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2, T3, T4, T5, T6>(v1, v2, v3, v4, v5, v6);
	}

	public void Poll<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7>(v1, v2, v3, v4, v5, v6, v7);
	}

	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
	{
		this.former = this.newest;
		this.newest = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
	}

	public int former;

	public int newest;
}
