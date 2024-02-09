using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour
{
	protected void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
	}

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

	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	[NonSerialized]
	public static ThermalManager instance;
}
