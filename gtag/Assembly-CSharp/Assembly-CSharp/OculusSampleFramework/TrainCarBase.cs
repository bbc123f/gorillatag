using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public abstract class TrainCarBase : MonoBehaviour
	{
		public float Distance { get; protected set; }

		public float Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		protected virtual void Awake()
		{
		}

		public void UpdatePose(float distance, TrainCarBase train, Pose pose)
		{
			distance = (train._trainTrack.TrackLength + distance) % train._trainTrack.TrackLength;
			if (distance < 0f)
			{
				distance += train._trainTrack.TrackLength;
			}
			TrackSegment segment = train._trainTrack.GetSegment(distance);
			float distanceIntoSegment = distance - segment.StartDistance;
			segment.UpdatePose(distanceIntoSegment, pose);
		}

		protected void UpdateCarPosition()
		{
			this.UpdatePose(this.Distance + this._frontWheels.transform.localPosition.z * this.scale, this, this._frontPose);
			this.UpdatePose(this.Distance + this._rearWheels.transform.localPosition.z * this.scale, this, this._rearPose);
			Vector3 a = 0.5f * (this._frontPose.Position + this._rearPose.Position);
			Vector3 forward = this._frontPose.Position - this._rearPose.Position;
			base.transform.position = a + TrainCarBase.OFFSET;
			base.transform.rotation = Quaternion.LookRotation(forward, base.transform.up);
			this._frontWheels.transform.rotation = this._frontPose.Rotation;
			this._rearWheels.transform.rotation = this._rearPose.Rotation;
		}

		protected void RotateCarWheels()
		{
			float num = this.Distance / 0.027f % 6.2831855f;
			Transform[] individualWheels = this._individualWheels;
			for (int i = 0; i < individualWheels.Length; i++)
			{
				individualWheels[i].localRotation = Quaternion.AngleAxis(57.29578f * num, Vector3.right);
			}
		}

		public abstract void UpdatePosition();

		private static Vector3 OFFSET = new Vector3(0f, 0.0195f, 0f);

		private const float WHEEL_RADIUS = 0.027f;

		private const float TWO_PI = 6.2831855f;

		[SerializeField]
		protected Transform _frontWheels;

		[SerializeField]
		protected Transform _rearWheels;

		[SerializeField]
		protected TrainTrack _trainTrack;

		[SerializeField]
		protected Transform[] _individualWheels;

		protected float scale = 1f;

		private Pose _frontPose = new Pose();

		private Pose _rearPose = new Pose();
	}
}
