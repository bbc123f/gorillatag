using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x0200033F RID: 831
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x060017F5 RID: 6133 RVA: 0x000817C0 File Offset: 0x0007F9C0
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

		// Token: 0x0400190F RID: 6415
		[SerializeField]
		protected int bits;
	}
}
