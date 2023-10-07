using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200004A RID: 74
public class DevConsoleHand : DevConsoleInstance
{
	// Token: 0x04000236 RID: 566
	public List<GameObject> otherButtonsList;

	// Token: 0x04000237 RID: 567
	private bool isActuallyEnabled = true;

	// Token: 0x04000238 RID: 568
	public bool isStillEnabled = true;

	// Token: 0x04000239 RID: 569
	public bool isLeftHand;

	// Token: 0x0400023A RID: 570
	public ConsoleMode mode;

	// Token: 0x0400023B RID: 571
	public double debugScale;

	// Token: 0x0400023C RID: 572
	public double inspectorScale;

	// Token: 0x0400023D RID: 573
	public double componentInspectorScale;

	// Token: 0x0400023E RID: 574
	public List<GameObject> consoleButtons;

	// Token: 0x0400023F RID: 575
	public List<GameObject> inspectorButtons;

	// Token: 0x04000240 RID: 576
	public List<GameObject> componentInspectorButtons;

	// Token: 0x04000241 RID: 577
	public GorillaDevButton consoleButton;

	// Token: 0x04000242 RID: 578
	public GorillaDevButton inspectorButton;

	// Token: 0x04000243 RID: 579
	public GorillaDevButton componentInspectorButton;

	// Token: 0x04000244 RID: 580
	public GorillaDevButton showNonStarItems;

	// Token: 0x04000245 RID: 581
	public GorillaDevButton showPrivateItems;

	// Token: 0x04000246 RID: 582
	public Text componentInspectionText;

	// Token: 0x04000247 RID: 583
	public DevInspector selectedInspector;
}
