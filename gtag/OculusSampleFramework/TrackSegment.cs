using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class TrackSegment : MonoBehaviour
	{
		public float StartDistance { get; set; }

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

		public TrackSegment.SegmentType Type
		{
			get
			{
				return this._segmentType;
			}
		}

		public Pose EndPose
		{
			get
			{
				this.UpdatePose(this.SegmentLength, this._endPose);
				return this._endPose;
			}
		}

		public float Radius
		{
			get
			{
				return 0.5f * this.GridSize;
			}
		}

		public float setGridSize(float size)
		{
			this.GridSize = size;
			return this.GridSize / 0.8f;
		}

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

		private void Awake()
		{
		}

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
				Vector3 vector = new Vector3(this.Radius * Mathf.Cos(num2) - this.Radius, 0f, this.Radius * Mathf.Sin(num2));
				Quaternion quaternion = Quaternion.Euler(0f, -num2 * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(vector);
				pose.Rotation = base.transform.rotation * quaternion;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.RightTurn)
			{
				float num3 = 3.1415927f - 1.5707964f * distanceIntoSegment / this.SegmentLength;
				Vector3 vector2 = new Vector3(this.Radius * Mathf.Cos(num3) + this.Radius, 0f, this.Radius * Mathf.Sin(num3));
				Quaternion quaternion2 = Quaternion.Euler(0f, (3.1415927f - num3) * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(vector2);
				pose.Rotation = base.transform.rotation * quaternion2;
				return;
			}
			pose.Position = Vector3.zero;
			pose.Rotation = Quaternion.identity;
		}

		private void Update()
		{
		}

		private void OnDisable()
		{
			Object.Destroy(this._mesh);
		}

		private void DrawDebugLines()
		{
			for (int i = 1; i < this.SubDivCount + 1; i++)
			{
				float num = this.SegmentLength / (float)this.SubDivCount;
				this.UpdatePose((float)(i - 1) * num, this._p1);
				this.UpdatePose((float)i * num, this._p2);
				float num2 = 0.075f;
				Debug.DrawLine(this._p1.Position + num2 * (this._p1.Rotation * Vector3.right), this._p2.Position + num2 * (this._p2.Rotation * Vector3.right));
				Debug.DrawLine(this._p1.Position - num2 * (this._p1.Rotation * Vector3.right), this._p2.Position - num2 * (this._p2.Rotation * Vector3.right));
			}
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position + 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
		}

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

		[SerializeField]
		private TrackSegment.SegmentType _segmentType;

		[SerializeField]
		private MeshFilter _straight;

		[SerializeField]
		private MeshFilter _leftTurn;

		[SerializeField]
		private MeshFilter _rightTurn;

		private float _gridSize = 0.8f;

		private int _subDivCount = 20;

		private const float _originalGridSize = 0.8f;

		private const float _trackWidth = 0.15f;

		private GameObject _mesh;

		private Pose _p1 = new Pose();

		private Pose _p2 = new Pose();

		private Pose _endPose = new Pose();

		public enum SegmentType
		{
			Straight,
			LeftTurn,
			RightTurn,
			Switch
		}
	}
}
