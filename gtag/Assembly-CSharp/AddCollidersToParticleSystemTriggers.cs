using System;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class AddCollidersToParticleSystemTriggers : MonoBehaviour
{
	// Token: 0x06000848 RID: 2120 RVA: 0x000334D0 File Offset: 0x000316D0
	private void Update()
	{
		this.count = 0;
		while (this.count < 6)
		{
			this.index++;
			if (this.index >= this.collidersToAdd.Length)
			{
				if (BetterDayNightManager.instance.collidersToAddToWeatherSystems.Count >= this.index - this.collidersToAdd.Length)
				{
					this.index = 0;
				}
				else
				{
					this.particleSystemToUpdate.trigger.SetCollider(this.count, BetterDayNightManager.instance.collidersToAddToWeatherSystems[this.index - this.collidersToAdd.Length]);
				}
			}
			if (this.index < this.collidersToAdd.Length)
			{
				this.particleSystemToUpdate.trigger.SetCollider(this.count, this.collidersToAdd[this.index]);
			}
			this.count++;
		}
	}

	// Token: 0x04000A2C RID: 2604
	public Collider[] collidersToAdd;

	// Token: 0x04000A2D RID: 2605
	public ParticleSystem particleSystemToUpdate;

	// Token: 0x04000A2E RID: 2606
	private int count;

	// Token: 0x04000A2F RID: 2607
	private int index;
}
