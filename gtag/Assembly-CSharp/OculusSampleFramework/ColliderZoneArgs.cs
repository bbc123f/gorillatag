using System;

namespace OculusSampleFramework
{
	// Token: 0x020002D1 RID: 721
	public class ColliderZoneArgs : EventArgs
	{
		// Token: 0x0600136A RID: 4970 RVA: 0x000700FB File Offset: 0x0006E2FB
		public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
		{
			this.Collider = collider;
			this.FrameTime = frameTime;
			this.CollidingTool = collidingTool;
			this.InteractionT = interactionType;
		}

		// Token: 0x04001641 RID: 5697
		public readonly ColliderZone Collider;

		// Token: 0x04001642 RID: 5698
		public readonly float FrameTime;

		// Token: 0x04001643 RID: 5699
		public readonly InteractableTool CollidingTool;

		// Token: 0x04001644 RID: 5700
		public readonly InteractionType InteractionT;
	}
}
