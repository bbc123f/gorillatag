using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x020001CB RID: 459
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x06000BA4 RID: 2980 RVA: 0x00049194 File Offset: 0x00047394
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

	// Token: 0x06000BA5 RID: 2981 RVA: 0x000491E8 File Offset: 0x000473E8
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0004923C File Offset: 0x0004743C
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00049290 File Offset: 0x00047490
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x000492E4 File Offset: 0x000474E4
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

	// Token: 0x06000BA9 RID: 2985 RVA: 0x00049350 File Offset: 0x00047550
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

	// Token: 0x06000BAA RID: 2986 RVA: 0x000493CD File Offset: 0x000475CD
	[return: TupleElementNames(new string[]
	{
		"l1",
		"l2"
	})]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x000493E0 File Offset: 0x000475E0
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

	// Token: 0x06000BAC RID: 2988 RVA: 0x000493FF File Offset: 0x000475FF
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0004940C File Offset: 0x0004760C
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0004942C File Offset: 0x0004762C
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0004943A File Offset: 0x0004763A
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00049448 File Offset: 0x00047648
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

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0004949B File Offset: 0x0004769B
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x000494AE File Offset: 0x000476AE
	public override int GetHashCode()
	{
		return StaticHash.Combine(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x000494D0 File Offset: 0x000476D0
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00049508 File Offset: 0x00047708
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

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0004956E File Offset: 0x0004776E
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0004957C File Offset: 0x0004777C
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

	// Token: 0x06000BB7 RID: 2999 RVA: 0x000495D8 File Offset: 0x000477D8
	public static Id128 CalculateSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x000495F3 File Offset: 0x000477F3
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x000495FD File Offset: 0x000477FD
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0004960A File Offset: 0x0004780A
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x00049614 File Offset: 0x00047814
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x00049621 File Offset: 0x00047821
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x00049630 File Offset: 0x00047830
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00049642 File Offset: 0x00047842
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0004964C File Offset: 0x0004784C
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00049659 File Offset: 0x00047859
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x00049668 File Offset: 0x00047868
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0004967A File Offset: 0x0004787A
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00049687 File Offset: 0x00047887
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x00049694 File Offset: 0x00047894
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x000496A4 File Offset: 0x000478A4
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x000496B4 File Offset: 0x000478B4
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x000496BC File Offset: 0x000478BC
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x000496C4 File Offset: 0x000478C4
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x000496CC File Offset: 0x000478CC
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x000496D4 File Offset: 0x000478D4
	public static explicit operator Id128(string s)
	{
		return Id128.CalculateMD5(s);
	}

	// Token: 0x04000F4D RID: 3917
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x04000F4E RID: 3918
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x04000F4F RID: 3919
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x04000F50 RID: 3920
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x04000F51 RID: 3921
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x04000F52 RID: 3922
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x04000F53 RID: 3923
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x04000F54 RID: 3924
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x04000F55 RID: 3925
	public static readonly Id128 Empty;
}
