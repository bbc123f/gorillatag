using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000072 RID: 114
[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour
{
	// Token: 0x0600023D RID: 573 RVA: 0x0000F39F File Offset: 0x0000D59F
	protected void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000F3C0 File Offset: 0x0000D5C0
	protected void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < ThermalManager.receivers.Count; i++)
		{
			ThermalReceiver thermalReceiver = ThermalManager.receivers[i];
			Transform transform = thermalReceiver.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = 20f;
			for (int j = 0; j < ThermalManager.sources.Count; j++)
			{
				ThermalSourceVolume thermalSourceVolume = ThermalManager.sources[j];
				Transform transform2 = thermalSourceVolume.transform;
				float x2 = transform2.lossyScale.x;
				float num2 = Vector3.Distance(transform2.position, position);
				float num3 = 1f - Mathf.InverseLerp(thermalSourceVolume.innerRadius * x2, thermalSourceVolume.outerRadius * x2, num2 - thermalReceiver.radius * x);
				num += thermalSourceVolume.celsius * num3;
			}
			thermalReceiver.celsius = Mathf.Lerp(thermalReceiver.celsius, num, deltaTime * thermalReceiver.conductivity);
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000F4B9 File Offset: 0x0000D6B9
	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000F4C6 File Offset: 0x0000D6C6
	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000F4D4 File Offset: 0x0000D6D4
	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000F4E1 File Offset: 0x0000D6E1
	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	// Token: 0x040002EC RID: 748
	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	// Token: 0x040002ED RID: 749
	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	// Token: 0x040002EE RID: 750
	[NonSerialized]
	public static ThermalManager instance;
}
