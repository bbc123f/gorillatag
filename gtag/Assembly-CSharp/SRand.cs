using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001D4 RID: 468
[Serializable]
public struct SRand
{
	// Token: 0x06000BFE RID: 3070 RVA: 0x00049E90 File Offset: 0x00048090
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00049EA5 File Offset: 0x000480A5
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x00049EBA File Offset: 0x000480BA
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00049ED4 File Offset: 0x000480D4
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00049F06 File Offset: 0x00048106
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00049F37 File Offset: 0x00048137
	public double NextDouble()
	{
		return this.NextState() % 268435457U / 268435456.0;
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00049F51 File Offset: 0x00048151
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00049F74 File Offset: 0x00048174
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

	// Token: 0x06000C06 RID: 3078 RVA: 0x00049F9F File Offset: 0x0004819F
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00049FA8 File Offset: 0x000481A8
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00049FB3 File Offset: 0x000481B3
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00049FC0 File Offset: 0x000481C0
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00049FCD File Offset: 0x000481CD
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00049FD5 File Offset: 0x000481D5
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00049FDD File Offset: 0x000481DD
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00049FF0 File Offset: 0x000481F0
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0004A010 File Offset: 0x00048210
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

	// Token: 0x06000C0F RID: 3087 RVA: 0x0004A054 File Offset: 0x00048254
	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int index = this.NextInt(i--);
			T value = list[i];
			list[i] = list[index];
			list[index] = value;
		}
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0004A098 File Offset: 0x00048298
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0004A0A6 File Offset: 0x000482A6
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x0004A0BB File Offset: 0x000482BB
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0004A0D0 File Offset: 0x000482D0
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0004A0EA File Offset: 0x000482EA
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0004A11C File Offset: 0x0004831C
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x0004A150 File Offset: 0x00048350
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0004A178 File Offset: 0x00048378
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = (x >> 16 ^ x) * 73244475U;
		x = (x >> 16 ^ x) * 73244475U;
		x = (x >> 16 ^ x);
		return x;
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0004A19F File Offset: 0x0004839F
	public override int GetHashCode()
	{
		return StaticHash.Combine((int)this._seed, (int)this._state);
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0004A1B4 File Offset: 0x000483B4
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[]
		{
			"SRand",
			"_seed",
			this._seed,
			"_state",
			this._state
		});
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x0004A208 File Offset: 0x00048408
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow.ToBinary());
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0004A227 File Offset: 0x00048427
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0004A22F File Offset: 0x0004842F
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x0004A237 File Offset: 0x00048437
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0004A23F File Offset: 0x0004843F
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0004A247 File Offset: 0x00048447
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x04000F78 RID: 3960
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x04000F79 RID: 3961
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x04000F7A RID: 3962
	[SerializeField]
	private uint _seed;

	// Token: 0x04000F7B RID: 3963
	[SerializeField]
	private uint _state;
}
