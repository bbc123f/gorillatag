using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001D4 RID: 468
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x06000C1A RID: 3098 RVA: 0x00049FE8 File Offset: 0x000481E8
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

	// Token: 0x06000C1B RID: 3099 RVA: 0x0004A0A4 File Offset: 0x000482A4
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0004A0B4 File Offset: 0x000482B4
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

	// Token: 0x06000C1D RID: 3101 RVA: 0x0004A0D4 File Offset: 0x000482D4
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0004A0DC File Offset: 0x000482DC
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

	// Token: 0x06000C1F RID: 3103 RVA: 0x0004A10C File Offset: 0x0004830C
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

	// Token: 0x06000C20 RID: 3104 RVA: 0x0004A177 File Offset: 0x00048377
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0004A189 File Offset: 0x00048389
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0004A19A File Offset: 0x0004839A
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04000F78 RID: 3960
	public ParticleSystem target;

	// Token: 0x04000F79 RID: 3961
	public ParticleSystem subEmitter;

	// Token: 0x04000F7A RID: 3962
	public int subEmitterIndex;

	// Token: 0x04000F7B RID: 3963
	public UnityEvent onSubEmit;

	// Token: 0x04000F7C RID: 3964
	public float intervalScale = 1f;

	// Token: 0x04000F7D RID: 3965
	public float interval;

	// Token: 0x04000F7E RID: 3966
	[NonSerialized]
	private bool _canListen;

	// Token: 0x04000F7F RID: 3967
	[NonSerialized]
	private bool _listening;

	// Token: 0x04000F80 RID: 3968
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x04000F81 RID: 3969
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
