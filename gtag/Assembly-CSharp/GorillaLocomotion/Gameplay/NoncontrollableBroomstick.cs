using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	public class NoncontrollableBroomstick : MonoBehaviourPun, IGorillaGrabable, IPunObservable
	{
		protected virtual void Awake()
		{
			this._view = base.GetComponent<PhotonView>();
			this.progress = this.SplineProgressOffet % 1f;
		}

		protected virtual void Update()
		{
			if (!base.photonView.IsMine && this.progressLerpStartTime + 1f > Time.time)
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
			Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
			}
		}

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

		protected float GetProgress()
		{
			return this.progress;
		}

		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		void IGorillaGrabable.OnGrabbed()
		{
			base.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
			this.isHeldByLocalPlayer = true;
		}

		void IGorillaGrabable.OnGrabReleased()
		{
			this.isHeldByLocalPlayer = false;
		}

		public BezierSpline spline;

		public float duration = 30f;

		public float speedMultiplierWhileHeld = 2f;

		private float currentSpeedMultiplier;

		public float acceleration = 1f;

		public float deceleration = 1f;

		private bool isHeldByLocalPlayer;

		public bool lookForward = true;

		public SplineWalkerMode mode;

		[SerializeField]
		private float SplineProgressOffet;

		private float progress;

		private float progressLerpStart;

		private float progressLerpEnd;

		private const float progressLerpDuration = 1f;

		private float progressLerpStartTime;

		private bool goingForward = true;

		private PhotonView _view;

		[SerializeField]
		private bool constantVelocity;
	}
}
