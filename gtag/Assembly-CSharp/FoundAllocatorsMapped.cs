using System;
using System.Collections.Generic;

// Token: 0x02000043 RID: 67
[Serializable]
public class FoundAllocatorsMapped
{
	// Token: 0x0400020F RID: 527
	public string path;

	// Token: 0x04000210 RID: 528
	public List<ViewsAndAllocator> allocators = new List<ViewsAndAllocator>();

	// Token: 0x04000211 RID: 529
	public List<FoundAllocatorsMapped> subGroups = new List<FoundAllocatorsMapped>();
}
