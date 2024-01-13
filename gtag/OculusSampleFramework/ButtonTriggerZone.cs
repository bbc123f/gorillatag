using UnityEngine;

namespace OculusSampleFramework;

public class ButtonTriggerZone : MonoBehaviour, ColliderZone
{
	[SerializeField]
	private GameObject _parentInteractableObj;

	public Collider Collider { get; private set; }

	public Interactable ParentInteractable { get; private set; }

	public InteractableCollisionDepth CollisionDepth
	{
		get
		{
			if (ParentInteractable.ProximityCollider != this)
			{
				if (ParentInteractable.ContactCollider != this)
				{
					if (ParentInteractable.ActionCollider != this)
					{
						return InteractableCollisionDepth.None;
					}
					return InteractableCollisionDepth.Action;
				}
				return InteractableCollisionDepth.Contact;
			}
			return InteractableCollisionDepth.Proximity;
		}
	}

	private void Awake()
	{
		Collider = GetComponent<Collider>();
		ParentInteractable = _parentInteractableObj.GetComponent<Interactable>();
	}
}
