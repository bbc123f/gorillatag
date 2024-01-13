using UnityEngine;

namespace OculusSampleFramework;

public interface ColliderZone
{
	Collider Collider { get; }

	Interactable ParentInteractable { get; }

	InteractableCollisionDepth CollisionDepth { get; }
}
