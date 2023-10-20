using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002F2 RID: 754
	public class WindmillBladesController : MonoBehaviour
	{
		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06001472 RID: 5234 RVA: 0x000737E2 File Offset: 0x000719E2
		// (set) Token: 0x06001473 RID: 5235 RVA: 0x000737EA File Offset: 0x000719EA
		public bool IsMoving { get; private set; }

		// Token: 0x06001474 RID: 5236 RVA: 0x000737F3 File Offset: 0x000719F3
		private void Start()
		{
			this._originalRotation = base.transform.localRotation;
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x00073808 File Offset: 0x00071A08
		private void Update()
		{
			this._rotAngle += this._currentSpeed * Time.deltaTime;
			if (this._rotAngle > 360f)
			{
				this._rotAngle = 0f;
			}
			base.transform.localRotation = this._originalRotation * Quaternion.AngleAxis(this._rotAngle, Vector3.forward);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x0007386C File Offset: 0x00071A6C
		public void SetMoveState(bool newMoveState, float goalSpeed)
		{
			this.IsMoving = newMoveState;
			if (this._lerpSpeedCoroutine != null)
			{
				base.StopCoroutine(this._lerpSpeedCoroutine);
			}
			this._lerpSpeedCoroutine = base.StartCoroutine(this.LerpToSpeed(goalSpeed));
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x0007389C File Offset: 0x00071A9C
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

		// Token: 0x06001478 RID: 5240 RVA: 0x000738B2 File Offset: 0x00071AB2
		private IEnumerator PlaySoundDelayed(AudioClip initial, AudioClip clip, float timeDelayAfterInitial)
		{
			this.PlaySound(initial, false);
			yield return new WaitForSeconds(timeDelayAfterInitial);
			this.PlaySound(clip, true);
			yield break;
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x000738D6 File Offset: 0x00071AD6
		private void PlaySound(AudioClip clip, bool loop = false)
		{
			this._audioSource.loop = loop;
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x0400172D RID: 5933
		private const float MAX_TIME = 1f;

		// Token: 0x0400172E RID: 5934
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x0400172F RID: 5935
		[SerializeField]
		private AudioClip _windMillRotationSound;

		// Token: 0x04001730 RID: 5936
		[SerializeField]
		private AudioClip _windMillStartSound;

		// Token: 0x04001731 RID: 5937
		[SerializeField]
		private AudioClip _windMillStopSound;

		// Token: 0x04001733 RID: 5939
		private float _currentSpeed;

		// Token: 0x04001734 RID: 5940
		private Coroutine _lerpSpeedCoroutine;

		// Token: 0x04001735 RID: 5941
		private Coroutine _audioChangeCr;

		// Token: 0x04001736 RID: 5942
		private Quaternion _originalRotation;

		// Token: 0x04001737 RID: 5943
		private float _rotAngle;
	}
}
