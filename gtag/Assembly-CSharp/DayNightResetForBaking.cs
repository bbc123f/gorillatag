using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class DayNightResetForBaking : MonoBehaviour
{
	// Token: 0x06000AFF RID: 2815 RVA: 0x00044060 File Offset: 0x00042260
	public void SetMaterialsForBaking()
	{
		foreach (Material material in this.dayNightManager.dayNightSupportedMaterials)
		{
			if (material != null)
			{
				material.shader = this.dayNightManager.standard;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
		foreach (Material material2 in this.dayNightManager.dayNightSupportedMaterialsCutout)
		{
			if (material2 != null)
			{
				material2.shader = this.dayNightManager.standardCutout;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials cutout in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00044104 File Offset: 0x00042304
	public void SetMaterialsForGame()
	{
		foreach (Material material in this.dayNightManager.dayNightSupportedMaterials)
		{
			if (material != null)
			{
				material.shader = this.dayNightManager.gorillaUnlit;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
		foreach (Material material2 in this.dayNightManager.dayNightSupportedMaterialsCutout)
		{
			if (material2 != null)
			{
				material2.shader = this.dayNightManager.gorillaUnlitCutout;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materialsc cutout in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
	}

	// Token: 0x04000E24 RID: 3620
	public BetterDayNightManager dayNightManager;
}
