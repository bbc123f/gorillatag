namespace OculusSampleFramework;

public class InteractableCollisionInfo
{
	public ColliderZone InteractableCollider;

	public InteractableCollisionDepth CollisionDepth;

	public InteractableTool CollidingTool;

	public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
	{
		InteractableCollider = collider;
		CollisionDepth = collisionDepth;
		CollidingTool = collidingTool;
	}
}
