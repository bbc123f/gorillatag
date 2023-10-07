using System;

namespace BoingKit
{
	// Token: 0x02000378 RID: 888
	public struct BitArray
	{
		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06001A38 RID: 6712 RVA: 0x00091F5C File Offset: 0x0009015C
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x00091F64 File Offset: 0x00090164
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x00091F69 File Offset: 0x00090169
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x00091F70 File Offset: 0x00090170
		private static void SetBit(int index, bool value, int[] blocks)
		{
			int blockIndex = BitArray.GetBlockIndex(index);
			int subIndex = BitArray.GetSubIndex(index);
			if (value)
			{
				blocks[blockIndex] |= 1 << subIndex;
				return;
			}
			blocks[blockIndex] &= ~(1 << subIndex);
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x00091FB2 File Offset: 0x000901B2
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & 1 << BitArray.GetSubIndex(index)) != 0;
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x00091FCC File Offset: 0x000901CC
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x00091FF4 File Offset: 0x000901F4
		public void Resize(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			if (num <= this.m_aBlock.Length)
			{
				return;
			}
			int[] array = new int[num];
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				array[i] = this.m_aBlock[i];
				i++;
			}
			this.m_aBlock = array;
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x00092043 File Offset: 0x00090243
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x0009204C File Offset: 0x0009024C
		public void SetAllBits(bool value)
		{
			int num = value ? -1 : 1;
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				this.m_aBlock[i] = num;
				i++;
			}
		}

		// Token: 0x06001A41 RID: 6721 RVA: 0x0009207F File Offset: 0x0009027F
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x0009208E File Offset: 0x0009028E
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x04001AA7 RID: 6823
		private int[] m_aBlock;
	}
}
