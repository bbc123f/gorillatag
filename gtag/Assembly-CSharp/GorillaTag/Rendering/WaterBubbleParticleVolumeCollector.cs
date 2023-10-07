using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	// Token: 0x02000327 RID: 807
	public class WaterBubbleParticleVolumeCollector : MonoBehaviour
	{
		// Token: 0x06001676 RID: 5750 RVA: 0x0007D01C File Offset: 0x0007B21C
		protected void Awake()
		{
			List<WaterVolume> componentsInHierarchy = SceneManager.GetActiveScene().GetComponentsInHierarchy(true, 64);
			List<Collider> list = new List<Collider>(componentsInHierarchy.Count * 4);
			foreach (WaterVolume waterVolume in componentsInHierarchy)
			{
				if (!(waterVolume.Parameters != null) || waterVolume.Parameters.allowBubblesInVolume)
				{
					foreach (Collider collider in waterVolume.volumeColliders)
					{
						if (!(collider == null))
						{
							list.Add(collider);
						}
					}
				}
			}
			this.bubbleableVolumeColliders = list.ToArray();
			this.particleTriggerModules = new ParticleSystem.TriggerModule[this.particleSystems.Length];
			this.particleEmissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleTriggerModules[i] = this.particleSystems[i].trigger;
				this.particleEmissionModules[i] = this.particleSystems[i].emission;
			}
			for (int j = 0; j < this.particleSystems.Length; j++)
			{
				ParticleSystem.TriggerModule triggerModule = this.particleTriggerModules[j];
				for (int k = 0; k < list.Count; k++)
				{
					triggerModule.SetCollider(k, this.bubbleableVolumeColliders[k]);
				}
			}
			this.SetEmissionState(false);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x0007D1BC File Offset: 0x0007B3BC
		protected void LateUpdate()
		{
			bool headInWater = Player.Instance.HeadInWater;
			if (headInWater && !this.emissionEnabled)
			{
				this.SetEmissionState(true);
				return;
			}
			if (!headInWater && this.emissionEnabled)
			{
				this.SetEmissionState(false);
			}
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x0007D1FC File Offset: 0x0007B3FC
		private void SetEmissionState(bool setEnabled)
		{
			float rateOverTimeMultiplier = setEnabled ? 1f : 0f;
			for (int i = 0; i < this.particleEmissionModules.Length; i++)
			{
				this.particleEmissionModules[i].rateOverTimeMultiplier = rateOverTimeMultiplier;
			}
			this.emissionEnabled = setEnabled;
		}

		// Token: 0x04001886 RID: 6278
		public ParticleSystem[] particleSystems;

		// Token: 0x04001887 RID: 6279
		private ParticleSystem.TriggerModule[] particleTriggerModules;

		// Token: 0x04001888 RID: 6280
		private ParticleSystem.EmissionModule[] particleEmissionModules;

		// Token: 0x04001889 RID: 6281
		private Collider[] bubbleableVolumeColliders;

		// Token: 0x0400188A RID: 6282
		private bool emissionEnabled;
	}
}
