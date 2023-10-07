using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029D RID: 669
	public class NoncontrollableBroomstick : MonoBehaviour, IPunObservable
	{
		// Token: 0x0600116B RID: 4459 RVA: 0x000621B7 File Offset: 0x000603B7
		private void Awake()
		{
			this._view = base.GetComponent<PhotonView>();
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x000621C8 File Offset: 0x000603C8
		private void Update()
		{
			if (!this._view.IsMine && this.progressLerpStartTime + 1f > Time.time)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f);
			}
			else
			{
				if (this.isHeldByLocalPlayer)
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
				}
				else
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime);
				}
				if (this.goingForward)
				{
					this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress > 1f)
					{
						if (this.mode == SplineWalkerMode.Once)
						{
							this.progress = 1f;
						}
						else if (this.mode == SplineWalkerMode.Loop)
						{
							this.progress -= 1f;
						}
						else
						{
							this.progress = 2f - this.progress;
							this.goingForward = false;
						}
					}
				}
				else
				{
					this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress < 0f)
					{
						this.progress = -this.progress;
						this.goingForward = true;
					}
				}
			}
			Vector3 point = this.spline.GetPoint(this.progress);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress));
			}
			if (this.audioSource && this.audioSource.clip && !this.audioSource.isPlaying)
			{
				this.audioSource.PlayOneShot(this.audioSource.clip);
			}
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x000623CC File Offset: 0x000605CC
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
				return;
			}
			this.progressLerpEnd = (float)stream.ReceiveNext();
			if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
			{
				this.progressLerpEnd = 1f;
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x0006248E File Offset: 0x0006068E
		public void OnGrabbed()
		{
			this._view.TransferOwnership(PhotonNetwork.LocalPlayer);
			this.isHeldByLocalPlayer = true;
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x000624A7 File Offset: 0x000606A7
		public void OnGrabReleased()
		{
			this.isHeldByLocalPlayer = false;
		}

		// Token: 0x04001401 RID: 5121
		public BezierSpline spline;

		// Token: 0x04001402 RID: 5122
		public float duration = 30f;

		// Token: 0x04001403 RID: 5123
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04001404 RID: 5124
		private float currentSpeedMultiplier;

		// Token: 0x04001405 RID: 5125
		public float acceleration = 1f;

		// Token: 0x04001406 RID: 5126
		public float deceleration = 1f;

		// Token: 0x04001407 RID: 5127
		private bool isHeldByLocalPlayer;

		// Token: 0x04001408 RID: 5128
		public bool lookForward = true;

		// Token: 0x04001409 RID: 5129
		public SplineWalkerMode mode;

		// Token: 0x0400140A RID: 5130
		private float progress;

		// Token: 0x0400140B RID: 5131
		private float progressLerpStart;

		// Token: 0x0400140C RID: 5132
		private float progressLerpEnd;

		// Token: 0x0400140D RID: 5133
		private const float progressLerpDuration = 1f;

		// Token: 0x0400140E RID: 5134
		private float progressLerpStartTime;

		// Token: 0x0400140F RID: 5135
		private bool goingForward = true;

		// Token: 0x04001410 RID: 5136
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04001411 RID: 5137
		private PhotonView _view;
	}
}
