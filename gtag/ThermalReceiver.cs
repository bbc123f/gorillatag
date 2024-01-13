using GorillaTag;
using UnityEngine;

public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	public float radius = 0.2f;

	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	[DebugOption]
	public float celsius;

	private float defaultCelsius;

	public float Farenheit => celsius * 1.8f + 32f;

	public float floatValue => celsius;

	protected void Awake()
	{
		defaultCelsius = celsius;
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
		celsius = defaultCelsius;
	}
}
