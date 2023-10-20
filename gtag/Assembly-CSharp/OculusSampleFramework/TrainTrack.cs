using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002F0 RID: 752
	public class TrainTrack : MonoBehaviour
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06001469 RID: 5225 RVA: 0x000735D8 File Offset: 0x000717D8
		// (set) Token: 0x0600146A RID: 5226 RVA: 0x000735E0 File Offset: 0x000717E0
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

		// Token: 0x0600146B RID: 5227 RVA: 0x000735E9 File Offset: 0x000717E9
		private void Awake()
		{
			this.Regenerate();
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x000735F4 File Offset: 0x000717F4
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

		// Token: 0x0600146D RID: 5229 RVA: 0x0007364C File Offset: 0x0007184C
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

		// Token: 0x0600146E RID: 5230 RVA: 0x00073734 File Offset: 0x00071934
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

		// Token: 0x04001724 RID: 5924
		[SerializeField]
		private float _gridSize = 0.5f;

		// Token: 0x04001725 RID: 5925
		[SerializeField]
		private int _subDivCount = 20;

		// Token: 0x04001726 RID: 5926
		[SerializeField]
		private Transform _segmentParent;

		// Token: 0x04001727 RID: 5927
		[SerializeField]
		private Transform _trainParent;

		// Token: 0x04001728 RID: 5928
		[SerializeField]
		private bool _regnerateTrackMeshOnAwake;

		// Token: 0x04001729 RID: 5929
		private float _trainLength = -1f;

		// Token: 0x0400172A RID: 5930
		private TrackSegment[] _trackSegments;
	}
}
