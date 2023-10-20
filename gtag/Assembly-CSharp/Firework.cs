using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class Firework : MonoBehaviour
{
	// Token: 0x06000DE7 RID: 3559 RVA: 0x00050C5C File Offset: 0x0004EE5C
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

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00050C80 File Offset: 0x0004EE80
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

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00050D10 File Offset: 0x0004EF10
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x00050D31 File Offset: 0x0004EF31
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x040010CF RID: 4303
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x040010D0 RID: 4304
	[Space]
	public Transform origin;

	// Token: 0x040010D1 RID: 4305
	public Transform target;

	// Token: 0x040010D2 RID: 4306
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x040010D3 RID: 4307
	public Color colorTarget = Color.magenta;

	// Token: 0x040010D4 RID: 4308
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x040010D5 RID: 4309
	public AudioSource sourceTarget;

	// Token: 0x040010D6 RID: 4310
	[Space]
	public ParticleSystem trail;

	// Token: 0x040010D7 RID: 4311
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x040010D8 RID: 4312
	[Space]
	public bool doTrail = true;

	// Token: 0x040010D9 RID: 4313
	public bool doTrailAudio = true;

	// Token: 0x040010DA RID: 4314
	public bool doExplosion = true;
}
