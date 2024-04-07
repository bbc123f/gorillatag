using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class FPSController : MonoBehaviour
{
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent
	{
		[CompilerGenerated]
		add
		{
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler = this.OnStartEvent;
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler2;
			do
			{
				onStateChangeEventHandler2 = onStateChangeEventHandler;
				FPSController.OnStateChangeEventHandler onStateChangeEventHandler3 = (FPSController.OnStateChangeEventHandler)Delegate.Combine(onStateChangeEventHandler2, value);
				onStateChangeEventHandler = Interlocked.CompareExchange<FPSController.OnStateChangeEventHandler>(ref this.OnStartEvent, onStateChangeEventHandler3, onStateChangeEventHandler2);
			}
			while (onStateChangeEventHandler != onStateChangeEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler = this.OnStartEvent;
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler2;
			do
			{
				onStateChangeEventHandler2 = onStateChangeEventHandler;
				FPSController.OnStateChangeEventHandler onStateChangeEventHandler3 = (FPSController.OnStateChangeEventHandler)Delegate.Remove(onStateChangeEventHandler2, value);
				onStateChangeEventHandler = Interlocked.CompareExchange<FPSController.OnStateChangeEventHandler>(ref this.OnStartEvent, onStateChangeEventHandler3, onStateChangeEventHandler2);
			}
			while (onStateChangeEventHandler != onStateChangeEventHandler2);
		}
	}

	public event FPSController.OnStateChangeEventHandler OnStopEvent
	{
		[CompilerGenerated]
		add
		{
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler = this.OnStopEvent;
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler2;
			do
			{
				onStateChangeEventHandler2 = onStateChangeEventHandler;
				FPSController.OnStateChangeEventHandler onStateChangeEventHandler3 = (FPSController.OnStateChangeEventHandler)Delegate.Combine(onStateChangeEventHandler2, value);
				onStateChangeEventHandler = Interlocked.CompareExchange<FPSController.OnStateChangeEventHandler>(ref this.OnStopEvent, onStateChangeEventHandler3, onStateChangeEventHandler2);
			}
			while (onStateChangeEventHandler != onStateChangeEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler = this.OnStopEvent;
			FPSController.OnStateChangeEventHandler onStateChangeEventHandler2;
			do
			{
				onStateChangeEventHandler2 = onStateChangeEventHandler;
				FPSController.OnStateChangeEventHandler onStateChangeEventHandler3 = (FPSController.OnStateChangeEventHandler)Delegate.Remove(onStateChangeEventHandler2, value);
				onStateChangeEventHandler = Interlocked.CompareExchange<FPSController.OnStateChangeEventHandler>(ref this.OnStopEvent, onStateChangeEventHandler3, onStateChangeEventHandler2);
			}
			while (onStateChangeEventHandler != onStateChangeEventHandler2);
		}
	}

	public FPSController()
	{
	}

	public float baseMoveSpeed = 4f;

	public float shiftMoveSpeed = 8f;

	public float ctrlMoveSpeed = 1f;

	public float lookHorizontal = 0.4f;

	public float lookVertical = 0.25f;

	[SerializeField]
	private Vector3 leftControllerPosOffset = new Vector3(-0.2f, -0.25f, 0.3f);

	[SerializeField]
	private Vector3 leftControllerRotationOffset = new Vector3(265f, -82f, 28f);

	[SerializeField]
	private Vector3 rightControllerPosOffset = new Vector3(0.2f, -0.25f, 0.3f);

	[SerializeField]
	private Vector3 rightControllerRotationOffset = new Vector3(263f, 318f, 485f);

	[CompilerGenerated]
	private FPSController.OnStateChangeEventHandler OnStartEvent;

	[CompilerGenerated]
	private FPSController.OnStateChangeEventHandler OnStopEvent;

	public delegate void OnStateChangeEventHandler();
}
