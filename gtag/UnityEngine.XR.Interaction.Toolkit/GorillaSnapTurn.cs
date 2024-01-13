using System;
using System.Collections.Generic;
using GorillaLocomotion;

namespace UnityEngine.XR.Interaction.Toolkit;

public class GorillaSnapTurn : LocomotionProvider
{
	public enum InputAxes
	{
		Primary2DAxis,
		Secondary2DAxis
	}

	private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[2]
	{
		CommonUsages.primary2DAxis,
		CommonUsages.secondary2DAxis
	};

	[SerializeField]
	[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
	private InputAxes m_TurnUsage;

	[SerializeField]
	[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
	private List<XRController> m_Controllers = new List<XRController>();

	[SerializeField]
	[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
	private float m_TurnAmount = 45f;

	[SerializeField]
	[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
	private float m_DebounceTime = 0.5f;

	[SerializeField]
	[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
	private float m_DeadZone = 0.75f;

	private float m_CurrentTurnAmount;

	private float m_TimeStarted;

	private bool m_AxisReset;

	public float turnSpeed = 1f;

	private List<bool> m_ControllersWereActive = new List<bool>();

	public InputAxes turnUsage
	{
		get
		{
			return m_TurnUsage;
		}
		set
		{
			m_TurnUsage = value;
		}
	}

	public List<XRController> controllers
	{
		get
		{
			return m_Controllers;
		}
		set
		{
			m_Controllers = value;
		}
	}

	public float turnAmount
	{
		get
		{
			return m_TurnAmount;
		}
		set
		{
			m_TurnAmount = value;
		}
	}

	public float debounceTime
	{
		get
		{
			return m_DebounceTime;
		}
		set
		{
			m_DebounceTime = value;
		}
	}

	public float deadZone
	{
		get
		{
			return m_DeadZone;
		}
		set
		{
			m_DeadZone = value;
		}
	}

	private void Update()
	{
		if (m_Controllers.Count > 0)
		{
			EnsureControllerDataListSize();
			InputFeatureUsage<Vector2> usage = m_Vec2UsageList[(int)m_TurnUsage];
			for (int i = 0; i < m_Controllers.Count; i++)
			{
				XRController xRController = m_Controllers[i];
				if (xRController != null && xRController.enableInputActions && xRController.inputDevice.TryGetFeatureValue(usage, out var value))
				{
					if (value.x > deadZone)
					{
						StartTurn(m_TurnAmount);
					}
					else if (value.x < 0f - deadZone)
					{
						StartTurn(0f - m_TurnAmount);
					}
					else
					{
						m_AxisReset = true;
					}
				}
			}
		}
		if (Math.Abs(m_CurrentTurnAmount) > 0f && BeginLocomotion())
		{
			if (base.system.xrRig != null)
			{
				Player.Instance.Turn(m_CurrentTurnAmount);
			}
			m_CurrentTurnAmount = 0f;
			EndLocomotion();
		}
	}

	private void EnsureControllerDataListSize()
	{
		if (m_Controllers.Count != m_ControllersWereActive.Count)
		{
			while (m_ControllersWereActive.Count < m_Controllers.Count)
			{
				m_ControllersWereActive.Add(item: false);
			}
			while (m_ControllersWereActive.Count < m_Controllers.Count)
			{
				m_ControllersWereActive.RemoveAt(m_ControllersWereActive.Count - 1);
			}
		}
	}

	internal void FakeStartTurn(bool isLeft)
	{
		StartTurn(isLeft ? (0f - m_TurnAmount) : m_TurnAmount);
	}

	private void StartTurn(float amount)
	{
		if ((!(m_TimeStarted + m_DebounceTime > Time.time) || m_AxisReset) && CanBeginLocomotion())
		{
			m_TimeStarted = Time.time;
			m_CurrentTurnAmount = amount;
			m_AxisReset = false;
		}
	}

	public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
	{
		if (!(turnMode == "SNAP"))
		{
			if (turnMode == "SMOOTH")
			{
				m_DebounceTime = 0f;
				m_TurnAmount = 360f * Time.fixedDeltaTime * ConvertedTurnFactor(turnSpeedFactor);
			}
			else
			{
				m_DebounceTime = 0f;
				m_TurnAmount = 0f;
			}
		}
		else
		{
			m_DebounceTime = 0.5f;
			m_TurnAmount = 60f * ConvertedTurnFactor(turnSpeedFactor);
		}
	}

	public float ConvertedTurnFactor(float newTurnSpeed)
	{
		return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
	}
}
