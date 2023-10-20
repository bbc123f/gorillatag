using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000234 RID: 564
public class FireworksController : MonoBehaviour
{
	// Token: 0x06000DEC RID: 3564 RVA: 0x00050D85 File Offset: 0x0004EF85
	private void Awake()
	{
		this._launchOrder = this.fireworks.ToArray<Firework>();
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00050DAC File Offset: 0x0004EFAC
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

	// Token: 0x06000DEE RID: 3566 RVA: 0x00050E10 File Offset: 0x0004F010
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

	// Token: 0x06000DEF RID: 3567 RVA: 0x00050E74 File Offset: 0x0004F074
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

	// Token: 0x06000DF0 RID: 3568 RVA: 0x000510A4 File Offset: 0x0004F2A4
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

	// Token: 0x06000DF1 RID: 3569 RVA: 0x000510E5 File Offset: 0x0004F2E5
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x000510F0 File Offset: 0x0004F2F0
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

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00051164 File Offset: 0x0004F364
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

	// Token: 0x06000DF4 RID: 3572 RVA: 0x000511F4 File Offset: 0x0004F3F4
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

	// Token: 0x040010DB RID: 4315
	public Firework[] fireworks;

	// Token: 0x040010DC RID: 4316
	public AudioClip[] whistles;

	// Token: 0x040010DD RID: 4317
	public AudioClip[] bursts;

	// Token: 0x040010DE RID: 4318
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x040010DF RID: 4319
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x040010E0 RID: 4320
	public float minWhistleDelay = 1f;

	// Token: 0x040010E1 RID: 4321
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x040010E2 RID: 4322
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x040010E3 RID: 4323
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x040010E4 RID: 4324
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x040010E5 RID: 4325
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x040010E6 RID: 4326
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x040010E7 RID: 4327
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x040010E8 RID: 4328
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x040010E9 RID: 4329
	public uint roundLength = 6U;

	// Token: 0x040010EA RID: 4330
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeOfDayEvent _fireworksEvent;

	// Token: 0x02000481 RID: 1153
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x04001EBF RID: 7871
		public TimeSince timeSince;

		// Token: 0x04001EC0 RID: 7872
		public double delay;

		// Token: 0x04001EC1 RID: 7873
		public int explosionIndex;

		// Token: 0x04001EC2 RID: 7874
		public int burstIndex;

		// Token: 0x04001EC3 RID: 7875
		public bool active;

		// Token: 0x04001EC4 RID: 7876
		public Firework firework;
	}
}
