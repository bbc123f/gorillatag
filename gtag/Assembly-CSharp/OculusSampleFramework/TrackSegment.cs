using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E8 RID: 744
	public class TrackSegment : MonoBehaviour
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600141E RID: 5150 RVA: 0x00071E43 File Offset: 0x00070043
		// (set) Token: 0x0600141F RID: 5151 RVA: 0x00071E4B File Offset: 0x0007004B
		public float StartDistance { get; set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06001420 RID: 5152 RVA: 0x00071E54 File Offset: 0x00070054
		// (set) Token: 0x06001421 RID: 5153 RVA: 0x00071E5C File Offset: 0x0007005C
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

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06001422 RID: 5154 RVA: 0x00071E65 File Offset: 0x00070065
		// (set) Token: 0x06001423 RID: 5155 RVA: 0x00071E6D File Offset: 0x0007006D
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

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001424 RID: 5156 RVA: 0x00071E76 File Offset: 0x00070076
		public TrackSegment.SegmentType Type
		{
			get
			{
				return this._segmentType;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x00071E7E File Offset: 0x0007007E
		public Pose EndPose
		{
			get
			{
				this.UpdatePose(this.SegmentLength, this._endPose);
				return this._endPose;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x00071E98 File Offset: 0x00070098
		public float Radius
		{
			get
			{
				return 0.5f * this.GridSize;
			}
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x00071EA6 File Offset: 0x000700A6
		public float setGridSize(float size)
		{
			this.GridSize = size;
			return this.GridSize / 0.8f;
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x00071EBC File Offset: 0x000700BC
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

		// Token: 0x06001429 RID: 5161 RVA: 0x00071EF4 File Offset: 0x000700F4
		private void Awake()
		{
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x00071EF8 File Offset: 0x000700F8
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

		// Token: 0x0600142B RID: 5163 RVA: 0x00072084 File Offset: 0x00070284
		private void Update()
		{
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x00072086 File Offset: 0x00070286
		private void OnDisable()
		{
			Object.Destroy(this._mesh);
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x00072094 File Offset: 0x00070294
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

		// Token: 0x0600142E RID: 5166 RVA: 0x0007239C File Offset: 0x0007059C
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

		// Token: 0x040016C5 RID: 5829
		[SerializeField]
		private TrackSegment.SegmentType _segmentType;

		// Token: 0x040016C6 RID: 5830
		[SerializeField]
		private MeshFilter _straight;

		// Token: 0x040016C7 RID: 5831
		[SerializeField]
		private MeshFilter _leftTurn;

		// Token: 0x040016C8 RID: 5832
		[SerializeField]
		private MeshFilter _rightTurn;

		// Token: 0x040016C9 RID: 5833
		private float _gridSize = 0.8f;

		// Token: 0x040016CA RID: 5834
		private int _subDivCount = 20;

		// Token: 0x040016CB RID: 5835
		private const float _originalGridSize = 0.8f;

		// Token: 0x040016CC RID: 5836
		private const float _trackWidth = 0.15f;

		// Token: 0x040016CD RID: 5837
		private GameObject _mesh;

		// Token: 0x040016CF RID: 5839
		private Pose _p1 = new Pose();

		// Token: 0x040016D0 RID: 5840
		private Pose _p2 = new Pose();

		// Token: 0x040016D1 RID: 5841
		private Pose _endPose = new Pose();

		// Token: 0x020004EE RID: 1262
		public enum SegmentType
		{
			// Token: 0x04002085 RID: 8325
			Straight,
			// Token: 0x04002086 RID: 8326
			LeftTurn,
			// Token: 0x04002087 RID: 8327
			RightTurn,
			// Token: 0x04002088 RID: 8328
			Switch
		}
	}
}
