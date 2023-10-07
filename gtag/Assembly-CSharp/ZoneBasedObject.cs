using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class ZoneBasedObject : MonoBehaviour
{
	// Token: 0x06000B83 RID: 2947 RVA: 0x00046F08 File Offset: 0x00045108
	public bool IsLocalPlayerInZone()
	{
		GTZone[] array = this.zones;
		for (int i = 0; i < array.Length; i++)
		{
			if (ZoneManagement.IsInZone(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00046F38 File Offset: 0x00045138
	public static ZoneBasedObject SelectRandomEligible(ZoneBasedObject[] objects, string overrideChoice = "")
	{
		if (overrideChoice != "")
		{
			foreach (ZoneBasedObject zoneBasedObject in objects)
			{
				if (zoneBasedObject.gameObject.name == overrideChoice)
				{
					return zoneBasedObject;
				}
			}
		}
		ZoneBasedObject result = null;
		int num = 0;
		foreach (ZoneBasedObject zoneBasedObject2 in objects)
		{
			if (zoneBasedObject2.gameObject.activeInHierarchy)
			{
				GTZone[] array = zoneBasedObject2.zones;
				for (int j = 0; j < array.Length; j++)
				{
					if (ZoneManagement.IsInZone(array[j]))
					{
						if (Random.Range(0, num) == 0)
						{
							result = zoneBasedObject2;
						}
						num++;
						break;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x04000F13 RID: 3859
	public GTZone[] zones;
}
