using System;

namespace OculusSampleFramework
{
	// Token: 0x020002D7 RID: 727
	public class InteractableStateArgs : EventArgs
	{
		// Token: 0x0600139A RID: 5018 RVA: 0x000707DF File Offset: 0x0006E9DF
		public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
		{
			this.Interactable = interactable;
			this.Tool = tool;
			this.NewInteractableState = newInteractableState;
			this.OldInteractableState = oldState;
			this.ColliderArgs = colliderArgs;
		}

		// Token: 0x0400166A RID: 5738
		public readonly Interactable Interactable;

		// Token: 0x0400166B RID: 5739
		public readonly InteractableTool Tool;

		// Token: 0x0400166C RID: 5740
		public readonly InteractableState OldInteractableState;

		// Token: 0x0400166D RID: 5741
		public readonly InteractableState NewInteractableState;

		// Token: 0x0400166E RID: 5742
		public readonly ColliderZoneArgs ColliderArgs;
	}
}
