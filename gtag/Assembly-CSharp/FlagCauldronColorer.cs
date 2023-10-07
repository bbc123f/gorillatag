using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x04000277 RID: 631
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x04000278 RID: 632
	public Transform colorPoint;

	// Token: 0x02000399 RID: 921
	public enum ColorMode
	{
		// Token: 0x04001B40 RID: 6976
		None,
		// Token: 0x04001B41 RID: 6977
		Red,
		// Token: 0x04001B42 RID: 6978
		Green,
		// Token: 0x04001B43 RID: 6979
		Blue,
		// Token: 0x04001B44 RID: 6980
		Black,
		// Token: 0x04001B45 RID: 6981
		Clear
	}
}
