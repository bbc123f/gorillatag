using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Valve.VR;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x020002AD RID: 685
	public class GorillaSnapTurn : LocomotionProvider
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060011D5 RID: 4565 RVA: 0x00065DBE File Offset: 0x00063FBE
		// (set) Token: 0x060011D6 RID: 4566 RVA: 0x00065DC6 File Offset: 0x00063FC6
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

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060011D7 RID: 4567 RVA: 0x00065DCF File Offset: 0x00063FCF
		// (set) Token: 0x060011D8 RID: 4568 RVA: 0x00065DD7 File Offset: 0x00063FD7
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

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060011D9 RID: 4569 RVA: 0x00065DE0 File Offset: 0x00063FE0
		// (set) Token: 0x060011DA RID: 4570 RVA: 0x00065DE8 File Offset: 0x00063FE8
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

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060011DB RID: 4571 RVA: 0x00065DF1 File Offset: 0x00063FF1
		// (set) Token: 0x060011DC RID: 4572 RVA: 0x00065DF9 File Offset: 0x00063FF9
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

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060011DD RID: 4573 RVA: 0x00065E02 File Offset: 0x00064002
		// (set) Token: 0x060011DE RID: 4574 RVA: 0x00065E0A File Offset: 0x0006400A
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

		// Token: 0x060011DF RID: 4575 RVA: 0x00065E14 File Offset: 0x00064014
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

		// Token: 0x060011E0 RID: 4576 RVA: 0x00065F3C File Offset: 0x0006413C
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

		// Token: 0x060011E1 RID: 4577 RVA: 0x00065FB9 File Offset: 0x000641B9
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x00065FD4 File Offset: 0x000641D4
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

		// Token: 0x060011E3 RID: 4579 RVA: 0x00066020 File Offset: 0x00064220
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

		// Token: 0x060011E4 RID: 4580 RVA: 0x000660A5 File Offset: 0x000642A5
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x040014C2 RID: 5314
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x040014C3 RID: 5315
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x040014C4 RID: 5316
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x040014C5 RID: 5317
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x040014C6 RID: 5318
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x040014C7 RID: 5319
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x040014C8 RID: 5320
		private float m_CurrentTurnAmount;

		// Token: 0x040014C9 RID: 5321
		private float m_TimeStarted;

		// Token: 0x040014CA RID: 5322
		private bool m_AxisReset;

		// Token: 0x040014CB RID: 5323
		public float turnSpeed = 1f;

		// Token: 0x040014CC RID: 5324
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x020004B6 RID: 1206
		public enum InputAxes
		{
			// Token: 0x04001F89 RID: 8073
			Primary2DAxis,
			// Token: 0x04001F8A RID: 8074
			Secondary2DAxis
		}
	}
}
