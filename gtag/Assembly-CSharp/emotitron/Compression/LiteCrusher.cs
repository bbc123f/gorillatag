using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000341 RID: 833
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x060017FE RID: 6142 RVA: 0x00081CA8 File Offset: 0x0007FEA8
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x0400191C RID: 6428
		[SerializeField]
		protected int bits;
	}
}
