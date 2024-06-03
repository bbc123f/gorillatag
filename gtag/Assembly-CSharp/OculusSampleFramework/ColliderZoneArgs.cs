using System;

namespace OculusSampleFramework
{
	public class ColliderZoneArgs : EventArgs
	{
		public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
		{
			this.Collider = collider;
			this.FrameTime = frameTime;
			this.CollidingTool = collidingTool;
			this.InteractionT = interactionType;
		}

		public readonly ColliderZone Collider;

		public readonly float FrameTime;

		public readonly InteractableTool CollidingTool;

		public readonly InteractionType InteractionT;
	}
}
