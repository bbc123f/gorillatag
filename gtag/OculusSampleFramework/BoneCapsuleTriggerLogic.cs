using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class BoneCapsuleTriggerLogic : MonoBehaviour
{
	public InteractableToolTags ToolTags;

	public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

	private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();

	private void OnDisable()
	{
		CollidersTouchingUs.Clear();
	}

	private void Update()
	{
		CleanUpDeadColliders();
	}

	private void OnTriggerEnter(Collider other)
	{
		ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
		if (component != null && ((uint)component.ParentInteractable.ValidToolTagsMask & (uint)ToolTags) != 0)
		{
			CollidersTouchingUs.Add(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
		if (component != null && ((uint)component.ParentInteractable.ValidToolTagsMask & (uint)ToolTags) != 0)
		{
			CollidersTouchingUs.Remove(component);
		}
	}

	private void CleanUpDeadColliders()
	{
		_elementsToCleanUp.Clear();
		foreach (ColliderZone collidersTouchingU in CollidersTouchingUs)
		{
			if (!collidersTouchingU.Collider.gameObject.activeInHierarchy)
			{
				_elementsToCleanUp.Add(collidersTouchingU);
			}
		}
		foreach (ColliderZone item in _elementsToCleanUp)
		{
			CollidersTouchingUs.Remove(item);
		}
	}
}
