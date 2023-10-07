using System;

namespace OculusSampleFramework
{
	// Token: 0x020002DD RID: 733
	public class InteractableCollisionInfo
	{
		// Token: 0x060013BE RID: 5054 RVA: 0x00070B89 File Offset: 0x0006ED89
		public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
		{
			this.InteractableCollider = collider;
			this.CollisionDepth = collisionDepth;
			this.CollidingTool = collidingTool;
		}

		// Token: 0x04001685 RID: 5765
		public ColliderZone InteractableCollider;

		// Token: 0x04001686 RID: 5766
		public InteractableCollisionDepth CollisionDepth;

		// Token: 0x04001687 RID: 5767
		public InteractableTool CollidingTool;
	}
}
