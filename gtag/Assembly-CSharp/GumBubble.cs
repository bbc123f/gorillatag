using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GumBubble : LerpComponent
{
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

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

	public GumBubble()
	{
	}

	public Transform target;

	public Vector3 targetScale = Vector3.one;

	[SerializeField]
	private AnimationCurve _lerpCurve;

	public AudioSource audioSource;

	[SerializeField]
	private AudioClip _sfxInflate;

	[SerializeField]
	private AudioClip _sfxPop;

	[SerializeField]
	private float _delayInflate = 1.16f;

	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	[SerializeField]
	private bool _animating;

	public UnityEvent onPop;

	public UnityEvent onInflate;

	[NonSerialized]
	private bool _done;

	[NonSerialized]
	private TimeSince _sinceInflate;
}
