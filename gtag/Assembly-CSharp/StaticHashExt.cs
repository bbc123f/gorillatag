using System;

// Token: 0x02000229 RID: 553
public static class StaticHashExt
{
	// Token: 0x06000DC1 RID: 3521 RVA: 0x000503D5 File Offset: 0x0004E5D5
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Calculate(i);
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x000503DD File Offset: 0x0004E5DD
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Calculate(f);
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x000503E5 File Offset: 0x0004E5E5
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Calculate(l);
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x000503ED File Offset: 0x0004E5ED
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Calculate(d);
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x000503F5 File Offset: 0x0004E5F5
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Calculate(b);
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x000503FD File Offset: 0x0004E5FD
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Calculate(dt);
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x00050405 File Offset: 0x0004E605
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Calculate(s);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0005040D File Offset: 0x0004E60D
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Calculate(bytes);
	}
}
