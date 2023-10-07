using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002D6 RID: 726
	public class InteractableRegistry : MonoBehaviour
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06001394 RID: 5012 RVA: 0x00070340 File Offset: 0x0006E540
		public static HashSet<Interactable> Interactables
		{
			get
			{
				return InteractableRegistry._interactables;
			}
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x00070347 File Offset: 0x0006E547
		public static void RegisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Add(interactable);
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x00070355 File Offset: 0x0006E555
		public static void UnregisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Remove(interactable);
		}

		// Token: 0x04001662 RID: 5730
		public static HashSet<Interactable> _interactables = new HashSet<Interactable>();
	}
}
