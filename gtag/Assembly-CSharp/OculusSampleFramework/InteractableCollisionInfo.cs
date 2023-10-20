using System;

namespace OculusSampleFramework
{
	// Token: 0x020002DF RID: 735
	public class InteractableCollisionInfo
	{
		// Token: 0x060013C5 RID: 5061 RVA: 0x00071055 File Offset: 0x0006F255
		public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
		{
			this.InteractableCollider = collider;
			this.CollisionDepth = collisionDepth;
			this.CollidingTool = collidingTool;
		}

		// Token: 0x04001692 RID: 5778
		public ColliderZone InteractableCollider;

		// Token: 0x04001693 RID: 5779
		public InteractableCollisionDepth CollisionDepth;

		// Token: 0x04001694 RID: 5780
		public InteractableTool CollidingTool;
	}
}
