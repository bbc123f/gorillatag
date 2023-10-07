using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class Firework : MonoBehaviour
{
	// Token: 0x06000DE0 RID: 3552 RVA: 0x0005087F File Offset: 0x0004EA7F
	private void Launch()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._controller)
		{
			this._controller.Launch(this);
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x000508A4 File Offset: 0x0004EAA4
	private void OnValidate()
	{
		if (!this._controller)
		{
			this._controller = base.GetComponentInParent<FireworksController>();
		}
		if (!this._controller)
		{
			return;
		}
		Firework[] array = this._controller.fireworks;
		if (array.Contains(this))
		{
			return;
		}
		array = (from x in array.Concat(new Firework[]
		{
			this
		})
		where x != null
		select x).ToArray<Firework>();
		this._controller.fireworks = array;
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00050934 File Offset: 0x0004EB34
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00050955 File Offset: 0x0004EB55
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x040010C9 RID: 4297
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x040010CA RID: 4298
	[Space]
	public Transform origin;

	// Token: 0x040010CB RID: 4299
	public Transform target;

	// Token: 0x040010CC RID: 4300
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x040010CD RID: 4301
	public Color colorTarget = Color.magenta;

	// Token: 0x040010CE RID: 4302
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x040010CF RID: 4303
	public AudioSource sourceTarget;

	// Token: 0x040010D0 RID: 4304
	[Space]
	public ParticleSystem trail;

	// Token: 0x040010D1 RID: 4305
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x040010D2 RID: 4306
	[Space]
	public bool doTrail = true;

	// Token: 0x040010D3 RID: 4307
	public bool doTrailAudio = true;

	// Token: 0x040010D4 RID: 4308
	public bool doExplosion = true;
}
