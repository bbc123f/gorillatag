using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class SetMeshBarycentricCoordinates : MonoBehaviour
{
	private void Start()
	{
		this._meshFilter = base.GetComponent<MeshFilter>();
		base.StartCoroutine(this.CheckMeshData());
	}

	private IEnumerator CheckMeshData()
	{
		yield return null;
		if (this._meshFilter.mesh == null || this._mesh == this._meshFilter.mesh || this._meshFilter.mesh.vertexCount == 0)
		{
			yield return new WaitForSeconds(1f);
		}
		this.CreateBarycentricCoordinates();
		yield break;
	}

	private void CreateBarycentricCoordinates()
	{
		Mesh mesh = this._meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.GetTriangles(0);
		Color[] array = new Color[triangles.Length];
		Vector3[] array2 = new Vector3[triangles.Length];
		int[] array3 = new int[triangles.Length];
		for (int i = 0; i < triangles.Length; i++)
		{
			array[i] = new Color((i % 3 == 0) ? 1f : 0f, (i % 3 == 1) ? 1f : 0f, (i % 3 == 2) ? 1f : 0f);
			array2[i] = vertices[triangles[i]];
			array3[i] = i;
		}
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.SetVertices(array2);
		mesh.SetColors(array);
		mesh.SetIndices(array3, MeshTopology.Triangles, 0, true, 0);
		this._mesh = mesh;
		this._meshFilter.mesh = this._mesh;
		this._meshFilter.mesh.RecalculateNormals();
	}

	public SetMeshBarycentricCoordinates()
	{
	}

	private MeshFilter _meshFilter;

	private Mesh _mesh;

	[CompilerGenerated]
	private sealed class <CheckMeshData>d__3 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <CheckMeshData>d__3(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SetMeshBarycentricCoordinates setMeshBarycentricCoordinates = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				if (setMeshBarycentricCoordinates._meshFilter.mesh == null || setMeshBarycentricCoordinates._mesh == setMeshBarycentricCoordinates._meshFilter.mesh || setMeshBarycentricCoordinates._meshFilter.mesh.vertexCount == 0)
				{
					this.<>2__current = new WaitForSeconds(1f);
					this.<>1__state = 2;
					return true;
				}
				break;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			setMeshBarycentricCoordinates.CreateBarycentricCoordinates();
			return false;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public SetMeshBarycentricCoordinates <>4__this;
	}
}
