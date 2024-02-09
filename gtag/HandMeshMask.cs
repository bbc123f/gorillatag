using System;
using UnityEngine;

public class HandMeshMask : MonoBehaviour
{
	private void Awake()
	{
		base.transform.position = Vector3.zero;
		this.maskMesh = new Mesh();
		this.maskMeshObject = new GameObject("MeshMask");
		this.maskMeshObject.transform.parent = base.transform;
		this.maskMeshObject.transform.localPosition = Vector3.zero;
		this.maskMeshObject.AddComponent<MeshFilter>();
		this.maskMeshObject.AddComponent<MeshRenderer>();
		this.maskMeshObject.GetComponent<MeshFilter>().mesh = this.maskMesh;
		this.maskMeshObject.GetComponent<MeshRenderer>().material = this.maskMaterial;
	}

	private void Update()
	{
		this.radialDivisions = Mathf.Max(2, this.radialDivisions);
		if (this.referenceHand)
		{
			this.handScale = this.referenceHand.GetComponent<OVRHand>().HandScale;
			this.CreateHandMesh();
		}
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.Hands || OVRInput.GetActiveController() == OVRInput.Controller.LHand || OVRInput.GetActiveController() == OVRInput.Controller.RHand;
		this.maskMeshObject.SetActive(flag);
	}

	private void CreateHandMesh()
	{
		int num = 8 + (this.radialDivisions - 2) * 2;
		int num2 = 25;
		int num3 = 12;
		int num4 = 66;
		this.handVertices = new Vector3[num * num2 + num3];
		this.handUVs = new Vector2[this.handVertices.Length];
		this.handColors = new Color32[this.handVertices.Length];
		this.handTriangles = new int[this.handVertices.Length * 3 + num4];
		this.vertCounter = 0;
		this.triCounter = 0;
		for (int i = 0; i < 5; i++)
		{
			int num5 = ((i < 4) ? 0 : 1);
			int num6 = 3 + i * 3 + num5;
			int num7 = 19 + i;
			float num8 = 1f;
			float num9 = 1f - this.fingerTaper;
			float num10 = 1f - this.fingerTaper * 2f;
			float num11 = 1f - this.fingerTaper * 3f;
			if (i == 0)
			{
				num8 *= 1.2f;
				num9 *= 1.1f;
			}
			this.AddKnuckleMesh(num, num8, num9, this.referenceHand.Bones[num6].Transform.position, this.referenceHand.Bones[num6 + 1].Transform.position);
			this.AddKnuckleMesh(num, num9, num10, this.referenceHand.Bones[num6 + 1].Transform.position, this.referenceHand.Bones[num6 + 2].Transform.position);
			Vector3 position = this.referenceHand.Bones[num6 + 2].Transform.position;
			Vector3 vector = (this.referenceHand.Bones[num7].Transform.position - position) * this.fingerTipLength + position;
			this.AddKnuckleMesh(num, num10, num11, position, vector);
		}
		this.AddPalmMesh(num);
		this.maskMesh.Clear();
		this.maskMesh.name = "HandMeshMask";
		this.maskMesh.vertices = this.handVertices;
		this.maskMesh.uv = this.handUVs;
		this.maskMesh.colors32 = this.handColors;
		this.maskMesh.triangles = this.handTriangles;
	}

	private void AddKnuckleMesh(int knuckleVerts, float point1scale, float point2scale, Vector3 point1, Vector3 point2)
	{
		int num = this.vertCounter;
		Vector3 position = Camera.main.transform.position;
		Vector3 vector = (point1 + point2) * 0.5f - position;
		Vector3 vector2 = point2 - point1;
		Vector3.OrthoNormalize(ref vector, ref vector2);
		Vector3 vector3 = Vector3.Cross(vector2, vector);
		this.AddVertex(point2, Vector2.zero, Color.black);
		this.AddVertex(point1, Vector2.zero, Color.black);
		int num2 = this.radialDivisions + 1;
		Vector3 vector4 = vector3;
		for (int i = 0; i < num2 * 2; i++)
		{
			int num3 = i / num2 + num;
			Vector3 vector5 = this.handVertices[num3] + vector4 * this.borderSize * this.handScale * ((num3 != num) ? point1scale : point2scale);
			this.AddVertex(vector5, new Vector2(1f, 0f), Color.black);
			if (i != this.radialDivisions)
			{
				vector4 = Quaternion.AngleAxis(180f / (float)this.radialDivisions, vector) * vector4;
			}
		}
		int[] array = this.handTriangles;
		int num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array[num4] = num;
		int[] array2 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array2[num4] = num + 1;
		int[] array3 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array3[num4] = num + this.radialDivisions + 3;
		int[] array4 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array4[num4] = num;
		int[] array5 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array5[num4] = num + this.radialDivisions + 3;
		int[] array6 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array6[num4] = num + this.radialDivisions + 2;
		int[] array7 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array7[num4] = num + 2;
		int[] array8 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array8[num4] = num + knuckleVerts - 1;
		int[] array9 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array9[num4] = num;
		int[] array10 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array10[num4] = num;
		int[] array11 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array11[num4] = num + knuckleVerts - 1;
		int[] array12 = this.handTriangles;
		num4 = this.triCounter;
		this.triCounter = num4 + 1;
		array12[num4] = num + 1;
		for (int j = 0; j < this.radialDivisions; j++)
		{
			int[] array13 = this.handTriangles;
			num4 = this.triCounter;
			this.triCounter = num4 + 1;
			array13[num4] = num + 2 + j;
			int[] array14 = this.handTriangles;
			num4 = this.triCounter;
			this.triCounter = num4 + 1;
			array14[num4] = num;
			int[] array15 = this.handTriangles;
			num4 = this.triCounter;
			this.triCounter = num4 + 1;
			array15[num4] = num + 3 + j;
		}
		for (int k = 0; k < this.radialDivisions; k++)
		{
			int[] array16 = this.handTriangles;
			num4 = this.triCounter;
			this.triCounter = num4 + 1;
			array16[num4] = num + num2 + 2 + k;
			int[] array17 = this.handTriangles;
			num4 = this.triCounter;
			this.triCounter = num4 + 1;
			array17[num4] = num + 1;
			int[] array18 = this.handTriangles;
			num4 = this.triCounter;
			this.triCounter = num4 + 1;
			array18[num4] = num + num2 + 3 + k;
		}
	}

