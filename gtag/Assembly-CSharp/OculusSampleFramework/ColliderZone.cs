using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CE RID: 718
	public interface ColliderZone
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06001360 RID: 4960
		Collider Collider { get; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06001361 RID: 4961
		Interactable ParentInteractable { get; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06001362 RID: 4962
		InteractableCollisionDepth CollisionDepth { get; }
	}
}
