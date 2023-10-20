using System;

// Token: 0x0200022A RID: 554
public static class StaticHashExt
{
	// Token: 0x06000DC7 RID: 3527 RVA: 0x00050635 File Offset: 0x0004E835
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Calculate(i);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0005063D File Offset: 0x0004E83D
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Calculate(f);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00050645 File Offset: 0x0004E845
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Calculate(l);
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x0005064D File Offset: 0x0004E84D
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Calculate(d);
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00050655 File Offset: 0x0004E855
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Calculate(b);
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0005065D File Offset: 0x0004E85D
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Calculate(dt);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00050665 File Offset: 0x0004E865
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Calculate(s);
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x0005066D File Offset: 0x0004E86D
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Calculate(bytes);
	}
}
