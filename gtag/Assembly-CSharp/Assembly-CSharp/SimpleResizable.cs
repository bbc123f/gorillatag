using System;
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

	public Vector3 NewSize { get; set; }

	public Vector3 DefaultSize { get; private set; }

	public Mesh Mesh { get; private set; }

	private void Awake()
	{
		this.Mesh = base.GetComponent<MeshFilter>().sharedMesh;
		this.DefaultSize = this.Mesh.bounds.size;
		if (!this._pivotTransform)
		{
			this._pivotTransform = base.transform.Find("Pivot");
		}
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

	private Bounds _bounds;

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