	private void AddPalmMesh(int knuckleVerts)
	{
		int num = this.vertCounter;
		Vector3 vector = (this.referenceHand.Bones[9].Transform.position + this.referenceHand.Bones[12].Transform.position) * 0.5f;
		Vector3 vector2 = (this.referenceHand.Bones[4].Transform.position + this.referenceHand.Bones[6].Transform.position) * 0.5f;
		vector2 = (vector2 - this.referenceHand.Bones[15].Transform.position) * 0.9f + this.referenceHand.Bones[15].Transform.position;
		Vector3 vector3 = (this.referenceHand.Bones[5].Transform.position - this.referenceHand.Bones[4].Transform.position) * this.webOffset;
		vector3 += this.referenceHand.Bones[4].Transform.position;
		Vector3 vector4 = (this.referenceHand.Bones[7].Transform.position - this.referenceHand.Bones[6].Transform.position) * this.webOffset;
		vector4 += this.referenceHand.Bones[6].Transform.position;
		Vector3 vector5 = (this.referenceHand.Bones[17].Transform.position - this.referenceHand.Bones[16].Transform.position) * this.webOffset;
		vector5 += this.referenceHand.Bones[16].Transform.position;
		Vector3 vector6 = this.referenceHand.Bones[10].Transform.position - this.referenceHand.Bones[9].Transform.position + (this.referenceHand.Bones[13].Transform.position - this.referenceHand.Bones[12].Transform.position);
		vector6 *= 0.5f * this.webOffset;
		vector6 += vector;
		this.AddVertex(this.referenceHand.Bones[0].Transform.position, Vector2.zero, Color.black);
		this.AddVertex(this.referenceHand.Bones[3].Transform.position, Vector2.zero, Color.black);
		this.AddVertex(this.referenceHand.Bones[4].Transform.position, Vector2.zero, Color.black);
		this.AddVertex(vector3, Vector2.zero, Color.black);
		this.AddVertex(vector2, Vector2.zero, Color.black);
		this.AddVertex(this.referenceHand.Bones[6].Transform.position, Vector2.zero, Color.black);
		this.AddVertex(vector, Vector2.zero, Color.black);
		this.AddVertex(this.referenceHand.Bones[15].Transform.position, Vector2.zero, Color.black);
		this.AddVertex(this.referenceHand.Bones[16].Transform.position, Vector2.zero, Color.black);
		this.AddVertex(vector4, Vector2.zero, Color.black);
		this.AddVertex(vector6, Vector2.zero, Color.black);
		this.AddVertex(vector5, Vector2.zero, Color.black);
		int[] array = this.handTriangles;
		int num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array[num2] = num;
		int[] array2 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array2[num2] = num + 1;
		int[] array3 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array3[num2] = num + 4;
		int[] array4 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array4[num2] = num + 1;
		int[] array5 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array5[num2] = num + 2;
		int[] array6 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array6[num2] = num + 4;
		int[] array7 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array7[num2] = num + 2;
		int[] array8 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array8[num2] = num + 3;
		int[] array9 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array9[num2] = num + 4;
		int[] array10 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array10[num2] = num;
		int[] array11 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array11[num2] = num + 4;
		int[] array12 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array12[num2] = num + 5;
		int[] array13 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array13[num2] = num;
		int[] array14 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array14[num2] = num + 5;
		int[] array15 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array15[num2] = num + 6;
		int[] array16 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array16[num2] = num;
		int[] array17 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array17[num2] = num + 6;
		int[] array18 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array18[num2] = num + 8;
		int[] array19 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array19[num2] = num;
		int[] array20 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array20[num2] = num + 8;
		int[] array21 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array21[num2] = num + 7;
		int[] array22 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array22[num2] = num + 6;
		int[] array23 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array23[num2] = num + 5;
		int[] array24 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array24[num2] = num + 9;
		int[] array25 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array25[num2] = num + 6;
		int[] array26 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array26[num2] = num + 9;
		int[] array27 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array27[num2] = num + 10;
		int[] array28 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array28[num2] = num + 6;
		int[] array29 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array29[num2] = num + 10;
		int[] array30 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array30[num2] = num + 11;
		int[] array31 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array31[num2] = num + 6;
		int[] array32 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array32[num2] = num + 11;
		int[] array33 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array33[num2] = num + 8;
		int[] array34 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array34[num2] = num;
		int[] array35 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array35[num2] = num + 4;
		int[] array36 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array36[num2] = num + 1;
		int[] array37 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array37[num2] = num + 1;
		int[] array38 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array38[num2] = num + 4;
		int[] array39 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array39[num2] = num + 2;
		int[] array40 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array40[num2] = num + 2;
		int[] array41 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array41[num2] = num + 4;
		int[] array42 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array42[num2] = num + 3;
		int[] array43 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array43[num2] = num;
		int[] array44 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array44[num2] = num + 5;
		int[] array45 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array45[num2] = num + 4;
		int[] array46 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array46[num2] = num;
		int[] array47 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array47[num2] = num + 6;
		int[] array48 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array48[num2] = num + 5;
		int[] array49 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array49[num2] = num;
		int[] array50 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array50[num2] = num + 8;
		int[] array51 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array51[num2] = num + 6;
		int[] array52 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array52[num2] = num;
		int[] array53 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array53[num2] = num + 7;
		int[] array54 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array54[num2] = num + 8;
		int[] array55 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array55[num2] = num + 6;
		int[] array56 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array56[num2] = num + 9;
		int[] array57 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array57[num2] = num + 5;
		int[] array58 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array58[num2] = num + 6;
		int[] array59 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array59[num2] = num + 10;
		int[] array60 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array60[num2] = num + 9;
		int[] array61 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array61[num2] = num + 6;
		int[] array62 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array62[num2] = num + 11;
		int[] array63 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array63[num2] = num + 10;
		int[] array64 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array64[num2] = num + 6;
		int[] array65 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array65[num2] = num + 8;
		int[] array66 = this.handTriangles;
		num2 = this.triCounter;
		this.triCounter = num2 + 1;
		array66[num2] = num + 11;
		this.AddKnuckleMesh(knuckleVerts, 1f, 1f, vector2, this.referenceHand.Bones[6].Transform.position);
		this.AddKnuckleMesh(knuckleVerts, 1f - this.fingerTaper, 1f, vector3, vector2);
		this.AddKnuckleMesh(knuckleVerts, 1f, 1f, vector4, vector6);
		this.AddKnuckleMesh(knuckleVerts, 1f, 1f, vector6, vector5);
		this.AddKnuckleMesh(knuckleVerts, 1f, 1f, this.referenceHand.Bones[6].Transform.position, vector);
		this.AddKnuckleMesh(knuckleVerts, 1f, 1f, vector, this.referenceHand.Bones[16].Transform.position);
		this.AddKnuckleMesh(knuckleVerts, 1.2f, 1f, this.referenceHand.Bones[15].Transform.position, this.referenceHand.Bones[16].Transform.position);
		this.AddKnuckleMesh(knuckleVerts, 1.3f, 1.2f, this.referenceHand.Bones[0].Transform.position, this.referenceHand.Bones[15].Transform.position);
		this.AddKnuckleMesh(knuckleVerts, 1.3f, 1.2f, this.referenceHand.Bones[0].Transform.position, this.referenceHand.Bones[3].Transform.position);
		this.AddKnuckleMesh(knuckleVerts, 1.3f, 1f, this.referenceHand.Bones[0].Transform.position, this.referenceHand.Bones[6].Transform.position);
	}

