using System;
using UnityEngine;

// Token: 0x020000C2 RID: 194
[ExecuteInEditMode]
public class SimpleResizable : MonoBehaviour
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000439 RID: 1081 RVA: 0x0001B831 File Offset: 0x00019A31
	public Vector3 PivotPosition
	{
		get
		{
			return this._pivotTransform.position;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600043A RID: 1082 RVA: 0x0001B83E File Offset: 0x00019A3E
	// (set) Token: 0x0600043B RID: 1083 RVA: 0x0001B846 File Offset: 0x00019A46
	public Vector3 NewSize { get; set; }

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600043C RID: 1084 RVA: 0x0001B84F File Offset: 0x00019A4F
	// (set) Token: 0x0600043D RID: 1085 RVA: 0x0001B857 File Offset: 0x00019A57
	public Vector3 DefaultSize { get; private set; }

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x0600043E RID: 1086 RVA: 0x0001B860 File Offset: 0x00019A60
	// (set) Token: 0x0600043F RID: 1087 RVA: 0x0001B868 File Offset: 0x00019A68
	public Mesh Mesh { get; private set; }

	// Token: 0x06000440 RID: 1088 RVA: 0x0001B874 File Offset: 0x00019A74
	private void Awake()
	{
		this.Mesh = base.GetComponent<MeshFilter>().sharedMesh;
		this.DefaultSize = this.Mesh.bounds.size;
		if (!this._pivotTransform)
		{
			this._pivotTransform = base.transform.Find("Pivot");
		}
	}

	// Token: 0x040004E6 RID: 1254
	[Space(15f)]
	public SimpleResizable.Method ScalingX;

	// Token: 0x040004E7 RID: 1255
	[Range(0f, 0.5f)]
	public float PaddingX;

	// Token: 0x040004E8 RID: 1256
	[Range(-0.5f, 0f)]
	public float PaddingXMax;

	// Token: 0x040004E9 RID: 1257
	[Space(15f)]
	public SimpleResizable.Method ScalingY;

	// Token: 0x040004EA RID: 1258
	[Range(0f, 0.5f)]
	public float PaddingY;

	// Token: 0x040004EB RID: 1259
	[Range(-0.5f, 0f)]
	public float PaddingYMax;

	// Token: 0x040004EC RID: 1260
	[Space(15f)]
	public SimpleResizable.Method ScalingZ;

	// Token: 0x040004ED RID: 1261
	[Range(0f, 0.5f)]
	public float PaddingZ;

	// Token: 0x040004EE RID: 1262
	[Range(-0.5f, 0f)]
	public float PaddingZMax;

	// Token: 0x040004F2 RID: 1266
	private Bounds _bounds;

	// Token: 0x040004F3 RID: 1267
	[SerializeField]
	private Transform _pivotTransform;

	// Token: 0x020003DB RID: 987
	public enum Method
	{
		// Token: 0x04001C41 RID: 7233
		Adapt,
		// Token: 0x04001C42 RID: 7234
		AdaptWithAsymmetricalPadding,
		// Token: 0x04001C43 RID: 7235
		Scale,
		// Token: 0x04001C44 RID: 7236
		None
	}
}
