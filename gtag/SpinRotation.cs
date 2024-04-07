using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpinRotation : MonoBehaviour, ITickSystemTick
{
	public bool TickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<TickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<TickRunning>k__BackingField = value;
		}
	}

	public void Tick()
	{
		base.transform.localRotation = Quaternion.Euler(this.rotationPerSecondEuler * (Time.time - this.baseTime)) * this.baseRotation;
	}

	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.baseTime = Time.time;
	}

	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	public SpinRotation()
	{
	}

	[SerializeField]
	private Vector3 rotationPerSecondEuler;

	private Quaternion baseRotation;

	private float baseTime;

	[CompilerGenerated]
	private bool <TickRunning>k__BackingField;
}
