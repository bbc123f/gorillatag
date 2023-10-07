using System;

namespace OculusSampleFramework
{
	// Token: 0x020002D5 RID: 725
	public class InteractableStateArgs : EventArgs
	{
		// Token: 0x06001393 RID: 5011 RVA: 0x00070313 File Offset: 0x0006E513
		public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
		{
			this.Interactable = interactable;
			this.Tool = tool;
			this.NewInteractableState = newInteractableState;
			this.OldInteractableState = oldState;
			this.ColliderArgs = colliderArgs;
		}

		// Token: 0x0400165D RID: 5725
		public readonly Interactable Interactable;

		// Token: 0x0400165E RID: 5726
		public readonly InteractableTool Tool;

		// Token: 0x0400165F RID: 5727
		public readonly InteractableState OldInteractableState;

		// Token: 0x04001660 RID: 5728
		public readonly InteractableState NewInteractableState;

		// Token: 0x04001661 RID: 5729
		public readonly ColliderZoneArgs ColliderArgs;
	}
}
