using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002D0 RID: 720
	public interface ColliderZone
	{
		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06001367 RID: 4967
		Collider Collider { get; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06001368 RID: 4968
		Interactable ParentInteractable { get; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06001369 RID: 4969
		InteractableCollisionDepth CollisionDepth { get; }
	}
}
