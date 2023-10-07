using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000377 RID: 887
	[Serializable]
	public struct Bits32
	{
		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06001A33 RID: 6707 RVA: 0x00091F03 File Offset: 0x00090103
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x00091F0B File Offset: 0x0009010B
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x00091F14 File Offset: 0x00090114
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x00091F1D File Offset: 0x0009011D
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x00091F4A File Offset: 0x0009014A
		public bool IsBitSet(int index)
		{
			return (this.m_bits & 1 << index) != 0;
		}

		// Token: 0x04001AA6 RID: 6822
		[SerializeField]
		private int m_bits;
	}
}
