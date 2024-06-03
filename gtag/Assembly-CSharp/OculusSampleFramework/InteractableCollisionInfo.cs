using System;

namespace OculusSampleFramework
{
	public class InteractableCollisionInfo
	{
		public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
		{
			this.InteractableCollider = collider;
			this.CollisionDepth = collisionDepth;
			this.CollidingTool = collidingTool;
		}

		public ColliderZone InteractableCollider;

		public InteractableCollisionDepth CollisionDepth;

		public InteractableTool CollidingTool;
	}
}
