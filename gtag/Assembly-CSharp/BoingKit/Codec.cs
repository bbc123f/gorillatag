using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000372 RID: 882
	public class Codec
	{
		// Token: 0x060019F0 RID: 6640 RVA: 0x000914A8 File Offset: 0x0008F6A8
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x000914CF File Offset: 0x0008F6CF
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x000914E2 File Offset: 0x0008F6E2
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0009150C File Offset: 0x0008F70C
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x00091560 File Offset: 0x0008F760
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x000915F4 File Offset: 0x0008F7F4
		public static Vector3 UnpackNormal(float f)
		{
			Vector2 vector = Codec.UnpackSaturated(f);
			vector = vector * 2f - Vector2.one;
			Vector3 vector2 = new Vector3(vector.x, vector.y, 1f - Mathf.Abs(vector.x) - Mathf.Abs(vector.y));
			float num = Mathf.Clamp01(-vector2.z);
			vector2.x += ((vector2.x >= 0f) ? (-num) : num);
			vector2.y += ((vector2.y >= 0f) ? (-num) : num);
			return vector2.normalized;
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x0009169C File Offset: 0x0008F89C
		public static uint PackRgb(Color color)
		{
			return (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x000916CC File Offset: 0x0008F8CC
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x00091708 File Offset: 0x0008F908
		public static uint PackRgba(Color color)
		{
			return (uint)(color.a * 255f) << 24 | (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x00091754 File Offset: 0x0008F954
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x000917AA File Offset: 0x0008F9AA
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return (x & 255U) << 24 | (y & 255U) << 16 | (z & 255U) << 8 | (w & 255U);
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x000917D3 File Offset: 0x0008F9D3
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24 & 255U);
			y = (i >> 16 & 255U);
			z = (i >> 8 & 255U);
			w = (i & 255U);
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x00091804 File Offset: 0x0008FA04
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x00091827 File Offset: 0x0008FA27
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x00091832 File Offset: 0x0008FA32
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)-1)));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x0009184F File Offset: 0x0008FA4F
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x0009185D File Offset: 0x0008FA5D
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x0009186C File Offset: 0x0008FA6C
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int i2 in ints)
			{
				hash = Codec.HashConcat(hash, i2);
			}
			return hash;
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x00091898 File Offset: 0x0008FA98
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float f in floats)
			{
				hash = Codec.HashConcat(hash, f);
			}
			return hash;
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x000918C3 File Offset: 0x0008FAC3
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y
			});
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x000918E3 File Offset: 0x0008FAE3
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z
			});
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x0009190C File Offset: 0x0008FB0C
		public static int HashConcat(int hash, Vector4 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z,
				v.w
			});
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x0009193E File Offset: 0x0008FB3E
		public static int HashConcat(int hash, Quaternion q)
		{
			return Codec.HashConcat(hash, new float[]
			{
				q.x,
				q.y,
				q.z,
				q.w
			});
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x00091970 File Offset: 0x0008FB70
		public static int HashConcat(int hash, Color c)
		{
			return Codec.HashConcat(hash, new float[]
			{
				c.r,
				c.g,
				c.b,
				c.a
			});
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x000919A2 File Offset: 0x0008FBA2
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x000919B0 File Offset: 0x0008FBB0
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x000919BD File Offset: 0x0008FBBD
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x000919CA File Offset: 0x0008FBCA
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x000919D7 File Offset: 0x0008FBD7
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x000919E4 File Offset: 0x0008FBE4
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x000919F1 File Offset: 0x0008FBF1
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x000919FE File Offset: 0x0008FBFE
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x00091A0B File Offset: 0x0008FC0B
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x00091A18 File Offset: 0x0008FC18
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x00091A25 File Offset: 0x0008FC25
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x00091A32 File Offset: 0x0008FC32
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x00091A40 File Offset: 0x0008FC40
		private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
		{
			hash = Codec.HashConcat(hash, t);
			hash = Codec.HashConcat(hash, t.childCount);
			for (int i = 0; i < t.childCount; i++)
			{
				hash = Codec.HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
			}
			return hash;
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x00091A85 File Offset: 0x0008FC85
		public static int HashTransformHierarchy(Transform t)
		{
			return Codec.HashTransformHierarchyRecurvsive(Codec.FnvDefaultBasis, t);
		}

		// Token: 0x04001A99 RID: 6809
		public static readonly int FnvDefaultBasis = -2128831035;

		// Token: 0x04001A9A RID: 6810
		public static readonly int FnvPrime = 16777619;

		// Token: 0x0200053D RID: 1341
		[StructLayout(LayoutKind.Explicit)]
		private struct IntFloat
		{
			// Token: 0x04002219 RID: 8729
			[FieldOffset(0)]
			public int IntValue;

			// Token: 0x0400221A RID: 8730
			[FieldOffset(0)]
			public float FloatValue;
		}
	}
}
