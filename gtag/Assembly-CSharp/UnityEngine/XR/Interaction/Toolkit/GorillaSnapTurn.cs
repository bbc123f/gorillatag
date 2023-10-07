using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Valve.VR;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x020002AB RID: 683
	public class GorillaSnapTurn : LocomotionProvider
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x000658F2 File Offset: 0x00063AF2
		// (set) Token: 0x060011CF RID: 4559 RVA: 0x000658FA File Offset: 0x00063AFA
		public GorillaSnapTurn.InputAxes turnUsage
		{
			get
			{
				return this.m_TurnUsage;
			}
			set
			{
				this.m_TurnUsage = value;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060011D0 RID: 4560 RVA: 0x00065903 File Offset: 0x00063B03
		// (set) Token: 0x060011D1 RID: 4561 RVA: 0x0006590B File Offset: 0x00063B0B
		public List<XRController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060011D2 RID: 4562 RVA: 0x00065914 File Offset: 0x00063B14
		// (set) Token: 0x060011D3 RID: 4563 RVA: 0x0006591C File Offset: 0x00063B1C
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060011D4 RID: 4564 RVA: 0x00065925 File Offset: 0x00063B25
		// (set) Token: 0x060011D5 RID: 4565 RVA: 0x0006592D File Offset: 0x00063B2D
		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060011D6 RID: 4566 RVA: 0x00065936 File Offset: 0x00063B36
		// (set) Token: 0x060011D7 RID: 4567 RVA: 0x0006593E File Offset: 0x00063B3E
		public float deadZone
		{
			get
			{
				return this.m_DeadZone;
			}
			set
			{
				this.m_DeadZone = value;
			}
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x00065948 File Offset: 0x00063B48
		private void Update()
		{
			if (this.m_Controllers.Count > 0)
			{
				this.EnsureControllerDataListSize();
				InputFeatureUsage<Vector2>[] vec2UsageList = GorillaSnapTurn.m_Vec2UsageList;
				GorillaSnapTurn.InputAxes turnUsage = this.m_TurnUsage;
				for (int i = 0; i < this.m_Controllers.Count; i++)
				{
					XRController xrcontroller = this.m_Controllers[i];
					if (xrcontroller != null && xrcontroller.enableInputActions)
					{
						InputDevice inputDevice = xrcontroller.inputDevice;
						Vector2 vector = (xrcontroller.controllerNode == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
						if (vector.x > this.deadZone)
						{
							this.StartTurn(this.m_TurnAmount);
						}
						else if (vector.x < -this.deadZone)
						{
							this.StartTurn(-this.m_TurnAmount);
						}
						else
						{
							this.m_AxisReset = true;
						}
					}
				}
			}
			if (Math.Abs(this.m_CurrentTurnAmount) > 0f && base.BeginLocomotion())
			{
				if (base.system.xrRig != null)
				{
					Player.Instance.Turn(this.m_CurrentTurnAmount);
				}
				this.m_CurrentTurnAmount = 0f;
				base.EndLocomotion();
			}
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x00065A70 File Offset: 0x00063C70
		private void EnsureControllerDataListSize()
		{
			if (this.m_Controllers.Count != this.m_ControllersWereActive.Count)
			{
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.Add(false);
				}
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.RemoveAt(this.m_ControllersWereActive.Count - 1);
				}
			}
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x00065AED File Offset: 0x00063CED
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00065B08 File Offset: 0x00063D08
		private void StartTurn(float amount)
		{
			if (this.m_TimeStarted + this.m_DebounceTime > Time.time && !this.m_AxisReset)
			{
				return;
			}
			if (!base.CanBeginLocomotion())
			{
				return;
			}
			this.m_TimeStarted = Time.time;
			this.m_CurrentTurnAmount = amount;
			this.m_AxisReset = false;
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x00065B54 File Offset: 0x00063D54
		public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
		{
			if (turnMode == "SNAP")
			{
				this.m_DebounceTime = 0.5f;
				this.m_TurnAmount = 60f * this.ConvertedTurnFactor((float)turnSpeedFactor);
				return;
			}
			if (!(turnMode == "SMOOTH"))
			{
				this.m_DebounceTime = 0f;
				this.m_TurnAmount = 0f;
				return;
			}
			this.m_DebounceTime = 0f;
			this.m_TurnAmount = 360f * Time.fixedDeltaTime * this.ConvertedTurnFactor((float)turnSpeedFactor);
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x00065BD9 File Offset: 0x00063DD9
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x040014B5 RID: 5301
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x040014B6 RID: 5302
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x040014B7 RID: 5303
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x040014B8 RID: 5304
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x040014B9 RID: 5305
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x040014BA RID: 5306
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x040014BB RID: 5307
		private float m_CurrentTurnAmount;

		// Token: 0x040014BC RID: 5308
		private float m_TimeStarted;

		// Token: 0x040014BD RID: 5309
		private bool m_AxisReset;

		// Token: 0x040014BE RID: 5310
		public float turnSpeed = 1f;

		// Token: 0x040014BF RID: 5311
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x020004B4 RID: 1204
		public enum InputAxes
		{
			// Token: 0x04001F7C RID: 8060
			Primary2DAxis,
			// Token: 0x04001F7D RID: 8061
			Secondary2DAxis
		}
	}
}
