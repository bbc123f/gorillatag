using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class DevConsoleInstance : MonoBehaviour
{
	// Token: 0x06000180 RID: 384 RVA: 0x0000C03C File Offset: 0x0000A23C
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000248 RID: 584
	public GorillaDevButton[] buttons;

	// Token: 0x04000249 RID: 585
	public GameObject[] disableWhileActive;

	// Token: 0x0400024A RID: 586
	public GameObject[] enableWhileActive;

	// Token: 0x0400024B RID: 587
	public float maxHeight;

	// Token: 0x0400024C RID: 588
	public float lineHeight;

	// Token: 0x0400024D RID: 589
	public int targetLogIndex = -1;

	// Token: 0x0400024E RID: 590
	public int currentLogIndex;

	// Token: 0x0400024F RID: 591
	public int expandAmount = 20;

	// Token: 0x04000250 RID: 592
	public int expandedMessageIndex = -1;

	// Token: 0x04000251 RID: 593
	public bool canExpand = true;

	// Token: 0x04000252 RID: 594
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04000253 RID: 595
	public HashSet<LogType> selectedLogTypes = new HashSet<LogType>
	{
		LogType.Error,
		LogType.Exception,
		LogType.Log,
		LogType.Warning,
		LogType.Assert
	};

	// Token: 0x04000254 RID: 596
	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	// Token: 0x04000255 RID: 597
	[SerializeField]
	private GorillaDevButton BottomButton;

	// Token: 0x04000256 RID: 598
	public float lineStartHeight;

	// Token: 0x04000257 RID: 599
	public float lineStartZ;

	// Token: 0x04000258 RID: 600
	public float textStartHeight;

	// Token: 0x04000259 RID: 601
	public float lineStartTextWidth;

	// Token: 0x0400025A RID: 602
	public double textScale = 0.5;

	// Token: 0x0400025B RID: 603
	public bool isEnabled = true;

	// Token: 0x0400025C RID: 604
	[SerializeField]
	private GameObject ConsoleLineExample;
}
