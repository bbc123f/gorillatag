using UnityEngine;

namespace OculusSampleFramework;

public class PinchStateModule
{
	private enum PinchState
	{
		None,
		PinchDown,
		PinchStay,
		PinchUp
	}

	private const float PINCH_STRENGTH_THRESHOLD = 1f;

	private PinchState _currPinchState;

	private Interactable _firstFocusedInteractable;

	public bool PinchUpAndDownOnFocusedObject
	{
		get
		{
			if (_currPinchState == PinchState.PinchUp)
			{
				return _firstFocusedInteractable != null;
			}
			return false;
		}
	}

	public bool PinchSteadyOnFocusedObject
	{
		get
		{
			if (_currPinchState == PinchState.PinchStay)
			{
				return _firstFocusedInteractable != null;
			}
			return false;
		}
	}

	public bool PinchDownOnFocusedObject
	{
		get
		{
			if (_currPinchState == PinchState.PinchDown)
			{
				return _firstFocusedInteractable != null;
			}
			return false;
		}
	}

	public PinchStateModule()
	{
		_currPinchState = PinchState.None;
		_firstFocusedInteractable = null;
	}

	public void UpdateState(OVRHand hand, Interactable currFocusedInteractable)
	{
		float fingerPinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
		bool flag = Mathf.Abs(1f - fingerPinchStrength) < Mathf.Epsilon;
		switch (_currPinchState)
		{
		case PinchState.PinchUp:
			if (flag)
			{
				_currPinchState = PinchState.PinchDown;
				if (currFocusedInteractable != _firstFocusedInteractable)
				{
					_firstFocusedInteractable = null;
				}
			}
			else
			{
				_currPinchState = PinchState.None;
				_firstFocusedInteractable = null;
			}
			break;
		case PinchState.PinchStay:
			if (!flag)
			{
				_currPinchState = PinchState.PinchUp;
			}
			if (currFocusedInteractable != _firstFocusedInteractable)
			{
				_firstFocusedInteractable = null;
			}
			break;
		case PinchState.PinchDown:
			_currPinchState = (flag ? PinchState.PinchStay : PinchState.PinchUp);
			if (_firstFocusedInteractable != currFocusedInteractable)
			{
				_firstFocusedInteractable = null;
			}
			break;
		default:
			if (flag)
			{
				_currPinchState = PinchState.PinchDown;
				_firstFocusedInteractable = currFocusedInteractable;
			}
			break;
		}
	}
}
