using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x020001CA RID: 458
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x06000B9E RID: 2974 RVA: 0x00048F2C File Offset: 0x0004712C
	public Id128(int a, int b, int c, int d)
	{
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = (this.y = 0L);
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00048F80 File Offset: 0x00047180
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00048FD4 File Offset: 0x000471D4
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00049028 File Offset: 0x00047228
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0004907C File Offset: 0x0004727C
	public Id128(string guid)
	{
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = Guid.Parse(guid);
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x000490E8 File Offset: 0x000472E8
	public Id128(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 16)
		{
			throw new ArgumentException("Input buffer must be exactly 16 bytes", "bytes");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = new Guid(bytes);
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x00049165 File Offset: 0x00047365
	[return: TupleElementNames(new string[]
	{
		"l1",
		"l2"
	})]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x00049178 File Offset: 0x00047378
	[return: TupleElementNames(new string[]
	{
		"i1",
		"i2",
		"i3",
		"i4"
	})]
	public ValueTuple<int, int, int, int> ToInts()
	{
		return new ValueTuple<int, int, int, int>(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x00049197 File Offset: 0x00047397
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x000491A4 File Offset: 0x000473A4
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x000491C4 File Offset: 0x000473C4
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x000491D2 File Offset: 0x000473D2
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x000491E0 File Offset: 0x000473E0
	public override bool Equals(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.Equals(id);
		}
		if (obj is Guid)
		{
			Guid g = (Guid)obj;
			return this.Equals(g);
		}
		if (obj is Hash128)
		{
			Hash128 h = (Hash128)obj;
			return this.Equals(h);
		}
		return false;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00049233 File Offset: 0x00047433
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x00049246 File Offset: 0x00047446
	public override int GetHashCode()
	{
		return StaticHash.Combine(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00049268 File Offset: 0x00047468
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x000492A0 File Offset: 0x000474A0
	public int CompareTo(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.CompareTo(id);
		}
		if (obj is Guid)
		{
			Guid value = (Guid)obj;
			return this.guid.CompareTo(value);
		}
		if (obj is Hash128)
		{
			Hash128 rhs = (Hash128)obj;
			return this.h128.CompareTo(rhs);
		}
		throw new ArgumentException("Object must be of type Id128 or Guid");
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x00049306 File Offset: 0x00047506
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00049314 File Offset: 0x00047514
	public static Id128 CalculateMD5(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		Id128 result;
		using (MD5 md = MD5.Create())
		{
			result = new Guid(md.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		return result;
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00049370 File Offset: 0x00047570
	public static Id128 CalculateSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0004938B File Offset: 0x0004758B
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00049395 File Offset: 0x00047595
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x000493A2 File Offset: 0x000475A2
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x000493AC File Offset: 0x000475AC
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x000493B9 File Offset: 0x000475B9
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x000493C8 File Offset: 0x000475C8
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x000493DA File Offset: 0x000475DA
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x000493E4 File Offset: 0x000475E4
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x000493F1 File Offset: 0x000475F1
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x00049400 File Offset: 0x00047600
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x00049412 File Offset: 0x00047612
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0004941F File Offset: 0x0004761F
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0004942C File Offset: 0x0004762C
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0004943C File Offset: 0x0004763C
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0004944C File Offset: 0x0004764C
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x00049454 File Offset: 0x00047654
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0004945C File Offset: 0x0004765C
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00049464 File Offset: 0x00047664
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0004946C File Offset: 0x0004766C
	public static explicit operator Id128(string s)
	{
		return Id128.CalculateMD5(s);
	}

	// Token: 0x04000F49 RID: 3913
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x04000F4A RID: 3914
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x04000F4B RID: 3915
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x04000F4C RID: 3916
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x04000F4D RID: 3917
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x04000F4E RID: 3918
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x04000F4F RID: 3919
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x04000F50 RID: 3920
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x04000F51 RID: 3921
	public static readonly Id128 Empty;
}