	private void AddVertex(Vector3 position, Vector2 uv, Color color)
	{
		this.handVertices[this.vertCounter] = position;
		this.handUVs[this.vertCounter] = uv;
		this.handColors[this.vertCounter] = color;
		this.vertCounter++;
	}

	public OVRSkeleton referenceHand;

	public Material maskMaterial;

	[Tooltip("The segments around the tip of a finger")]
	public int radialDivisions = 9;

	[Tooltip("The fade range (finger width is 2x this)")]
	public float borderSize = 0.2f;

	[Tooltip("Along the fingers, each knuckle scales down by this amount.  Default is zero for uniform width along entire finger.")]
	public float fingerTaper = 0.13f;

	[Tooltip("Shorten the last bone of each finger; need this to account for bone structure (end bone is at finger tip instead of center). Default is 1.")]
	public float fingerTipLength = 0.8f;

	[Tooltip("Move the base of the 4 main fingers towards the tips, to avoid a visible mesh crack between finger webbing. Default is 0.")]
	public float webOffset = 0.25f;

	private float handScale = 1f;

	private GameObject maskMeshObject;

	private Mesh maskMesh;

	private Vector3[] handVertices;

	private Vector2[] handUVs;

	private Color32[] handColors;

	private int[] handTriangles;

	private int vertCounter;

	private int triCounter;
}
