using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class LocalActivateOnDateRange : MonoBehaviour
{
	// Token: 0x060001DF RID: 479 RVA: 0x0000D3F4 File Offset: 0x0000B5F4
	private void Awake()
	{
		GameObject[] array = this.gameObjectsToActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000D41F File Offset: 0x0000B61F
	private void OnEnable()
	{
		this.InitActiveTimes();
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000D428 File Offset: 0x0000B628
	private void InitActiveTimes()
	{
		this.activationTime = new DateTime(this.activationYear, this.activationMonth, this.activationDay, this.activationHour, this.activationMinute, this.activationSecond, DateTimeKind.Utc);
		this.deactivationTime = new DateTime(this.deactivationYear, this.deactivationMonth, this.deactivationDay, this.deactivationHour, this.deactivationMinute, this.deactivationSecond, DateTimeKind.Utc);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000D498 File Offset: 0x0000B698
	private void LateUpdate()
	{
		DateTime utcNow = DateTime.UtcNow;
		this.dbgTimeUntilActivation = (this.activationTime - utcNow).TotalSeconds;
		this.dbgTimeUntilDeactivation = (this.deactivationTime - utcNow).TotalSeconds;
		bool flag = utcNow >= this.activationTime && utcNow <= this.deactivationTime;
		if (flag != this.isActive)
		{
			GameObject[] array = this.gameObjectsToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			this.isActive = flag;
		}
	}

	// Token: 0x040002A3 RID: 675
	[Header("Activation Date and Time (UTC)")]
	public int activationYear = 2023;

	// Token: 0x040002A4 RID: 676
	public int activationMonth = 4;

	// Token: 0x040002A5 RID: 677
	public int activationDay = 1;

	// Token: 0x040002A6 RID: 678
	public int activationHour = 7;

	// Token: 0x040002A7 RID: 679
	public int activationMinute;

	// Token: 0x040002A8 RID: 680
	public int activationSecond;

	// Token: 0x040002A9 RID: 681
	[Header("Deactivation Date and Time (UTC)")]
	public int deactivationYear = 2023;

	// Token: 0x040002AA RID: 682
	public int deactivationMonth = 4;

	// Token: 0x040002AB RID: 683
	public int deactivationDay = 2;

	// Token: 0x040002AC RID: 684
	public int deactivationHour = 7;

	// Token: 0x040002AD RID: 685
	public int deactivationMinute;

	// Token: 0x040002AE RID: 686
	public int deactivationSecond;

	// Token: 0x040002AF RID: 687
	public GameObject[] gameObjectsToActivate;

	// Token: 0x040002B0 RID: 688
	private bool isActive;

	// Token: 0x040002B1 RID: 689
	private DateTime activationTime;

	// Token: 0x040002B2 RID: 690
	private DateTime deactivationTime;

	// Token: 0x040002B3 RID: 691
	[DebugReadout]
	public double dbgTimeUntilActivation;

	// Token: 0x040002B4 RID: 692
	[DebugReadout]
	public double dbgTimeUntilDeactivation;
}
