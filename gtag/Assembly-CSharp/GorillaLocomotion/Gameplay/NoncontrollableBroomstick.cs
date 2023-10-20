using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029F RID: 671
	public class NoncontrollableBroomstick : MonoBehaviour, IPunObservable
	{
		// Token: 0x06001172 RID: 4466 RVA: 0x000625E7 File Offset: 0x000607E7
		private void Awake()
		{
			this._view = base.GetComponent<PhotonView>();
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x000625F8 File Offset: 0x000607F8
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
							this.progress %= 1f;
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

		// Token: 0x06001174 RID: 4468 RVA: 0x000627FC File Offset: 0x000609FC
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
			else
			{
				this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
				if (this.progressLerpEnd > 1f)
				{
					this.progressLerpEnd = (float)((double)this.progressLerpEnd % 1.0);
				}
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x000628F6 File Offset: 0x00060AF6
		public void OnGrabbed()
		{
			this._view.TransferOwnership(PhotonNetwork.LocalPlayer);
			this.isHeldByLocalPlayer = true;
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x0006290F File Offset: 0x00060B0F
		public void OnGrabReleased()
		{
			this.isHeldByLocalPlayer = false;
		}

		// Token: 0x0400140E RID: 5134
		public BezierSpline spline;

		// Token: 0x0400140F RID: 5135
		public float duration = 30f;

		// Token: 0x04001410 RID: 5136
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04001411 RID: 5137
		private float currentSpeedMultiplier;

		// Token: 0x04001412 RID: 5138
		public float acceleration = 1f;

		// Token: 0x04001413 RID: 5139
		public float deceleration = 1f;

		// Token: 0x04001414 RID: 5140
		private bool isHeldByLocalPlayer;

		// Token: 0x04001415 RID: 5141
		public bool lookForward = true;

		// Token: 0x04001416 RID: 5142
		public SplineWalkerMode mode;

		// Token: 0x04001417 RID: 5143
		private float progress;

		// Token: 0x04001418 RID: 5144
		private float progressLerpStart;

		// Token: 0x04001419 RID: 5145
		private float progressLerpEnd;

		// Token: 0x0400141A RID: 5146
		private const float progressLerpDuration = 1f;

		// Token: 0x0400141B RID: 5147
		private float progressLerpStartTime;

		// Token: 0x0400141C RID: 5148
		private bool goingForward = true;

		// Token: 0x0400141D RID: 5149
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400141E RID: 5150
		private PhotonView _view;
	}
}
