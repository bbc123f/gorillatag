using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001D3 RID: 467
[Serializable]
public struct SRand
{
	// Token: 0x06000BF8 RID: 3064 RVA: 0x00049C28 File Offset: 0x00047E28
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x00049C3D File Offset: 0x00047E3D
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00049C52 File Offset: 0x00047E52
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00049C6C File Offset: 0x00047E6C
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00049C9E File Offset: 0x00047E9E
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00049CCF File Offset: 0x00047ECF
	public double NextDouble()
	{
		return this.NextState() % 268435457U / 268435456.0;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00049CE9 File Offset: 0x00047EE9
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00049D0C File Offset: 0x00047F0C
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

	// Token: 0x06000C00 RID: 3072 RVA: 0x00049D37 File Offset: 0x00047F37
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00049D40 File Offset: 0x00047F40
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00049D4B File Offset: 0x00047F4B
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00049D58 File Offset: 0x00047F58
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00049D65 File Offset: 0x00047F65
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00049D6D File Offset: 0x00047F6D
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00049D75 File Offset: 0x00047F75
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00049D88 File Offset: 0x00047F88
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00049DA8 File Offset: 0x00047FA8
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

	// Token: 0x06000C09 RID: 3081 RVA: 0x00049DEC File Offset: 0x00047FEC
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

	// Token: 0x06000C0A RID: 3082 RVA: 0x00049E30 File Offset: 0x00048030
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00049E3E File Offset: 0x0004803E
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00049E53 File Offset: 0x00048053
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00049E68 File Offset: 0x00048068
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00049E82 File Offset: 0x00048082
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x00049EB4 File Offset: 0x000480B4
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Calculate(seed);
		this._state = this._seed;
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x00049EE8 File Offset: 0x000480E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00049F10 File Offset: 0x00048110
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = (x >> 16 ^ x) * 73244475U;
		x = (x >> 16 ^ x) * 73244475U;
		x = (x >> 16 ^ x);
		return x;
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00049F37 File Offset: 0x00048137
	public override int GetHashCode()
	{
		return StaticHash.Combine((int)this._seed, (int)this._state);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00049F4C File Offset: 0x0004814C
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

	// Token: 0x06000C14 RID: 3092 RVA: 0x00049FA0 File Offset: 0x000481A0
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow.ToBinary());
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x00049FBF File Offset: 0x000481BF
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x00049FC7 File Offset: 0x000481C7
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00049FCF File Offset: 0x000481CF
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00049FD7 File Offset: 0x000481D7
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00049FDF File Offset: 0x000481DF
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x04000F74 RID: 3956
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x04000F75 RID: 3957
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x04000F76 RID: 3958
	[SerializeField]
	private uint _seed;

	// Token: 0x04000F77 RID: 3959
	[SerializeField]
	private uint _state;
}
