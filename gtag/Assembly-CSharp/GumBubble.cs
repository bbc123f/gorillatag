using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000129 RID: 297
public class GumBubble : LerpComponent
{
	// Token: 0x060007C8 RID: 1992 RVA: 0x00031495 File Offset: 0x0002F695
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x000314AA File Offset: 0x0002F6AA
	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x000314B8 File Offset: 0x0002F6B8
	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x000314D8 File Offset: 0x0002F6D8
	public void Inflate()
	{
		base.gameObject.SetActive(true);
		base.enabled = true;
		if (this._animating)
		{
			return;
		}
		this._animating = true;
		this._sinceInflate = 0f;
		if (this.audioSource != null && this._sfxInflate != null)
		{
			this.audioSource.PlayOneShot(this._sfxInflate);
		}
		UnityEvent unityEvent = this.onInflate;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00031558 File Offset: 0x0002F758
	public void Pop()
	{
		this._lerp = 0f;
		base.RenderLerp();
		if (this.audioSource != null && this._sfxPop != null)
		{
			this.audioSource.PlayOneShot(this._sfxPop);
		}
		UnityEvent unityEvent = this.onPop;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this._done = false;
		this._animating = false;
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x000315D8 File Offset: 0x0002F7D8
	private void Update()
	{
		float t = Mathf.Clamp01(this._sinceInflate / this._lerpLength);
		this._lerp = Mathf.Lerp(0f, 1f, t);
		if (this._lerp <= 1f && !this._done)
		{
			base.RenderLerp();
			if (Mathf.Approximately(this._lerp, 1f))
			{
				this._done = true;
			}
		}
		float num = this._lerpLength + this._delayPop;
		if (this._sinceInflate >= num)
		{
			this.Pop();
		}
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0003166C File Offset: 0x0002F86C
	protected override void OnLerp(float t)
	{
		if (!this.target)
		{
			return;
		}
		if (this._lerpCurve == null)
		{
			GTDev.LogError("[GumBubble] Missing lerp curve", this, null);
			return;
		}
		this.target.localScale = this.targetScale * this._lerpCurve.Evaluate(t);
	}

	// Token: 0x0400095C RID: 2396
	public Transform target;

	// Token: 0x0400095D RID: 2397
	public Vector3 targetScale = Vector3.one;

	// Token: 0x0400095E RID: 2398
	[SerializeField]
	private AnimationCurve _lerpCurve;

	// Token: 0x0400095F RID: 2399
	public AudioSource audioSource;

	// Token: 0x04000960 RID: 2400
	[SerializeField]
	private AudioClip _sfxInflate;

	// Token: 0x04000961 RID: 2401
	[SerializeField]
	private AudioClip _sfxPop;

	// Token: 0x04000962 RID: 2402
	[SerializeField]
	private float _delayInflate = 1.16f;

	// Token: 0x04000963 RID: 2403
	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	// Token: 0x04000964 RID: 2404
	[SerializeField]
	private bool _animating;

	// Token: 0x04000965 RID: 2405
	public UnityEvent onPop;

	// Token: 0x04000966 RID: 2406
	public UnityEvent onInflate;

	// Token: 0x04000967 RID: 2407
	[NonSerialized]
	private bool _done;

	// Token: 0x04000968 RID: 2408
	[NonSerialized]
	private TimeSince _sinceInflate;
}
