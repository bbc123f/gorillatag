using System;

namespace OculusSampleFramework;

public class InteractableStateArgs : EventArgs
{
	public readonly Interactable Interactable;

	public readonly InteractableTool Tool;

	public readonly InteractableState OldInteractableState;

	public readonly InteractableState NewInteractableState;

	public readonly ColliderZoneArgs ColliderArgs;

	public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
	{
		Interactable = interactable;
		Tool = tool;
		NewInteractableState = newInteractableState;
		OldInteractableState = oldState;
		ColliderArgs = colliderArgs;
	}
}
