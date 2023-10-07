using System;

// Token: 0x020001A9 RID: 425
public interface ITimeOfDaySystem
{
	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000AF4 RID: 2804
	double currentTimeInSeconds { get; }

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000AF5 RID: 2805
	double totalTimeInSeconds { get; }
}
