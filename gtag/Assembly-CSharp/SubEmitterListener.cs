using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001D5 RID: 469
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x06000C20 RID: 3104 RVA: 0x0004A250 File Offset: 0x00048450
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.Disable();
			return;
		}
		ParticleSystem.SubEmittersModule subEmitters = this.target.subEmitters;
		if (this.subEmitterIndex < 0)
		{
			this.subEmitterIndex = 0;
		}
		this._canListen = (subEmitters.subEmittersCount > 0 && this.subEmitterIndex <= subEmitters.subEmittersCount - 1);
		if (!this._canListen)
		{
			this.Disable();
			return;
		}
		this.subEmitter = this.target.subEmitters.GetSubEmitterSystem(this.subEmitterIndex);
		ParticleSystem.MainModule main = this.subEmitter.main;
		this.interval = main.startLifetime.constantMax * main.startLifetimeMultiplier;
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0004A30C File Offset: 0x0004850C
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0004A31C File Offset: 0x0004851C
	public void ListenStart()
	{
		if (this._listening)
		{
			return;
		}
		if (this._canListen)
		{
			this.Enable();
			this._listening = true;
		}
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0004A33C File Offset: 0x0004853C
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0004A344 File Offset: 0x00048544
	public void ListenOnce()
	{
		if (this._listening)
		{
			return;
		}
		this.Enable();
		if (this._canListen)
		{
			this.Enable();
			this._listenOnce = true;
			this._listening = true;
		}
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0004A374 File Offset: 0x00048574
	private void Update()
	{
		if (!this._canListen)
		{
			return;
		}
		if (!this._listening)
		{
			return;
		}
		if (this.subEmitter.particleCount > 0 && this._sinceLastEmit >= this.interval * this.intervalScale)
		{
			this._sinceLastEmit = 0f;
			this.OnSubEmit();
			if (this._listenOnce)
			{
				this.Disable();
			}
		}
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0004A3DF File Offset: 0x000485DF
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0004A3F1 File Offset: 0x000485F1
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0004A402 File Offset: 0x00048602
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04000F7C RID: 3964
	public ParticleSystem target;

	// Token: 0x04000F7D RID: 3965
	public ParticleSystem subEmitter;

	// Token: 0x04000F7E RID: 3966
	public int subEmitterIndex;

	// Token: 0x04000F7F RID: 3967
	public UnityEvent onSubEmit;

	// Token: 0x04000F80 RID: 3968
	public float intervalScale = 1f;

	// Token: 0x04000F81 RID: 3969
	public float interval;

	// Token: 0x04000F82 RID: 3970
	[NonSerialized]
	private bool _canListen;

	// Token: 0x04000F83 RID: 3971
	[NonSerialized]
	private bool _listening;

	// Token: 0x04000F84 RID: 3972
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x04000F85 RID: 3973
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
