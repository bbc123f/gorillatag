using System;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class Flashlight : MonoBehaviour
{
	// Token: 0x060003CE RID: 974 RVA: 0x000177AC File Offset: 0x000159AC
	private void LateUpdate()
	{
		for (int i = 0; i < this.lightVolume.transform.childCount; i++)
		{
			this.lightVolume.transform.GetChild(i).rotation = Quaternion.LookRotation((this.lightVolume.transform.GetChild(i).position - Camera.main.transform.position).normalized);
		}
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00017824 File Offset: 0x00015A24
	public void ToggleFlashlight()
	{
		this.lightVolume.SetActive(!this.lightVolume.activeSelf);
		this.spotlight.enabled = !this.spotlight.enabled;
		this.bulbGlow.SetActive(this.lightVolume.activeSelf);
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00017879 File Offset: 0x00015A79
	public void EnableFlashlight(bool doEnable)
	{
		this.lightVolume.SetActive(doEnable);
		this.spotlight.enabled = doEnable;
		this.bulbGlow.SetActive(doEnable);
	}

	// Token: 0x04000466 RID: 1126
	public GameObject lightVolume;

	// Token: 0x04000467 RID: 1127
	public Light spotlight;

	// Token: 0x04000468 RID: 1128
	public GameObject bulbGlow;
}
