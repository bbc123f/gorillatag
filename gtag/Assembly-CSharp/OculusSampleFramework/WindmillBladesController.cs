using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002F0 RID: 752
	public class WindmillBladesController : MonoBehaviour
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x0600146B RID: 5227 RVA: 0x00073316 File Offset: 0x00071516
		// (set) Token: 0x0600146C RID: 5228 RVA: 0x0007331E File Offset: 0x0007151E
		public bool IsMoving { get; private set; }

		// Token: 0x0600146D RID: 5229 RVA: 0x00073327 File Offset: 0x00071527
		private void Start()
		{
			this._originalRotation = base.transform.localRotation;
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0007333C File Offset: 0x0007153C
		private void Update()
		{
			this._rotAngle += this._currentSpeed * Time.deltaTime;
			if (this._rotAngle > 360f)
			{
				this._rotAngle = 0f;
			}
			base.transform.localRotation = this._originalRotation * Quaternion.AngleAxis(this._rotAngle, Vector3.forward);
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x000733A0 File Offset: 0x000715A0
		public void SetMoveState(bool newMoveState, float goalSpeed)
		{
			this.IsMoving = newMoveState;
			if (this._lerpSpeedCoroutine != null)
			{
				base.StopCoroutine(this._lerpSpeedCoroutine);
			}
			this._lerpSpeedCoroutine = base.StartCoroutine(this.LerpToSpeed(goalSpeed));
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x000733D0 File Offset: 0x000715D0
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

		// Token: 0x06001471 RID: 5233 RVA: 0x000733E6 File Offset: 0x000715E6
		private IEnumerator PlaySoundDelayed(AudioClip initial, AudioClip clip, float timeDelayAfterInitial)
		{
			this.PlaySound(initial, false);
			yield return new WaitForSeconds(timeDelayAfterInitial);
			this.PlaySound(clip, true);
			yield break;
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x0007340A File Offset: 0x0007160A
		private void PlaySound(AudioClip clip, bool loop = false)
		{
			this._audioSource.loop = loop;
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x04001720 RID: 5920
		private const float MAX_TIME = 1f;

		// Token: 0x04001721 RID: 5921
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04001722 RID: 5922
		[SerializeField]
		private AudioClip _windMillRotationSound;

		// Token: 0x04001723 RID: 5923
		[SerializeField]
		private AudioClip _windMillStartSound;

		// Token: 0x04001724 RID: 5924
		[SerializeField]
		private AudioClip _windMillStopSound;

		// Token: 0x04001726 RID: 5926
		private float _currentSpeed;

		// Token: 0x04001727 RID: 5927
		private Coroutine _lerpSpeedCoroutine;

		// Token: 0x04001728 RID: 5928
		private Coroutine _audioChangeCr;

		// Token: 0x04001729 RID: 5929
		private Quaternion _originalRotation;

		// Token: 0x0400172A RID: 5930
		private float _rotAngle;
	}
}
