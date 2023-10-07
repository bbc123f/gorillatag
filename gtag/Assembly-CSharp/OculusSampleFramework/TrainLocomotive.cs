using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002ED RID: 749
	public class TrainLocomotive : TrainCarBase
	{
		// Token: 0x06001453 RID: 5203 RVA: 0x00072D20 File Offset: 0x00070F20
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

		// Token: 0x06001454 RID: 5204 RVA: 0x00072DAF File Offset: 0x00070FAF
		private void Update()
		{
			this.UpdatePosition();
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x00072DB8 File Offset: 0x00070FB8
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

		// Token: 0x06001456 RID: 5206 RVA: 0x00072E0B File Offset: 0x0007100B
		public void StartStopStateChanged()
		{
			if (this._startStopTrainCr == null)
			{
				this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(!this._isMoving));
			}
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00072E30 File Offset: 0x00071030
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

		// Token: 0x06001458 RID: 5208 RVA: 0x00072E48 File Offset: 0x00071048
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

		// Token: 0x06001459 RID: 5209 RVA: 0x00072EF4 File Offset: 0x000710F4
		private void UpdateDistance()
		{
			float num = this._reverse ? (-this._currentSpeed) : this._currentSpeed;
			base.Distance = (base.Distance + num * Time.deltaTime) % this._trainTrack.TrackLength;
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x00072F3C File Offset: 0x0007113C
		public void DecreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed - this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x00072F8C File Offset: 0x0007118C
		public void IncreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed + this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00072FDC File Offset: 0x000711DC
		private void UpdateSmokeEmissionBasedOnSpeed()
		{
			this._smoke1.emission.rateOverTimeMultiplier = this.GetCurrentSmokeEmission();
			this._smoke1.main.maxParticles = (int)Mathf.Lerp((float)this._standardMaxParticles, (float)(this._standardMaxParticles * 3), this._currentSpeed / 2.5f);
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x00073037 File Offset: 0x00071237
		private float GetCurrentSmokeEmission()
		{
			return Mathf.Lerp(this._standardRateOverTimeMultiplier, this._standardRateOverTimeMultiplier * 8f, this._currentSpeed / 2.5f);
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x0007305C File Offset: 0x0007125C
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

		// Token: 0x0600145F RID: 5215 RVA: 0x000730B4 File Offset: 0x000712B4
		public void WhistleButtonStateChanged()
		{
			if (this._whistleSound != null)
			{
				this._whistleAudioSource.clip = this._whistleSound;
				this._whistleAudioSource.timeSamples = 0;
				this._whistleAudioSource.Play();
			}
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x000730EC File Offset: 0x000712EC
		public void ReverseButtonStateChanged()
		{
			this._reverse = !this._reverse;
		}

		// Token: 0x040016FA RID: 5882
		private const float MIN_SPEED = 0.2f;

		// Token: 0x040016FB RID: 5883
		private const float MAX_SPEED = 2.7f;

		// Token: 0x040016FC RID: 5884
		private const float SMOKE_SPEED_MULTIPLIER = 8f;

		// Token: 0x040016FD RID: 5885
		private const int MAX_PARTICLES_MULTIPLIER = 3;

		// Token: 0x040016FE RID: 5886
		[SerializeField]
		[Range(0.2f, 2.7f)]
		protected float _initialSpeed;

		// Token: 0x040016FF RID: 5887
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04001700 RID: 5888
		[SerializeField]
		private GameObject _decreaseSpeedButton;

		// Token: 0x04001701 RID: 5889
		[SerializeField]
		private GameObject _increaseSpeedButton;

		// Token: 0x04001702 RID: 5890
		[SerializeField]
		private GameObject _smokeButton;

		// Token: 0x04001703 RID: 5891
		[SerializeField]
		private GameObject _whistleButton;

		// Token: 0x04001704 RID: 5892
		[SerializeField]
		private GameObject _reverseButton;

		// Token: 0x04001705 RID: 5893
		[SerializeField]
		private AudioSource _whistleAudioSource;

		// Token: 0x04001706 RID: 5894
		[SerializeField]
		private AudioClip _whistleSound;

		// Token: 0x04001707 RID: 5895
		[SerializeField]
		private AudioSource _engineAudioSource;

		// Token: 0x04001708 RID: 5896
		[SerializeField]
		private AudioClip[] _accelerationSounds;

		// Token: 0x04001709 RID: 5897
		[SerializeField]
		private AudioClip[] _decelerationSounds;

		// Token: 0x0400170A RID: 5898
		[SerializeField]
		private AudioClip _startUpSound;

		// Token: 0x0400170B RID: 5899
		[SerializeField]
		private AudioSource _smokeStackAudioSource;

		// Token: 0x0400170C RID: 5900
		[SerializeField]
		private AudioClip _smokeSound;

		// Token: 0x0400170D RID: 5901
		[SerializeField]
		private ParticleSystem _smoke1;

		// Token: 0x0400170E RID: 5902
		[SerializeField]
		private ParticleSystem _smoke2;

		// Token: 0x0400170F RID: 5903
		[SerializeField]
		private TrainCarBase[] _childCars;

		// Token: 0x04001710 RID: 5904
		private bool _isMoving = true;

		// Token: 0x04001711 RID: 5905
		private bool _reverse;

		// Token: 0x04001712 RID: 5906
		private float _currentSpeed;

		// Token: 0x04001713 RID: 5907
		private float _speedDiv;

		// Token: 0x04001714 RID: 5908
		private float _standardRateOverTimeMultiplier;

		// Token: 0x04001715 RID: 5909
		private int _standardMaxParticles;

		// Token: 0x04001716 RID: 5910
		private Coroutine _startStopTrainCr;

		// Token: 0x020004F1 RID: 1265
		private enum EngineSoundState
		{
			// Token: 0x0400209B RID: 8347
			Start,
			// Token: 0x0400209C RID: 8348
			AccelerateOrSetProperSpeed,
			// Token: 0x0400209D RID: 8349
			Stop
		}
	}
}
