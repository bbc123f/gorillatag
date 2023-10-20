using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x020002F3 RID: 755
	public class WindmillController : MonoBehaviour
	{
		// Token: 0x0600147B RID: 5243 RVA: 0x0007390F File Offset: 0x00071B0F
		private void Awake()
		{
			this._bladesRotation = base.GetComponentInChildren<WindmillBladesController>();
			this._bladesRotation.SetMoveState(true, this._maxSpeed);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0007392F File Offset: 0x00071B2F
		private void OnEnable()
		{
			this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x00073952 File Offset: 0x00071B52
		private void OnDisable()
		{
			if (this._startStopButton != null)
			{
				this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
			}
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x00073984 File Offset: 0x00071B84
		private void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (this._bladesRotation.IsMoving)
				{
					this._bladesRotation.SetMoveState(false, 0f);
				}
				else
				{
					this._bladesRotation.SetMoveState(true, this._maxSpeed);
				}
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x000739E8 File Offset: 0x00071BE8
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x04001738 RID: 5944
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04001739 RID: 5945
		[SerializeField]
		private float _maxSpeed = 10f;

		// Token: 0x0400173A RID: 5946
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x0400173B RID: 5947
		private WindmillBladesController _bladesRotation;

		// Token: 0x0400173C RID: 5948
		private InteractableTool _toolInteractingWithMe;
	}
}
