using System;
using TMPro;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class GorillaDebugUI : MonoBehaviour
{
	// Token: 0x040009B6 RID: 2486
	private readonly float Delay = 0.5f;

	// Token: 0x040009B7 RID: 2487
	public GameObject parentCanvas;

	// Token: 0x040009B8 RID: 2488
	public GameObject rayInteractorLeft;

	// Token: 0x040009B9 RID: 2489
	public GameObject rayInteractorRight;

	// Token: 0x040009BA RID: 2490
	[SerializeField]
	private TMP_Dropdown playfabIdDropdown;

	// Token: 0x040009BB RID: 2491
	[SerializeField]
	private TMP_Dropdown roomIdDropdown;

	// Token: 0x040009BC RID: 2492
	[SerializeField]
	private TMP_Dropdown locationDropdown;

	// Token: 0x040009BD RID: 2493
	[SerializeField]
	private TMP_Dropdown playerNameDropdown;

	// Token: 0x040009BE RID: 2494
	[SerializeField]
	private TMP_Dropdown gameModeDropdown;

	// Token: 0x040009BF RID: 2495
	[SerializeField]
	private TMP_Dropdown timeOfDayDropdown;

	// Token: 0x040009C0 RID: 2496
	[SerializeField]
	private TMP_Text networkStateTextBox;

	// Token: 0x040009C1 RID: 2497
	[SerializeField]
	private TMP_Text gameModeTextBox;

	// Token: 0x040009C2 RID: 2498
	[SerializeField]
	private TMP_Text currentRoomTextBox;

	// Token: 0x040009C3 RID: 2499
	[SerializeField]
	private TMP_Text playerCountTextBox;

	// Token: 0x040009C4 RID: 2500
	[SerializeField]
	private TMP_Text roomVisibilityTextBox;

	// Token: 0x040009C5 RID: 2501
	[SerializeField]
	private TMP_Text timeMultiplierTextBox;

	// Token: 0x040009C6 RID: 2502
	[SerializeField]
	private TMP_Text versionTextBox;
}
