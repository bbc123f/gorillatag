using System;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class ThermalSourceVolume : MonoBehaviour
{
	// Token: 0x0600024C RID: 588 RVA: 0x0000F7A1 File Offset: 0x0000D9A1
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000F7A9 File Offset: 0x0000D9A9
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x040002F3 RID: 755
	[Tooltip("Temperature in celsius. Default is 20 which is room temperature.")]
	public float celsius = 20f;

	// Token: 0x040002F4 RID: 756
	public float innerRadius = 0.1f;

	// Token: 0x040002F5 RID: 757
	public float outerRadius = 1f;
}
