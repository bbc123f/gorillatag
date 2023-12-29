using System;
using UnityEngine;

public class FPSController : MonoBehaviour
{
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	public float baseMoveSpeed = 4f;

	public float shiftMoveSpeed = 8f;

	public float ctrlMoveSpeed = 1f;

	public float lookHorizontal = 0.4f;

	public float lookVertical = 0.25f;

	public delegate void OnStateChangeEventHandler();
}
