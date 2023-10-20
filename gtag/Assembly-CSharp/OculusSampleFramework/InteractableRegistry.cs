using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002D8 RID: 728
	public class InteractableRegistry : MonoBehaviour
	{
		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x0007080C File Offset: 0x0006EA0C
		public static HashSet<Interactable> Interactables
		{
			get
			{
				return InteractableRegistry._interactables;
			}
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x00070813 File Offset: 0x0006EA13
		public static void RegisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Add(interactable);
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x00070821 File Offset: 0x0006EA21
		public static void UnregisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Remove(interactable);
		}

		// Token: 0x0400166F RID: 5743
		public static HashSet<Interactable> _interactables = new HashSet<Interactable>();
	}
}
