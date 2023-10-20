using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EA RID: 746
	public class TrackSegment : MonoBehaviour
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x0007230F File Offset: 0x0007050F
		// (set) Token: 0x06001426 RID: 5158 RVA: 0x00072317 File Offset: 0x00070517
		public float StartDistance { get; set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x00072320 File Offset: 0x00070520
		// (set) Token: 0x06001428 RID: 5160 RVA: 0x00072328 File Offset: 0x00070528
		public float GridSize
		{
			get
			{
				return this._gridSize;
			}
			private set
			{
				this._gridSize = value;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x00072331 File Offset: 0x00070531
		// (set) Token: 0x0600142A RID: 5162 RVA: 0x00072339 File Offset: 0x00070539
		public int SubDivCount
		{
			get
			{
				return this._subDivCount;
			}
			set
			{
				this._subDivCount = value;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600142B RID: 5163 RVA: 0x00072342 File Offset: 0x00070542
		public TrackSegment.SegmentType Type
		{
			get
			{
				return this._segmentType;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x0007234A File Offset: 0x0007054A
		public Pose EndPose
		{
			get
			{
				this.UpdatePose(this.SegmentLength, this._endPose);
				return this._endPose;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600142D RID: 5165 RVA: 0x00072364 File Offset: 0x00070564
		public float Radius
		{
			get
			{
				return 0.5f * this.GridSize;
			}
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x00072372 File Offset: 0x00070572
		public float setGridSize(float size)
		{
			this.GridSize = size;
			return this.GridSize / 0.8f;
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600142F RID: 5167 RVA: 0x00072388 File Offset: 0x00070588
		public float SegmentLength
		{
			get
			{
				TrackSegment.SegmentType type = this.Type;
				if (type == TrackSegment.SegmentType.Straight)
				{
					return this.GridSize;
				}
				if (type - TrackSegment.SegmentType.LeftTurn > 1)
				{
					return 1f;
				}
				return 1.5707964f * this.Radius;
			}
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x000723C0 File Offset: 0x000705C0
		private void Awake()
		{
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x000723C4 File Offset: 0x000705C4
		public void UpdatePose(float distanceIntoSegment, Pose pose)
		{
			if (this.Type == TrackSegment.SegmentType.Straight)
			{
				pose.Position = base.transform.position + distanceIntoSegment * base.transform.forward;
				pose.Rotation = base.transform.rotation;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.LeftTurn)
			{
				float num = distanceIntoSegment / this.SegmentLength;
				float num2 = 1.5707964f * num;
				Vector3 position = new Vector3(this.Radius * Mathf.Cos(num2) - this.Radius, 0f, this.Radius * Mathf.Sin(num2));
				Quaternion rhs = Quaternion.Euler(0f, -num2 * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(position);
				pose.Rotation = base.transform.rotation * rhs;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.RightTurn)
			{
				float num3 = 3.1415927f - 1.5707964f * distanceIntoSegment / this.SegmentLength;
				Vector3 position2 = new Vector3(this.Radius * Mathf.Cos(num3) + this.Radius, 0f, this.Radius * Mathf.Sin(num3));
				Quaternion rhs2 = Quaternion.Euler(0f, (3.1415927f - num3) * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(position2);
				pose.Rotation = base.transform.rotation * rhs2;
				return;
			}
			pose.Position = Vector3.zero;
			pose.Rotation = Quaternion.identity;
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00072550 File Offset: 0x00070750
		private void Update()
		{
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00072552 File Offset: 0x00070752
		private void OnDisable()
		{
			Object.Destroy(this._mesh);
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00072560 File Offset: 0x00070760
		private void DrawDebugLines()
		{
			for (int i = 1; i < this.SubDivCount + 1; i++)
			{
				float num = this.SegmentLength / (float)this.SubDivCount;
				this.UpdatePose((float)(i - 1) * num, this._p1);
				this.UpdatePose((float)i * num, this._p2);
				float d = 0.075f;
				Debug.DrawLine(this._p1.Position + d * (this._p1.Rotation * Vector3.right), this._p2.Position + d * (this._p2.Rotation * Vector3.right));
				Debug.DrawLine(this._p1.Position - d * (this._p1.Rotation * Vector3.right), this._p2.Position - d * (this._p2.Rotation * Vector3.right));
			}
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position + 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x00072868 File Offset: 0x00070A68
		public void RegenerateTrackAndMesh()
		{
			if (base.transform.childCount > 0 && !this._mesh)
			{
				this._mesh = base.transform.GetChild(0).gameObject;
			}
			if (this._mesh)
			{
				Object.DestroyImmediate(this._mesh);
			}
			if (this._segmentType == TrackSegment.SegmentType.LeftTurn)
			{
				this._mesh = Object.Instantiate<GameObject>(this._leftTurn.gameObject);
			}
			else if (this._segmentType == TrackSegment.SegmentType.RightTurn)
			{
				this._mesh = Object.Instantiate<GameObject>(this._rightTurn.gameObject);
			}
			else
			{
				this._mesh = Object.Instantiate<GameObject>(this._straight.gameObject);
			}
			this._mesh.transform.SetParent(base.transform, false);
			this._mesh.transform.position += this.GridSize / 2f * base.transform.forward;
			this._mesh.transform.localScale = new Vector3(this.GridSize / 0.8f, this.GridSize / 0.8f, this.GridSize / 0.8f);
		}

		// Token: 0x040016D2 RID: 5842
		[SerializeField]
		private TrackSegment.SegmentType _segmentType;

		// Token: 0x040016D3 RID: 5843
		[SerializeField]
		private MeshFilter _straight;

		// Token: 0x040016D4 RID: 5844
		[SerializeField]
		private MeshFilter _leftTurn;

		// Token: 0x040016D5 RID: 5845
		[SerializeField]
		private MeshFilter _rightTurn;

		// Token: 0x040016D6 RID: 5846
		private float _gridSize = 0.8f;

		// Token: 0x040016D7 RID: 5847
		private int _subDivCount = 20;

		// Token: 0x040016D8 RID: 5848
		private const float _originalGridSize = 0.8f;

		// Token: 0x040016D9 RID: 5849
		private const float _trackWidth = 0.15f;

		// Token: 0x040016DA RID: 5850
		private GameObject _mesh;

		// Token: 0x040016DC RID: 5852
		private Pose _p1 = new Pose();

		// Token: 0x040016DD RID: 5853
		private Pose _p2 = new Pose();

		// Token: 0x040016DE RID: 5854
		private Pose _endPose = new Pose();

		// Token: 0x020004F0 RID: 1264
		public enum SegmentType
		{
			// Token: 0x04002092 RID: 8338
			Straight,
			// Token: 0x04002093 RID: 8339
			LeftTurn,
			// Token: 0x04002094 RID: 8340
			RightTurn,
			// Token: 0x04002095 RID: 8341
			Switch
		}
	}
}
