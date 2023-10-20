using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x06000BF9 RID: 3065 RVA: 0x00049DD5 File Offset: 0x00047FD5
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00049DE2 File Offset: 0x00047FE2
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00049DE4 File Offset: 0x00047FE4
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.target.GetCollisionEvents(other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x04000F71 RID: 3953
	public ParticleSystem target;

	// Token: 0x04000F72 RID: 3954
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
