using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utilities;

namespace GT;

[Serializable]
public struct SRand
{
	private const uint INT32_MAX_PLUS_ONE = 2147483648u;

	private const double INT32_MAX_AS_DOUBLE = 2147483647.0;

	[SerializeField]
	private uint _seed;

	[SerializeField]
	private uint _state;

	public SRand(int seed)
	{
		_seed = (uint)seed;
		_state = _seed;
	}

	public SRand(uint seed)
	{
		_seed = seed;
		_state = _seed;
	}

	public SRand(long seed)
	{
		_seed = (uint)StaticHash.Calculate(seed);
		_state = _seed;
	}

	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		_seed = (uint)StaticHash.Calculate(seed);
		_state = _seed;
	}

	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		_seed = (uint)StaticHash.Calculate(seed);
		_state = _seed;
	}

	public double NextDouble()
	{
		return (double)(NextState() % 2147483648u) / 2147483647.0;
	}

	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return NextDouble() * max;
	}

	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = NextDouble() * num;
		return min + num2;
	}

	public float NextFloat()
	{
		return (float)NextDouble();
	}

	public float NextFloat(float max)
	{
		return (float)NextDouble(max);
	}

	public float NextFloat(float min, float max)
	{
		return (float)NextDouble(min, max);
	}

	public bool NextBool()
	{
		return NextState() % 2 == 1;
	}

	public uint NextUInt()
	{
		return NextState();
	}

	public int NextInt()
	{
		return (int)NextState();
	}

	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)(NextState() % max);
	}

	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + NextInt(num);
	}

	public void Shuffle<T>(T[] array)
	{
		int num = array.Length;
		while (num > 1)
		{
			int num2 = NextInt(num--);
			T val = array[num];
			array[num] = array[num2];
			array[num2] = val;
		}
	}

	public void Shuffle<T>(List<T> list)
	{
		int count = list.Count;
		while (count > 1)
		{
			int index = NextInt(count--);
			T value = list[count];
			list[count] = list[index];
			list[index] = value;
		}
	}

	public void Reset()
	{
		_state = _seed;
	}

	public void Reset(int seed)
	{
		_seed = (uint)seed;
		_state = _seed;
	}

	public void Reset(uint seed)
	{
		_seed = seed;
		_state = _seed;
	}

	public void Reset(long seed)
	{
		_seed = (uint)StaticHash.Calculate(seed);
		_state = _seed;
	}

	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		_seed = (uint)StaticHash.Calculate(seed);
		_state = _seed;
	}

	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		_seed = (uint)StaticHash.Calculate(seed);
		_state = _seed;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return _state = Mix(_state + 184402071);
	}

	private uint Mix(uint x)
	{
		x = ((x >> 16) ^ x) * 73244475;
		x = ((x >> 16) ^ x) * 73244475;
		x = (x >> 16) ^ x;
		return x;
	}

	public override int GetHashCode()
	{
		return StaticHash.Combine((int)_seed, (int)_state);
	}

	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", "SRand", "_seed", _seed, "_state", _state);
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
}
