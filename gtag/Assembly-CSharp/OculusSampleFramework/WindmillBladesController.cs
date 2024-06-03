using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class WindmillBladesController : MonoBehaviour
	{
		public bool IsMoving
		{
			[CompilerGenerated]
			get
			{
				return this.<IsMoving>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<IsMoving>k__BackingField = value;
			}
		}

		private void Start()
		{
			this._originalRotation = base.transform.localRotation;
		}

		private void Update()
		{
			this._rotAngle += this._currentSpeed * Time.deltaTime;
			if (this._rotAngle > 360f)
			{
				this._rotAngle = 0f;
			}
			base.transform.localRotation = this._originalRotation * Quaternion.AngleAxis(this._rotAngle, Vector3.forward);
		}

		public void SetMoveState(bool newMoveState, float goalSpeed)
		{
			this.IsMoving = newMoveState;
			if (this._lerpSpeedCoroutine != null)
			{
				base.StopCoroutine(this._lerpSpeedCoroutine);
			}
			this._lerpSpeedCoroutine = base.StartCoroutine(this.LerpToSpeed(goalSpeed));
		}

		private IEnumerator LerpToSpeed(float goalSpeed)
		{
			float totalTime = 0f;
			float startSpeed = this._currentSpeed;
			if (this._audioChangeCr != null)
			{
				base.StopCoroutine(this._audioChangeCr);
			}
			if (this.IsMoving)
			{
				this._audioChangeCr = base.StartCoroutine(this.PlaySoundDelayed(this._windMillStartSound, this._windMillRotationSound, this._windMillStartSound.length * 0.95f));
			}
			else
			{
				this.PlaySound(this._windMillStopSound, false);
			}
			for (float num = Mathf.Abs(this._currentSpeed - goalSpeed); num > Mathf.Epsilon; num = Mathf.Abs(this._currentSpeed - goalSpeed))
			{
				this._currentSpeed = Mathf.Lerp(startSpeed, goalSpeed, totalTime / 1f);
				totalTime += Time.deltaTime;
				yield return null;
			}
			this._lerpSpeedCoroutine = null;
			yield break;
		}

		private IEnumerator PlaySoundDelayed(AudioClip initial, AudioClip clip, float timeDelayAfterInitial)
		{
			this.PlaySound(initial, false);
			yield return new WaitForSeconds(timeDelayAfterInitial);
			this.PlaySound(clip, true);
			yield break;
		}

		private void PlaySound(AudioClip clip, bool loop = false)
		{
			this._audioSource.loop = loop;
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		public WindmillBladesController()
		{
		}

		private const float MAX_TIME = 1f;

		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private AudioClip _windMillRotationSound;

		[SerializeField]
		private AudioClip _windMillStartSound;

		[SerializeField]
		private AudioClip _windMillStopSound;

		[CompilerGenerated]
		private bool <IsMoving>k__BackingField;

		private float _currentSpeed;

		private Coroutine _lerpSpeedCoroutine;

		private Coroutine _audioChangeCr;

		private Quaternion _originalRotation;

		private float _rotAngle;

		[CompilerGenerated]
		private sealed class <LerpToSpeed>d__17 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <LerpToSpeed>d__17(int <>1__state)
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
				WindmillBladesController windmillBladesController = this;
				float num2;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
					num2 = Mathf.Abs(windmillBladesController._currentSpeed - goalSpeed);
				}
				else
				{
					this.<>1__state = -1;
					totalTime = 0f;
					startSpeed = windmillBladesController._currentSpeed;
					if (windmillBladesController._audioChangeCr != null)
					{
						windmillBladesController.StopCoroutine(windmillBladesController._audioChangeCr);
					}
					if (windmillBladesController.IsMoving)
					{
						windmillBladesController._audioChangeCr = windmillBladesController.StartCoroutine(windmillBladesController.PlaySoundDelayed(windmillBladesController._windMillStartSound, windmillBladesController._windMillRotationSound, windmillBladesController._windMillStartSound.length * 0.95f));
					}
					else
					{
						windmillBladesController.PlaySound(windmillBladesController._windMillStopSound, false);
					}
					num2 = Mathf.Abs(windmillBladesController._currentSpeed - goalSpeed);
				}
				if (num2 <= Mathf.Epsilon)
				{
					windmillBladesController._lerpSpeedCoroutine = null;
					return false;
				}
				windmillBladesController._currentSpeed = Mathf.Lerp(startSpeed, goalSpeed, totalTime / 1f);
				totalTime += Time.deltaTime;
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

			public WindmillBladesController <>4__this;

			public float goalSpeed;

			private float <totalTime>5__2;

			private float <startSpeed>5__3;
		}

		[CompilerGenerated]
		private sealed class <PlaySoundDelayed>d__18 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <PlaySoundDelayed>d__18(int <>1__state)
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
				WindmillBladesController windmillBladesController = this;
				if (num == 0)
				{
					this.<>1__state = -1;
					windmillBladesController.PlaySound(initial, false);
					this.<>2__current = new WaitForSeconds(timeDelayAfterInitial);
					this.<>1__state = 1;
					return true;
				}
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				windmillBladesController.PlaySound(clip, true);
				return false;
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

			public WindmillBladesController <>4__this;

			public AudioClip initial;

			public float timeDelayAfterInitial;

			public AudioClip clip;
		}
	}
}
