﻿using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CharacterCapsule : MonoBehaviour
{
	private void Update()
	{
		if (this._character == null)
		{
			this._character = base.GetComponentInParent<CharacterController>();
			if (this._character == null)
			{
				return;
			}
		}
		if (this._height == this._character.height && this._radius == this._character.radius && this._subdivisionU == this.SubdivisionsU && this._subdivisionV == this.SubdivisionsV)
		{
			return;
		}
		this._height = this._character.height;
		this._radius = this._character.radius;
		this._subdivisionU = this.SubdivisionsU;
		this._subdivisionV = this.SubdivisionsV;
		List<Vector3> list = new List<Vector3>();
		Vector3 point = new Vector3(1f, 0f, 0f);
		Vector3 b = new Vector3(0f, this._height / 2f - this._radius, 0f);
		Vector3 a = new Vector3(0f, this._radius - this._height / 2f, 0f);
		list.Add(new Vector3(0f, this._height / 2f, 0f));
		for (int i = this.SubdivisionsU - 1; i >= 0; i--)
		{
			float num = (float)i / (float)this.SubdivisionsU;
			for (int j = 0; j < this.SubdivisionsV; j++)
			{
				float num2 = (float)j / (float)this.SubdivisionsV;
				Vector3 item = Quaternion.Euler(0f, num2 * 360f, num * 90f) * point * this._radius + b;
				list.Add(item);
			}
		}
		for (int k = 0; k < this.SubdivisionsU; k++)
		{
			float num3 = (float)k / (float)this.SubdivisionsU;
			for (int l = 0; l < this.SubdivisionsV; l++)
			{
				float num4 = (float)l / (float)this.SubdivisionsV;
				Vector3 vector = Quaternion.Euler(0f, num4 * 360f + 180f, num3 * 90f) * point;
				vector *= this._radius;
				Vector3 item2 = a - vector;
				list.Add(item2);
			}
		}
		list.Add(new Vector3(0f, -this._height / 2f, 0f));
		List<int> list2 = new List<int>();
		int num5;
		for (int m = 0; m < this.SubdivisionsV; m++)
		{
			num5 = 0;
			list2.Add(num5);
			list2.Add(m);
			list2.Add(m + 1);
		}
		list2.Add(0);
		list2.Add(this.SubdivisionsV);
		list2.Add(1);
		int num6;
		for (int n = 0; n < this.SubdivisionsU - 1; n++)
		{
			num6 = n * this.SubdivisionsV + 1;
			for (int num7 = 0; num7 < this.SubdivisionsV - 1; num7++)
			{
				num5 = num6 + num7;
				list2.Add(num5);
				list2.Add(num5 + this.SubdivisionsV);
				list2.Add(num5 + 1);
				list2.Add(num5 + 1);
				list2.Add(num5 + this.SubdivisionsV);
				list2.Add(num5 + this.SubdivisionsV + 1);
			}
			num5 = num6 + this.SubdivisionsV - 1;
			list2.Add(num5);
			list2.Add(num5 + this.SubdivisionsV);
			list2.Add(num5 + 1 - this.SubdivisionsV);
			list2.Add(num5 + 1 - this.SubdivisionsV);
			list2.Add(num5 + this.SubdivisionsV);
			list2.Add(num5 + 1);
		}
		num6 = (this.SubdivisionsU - 1) * this.SubdivisionsV + 1;
		for (int num8 = 0; num8 < this.SubdivisionsV - 1; num8++)
		{
			num5 = num6 + num8;
			list2.Add(num5);
			list2.Add(num5 + this.SubdivisionsV);
			list2.Add(num5 + 1);
			list2.Add(num5 + 1);
			list2.Add(num5 + this.SubdivisionsV);
			list2.Add(num5 + this.SubdivisionsV + 1);
		}
		num5 = num6 + this.SubdivisionsV - 1;
		list2.Add(num5);
		list2.Add(num5 + this.SubdivisionsV);
		list2.Add(num5 + 1 - this.SubdivisionsV);
		list2.Add(num5 + 1 - this.SubdivisionsV);
		list2.Add(num5 + this.SubdivisionsV);
		list2.Add(num5 + 1);
		for (int num9 = 0; num9 < this.SubdivisionsU - 1; num9++)
		{
			num6 = num9 * this.SubdivisionsV + this.SubdivisionsU * this.SubdivisionsV + 1;
			for (int num10 = 0; num10 < this.SubdivisionsV - 1; num10++)
			{
				num5 = num6 + num10;
				list2.Add(num5);
				list2.Add(num5 + this.SubdivisionsV);
				list2.Add(num5 + 1);
				list2.Add(num5 + 1);
				list2.Add(num5 + this.SubdivisionsV);
				list2.Add(num5 + this.SubdivisionsV + 1);
			}
			num5 = num6 + this.SubdivisionsV - 1;
			list2.Add(num5);
			list2.Add(num5 + this.SubdivisionsV);
			list2.Add(num5 + 1 - this.SubdivisionsV);
			list2.Add(num5 + 1 - this.SubdivisionsV);
			list2.Add(num5 + this.SubdivisionsV);
			list2.Add(num5 + 1);
		}
		int num11 = list.Count - 1;
		int num12 = num11 - this.SubdivisionsV;
		for (int num13 = 0; num13 < this.SubdivisionsV; num13++)
		{
			list2.Add(num11);
			list2.Add(num12 + num13 + 1);
			list2.Add(num12 + num13);
		}
		list2.Add(num11);
		list2.Add(num12);
		list2.Add(num11 - 1);
		this._vertices = list.ToArray();
		this._triangles = list2.ToArray();
		this._meshFilter = base.gameObject.GetComponent<MeshFilter>();
		this._meshFilter.mesh = new Mesh();
		this._meshFilter.sharedMesh.vertices = this._vertices;
		this._meshFilter.sharedMesh.triangles = this._triangles;
		this._meshFilter.sharedMesh.RecalculateNormals();
	}

	private CharacterController _character;

	private MeshFilter _meshFilter;

	private float _height;

	private float _radius;

	[Range(4f, 32f)]
	public int SubdivisionsU;

	[Range(4f, 32f)]
	public int SubdivisionsV;

	private int _subdivisionU;

	private int _subdivisionV;

	private Vector3[] _vertices;

	private int[] _triangles;
}
