using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E2 RID: 738
	public class PinchStateModule
	{
		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060013E2 RID: 5090 RVA: 0x000713F9 File Offset: 0x0006F5F9
		public bool PinchUpAndDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchUp && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060013E3 RID: 5091 RVA: 0x00071412 File Offset: 0x0006F612
		public bool PinchSteadyOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchStay && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060013E4 RID: 5092 RVA: 0x0007142B File Offset: 0x0006F62B
		public bool PinchDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchDown && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x00071444 File Offset: 0x0006F644
		public PinchStateModule()
		{
			this._currPinchState = PinchStateModule.PinchState.None;
			this._firstFocusedInteractable = null;
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0007145C File Offset: 0x0006F65C
		public void UpdateState(OVRHand hand, Interactable currFocusedInteractable)
		{
			float fingerPinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
			bool flag = Mathf.Abs(1f - fingerPinchStrength) < Mathf.Epsilon;
			switch (this._currPinchState)
			{
			case PinchStateModule.PinchState.PinchDown:
				this._currPinchState = (flag ? PinchStateModule.PinchState.PinchStay : PinchStateModule.PinchState.PinchUp);
				if (this._firstFocusedInteractable != currFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			case PinchStateModule.PinchState.PinchStay:
				if (!flag)
				{
					this._currPinchState = PinchStateModule.PinchState.PinchUp;
				}
				if (currFocusedInteractable != this._firstFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			case PinchStateModule.PinchState.PinchUp:
				if (!flag)
				{
					this._currPinchState = PinchStateModule.PinchState.None;
					this._firstFocusedInteractable = null;
					return;
				}
				this._currPinchState = PinchStateModule.PinchState.PinchDown;
				if (currFocusedInteractable != this._firstFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			default:
				if (flag)
				{
					this._currPinchState = PinchStateModule.PinchState.PinchDown;
					this._firstFocusedInteractable = currFocusedInteractable;
				}
				break;
			}
		}

		// Token: 0x0400169E RID: 5790
		private const float PINCH_STRENGTH_THRESHOLD = 1f;

		// Token: 0x0400169F RID: 5791
		private PinchStateModule.PinchState _currPinchState;

		// Token: 0x040016A0 RID: 5792
		private Interactable _firstFocusedInteractable;

		// Token: 0x020004ED RID: 1261
		private enum PinchState
		{
			// Token: 0x04002083 RID: 8323
			None,
			// Token: 0x04002084 RID: 8324
			PinchDown,
			// Token: 0x04002085 RID: 8325
			PinchStay,
			// Token: 0x04002086 RID: 8326
			PinchUp
		}
	}
}
