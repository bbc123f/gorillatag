using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x06000BF3 RID: 3059 RVA: 0x00049B6D File Offset: 0x00047D6D
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00049B7A File Offset: 0x00047D7A
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00049B7C File Offset: 0x00047D7C
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.target.GetCollisionEvents(other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x04000F6D RID: 3949
	public ParticleSystem target;

	// Token: 0x04000F6E RID: 3950
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
