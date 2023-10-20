using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x04000277 RID: 631
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x04000278 RID: 632
	public Transform colorPoint;

	// Token: 0x0200039B RID: 923
	public enum ColorMode
	{
		// Token: 0x04001B4D RID: 6989
		None,
		// Token: 0x04001B4E RID: 6990
		Red,
		// Token: 0x04001B4F RID: 6991
		Green,
		// Token: 0x04001B50 RID: 6992
		Blue,
		// Token: 0x04001B51 RID: 6993
		Black,
		// Token: 0x04001B52 RID: 6994
		Clear
	}
}
