using System;

namespace Utilities;

public static class StaticHashExt
{
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Calculate(i);
	}

	public static int GetStaticHash(this float f)
	{
		return StaticHash.Calculate(f);
	}

	public static int GetStaticHash(this long l)
	{
		return StaticHash.Calculate(l);
	}

	public static int GetStaticHash(this double d)
	{
		return StaticHash.Calculate(d);
	}

	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Calculate(b);
	}

	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Calculate(dt);
	}

	public static int GetStaticHash(this string s)
	{
		return StaticHash.Calculate(s);
	}

	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Calculate(bytes);
	}

	public static int GetStaticHash(this (string s1, string s2) x)
	{
		return StaticHash.Combine(x.s1, x.s2);
	}

	public static int GetStaticHash(this (string s1, string s2, string s3) x)
	{
		return StaticHash.Combine(x.s1, x.s2, x.s3);
	}

	public static int GetStaticHash(this (string s1, string s2, string s3, string s4) x)
	{
		return StaticHash.Combine(x.s1, x.s2, x.s3, x.s4);
	}

	public static int GetStaticHash(this (int i1, int i2) x)
	{
		return StaticHash.Combine(x.i1, x.i2);
	}

	public static int GetStaticHash(this (int i1, int i2, int i3) x)
	{
		return StaticHash.Combine(x.i1, x.i2, x.i3);
	}

	public static int GetStaticHash(this (int i1, int i2, int i3, int i4) x)
	{
		return StaticHash.Combine(x.i1, x.i2, x.i3, x.i4);
	}
}
