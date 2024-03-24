using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class SizeChangerTrigger : MonoBehaviour
{
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter
	{
		[CompilerGenerated]
		add
		{
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent = this.OnEnter;
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent2;
			do
			{
				sizeChangerTriggerEvent2 = sizeChangerTriggerEvent;
				SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent3 = (SizeChangerTrigger.SizeChangerTriggerEvent)Delegate.Combine(sizeChangerTriggerEvent2, value);
				sizeChangerTriggerEvent = Interlocked.CompareExchange<SizeChangerTrigger.SizeChangerTriggerEvent>(ref this.OnEnter, sizeChangerTriggerEvent3, sizeChangerTriggerEvent2);
			}
			while (sizeChangerTriggerEvent != sizeChangerTriggerEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent = this.OnEnter;
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent2;
			do
			{
				sizeChangerTriggerEvent2 = sizeChangerTriggerEvent;
				SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent3 = (SizeChangerTrigger.SizeChangerTriggerEvent)Delegate.Remove(sizeChangerTriggerEvent2, value);
				sizeChangerTriggerEvent = Interlocked.CompareExchange<SizeChangerTrigger.SizeChangerTriggerEvent>(ref this.OnEnter, sizeChangerTriggerEvent3, sizeChangerTriggerEvent2);
			}
			while (sizeChangerTriggerEvent != sizeChangerTriggerEvent2);
		}
	}

	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit
	{
		[CompilerGenerated]
		add
		{
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent = this.OnExit;
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent2;
			do
			{
				sizeChangerTriggerEvent2 = sizeChangerTriggerEvent;
				SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent3 = (SizeChangerTrigger.SizeChangerTriggerEvent)Delegate.Combine(sizeChangerTriggerEvent2, value);
				sizeChangerTriggerEvent = Interlocked.CompareExchange<SizeChangerTrigger.SizeChangerTriggerEvent>(ref this.OnExit, sizeChangerTriggerEvent3, sizeChangerTriggerEvent2);
			}
			while (sizeChangerTriggerEvent != sizeChangerTriggerEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent = this.OnExit;
			SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent2;
			do
			{
				sizeChangerTriggerEvent2 = sizeChangerTriggerEvent;
				SizeChangerTrigger.SizeChangerTriggerEvent sizeChangerTriggerEvent3 = (SizeChangerTrigger.SizeChangerTriggerEvent)Delegate.Remove(sizeChangerTriggerEvent2, value);
				sizeChangerTriggerEvent = Interlocked.CompareExchange<SizeChangerTrigger.SizeChangerTriggerEvent>(ref this.OnExit, sizeChangerTriggerEvent3, sizeChangerTriggerEvent2);
			}
			while (sizeChangerTriggerEvent != sizeChangerTriggerEvent2);
		}
	}

	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	public SizeChangerTrigger()
	{
	}

	private Collider myCollider;

	[CompilerGenerated]
	private SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	[CompilerGenerated]
	private SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	public delegate void SizeChangerTriggerEvent(Collider other);
}
