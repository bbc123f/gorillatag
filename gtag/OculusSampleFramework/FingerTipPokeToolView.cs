using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		public InteractableTool InteractableTool { get; set; }

		public bool EnableState
		{
			get
			{
				return this._sphereMeshRenderer.enabled;
			}
			set
			{
				this._sphereMeshRenderer.enabled = value;
			}
		}

		public bool ToolActivateState { get; set; }

		public float SphereRadius { get; private set; }

		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;
	}
}
