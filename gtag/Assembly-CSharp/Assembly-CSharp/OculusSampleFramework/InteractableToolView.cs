using System;

namespace OculusSampleFramework
{
	public interface InteractableToolView
	{
		InteractableTool InteractableTool { get; }

		void SetFocusedInteractable(Interactable interactable);

		bool EnableState { get; set; }

		bool ToolActivateState { get; set; }
	}
}
