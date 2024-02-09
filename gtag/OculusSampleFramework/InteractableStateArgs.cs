using System;

namespace OculusSampleFramework
{
	public class InteractableStateArgs : EventArgs
	{
		public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
		{
			this.Interactable = interactable;
			this.Tool = tool;
			this.NewInteractableState = newInteractableState;
			this.OldInteractableState = oldState;
			this.ColliderArgs = colliderArgs;
		}

		public readonly Interactable Interactable;

		public readonly InteractableTool Tool;

		public readonly InteractableState OldInteractableState;

		public readonly InteractableState NewInteractableState;

		public readonly ColliderZoneArgs ColliderArgs;
	}
}
