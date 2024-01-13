using UnityEngine;

namespace OculusSampleFramework;

public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
{
	[SerializeField]
	private MeshRenderer _sphereMeshRenderer;

	public InteractableTool InteractableTool { get; set; }

	public bool EnableState
	{
		get
		{
			return _sphereMeshRenderer.enabled;
		}
		set
		{
			_sphereMeshRenderer.enabled = value;
		}
	}

	public bool ToolActivateState { get; set; }

	public float SphereRadius { get; private set; }

	private void Awake()
	{
		SphereRadius = _sphereMeshRenderer.transform.localScale.z * 0.5f;
	}

	public void SetFocusedInteractable(Interactable interactable)
	{
	}
}
