using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000233 RID: 563
public class FireworksController : MonoBehaviour
{
	// Token: 0x06000DE5 RID: 3557 RVA: 0x000509A9 File Offset: 0x0004EBA9
	private void Awake()
	{
		this._launchOrder = this.fireworks.ToArray<Firework>();
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x000509D0 File Offset: 0x0004EBD0
	public void LaunchVolley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this._rnd.Shuffle<Firework>(this._launchOrder);
		for (int i = 0; i < this._launchOrder.Length; i++)
		{
			MonoBehaviour monoBehaviour = this._launchOrder[i];
			float time = this._rnd.NextFloat() * this.roundLength;
			monoBehaviour.Invoke("Launch", time);
		}
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00050A34 File Offset: 0x0004EC34
	public void LaunchVolleyRound()
	{
		int num = 0;
		while ((long)num < (long)((ulong)this.roundNumVolleys))
		{
			float time = this._rnd.NextFloat() * this.roundLength;
			base.Invoke("LaunchVolley", time);
			num++;
		}
		if (this._fireworksEvent)
		{
			base.Invoke("LaunchVolleyRound", this.roundLength);
		}
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00050A98 File Offset: 0x0004EC98
	public void Launch(Firework fw)
	{
		if (!fw)
		{
			return;
		}
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		AudioSource sourceOrigin = fw.sourceOrigin;
		int num = this._rnd.NextInt(this.bursts.Length);
		AudioClip audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		AudioClip audioClip2 = this.bursts[num];
		while (this._lastWhistle == audioClip)
		{
			audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		}
		while (this._lastBurst == audioClip2)
		{
			num = this._rnd.NextInt(this.bursts.Length);
			audioClip2 = this.bursts[num];
		}
		this._lastWhistle = audioClip;
		this._lastBurst = audioClip2;
		int num2 = this._rnd.NextInt(fw.explosions.Length);
		ParticleSystem particleSystem = fw.explosions[num2];
		if (fw.doTrail)
		{
			ParticleSystem trail = fw.trail;
			trail.startColor = fw.colorOrigin;
			trail.subEmitters.GetSubEmitterSystem(0).colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fw.colorOrigin, fw.colorTarget);
			trail.Stop();
			trail.Play();
		}
		sourceOrigin.pitch = this._rnd.NextFloat(0.92f, 1f);
		fw.doTrailAudio = this._rnd.NextBool();
		FireworksController.ExplosionEvent ev = new FireworksController.ExplosionEvent
		{
			firework = fw,
			timeSince = TimeSince.Now(),
			burstIndex = num,
			explosionIndex = num2,
			delay = (double)(fw.doTrail ? audioClip.length : 0f),
			active = true
		};
		if (fw.doExplosion)
		{
			this.PostExplosionEvent(ev);
		}
		if (fw.doTrailAudio && this._timeSinceLastWhistle > this.minWhistleDelay)
		{
			this._timeSinceLastWhistle = TimeSince.Now();
			sourceOrigin.PlayOneShot(audioClip, this._rnd.NextFloat(this.whistleVolumeMin, this.whistleVolumeMax));
		}
		particleSystem.Stop();
		particleSystem.transform.position = position2;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00050CC8 File Offset: 0x0004EEC8
	private void PostExplosionEvent(FireworksController.ExplosionEvent ev)
	{
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			if (!this._explosionQueue[i].active)
			{
				this._explosionQueue[i] = ev;
				return;
			}
		}
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x00050D09 File Offset: 0x0004EF09
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00050D14 File Offset: 0x0004EF14
	private void ProcessEvents()
	{
		if (this._explosionQueue == null || this._explosionQueue.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			FireworksController.ExplosionEvent explosionEvent = this._explosionQueue[i];
			if (explosionEvent.active && explosionEvent.timeSince >= explosionEvent.delay)
			{
				this.DoExplosion(explosionEvent);
				this._explosionQueue[i] = default(FireworksController.ExplosionEvent);
			}
		}
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00050D88 File Offset: 0x0004EF88
	private void DoExplosion(FireworksController.ExplosionEvent ev)
	{
		Firework firework = ev.firework;
		ParticleSystem particleSystem = firework.explosions[ev.explosionIndex];
		ParticleSystem.MinMaxGradient color = new ParticleSystem.MinMaxGradient(firework.colorOrigin, firework.colorTarget);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.subEmitters.GetSubEmitterSystem(0).colorOverLifetime;
		colorOverLifetime.color = color;
		colorOverLifetime2.color = color;
		ParticleSystem particleSystem2 = firework.explosions[ev.explosionIndex];
		particleSystem2.Stop();
		particleSystem2.Play();
		firework.sourceTarget.PlayOneShot(this.bursts[ev.burstIndex]);
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00050E18 File Offset: 0x0004F018
	public void RenderGizmo(Firework fw, Color c)
	{
		if (!fw)
		{
			return;
		}
		if (!fw.origin || !fw.target)
		{
			return;
		}
		Gizmos.color = c;
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		Gizmos.DrawLine(position, position2);
		Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
		Gizmos.DrawWireCube(position2, Vector3.one * 0.5f);
	}

	// Token: 0x040010D5 RID: 4309
	public Firework[] fireworks;

	// Token: 0x040010D6 RID: 4310
	public AudioClip[] whistles;

	// Token: 0x040010D7 RID: 4311
	public AudioClip[] bursts;

	// Token: 0x040010D8 RID: 4312
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x040010D9 RID: 4313
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x040010DA RID: 4314
	public float minWhistleDelay = 1f;

	// Token: 0x040010DB RID: 4315
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x040010DC RID: 4316
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x040010DD RID: 4317
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x040010DE RID: 4318
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x040010DF RID: 4319
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x040010E0 RID: 4320
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x040010E1 RID: 4321
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x040010E2 RID: 4322
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x040010E3 RID: 4323
	public uint roundLength = 6U;

	// Token: 0x040010E4 RID: 4324
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeOfDayEvent _fireworksEvent;

	// Token: 0x0200047F RID: 1151
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x04001EB2 RID: 7858
		public TimeSince timeSince;

		// Token: 0x04001EB3 RID: 7859
		public double delay;

		// Token: 0x04001EB4 RID: 7860
		public int explosionIndex;

		// Token: 0x04001EB5 RID: 7861
		public int burstIndex;

		// Token: 0x04001EB6 RID: 7862
		public bool active;

		// Token: 0x04001EB7 RID: 7863
		public Firework firework;
	}
}
