using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000378 RID: 888
	public struct Aabb
	{
		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001A22 RID: 6690 RVA: 0x00091FAB File Offset: 0x000901AB
		// (set) Token: 0x06001A23 RID: 6691 RVA: 0x00091FB8 File Offset: 0x000901B8
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

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001A24 RID: 6692 RVA: 0x00091FC6 File Offset: 0x000901C6
		// (set) Token: 0x06001A25 RID: 6693 RVA: 0x00091FD3 File Offset: 0x000901D3
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

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06001A26 RID: 6694 RVA: 0x00091FE1 File Offset: 0x000901E1
		// (set) Token: 0x06001A27 RID: 6695 RVA: 0x00091FEE File Offset: 0x000901EE
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

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06001A28 RID: 6696 RVA: 0x00091FFC File Offset: 0x000901FC
		// (set) Token: 0x06001A29 RID: 6697 RVA: 0x00092009 File Offset: 0x00090209
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

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06001A2A RID: 6698 RVA: 0x00092017 File Offset: 0x00090217
		// (set) Token: 0x06001A2B RID: 6699 RVA: 0x00092024 File Offset: 0x00090224
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

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06001A2C RID: 6700 RVA: 0x00092032 File Offset: 0x00090232
		// (set) Token: 0x06001A2D RID: 6701 RVA: 0x0009203F File Offset: 0x0009023F
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

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06001A2E RID: 6702 RVA: 0x0009204D File Offset: 0x0009024D
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06001A2F RID: 6703 RVA: 0x0009206C File Offset: 0x0009026C
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

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06001A30 RID: 6704 RVA: 0x000920D1 File Offset: 0x000902D1
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x00092100 File Offset: 0x00090300
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x0009211C File Offset: 0x0009031C
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x00092140 File Offset: 0x00090340
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x00092150 File Offset: 0x00090350
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x000921E8 File Offset: 0x000903E8
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0009224E File Offset: 0x0009044E
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x00092271 File Offset: 0x00090471
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x00092294 File Offset: 0x00090494
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x000922B8 File Offset: 0x000904B8
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x00092324 File Offset: 0x00090524
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x00092384 File Offset: 0x00090584
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

		// Token: 0x04001AB1 RID: 6833
		public Vector3 Min;

		// Token: 0x04001AB2 RID: 6834
		public Vector3 Max;
	}
}
