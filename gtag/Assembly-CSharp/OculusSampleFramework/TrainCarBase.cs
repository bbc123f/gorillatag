using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002ED RID: 749
	public abstract class TrainCarBase : MonoBehaviour
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x00072DE8 File Offset: 0x00070FE8
		// (set) Token: 0x06001447 RID: 5191 RVA: 0x00072DF0 File Offset: 0x00070FF0
		public float Distance { get; protected set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06001448 RID: 5192 RVA: 0x00072DF9 File Offset: 0x00070FF9
		// (set) Token: 0x06001449 RID: 5193 RVA: 0x00072E01 File Offset: 0x00071001
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

		// Token: 0x0600144A RID: 5194 RVA: 0x00072E0A File Offset: 0x0007100A
		protected virtual void Awake()
		{
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00072E0C File Offset: 0x0007100C
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

		// Token: 0x0600144C RID: 5196 RVA: 0x00072E6C File Offset: 0x0007106C
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

		// Token: 0x0600144D RID: 5197 RVA: 0x00072F84 File Offset: 0x00071184
		protected void RotateCarWheels()
		{
			float num = this.Distance / 0.027f % 6.2831855f;
			Transform[] individualWheels = this._individualWheels;
			for (int i = 0; i < individualWheels.Length; i++)
			{
				individualWheels[i].localRotation = Quaternion.AngleAxis(57.29578f * num, Vector3.right);
			}
		}

		// Token: 0x0600144E RID: 5198
		public abstract void UpdatePosition();

		// Token: 0x040016F2 RID: 5874
		private static Vector3 OFFSET = new Vector3(0f, 0.0195f, 0f);

		// Token: 0x040016F3 RID: 5875
		private const float WHEEL_RADIUS = 0.027f;

		// Token: 0x040016F4 RID: 5876
		private const float TWO_PI = 6.2831855f;

		// Token: 0x040016F5 RID: 5877
		[SerializeField]
		protected Transform _frontWheels;

		// Token: 0x040016F6 RID: 5878
		[SerializeField]
		protected Transform _rearWheels;

		// Token: 0x040016F7 RID: 5879
		[SerializeField]
		protected TrainTrack _trainTrack;

		// Token: 0x040016F8 RID: 5880
		[SerializeField]
		protected Transform[] _individualWheels;

		// Token: 0x040016FA RID: 5882
		protected float scale = 1f;

		// Token: 0x040016FB RID: 5883
		private Pose _frontPose = new Pose();

		// Token: 0x040016FC RID: 5884
		private Pose _rearPose = new Pose();
	}
}
