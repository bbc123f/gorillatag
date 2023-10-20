using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EF RID: 751
	public class TrainLocomotive : TrainCarBase
	{
		// Token: 0x0600145A RID: 5210 RVA: 0x000731EC File Offset: 0x000713EC
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

		// Token: 0x0600145B RID: 5211 RVA: 0x0007327B File Offset: 0x0007147B
		private void Update()
		{
			this.UpdatePosition();
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00073284 File Offset: 0x00071484
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

		// Token: 0x0600145D RID: 5213 RVA: 0x000732D7 File Offset: 0x000714D7
		public void StartStopStateChanged()
		{
			if (this._startStopTrainCr == null)
			{
				this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(!this._isMoving));
			}
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x000732FC File Offset: 0x000714FC
		private IEnumerator StartStopTrain(bool startTrain)
		{
			float endSpeed = startTrain ? this._initialSpeed : 0f;
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

		// Token: 0x0600145F RID: 5215 RVA: 0x00073314 File Offset: 0x00071514
		private float PlayEngineSound(TrainLocomotive.EngineSoundState engineSoundState)
		{
			AudioClip audioClip;
			if (engineSoundState == TrainLocomotive.EngineSoundState.Start)
			{
				audioClip = this._startUpSound;
			}
			else
			{
				AudioClip[] array = (engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed) ? this._accelerationSounds : this._decelerationSounds;
				int num = array.Length;
				int value = (int)Mathf.Round((this._currentSpeed - 0.2f) / this._speedDiv);
				audioClip = array[Mathf.Clamp(value, 0, num - 1)];
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

		// Token: 0x06001460 RID: 5216 RVA: 0x000733C0 File Offset: 0x000715C0
		private void UpdateDistance()
		{
			float num = this._reverse ? (-this._currentSpeed) : this._currentSpeed;
			base.Distance = (base.Distance + num * Time.deltaTime) % this._trainTrack.TrackLength;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x00073408 File Offset: 0x00071608
		public void DecreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed - this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00073458 File Offset: 0x00071658
		public void IncreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed + this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x000734A8 File Offset: 0x000716A8
		private void UpdateSmokeEmissionBasedOnSpeed()
		{
			this._smoke1.emission.rateOverTimeMultiplier = this.GetCurrentSmokeEmission();
			this._smoke1.main.maxParticles = (int)Mathf.Lerp((float)this._standardMaxParticles, (float)(this._standardMaxParticles * 3), this._currentSpeed / 2.5f);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x00073503 File Offset: 0x00071703
		private float GetCurrentSmokeEmission()
		{
			return Mathf.Lerp(this._standardRateOverTimeMultiplier, this._standardRateOverTimeMultiplier * 8f, this._currentSpeed / 2.5f);
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00073528 File Offset: 0x00071728
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

		// Token: 0x06001466 RID: 5222 RVA: 0x00073580 File Offset: 0x00071780
		public void WhistleButtonStateChanged()
		{
			if (this._whistleSound != null)
			{
				this._whistleAudioSource.clip = this._whistleSound;
				this._whistleAudioSource.timeSamples = 0;
				this._whistleAudioSource.Play();
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000735B8 File Offset: 0x000717B8
		public void ReverseButtonStateChanged()
		{
			this._reverse = !this._reverse;
		}

		// Token: 0x04001707 RID: 5895
		private const float MIN_SPEED = 0.2f;

		// Token: 0x04001708 RID: 5896
		private const float MAX_SPEED = 2.7f;

		// Token: 0x04001709 RID: 5897
		private const float SMOKE_SPEED_MULTIPLIER = 8f;

		// Token: 0x0400170A RID: 5898
		private const int MAX_PARTICLES_MULTIPLIER = 3;

		// Token: 0x0400170B RID: 5899
		[SerializeField]
		[Range(0.2f, 2.7f)]
		protected float _initialSpeed;

		// Token: 0x0400170C RID: 5900
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x0400170D RID: 5901
		[SerializeField]
		private GameObject _decreaseSpeedButton;

		// Token: 0x0400170E RID: 5902
		[SerializeField]
		private GameObject _increaseSpeedButton;

		// Token: 0x0400170F RID: 5903
		[SerializeField]
		private GameObject _smokeButton;

		// Token: 0x04001710 RID: 5904
		[SerializeField]
		private GameObject _whistleButton;

		// Token: 0x04001711 RID: 5905
		[SerializeField]
		private GameObject _reverseButton;

		// Token: 0x04001712 RID: 5906
		[SerializeField]
		private AudioSource _whistleAudioSource;

		// Token: 0x04001713 RID: 5907
		[SerializeField]
		private AudioClip _whistleSound;

		// Token: 0x04001714 RID: 5908
		[SerializeField]
		private AudioSource _engineAudioSource;

		// Token: 0x04001715 RID: 5909
		[SerializeField]
		private AudioClip[] _accelerationSounds;

		// Token: 0x04001716 RID: 5910
		[SerializeField]
		private AudioClip[] _decelerationSounds;

		// Token: 0x04001717 RID: 5911
		[SerializeField]
		private AudioClip _startUpSound;

		// Token: 0x04001718 RID: 5912
		[SerializeField]
		private AudioSource _smokeStackAudioSource;

		// Token: 0x04001719 RID: 5913
		[SerializeField]
		private AudioClip _smokeSound;

		// Token: 0x0400171A RID: 5914
		[SerializeField]
		private ParticleSystem _smoke1;

		// Token: 0x0400171B RID: 5915
		[SerializeField]
		private ParticleSystem _smoke2;

		// Token: 0x0400171C RID: 5916
		[SerializeField]
		private TrainCarBase[] _childCars;

		// Token: 0x0400171D RID: 5917
		private bool _isMoving = true;

		// Token: 0x0400171E RID: 5918
		private bool _reverse;

		// Token: 0x0400171F RID: 5919
		private float _currentSpeed;

		// Token: 0x04001720 RID: 5920
		private float _speedDiv;

		// Token: 0x04001721 RID: 5921
		private float _standardRateOverTimeMultiplier;

		// Token: 0x04001722 RID: 5922
		private int _standardMaxParticles;

		// Token: 0x04001723 RID: 5923
		private Coroutine _startStopTrainCr;

		// Token: 0x020004F3 RID: 1267
		private enum EngineSoundState
		{
			// Token: 0x040020A8 RID: 8360
			Start,
			// Token: 0x040020A9 RID: 8361
			AccelerateOrSetProperSpeed,
			// Token: 0x040020AA RID: 8362
			Stop
		}
	}
}
