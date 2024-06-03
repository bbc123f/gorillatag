using System;

public static class StaticHashExt
{
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Calculate(i);
	}

	public static int GetStaticHash(this uint u)
	{
		return StaticHash.Calculate(u);
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
}
