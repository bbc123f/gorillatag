using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E0 RID: 736
	public class PinchStateModule
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060013DB RID: 5083 RVA: 0x00070F2D File Offset: 0x0006F12D
		public bool PinchUpAndDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchUp && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060013DC RID: 5084 RVA: 0x00070F46 File Offset: 0x0006F146
		public bool PinchSteadyOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchStay && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060013DD RID: 5085 RVA: 0x00070F5F File Offset: 0x0006F15F
		public bool PinchDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchDown && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x00070F78 File Offset: 0x0006F178
		public PinchStateModule()
		{
			this._currPinchState = PinchStateModule.PinchState.None;
			this._firstFocusedInteractable = null;
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x00070F90 File Offset: 0x0006F190
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

		// Token: 0x04001691 RID: 5777
		private const float PINCH_STRENGTH_THRESHOLD = 1f;

		// Token: 0x04001692 RID: 5778
		private PinchStateModule.PinchState _currPinchState;

		// Token: 0x04001693 RID: 5779
		private Interactable _firstFocusedInteractable;

		// Token: 0x020004EB RID: 1259
		private enum PinchState
		{
			// Token: 0x04002076 RID: 8310
			None,
			// Token: 0x04002077 RID: 8311
			PinchDown,
			// Token: 0x04002078 RID: 8312
			PinchStay,
			// Token: 0x04002079 RID: 8313
			PinchUp
		}
	}
}
