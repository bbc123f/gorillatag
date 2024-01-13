using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class InteractableRegistry : MonoBehaviour
{
	public static HashSet<Interactable> _interactables = new HashSet<Interactable>();

	public static HashSet<Interactable> Interactables => _interactables;

	public static void RegisterInteractable(Interactable interactable)
	{
		Interactables.Add(interactable);
	}

	public static void UnregisterInteractable(Interactable interactable)
	{
		Interactables.Remove(interactable);
	}
}
