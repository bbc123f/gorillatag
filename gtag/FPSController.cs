using UnityEngine;

public class FPSController : MonoBehaviour
{
	public delegate void OnStateChangeEventHandler();

	public float baseMoveSpeed = 4f;

	public float shiftMoveSpeed = 8f;

	public float lookHorizontal = 0.4f;

	public float lookVertical = 0.25f;

	[HideInInspector]
	public event OnStateChangeEventHandler OnStartEvent;

	public event OnStateChangeEventHandler OnStopEvent;
}
