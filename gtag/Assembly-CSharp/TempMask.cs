using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class TempMask : MonoBehaviour
{
	// Token: 0x060000F1 RID: 241 RVA: 0x00009370 File Offset: 0x00007570
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.photonView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000093DE File Offset: 0x000075DE
	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x000093ED File Offset: 0x000075ED
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x000093F5 File Offset: 0x000075F5
	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0400014E RID: 334
	public int year;

	// Token: 0x0400014F RID: 335
	public int month;

	// Token: 0x04000150 RID: 336
	public int day;

	// Token: 0x04000151 RID: 337
	public DateTime dayOn;

	// Token: 0x04000152 RID: 338
	public MeshRenderer myRenderer;

	// Token: 0x04000153 RID: 339
	private DateTime myDate;

	// Token: 0x04000154 RID: 340
	private VRRig myRig;
}
