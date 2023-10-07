using System;

namespace OculusSampleFramework
{
	// Token: 0x020002CF RID: 719
	public class ColliderZoneArgs : EventArgs
	{
		// Token: 0x06001363 RID: 4963 RVA: 0x0006FC2F File Offset: 0x0006DE2F
		public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
		{
			this.Collider = collider;
			this.FrameTime = frameTime;
			this.CollidingTool = collidingTool;
			this.InteractionT = interactionType;
		}

		// Token: 0x04001634 RID: 5684
		public readonly ColliderZone Collider;

		// Token: 0x04001635 RID: 5685
		public readonly float FrameTime;

		// Token: 0x04001636 RID: 5686
		public readonly InteractableTool CollidingTool;

		// Token: 0x04001637 RID: 5687
		public readonly InteractionType InteractionT;
	}
}
