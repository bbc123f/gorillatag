using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EB RID: 747
	public abstract class TrainCarBase : MonoBehaviour
	{
		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600143F RID: 5183 RVA: 0x0007291C File Offset: 0x00070B1C
		// (set) Token: 0x06001440 RID: 5184 RVA: 0x00072924 File Offset: 0x00070B24
		public float Distance { get; protected set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x0007292D File Offset: 0x00070B2D
		// (set) Token: 0x06001442 RID: 5186 RVA: 0x00072935 File Offset: 0x00070B35
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

		// Token: 0x06001443 RID: 5187 RVA: 0x0007293E File Offset: 0x00070B3E
		protected virtual void Awake()
		{
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x00072940 File Offset: 0x00070B40
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

		// Token: 0x06001445 RID: 5189 RVA: 0x000729A0 File Offset: 0x00070BA0
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

		// Token: 0x06001446 RID: 5190 RVA: 0x00072AB8 File Offset: 0x00070CB8
		protected void RotateCarWheels()
		{
			float num = this.Distance / 0.027f % 6.2831855f;
			Transform[] individualWheels = this._individualWheels;
			for (int i = 0; i < individualWheels.Length; i++)
			{
				individualWheels[i].localRotation = Quaternion.AngleAxis(57.29578f * num, Vector3.right);
			}
		}

		// Token: 0x06001447 RID: 5191
		public abstract void UpdatePosition();

		// Token: 0x040016E5 RID: 5861
		private static Vector3 OFFSET = new Vector3(0f, 0.0195f, 0f);

		// Token: 0x040016E6 RID: 5862
		private const float WHEEL_RADIUS = 0.027f;

		// Token: 0x040016E7 RID: 5863
		private const float TWO_PI = 6.2831855f;

		// Token: 0x040016E8 RID: 5864
		[SerializeField]
		protected Transform _frontWheels;

		// Token: 0x040016E9 RID: 5865
		[SerializeField]
		protected Transform _rearWheels;

		// Token: 0x040016EA RID: 5866
		[SerializeField]
		protected TrainTrack _trainTrack;

		// Token: 0x040016EB RID: 5867
		[SerializeField]
		protected Transform[] _individualWheels;

		// Token: 0x040016ED RID: 5869
		protected float scale = 1f;

		// Token: 0x040016EE RID: 5870
		private Pose _frontPose = new Pose();

		// Token: 0x040016EF RID: 5871
		private Pose _rearPose = new Pose();
	}
}
