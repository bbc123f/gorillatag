using System;

// Token: 0x020001AA RID: 426
public interface ITimeOfDaySystem
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000AF9 RID: 2809
	double currentTimeInSeconds { get; }

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000AFA RID: 2810
	double totalTimeInSeconds { get; }
}
