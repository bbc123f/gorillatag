using System;

namespace OculusSampleFramework;

public class ColliderZoneArgs : EventArgs
{
	public readonly ColliderZone Collider;

	public readonly float FrameTime;

	public readonly InteractableTool CollidingTool;

	public readonly InteractionType InteractionT;

	public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
	{
		Collider = collider;
		FrameTime = frameTime;
		CollidingTool = collidingTool;
		InteractionT = interactionType;
	}
}
