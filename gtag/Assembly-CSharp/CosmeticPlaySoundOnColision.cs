using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

public class CosmeticPlaySoundOnColision : MonoBehaviour
{
	private void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.soundLookup = new Dictionary<int, int>();
		this.audioSource = base.GetComponent<AudioSource>();
		for (int i = 0; i < this.soundIdRemappings.Length; i++)
		{
			this.soundLookup.Add(this.soundIdRemappings[i].SoundIn, this.soundIdRemappings[i].SoundOut);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GorillaSurfaceOverride gorillaSurfaceOverride;
		if (this.speed >= this.minSpeed && other.TryGetComponent<GorillaSurfaceOverride>(out gorillaSurfaceOverride))
		{
			int soundIndex;
			if (this.soundLookup.TryGetValue(gorillaSurfaceOverride.overrideIndex, out soundIndex))
			{
				this.playSound(soundIndex, this.invokeEventOnOverideSound);
				return;
			}
			this.playSound(this.defaultSound, this.invokeEventOnDefaultSound);
		}
	}

	private void playSound(int soundIndex, bool invokeEvent)
	{
		if (soundIndex > -1 && soundIndex < Player.Instance.materialData.Count)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.Stop();
				if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
				{
					this.OnStopPlayback.Invoke();
				}
				if (this.crWaitForStopPlayback != null)
				{
					base.StopCoroutine(this.crWaitForStopPlayback);
					this.crWaitForStopPlayback = null;
				}
			}
			this.audioSource.clip = Player.Instance.materialData[soundIndex].audio;
			this.audioSource.Play();
			if (invokeEvent && (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem()))
			{
				this.OnStartPlayback.Invoke();
				this.crWaitForStopPlayback = base.StartCoroutine(this.waitForStopPlayback());
			}
		}
	}

	private IEnumerator waitForStopPlayback()
	{
		while (this.audioSource.isPlaying)
		{
			yield return null;
		}
		if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
		{
			this.OnStopPlayback.Invoke();
		}
		this.crWaitForStopPlayback = null;
		yield break;
	}

	private void FixedUpdate()
	{
		this.speed = Vector3.Distance(base.transform.position, this.previousFramePosition) * Time.fixedDeltaTime * 100f;
		this.previousFramePosition = base.transform.position;
	}

	public CosmeticPlaySoundOnColision()
	{
	}

	[GorillaSoundLookup]
	[SerializeField]
	private int defaultSound = 1;

	[SerializeField]
	private SoundIdRemapping[] soundIdRemappings;

	[SerializeField]
	private UnityEvent OnStartPlayback;

	[SerializeField]
	private UnityEvent OnStopPlayback;

	[SerializeField]
	private float minSpeed = 0.1f;

	private TransferrableObject transferrableObject;

	private Dictionary<int, int> soundLookup;

	private AudioSource audioSource;

	private Coroutine crWaitForStopPlayback;

	private float speed;

	private Vector3 previousFramePosition;

	[SerializeField]
	private bool invokeEventsOnAllClients;

	[SerializeField]
	private bool invokeEventOnOverideSound = true;

	[SerializeField]
	private bool invokeEventOnDefaultSound;

	[CompilerGenerated]
	private sealed class <waitForStopPlayback>d__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <waitForStopPlayback>d__17(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			CosmeticPlaySoundOnColision cosmeticPlaySoundOnColision = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
			}
			if (!cosmeticPlaySoundOnColision.audioSource.isPlaying)
			{
				if (cosmeticPlaySoundOnColision.invokeEventsOnAllClients || cosmeticPlaySoundOnColision.transferrableObject.IsMyItem())
				{
					cosmeticPlaySoundOnColision.OnStopPlayback.Invoke();
				}
				cosmeticPlaySoundOnColision.crWaitForStopPlayback = null;
				return false;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public CosmeticPlaySoundOnColision <>4__this;
	}
}
