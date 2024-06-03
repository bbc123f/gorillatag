using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		public InteractableTool InteractableTool
		{
			[CompilerGenerated]
			get
			{
				return this.<InteractableTool>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<InteractableTool>k__BackingField = value;
			}
		}

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

		public bool ToolActivateState
		{
			[CompilerGenerated]
			get
			{
				return this.<ToolActivateState>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<ToolActivateState>k__BackingField = value;
			}
		}

		public float SphereRadius
		{
			[CompilerGenerated]
			get
			{
				return this.<SphereRadius>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<SphereRadius>k__BackingField = value;
			}
		}

		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		public FingerTipPokeToolView()
		{
		}

		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;

		[CompilerGenerated]
		private InteractableTool <InteractableTool>k__BackingField;

		[CompilerGenerated]
		private bool <ToolActivateState>k__BackingField;

		[CompilerGenerated]
		private float <SphereRadius>k__BackingField;
	}
}
