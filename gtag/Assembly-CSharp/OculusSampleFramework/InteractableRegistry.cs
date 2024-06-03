using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	public class InteractableRegistry : MonoBehaviour
	{
		public static HashSet<Interactable> Interactables
		{
			get
			{
				return InteractableRegistry._interactables;
			}
		}

		public static void RegisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Add(interactable);
		}

		public static void UnregisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Remove(interactable);
		}

		public InteractableRegistry()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static InteractableRegistry()
		{
		}

		public static HashSet<Interactable> _interactables = new HashSet<Interactable>();
	}
}
