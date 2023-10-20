using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000379 RID: 889
	[Serializable]
	public struct Bits32
	{
		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06001A3C RID: 6716 RVA: 0x000923EB File Offset: 0x000905EB
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x000923F3 File Offset: 0x000905F3
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x000923FC File Offset: 0x000905FC
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x00092405 File Offset: 0x00090605
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x00092432 File Offset: 0x00090632
		public bool IsBitSet(int index)
		{
			return (this.m_bits & 1 << index) != 0;
		}

		// Token: 0x04001AB3 RID: 6835
		[SerializeField]
		private int m_bits;
	}
}
