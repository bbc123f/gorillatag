using System;
using GorillaTag;
using UnityEngine;

public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
	}

	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	public ThermalReceiver()
	{
	}

	public float radius = 0.2f;

	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	[DebugOption]
	public float celsius;

	private float defaultCelsius;
}
