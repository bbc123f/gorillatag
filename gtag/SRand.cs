using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public struct SRand
{
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	public double NextDouble()
	{
		return this.NextState() % 268435457U / 268435456.0;
	}

	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = this.NextDouble() * num;
		return min + num2;
	}

	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	public uint NextUInt()
	{
		return this.NextState();
	}

	public int NextInt()
	{
		return (int)this.NextState();
	}

	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	public void Shuffle<T>(T[] array)
	{
		int i = array.Length;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			T t = array[i];
			array[i] = array[num];
			array[num] = t;
		}
	}

	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			T t = list[i];
			list[i] = list[num];
			list[num] = t;
		}
	}

	public void Reset()
	{
		this._state = this._seed;
	}

	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = ((x >> 16) ^ x) * 73244475U;
		x = ((x >> 16) ^ x) * 73244475U;
		x = (x >> 16) ^ x;
		return x;
	}

	public override int GetHashCode()
	{
		return StaticHash.Combine((int)this._seed, (int)this._state);
	}

	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[] { "SRand", "_seed", this._seed, "_state", this._state });
	}

	public static SRand New()
	{
		return new SRand(DateTime.UtcNow.ToBinary());
	}

	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	private const uint MAX_PLUS_ONE = 268435457U;

	private const double MAX_AS_DOUBLE = 268435456.0;

	[SerializeField]
	private uint _seed;

	[SerializeField]
	private uint _state;
}
