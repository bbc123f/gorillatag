using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x020002F1 RID: 753
	public class WindmillController : MonoBehaviour
	{
		// Token: 0x06001474 RID: 5236 RVA: 0x00073443 File Offset: 0x00071643
		private void Awake()
		{
			this._bladesRotation = base.GetComponentInChildren<WindmillBladesController>();
			this._bladesRotation.SetMoveState(true, this._maxSpeed);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x00073463 File Offset: 0x00071663
		private void OnEnable()
		{
			this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x00073486 File Offset: 0x00071686
		private void OnDisable()
		{
			if (this._startStopButton != null)
			{
				this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
			}
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000734B8 File Offset: 0x000716B8
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

		// Token: 0x06001478 RID: 5240 RVA: 0x0007351C File Offset: 0x0007171C
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x0400172B RID: 5931
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x0400172C RID: 5932
		[SerializeField]
		private float _maxSpeed = 10f;

		// Token: 0x0400172D RID: 5933
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x0400172E RID: 5934
		private WindmillBladesController _bladesRotation;

		// Token: 0x0400172F RID: 5935
		private InteractableTool _toolInteractingWithMe;
	}
}
