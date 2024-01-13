namespace OculusSampleFramework;

public interface InteractableToolView
{
	InteractableTool InteractableTool { get; }

	bool EnableState { get; set; }

	bool ToolActivateState { get; set; }

	void SetFocusedInteractable(Interactable interactable);
}
