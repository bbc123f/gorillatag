using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000376 RID: 886
	public struct Aabb
	{
		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001A19 RID: 6681 RVA: 0x00091AC3 File Offset: 0x0008FCC3
		// (set) Token: 0x06001A1A RID: 6682 RVA: 0x00091AD0 File Offset: 0x0008FCD0
		public float MinX
		{
			get
			{
				return this.Min.x;
			}
			set
			{
				this.Min.x = value;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001A1B RID: 6683 RVA: 0x00091ADE File Offset: 0x0008FCDE
		// (set) Token: 0x06001A1C RID: 6684 RVA: 0x00091AEB File Offset: 0x0008FCEB
		public float MinY
		{
			get
			{
				return this.Min.y;
			}
			set
			{
				this.Min.y = value;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001A1D RID: 6685 RVA: 0x00091AF9 File Offset: 0x0008FCF9
		// (set) Token: 0x06001A1E RID: 6686 RVA: 0x00091B06 File Offset: 0x0008FD06
		public float MinZ
		{
			get
			{
				return this.Min.z;
			}
			set
			{
				this.Min.z = value;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001A1F RID: 6687 RVA: 0x00091B14 File Offset: 0x0008FD14
		// (set) Token: 0x06001A20 RID: 6688 RVA: 0x00091B21 File Offset: 0x0008FD21
		public float MaxX
		{
			get
			{
				return this.Max.x;
			}
			set
			{
				this.Max.x = value;
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06001A21 RID: 6689 RVA: 0x00091B2F File Offset: 0x0008FD2F
		// (set) Token: 0x06001A22 RID: 6690 RVA: 0x00091B3C File Offset: 0x0008FD3C
		public float MaxY
		{
			get
			{
				return this.Max.y;
			}
			set
			{
				this.Max.y = value;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06001A23 RID: 6691 RVA: 0x00091B4A File Offset: 0x0008FD4A
		// (set) Token: 0x06001A24 RID: 6692 RVA: 0x00091B57 File Offset: 0x0008FD57
		public float MaxZ
		{
			get
			{
				return this.Max.z;
			}
			set
			{
				this.Max.z = value;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06001A25 RID: 6693 RVA: 0x00091B65 File Offset: 0x0008FD65
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06001A26 RID: 6694 RVA: 0x00091B84 File Offset: 0x0008FD84
		public Vector3 Size
		{
			get
			{
				Vector3 vector = this.Max - this.Min;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				vector.z = Mathf.Max(0f, vector.z);
				return vector;
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06001A27 RID: 6695 RVA: 0x00091BE9 File Offset: 0x0008FDE9
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x00091C18 File Offset: 0x0008FE18
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x00091C34 File Offset: 0x0008FE34
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x00091C58 File Offset: 0x0008FE58
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x00091C68 File Offset: 0x0008FE68
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x00091D00 File Offset: 0x0008FF00
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x00091D66 File Offset: 0x0008FF66
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x00091D89 File Offset: 0x0008FF89
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x00091DAC File Offset: 0x0008FFAC
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x00091DD0 File Offset: 0x0008FFD0
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x00091E3C File Offset: 0x0009003C
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x00091E9C File Offset: 0x0009009C
		public Aabb Expand(float amount)
		{
			this.MinX -= amount;
			this.MinY -= amount;
			this.MinZ -= amount;
			this.MaxX += amount;
			this.MaxY += amount;
			this.MaxZ += amount;
			return this;
		}

		// Token: 0x04001AA4 RID: 6820
		public Vector3 Min;

		// Token: 0x04001AA5 RID: 6821
		public Vector3 Max;
	}
}
