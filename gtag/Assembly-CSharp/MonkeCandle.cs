using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class MonkeCandle : RubberDuck
{
	// Token: 0x060007EA RID: 2026 RVA: 0x00031D18 File Offset: 0x0002FF18
	protected override void Start()
	{
		base.Start();
		if (!this.IsMyItem())
		{
			this.movingFxAudio.volume = this.movingFxAudio.volume * 0.5f;
			this.fxExplodeAudio.volume = this.fxExplodeAudio.volume * 0.5f;
		}
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x00031D6C File Offset: 0x0002FF6C
	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!this.particleFX.isPlaying)
		{
			return;
		}
		int particles = this.particleFX.GetParticles(this.fxParticleArray);
		if (particles <= 0)
		{
			this.movingFxAudio.Stop();
			if (this.currentParticles.Count == 0)
			{
				return;
			}
		}
		for (int i = 0; i < particles; i++)
		{
			if (this.currentParticles.Contains(this.fxParticleArray[i].randomSeed))
			{
				this.currentParticles.Remove(this.fxParticleArray[i].randomSeed);
			}
		}
		foreach (uint key in this.currentParticles)
		{
			if (this.particleInfoDict.TryGetValue(key, out this.outPosition))
			{
				this.fxExplodeAudio.transform.position = this.outPosition;
				this.fxExplodeAudio.PlayOneShot(this.fxExplodeAudio.clip);
				this.particleInfoDict.Remove(key);
			}
		}
		this.currentParticles.Clear();
		for (int j = 0; j < particles; j++)
		{
			if (j == 0)
			{
				this.movingFxAudio.transform.position = this.fxParticleArray[j].position;
			}
			if (this.particleInfoDict.TryGetValue(this.fxParticleArray[j].randomSeed, out this.outPosition))
			{
				this.particleInfoDict[this.fxParticleArray[j].randomSeed] = this.fxParticleArray[j].position;
			}
			else
			{
				this.particleInfoDict.Add(this.fxParticleArray[j].randomSeed, this.fxParticleArray[j].position);
				if (j == 0 && !this.movingFxAudio.isPlaying)
				{
					this.movingFxAudio.Play();
				}
			}
			this.currentParticles.Add(this.fxParticleArray[j].randomSeed);
		}
	}

	// Token: 0x04000987 RID: 2439
	private ParticleSystem.Particle[] fxParticleArray = new ParticleSystem.Particle[20];

	// Token: 0x04000988 RID: 2440
	public AudioSource movingFxAudio;

	// Token: 0x04000989 RID: 2441
	public AudioSource fxExplodeAudio;

	// Token: 0x0400098A RID: 2442
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x0400098B RID: 2443
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x0400098C RID: 2444
	private Vector3 outPosition;
}
