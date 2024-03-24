using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class TrainLocomotive : TrainCarBase
	{
		private void Start()
		{
			this._standardRateOverTimeMultiplier = this._smoke1.emission.rateOverTimeMultiplier;
			this._standardMaxParticles = this._smoke1.main.maxParticles;
			base.Distance = 0f;
			this._speedDiv = 2.5f / (float)this._accelerationSounds.Length;
			this._currentSpeed = this._initialSpeed;
			base.UpdateCarPosition();
			this._smoke1.Stop();
			this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(true));
		}

		private void Update()
		{
			this.UpdatePosition();
		}

		public override void UpdatePosition()
		{
			if (!this._isMoving)
			{
				return;
			}
			if (this._trainTrack != null)
			{
				this.UpdateDistance();
				base.UpdateCarPosition();
				base.RotateCarWheels();
			}
			TrainCarBase[] childCars = this._childCars;
			for (int i = 0; i < childCars.Length; i++)
			{
				childCars[i].UpdatePosition();
			}
		}

		public void StartStopStateChanged()
		{
			if (this._startStopTrainCr == null)
			{
				this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(!this._isMoving));
			}
		}

		private IEnumerator StartStopTrain(bool startTrain)
		{
			float endSpeed = (startTrain ? this._initialSpeed : 0f);
			float timePeriodForSpeedChange = 3f;
			if (startTrain)
			{
				this._smoke1.Play();
				this._isMoving = true;
				ParticleSystem.EmissionModule emission = this._smoke1.emission;
				ParticleSystem.MainModule main = this._smoke1.main;
				emission.rateOverTimeMultiplier = this._standardRateOverTimeMultiplier;
				main.maxParticles = this._standardMaxParticles;
				timePeriodForSpeedChange = this.PlayEngineSound(TrainLocomotive.EngineSoundState.Start);
			}
			else
			{
				timePeriodForSpeedChange = this.PlayEngineSound(TrainLocomotive.EngineSoundState.Stop);
			}
			this._engineAudioSource.loop = false;
			timePeriodForSpeedChange *= 0.9f;
			float startTime = Time.time;
			float endTime = Time.time + timePeriodForSpeedChange;
			float startSpeed = this._currentSpeed;
			while (Time.time < endTime)
			{
				float num = (Time.time - startTime) / timePeriodForSpeedChange;
				this._currentSpeed = startSpeed * (1f - num) + endSpeed * num;
				this.UpdateSmokeEmissionBasedOnSpeed();
				yield return null;
			}
			this._currentSpeed = endSpeed;
			this._startStopTrainCr = null;
			this._isMoving = startTrain;
			if (!this._isMoving)
			{
				this._smoke1.Stop();
			}
			else
			{
				this._engineAudioSource.loop = true;
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
			yield break;
		}

		private float PlayEngineSound(TrainLocomotive.EngineSoundState engineSoundState)
		{
			AudioClip audioClip;
			if (engineSoundState == TrainLocomotive.EngineSoundState.Start)
			{
				audioClip = this._startUpSound;
			}
			else
			{
				AudioClip[] array = ((engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed) ? this._accelerationSounds : this._decelerationSounds);
				int num = array.Length;
				int num2 = (int)Mathf.Round((this._currentSpeed - 0.2f) / this._speedDiv);
				audioClip = array[Mathf.Clamp(num2, 0, num - 1)];
			}
			if (this._engineAudioSource.clip == audioClip && this._engineAudioSource.isPlaying && engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed)
			{
				return 0f;
			}
			this._engineAudioSource.clip = audioClip;
			this._engineAudioSource.timeSamples = 0;
			this._engineAudioSource.Play();
			return audioClip.length;
		}

		private void UpdateDistance()
		{
			float num = (this._reverse ? (-this._currentSpeed) : this._currentSpeed);
			base.Distance = (base.Distance + num * Time.deltaTime) % this._trainTrack.TrackLength;
		}

		public void DecreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed - this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		public void IncreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed + this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		private void UpdateSmokeEmissionBasedOnSpeed()
		{
			this._smoke1.emission.rateOverTimeMultiplier = this.GetCurrentSmokeEmission();
			this._smoke1.main.maxParticles = (int)Mathf.Lerp((float)this._standardMaxParticles, (float)(this._standardMaxParticles * 3), this._currentSpeed / 2.5f);
		}

		private float GetCurrentSmokeEmission()
		{
			return Mathf.Lerp(this._standardRateOverTimeMultiplier, this._standardRateOverTimeMultiplier * 8f, this._currentSpeed / 2.5f);
		}

		public void SmokeButtonStateChanged()
		{
			if (this._isMoving)
			{
				this._smokeStackAudioSource.clip = this._smokeSound;
				this._smokeStackAudioSource.timeSamples = 0;
				this._smokeStackAudioSource.Play();
				this._smoke2.time = 0f;
				this._smoke2.Play();
			}
		}

		public void WhistleButtonStateChanged()
		{
			if (this._whistleSound != null)
			{
				this._whistleAudioSource.clip = this._whistleSound;
				this._whistleAudioSource.timeSamples = 0;
				this._whistleAudioSource.Play();
			}
		}

		public void ReverseButtonStateChanged()
		{
			this._reverse = !this._reverse;
		}

		public TrainLocomotive()
		{
		}

		private const float MIN_SPEED = 0.2f;

		private const float MAX_SPEED = 2.7f;

		private const float SMOKE_SPEED_MULTIPLIER = 8f;

		private const int MAX_PARTICLES_MULTIPLIER = 3;

		[SerializeField]
		[Range(0.2f, 2.7f)]
		protected float _initialSpeed;

		[SerializeField]
		private GameObject _startStopButton;

		[SerializeField]
		private GameObject _decreaseSpeedButton;

		[SerializeField]
		private GameObject _increaseSpeedButton;

		[SerializeField]
		private GameObject _smokeButton;

		[SerializeField]
		private GameObject _whistleButton;

		[SerializeField]
		private GameObject _reverseButton;

		[SerializeField]
		private AudioSource _whistleAudioSource;

		[SerializeField]
		private AudioClip _whistleSound;

		[SerializeField]
		private AudioSource _engineAudioSource;

		[SerializeField]
		private AudioClip[] _accelerationSounds;

		[SerializeField]
		private AudioClip[] _decelerationSounds;

		[SerializeField]
		private AudioClip _startUpSound;

		[SerializeField]
		private AudioSource _smokeStackAudioSource;

		[SerializeField]
		private AudioClip _smokeSound;

		[SerializeField]
		private ParticleSystem _smoke1;

		[SerializeField]
		private ParticleSystem _smoke2;

		[SerializeField]
		private TrainCarBase[] _childCars;

		private bool _isMoving = true;

		private bool _reverse;

		private float _currentSpeed;

		private float _speedDiv;

		private float _standardRateOverTimeMultiplier;

		private int _standardMaxParticles;

		private Coroutine _startStopTrainCr;

		private enum EngineSoundState
		{
			Start,
			AccelerateOrSetProperSpeed,
			Stop
		}

		[CompilerGenerated]
		private sealed class <StartStopTrain>d__34 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <StartStopTrain>d__34(int <>1__state)
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
				TrainLocomotive trainLocomotive = this;
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
					endSpeed = (startTrain ? trainLocomotive._initialSpeed : 0f);
					timePeriodForSpeedChange = 3f;
					if (startTrain)
					{
						trainLocomotive._smoke1.Play();
						trainLocomotive._isMoving = true;
						ParticleSystem.EmissionModule emission = trainLocomotive._smoke1.emission;
						ParticleSystem.MainModule main = trainLocomotive._smoke1.main;
						emission.rateOverTimeMultiplier = trainLocomotive._standardRateOverTimeMultiplier;
						main.maxParticles = trainLocomotive._standardMaxParticles;
						timePeriodForSpeedChange = trainLocomotive.PlayEngineSound(TrainLocomotive.EngineSoundState.Start);
					}
					else
					{
						timePeriodForSpeedChange = trainLocomotive.PlayEngineSound(TrainLocomotive.EngineSoundState.Stop);
					}
					trainLocomotive._engineAudioSource.loop = false;
					timePeriodForSpeedChange *= 0.9f;
					startTime = Time.time;
					endTime = Time.time + timePeriodForSpeedChange;
					startSpeed = trainLocomotive._currentSpeed;
				}
				if (Time.time >= endTime)
				{
					trainLocomotive._currentSpeed = endSpeed;
					trainLocomotive._startStopTrainCr = null;
					trainLocomotive._isMoving = startTrain;
					if (!trainLocomotive._isMoving)
					{
						trainLocomotive._smoke1.Stop();
					}
					else
					{
						trainLocomotive._engineAudioSource.loop = true;
						trainLocomotive.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
					}
					return false;
				}
				float num2 = (Time.time - startTime) / timePeriodForSpeedChange;
				trainLocomotive._currentSpeed = startSpeed * (1f - num2) + endSpeed * num2;
				trainLocomotive.UpdateSmokeEmissionBasedOnSpeed();
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

			public bool startTrain;

			public TrainLocomotive <>4__this;

			private float <endSpeed>5__2;

			private float <timePeriodForSpeedChange>5__3;

			private float <startTime>5__4;

			private float <endTime>5__5;

			private float <startSpeed>5__6;
		}
	}
}
