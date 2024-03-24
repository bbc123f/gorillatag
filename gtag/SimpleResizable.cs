using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleResizable : MonoBehaviour
{
	public Vector3 PivotPosition
	{
		get
		{
			return this._pivotTransform.position;
		}
	}

	public Vector3 DefaultSize
	{
		[CompilerGenerated]
		get
		{
			return this.<DefaultSize>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<DefaultSize>k__BackingField = value;
		}
	}

	public Mesh OriginalMesh
	{
		[CompilerGenerated]
		get
		{
			return this.<OriginalMesh>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<OriginalMesh>k__BackingField = value;
		}
	}

	public void SetNewSize(Vector3 newSize)
	{
		this._newSize = newSize;
	}

	private void Awake()
	{
		this._meshFilter = base.GetComponent<MeshFilter>();
		this.OriginalMesh = base.GetComponent<MeshFilter>().sharedMesh;
		this.DefaultSize = this.OriginalMesh.bounds.size;
		this._newSize = this.DefaultSize;
		this._oldSize = this._newSize;
		if (!this._pivotTransform)
		{
			this._pivotTransform = base.transform.Find("Pivot");
		}
	}

	private void OnEnable()
	{
		this.DefaultSize = this.OriginalMesh.bounds.size;
		if (this._newSize == Vector3.zero)
		{
			this._newSize = this.DefaultSize;
		}
	}

	private void Update()
	{
		if (Application.isPlaying && !this._updateInPlayMode)
		{
			return;
		}
		if (this._newSize != this._oldSize)
		{
			this._oldSize = this._newSize;
			Mesh mesh = SimpleResizer.ProcessVertices(this, this._newSize, true);
			this._meshFilter.sharedMesh = mesh;
			this._meshFilter.sharedMesh.RecalculateBounds();
		}
	}

	private void OnDrawGizmos()
	{
		if (!this._pivotTransform)
		{
			return;
		}
		Gizmos.color = Color.red;
		float num = 0.1f;
		Vector3 vector = this._pivotTransform.position + Vector3.left * num * 0.5f;
		Vector3 vector2 = this._pivotTransform.position + Vector3.down * num * 0.5f;
		Vector3 vector3 = this._pivotTransform.position + Vector3.back * num * 0.5f;
		Gizmos.DrawRay(vector, Vector3.right * num);
		Gizmos.DrawRay(vector2, Vector3.up * num);
		Gizmos.DrawRay(vector3, Vector3.forward * num);
	}

	private void OnDrawGizmosSelected()
	{
		if (this._meshFilter.sharedMesh == null)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 center = this._meshFilter.sharedMesh.bounds.center;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		switch (this.ScalingX)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x * this.PaddingX * 2f, this._newSize.y, this._newSize.z));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(this._newSize.x * this.PaddingX, 0f, 0f), new Vector3(0f, this._newSize.y, this._newSize.z));
			Gizmos.DrawWireCube(center + new Vector3(this._newSize.x * this.PaddingXMax, 0f, 0f), new Vector3(0f, this._newSize.y, this._newSize.z));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		switch (this.ScalingY)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x, this._newSize.y * this.PaddingY * 2f, this._newSize.z));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(0f, this._newSize.y * this.PaddingY, 0f), new Vector3(this._newSize.x, 0f, this._newSize.z));
			Gizmos.DrawWireCube(center + new Vector3(0f, this._newSize.y * this.PaddingYMax, 0f), new Vector3(this._newSize.x, 0f, this._newSize.z));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		switch (this.ScalingZ)
		{
		case SimpleResizable.Method.Adapt:
			Gizmos.DrawWireCube(center, new Vector3(this._newSize.x, this._newSize.y, this._newSize.z * this.PaddingZ * 2f));
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			Gizmos.DrawWireCube(center + new Vector3(0f, 0f, this._newSize.z * this.PaddingZ), new Vector3(this._newSize.x, this._newSize.y, 0f));
			Gizmos.DrawWireCube(center + new Vector3(0f, 0f, this._newSize.z * this.PaddingZMax), new Vector3(this._newSize.x, this._newSize.y, 0f));
			break;
		case SimpleResizable.Method.None:
			Gizmos.DrawWireCube(center, this._newSize);
			break;
		}
		Gizmos.color = new Color(0f, 1f, 1f, 1f);
		Gizmos.DrawWireCube(center, this._newSize);
	}

	public SimpleResizable()
	{
	}

	[Space(15f)]
	public SimpleResizable.Method ScalingX;

	[Range(0f, 0.5f)]
	public float PaddingX;

	[Range(-0.5f, 0f)]
	public float PaddingXMax;

	[Space(15f)]
	public SimpleResizable.Method ScalingY;

	[Range(0f, 0.5f)]
	public float PaddingY;

	[Range(-0.5f, 0f)]
	public float PaddingYMax;

	[Space(15f)]
	public SimpleResizable.Method ScalingZ;

	[Range(0f, 0.5f)]
	public float PaddingZ;

	[Range(-0.5f, 0f)]
	public float PaddingZMax;

	[CompilerGenerated]
	private Vector3 <DefaultSize>k__BackingField;

	[CompilerGenerated]
	private Mesh <OriginalMesh>k__BackingField;

	private Vector3 _oldSize;

	private MeshFilter _meshFilter;

	[SerializeField]
	private Vector3 _newSize;

	[SerializeField]
	private bool _updateInPlayMode;

	[SerializeField]
	private Transform _pivotTransform;

	public enum Method
	{
		Adapt,
		AdaptWithAsymmetricalPadding,
		Scale,
		None
	}
}
