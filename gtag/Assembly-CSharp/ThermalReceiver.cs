using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000245 RID: 581 RVA: 0x0000F73B File Offset: 0x0000D93B
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000246 RID: 582 RVA: 0x0000F74F File Offset: 0x0000D94F
	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000F757 File Offset: 0x0000D957
	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000F765 File Offset: 0x0000D965
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000F76D File Offset: 0x0000D96D
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000F775 File Offset: 0x0000D975
	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	// Token: 0x040002EF RID: 751
	public float radius = 0.2f;

	// Token: 0x040002F0 RID: 752
	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	// Token: 0x040002F1 RID: 753
	[DebugOption]
	public float celsius;

	// Token: 0x040002F2 RID: 754
	private float defaultCelsius;
}
