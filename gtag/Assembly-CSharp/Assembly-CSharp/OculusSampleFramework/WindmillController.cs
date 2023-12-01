using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	public class WindmillController : MonoBehaviour
	{
		private void Awake()
		{
			this._bladesRotation = base.GetComponentInChildren<WindmillBladesController>();
			this._bladesRotation.SetMoveState(true, this._maxSpeed);
		}

		private void OnEnable()
		{
			this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
		}

		private void OnDisable()
		{
			if (this._startStopButton != null)
			{
				this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
			}
		}

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

		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		[SerializeField]
		private GameObject _startStopButton;

		[SerializeField]
		private float _maxSpeed = 10f;

		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		private WindmillBladesController _bladesRotation;

		private InteractableTool _toolInteractingWithMe;
	}
}
