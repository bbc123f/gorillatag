using System;

namespace BoingKit
{
	// Token: 0x0200037A RID: 890
	public struct BitArray
	{
		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x00092444 File Offset: 0x00090644
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x0009244C File Offset: 0x0009064C
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06001A43 RID: 6723 RVA: 0x00092451 File Offset: 0x00090651
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x00092458 File Offset: 0x00090658
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

		// Token: 0x06001A45 RID: 6725 RVA: 0x0009249A File Offset: 0x0009069A
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & 1 << BitArray.GetSubIndex(index)) != 0;
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x000924B4 File Offset: 0x000906B4
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x000924DC File Offset: 0x000906DC
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

		// Token: 0x06001A48 RID: 6728 RVA: 0x0009252B File Offset: 0x0009072B
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x00092534 File Offset: 0x00090734
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

		// Token: 0x06001A4A RID: 6730 RVA: 0x00092567 File Offset: 0x00090767
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x00092576 File Offset: 0x00090776
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x04001AB4 RID: 6836
		private int[] m_aBlock;
	}
}
