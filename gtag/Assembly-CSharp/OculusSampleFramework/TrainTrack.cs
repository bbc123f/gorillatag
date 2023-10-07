using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EE RID: 750
	public class TrainTrack : MonoBehaviour
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x0007310C File Offset: 0x0007130C
		// (set) Token: 0x06001463 RID: 5219 RVA: 0x00073114 File Offset: 0x00071314
		public float TrackLength
		{
			get
			{
				return this._trainLength;
			}
			private set
			{
				this._trainLength = value;
			}
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x0007311D File Offset: 0x0007131D
		private void Awake()
		{
			this.Regenerate();
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00073128 File Offset: 0x00071328
		public TrackSegment GetSegment(float distance)
		{
			int childCount = this._segmentParent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				TrackSegment trackSegment = this._trackSegments[i];
				TrackSegment trackSegment2 = this._trackSegments[(i + 1) % childCount];
				if (distance >= trackSegment.StartDistance && (distance < trackSegment2.StartDistance || i == childCount - 1))
				{
					return trackSegment;
				}
			}
			return null;
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x00073180 File Offset: 0x00071380
		public void Regenerate()
		{
			this._trackSegments = this._segmentParent.GetComponentsInChildren<TrackSegment>();
			this.TrackLength = 0f;
			int childCount = this._segmentParent.childCount;
			TrackSegment trackSegment = null;
			float scale = 0f;
			for (int i = 0; i < childCount; i++)
			{
				TrackSegment trackSegment2 = this._trackSegments[i];
				trackSegment2.SubDivCount = this._subDivCount;
				scale = trackSegment2.setGridSize(this._gridSize);
				if (trackSegment != null)
				{
					Pose endPose = trackSegment.EndPose;
					trackSegment2.transform.position = endPose.Position;
					trackSegment2.transform.rotation = endPose.Rotation;
					trackSegment2.StartDistance = this.TrackLength;
				}
				if (this._regnerateTrackMeshOnAwake)
				{
					trackSegment2.RegenerateTrackAndMesh();
				}
				this.TrackLength += trackSegment2.SegmentLength;
				trackSegment = trackSegment2;
			}
			this.SetScale(scale);
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00073268 File Offset: 0x00071468
		private void SetScale(float ratio)
		{
			this._trainParent.localScale = new Vector3(ratio, ratio, ratio);
			TrainCar[] componentsInChildren = this._trainParent.GetComponentsInChildren<TrainCar>();
			this._trainParent.GetComponentInChildren<TrainLocomotive>().Scale = ratio;
			TrainCar[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Scale = ratio;
			}
		}

		// Token: 0x04001717 RID: 5911
		[SerializeField]
		private float _gridSize = 0.5f;

		// Token: 0x04001718 RID: 5912
		[SerializeField]
		private int _subDivCount = 20;

		// Token: 0x04001719 RID: 5913
		[SerializeField]
		private Transform _segmentParent;

		// Token: 0x0400171A RID: 5914
		[SerializeField]
		private Transform _trainParent;

		// Token: 0x0400171B RID: 5915
		[SerializeField]
		private bool _regnerateTrackMeshOnAwake;

		// Token: 0x0400171C RID: 5916
		private float _trainLength = -1f;

		// Token: 0x0400171D RID: 5917
		private TrackSegment[] _trackSegments;
	}
}
