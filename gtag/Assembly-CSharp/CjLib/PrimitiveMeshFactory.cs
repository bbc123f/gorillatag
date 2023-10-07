using System;
using System.Collections.Generic;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000354 RID: 852
	public class PrimitiveMeshFactory
	{
		// Token: 0x060018B0 RID: 6320 RVA: 0x00084A94 File Offset: 0x00082C94
		private static Mesh GetPooledLineMesh()
		{
			if (PrimitiveMeshFactory.s_lineMeshPool == null)
			{
				PrimitiveMeshFactory.s_lineMeshPool = new List<Mesh>();
				PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
			}
			if (PrimitiveMeshFactory.s_lastDrawLineFrame != Time.frameCount)
			{
				PrimitiveMeshFactory.s_iPooledMesh = 0;
				PrimitiveMeshFactory.s_lastDrawLineFrame = Time.frameCount;
			}
			if (PrimitiveMeshFactory.s_iPooledMesh == PrimitiveMeshFactory.s_lineMeshPool.Count)
			{
				PrimitiveMeshFactory.s_lineMeshPool.Capacity *= 2;
				for (int i = PrimitiveMeshFactory.s_iPooledMesh; i < PrimitiveMeshFactory.s_lineMeshPool.Capacity; i++)
				{
					PrimitiveMeshFactory.s_lineMeshPool.Add(new Mesh());
				}
			}
			Mesh mesh = PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh++];
			if (mesh == null)
			{
				mesh = (PrimitiveMeshFactory.s_lineMeshPool[PrimitiveMeshFactory.s_iPooledMesh - 1] = new Mesh());
			}
			return mesh;
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x00084B64 File Offset: 0x00082D64
		public static Mesh Line(Vector3 v0, Vector3 v1)
		{
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			Vector3[] vertices = new Vector3[]
			{
				v0,
				v1
			};
			int[] indices = new int[]
			{
				0,
				1
			};
			pooledLineMesh.vertices = vertices;
			pooledLineMesh.SetIndices(indices, MeshTopology.Lines, 0);
			pooledLineMesh.RecalculateBounds();
			return pooledLineMesh;
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x00084BBC File Offset: 0x00082DBC
		public static Mesh Lines(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, MeshTopology.Lines, 0);
			return pooledLineMesh;
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x00084C10 File Offset: 0x00082E10
		public static Mesh LineStrip(Vector3[] aVert)
		{
			if (aVert.Length <= 1)
			{
				return null;
			}
			Mesh pooledLineMesh = PrimitiveMeshFactory.GetPooledLineMesh();
			if (pooledLineMesh == null)
			{
				return null;
			}
			int[] array = new int[aVert.Length];
			for (int i = 0; i < aVert.Length; i++)
			{
				array[i] = i;
			}
			pooledLineMesh.vertices = aVert;
			pooledLineMesh.SetIndices(array, MeshTopology.LineStrip, 0);
			return pooledLineMesh;
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x00084C64 File Offset: 0x00082E64
		public static Mesh BoxWireframe()
		{
			if (PrimitiveMeshFactory.s_boxWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_boxWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					0,
					2,
					6,
					6,
					7,
					7,
					3,
					7,
					4,
					4,
					5,
					5,
					6,
					5,
					1,
					1,
					0,
					0,
					4
				};
				PrimitiveMeshFactory.s_boxWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_boxWireframeMesh.SetIndices(indices, MeshTopology.Lines, 0);
			}
			return PrimitiveMeshFactory.s_boxWireframeMesh;
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x00084DA8 File Offset: 0x00082FA8
		public static Mesh BoxSolidColor()
		{
			if (PrimitiveMeshFactory.s_boxSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_boxSolidColorMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					3,
					2,
					6,
					3,
					6,
					7,
					7,
					6,
					5,
					7,
					5,
					4,
					4,
					5,
					1,
					4,
					1,
					0,
					1,
					5,
					6,
					1,
					6,
					2,
					0,
					3,
					7,
					0,
					7,
					4
				};
				PrimitiveMeshFactory.s_boxSolidColorMesh.vertices = vertices;
				PrimitiveMeshFactory.s_boxSolidColorMesh.SetIndices(indices, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_boxSolidColorMesh;
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x00084EE0 File Offset: 0x000830E0
		public static Mesh BoxFlatShaded()
		{
			if (PrimitiveMeshFactory.s_boxFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_boxFlatShadedMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f)
				};
				Vector3[] array2 = new Vector3[]
				{
					array[0],
					array[1],
					array[2],
					array[0],
					array[2],
					array[3],
					array[3],
					array[2],
					array[6],
					array[3],
					array[6],
					array[7],
					array[7],
					array[6],
					array[5],
					array[7],
					array[5],
					array[4],
					array[4],
					array[5],
					array[1],
					array[4],
					array[1],
					array[0],
					array[1],
					array[5],
					array[6],
					array[1],
					array[6],
					array[2],
					array[0],
					array[3],
					array[7],
					array[0],
					array[7],
					array[4]
				};
				Vector3[] array3 = new Vector3[]
				{
					new Vector3(0f, 0f, -1f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 0f, 1f),
					new Vector3(-1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				Vector3[] normals = new Vector3[]
				{
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[0],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[1],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[2],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[3],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[4],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5],
					array3[5]
				};
				int[] array4 = new int[array2.Length];
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i] = i;
				}
				PrimitiveMeshFactory.s_boxFlatShadedMesh.vertices = array2;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.normals = normals;
				PrimitiveMeshFactory.s_boxFlatShadedMesh.SetIndices(array4, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_boxFlatShadedMesh;
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x00085514 File Offset: 0x00083714
		public static Mesh RectWireframe()
		{
			if (PrimitiveMeshFactory.s_rectWireframeMesh == null)
			{
				PrimitiveMeshFactory.s_rectWireframeMesh = new Mesh();
				Vector3[] array = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					0
				};
				PrimitiveMeshFactory.s_rectWireframeMesh.vertices = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.normals = array;
				PrimitiveMeshFactory.s_rectWireframeMesh.SetIndices(indices, MeshTopology.Lines, 0);
			}
			return PrimitiveMeshFactory.s_rectWireframeMesh;
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x000855E8 File Offset: 0x000837E8
		public static Mesh RectSolidColor()
		{
			if (PrimitiveMeshFactory.s_rectSolidColorMesh == null)
			{
				PrimitiveMeshFactory.s_rectSolidColorMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					0,
					2,
					1,
					0,
					3,
					2
				};
				PrimitiveMeshFactory.s_rectSolidColorMesh.vertices = vertices;
				PrimitiveMeshFactory.s_rectSolidColorMesh.SetIndices(indices, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_rectSolidColorMesh;
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x000856B4 File Offset: 0x000838B4
		public static Mesh RectFlatShaded()
		{
			if (PrimitiveMeshFactory.s_rectFlatShadedMesh == null)
			{
				PrimitiveMeshFactory.s_rectFlatShadedMesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f)
				};
				Vector3[] normals = new Vector3[]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f),
					new Vector3(0f, -1f, 0f)
				};
				int[] indices = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					4,
					6,
					5,
					4,
					7,
					6
				};
				PrimitiveMeshFactory.s_rectFlatShadedMesh.vertices = vertices;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.normals = normals;
				PrimitiveMeshFactory.s_rectFlatShadedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
			}
			return PrimitiveMeshFactory.s_rectFlatShadedMesh;
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x000858D4 File Offset: 0x00083AD4
		public static Mesh CircleWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments];
				int[] array2 = new int[numSegments + 1];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					array2[i] = i;
					num2 += num;
				}
				array2[numSegments] = 0;
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.LineStrip, 0);
				if (PrimitiveMeshFactory.s_circleWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x000859C8 File Offset: 0x00083BC8
		public static Mesh CircleSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
				}
				array[numSegments] = Vector3.zero;
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_circleSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x00085AF8 File Offset: 0x00083CF8
		public static Mesh CircleFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_circleFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(numSegments + 1) * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 6];
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 += num2;
					array2[i] = new Vector3(0f, 1f, 0f);
					array3[num++] = numSegments;
					array3[num++] = (i + 1) % numSegments;
					array3[num++] = i;
				}
				array[numSegments] = Vector3.zero;
				array2[numSegments] = new Vector3(0f, 1f, 0f);
				num3 = 0f;
				for (int j = 0; j < numSegments; j++)
				{
					array[j + numSegments + 1] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					num3 -= num2;
					array2[j + numSegments + 1] = new Vector3(0f, -1f, 0f);
					array3[num++] = numSegments * 2 + 1;
					array3[num++] = (j + 1) % numSegments + numSegments + 1;
					array3[num++] = j + (numSegments + 1);
				}
				array[numSegments * 2 + 1] = Vector3.zero;
				array2[numSegments * 2 + 1] = new Vector3(0f, -1f, 0f);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_circleFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_circleFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x00085D34 File Offset: 0x00083F34
		public static Mesh CylinderWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2];
				int[] array2 = new int[numSegments * 6];
				Vector3 a = new Vector3(0f, -0.5f, 0f);
				Vector3 a2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 b = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array[i] = a + b;
					array[numSegments + i] = a2 + b;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + i;
					array2[num++] = numSegments + (i + 1) % numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_cylinderWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x00085EC4 File Offset: 0x000840C4
		public static Mesh CylinderSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 2];
				int[] array2 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 2 + 1;
				array[num2] = vector;
				array[num3] = vector2;
				int num4 = 0;
				float num5 = 6.2831855f / (float)numSegments;
				float num6 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 b = Mathf.Cos(num6) * Vector3.right + Mathf.Sin(num6) * Vector3.forward;
					array[num + i] = vector + b;
					array[numSegments + i] = vector2 + b;
					array2[num4++] = num2;
					array2[num4++] = num + i;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num + (i + 1) % numSegments;
					array2[num4++] = num + i;
					array2[num4++] = numSegments + i;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = num3;
					array2[num4++] = numSegments + (i + 1) % numSegments;
					array2[num4++] = numSegments + i;
					num6 += num5;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x000860DC File Offset: 0x000842DC
		public static Mesh CylinderFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 6 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 6;
				int num4 = numSegments * 6 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 b = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + b;
					array[numSegments + i] = vector2 + b;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num2 + i * 4] = vector + b;
					array[num2 + i * 4 + 1] = vector2 + b;
					array[num2 + i * 4 + 2] = vector + vector3;
					array[num2 + i * 4 + 3] = vector2 + vector3;
					Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - b).normalized;
					array2[num2 + i * 4] = normalized;
					array2[num2 + i * 4 + 1] = normalized;
					array2[num2 + i * 4 + 2] = normalized;
					array2[num2 + i * 4 + 3] = normalized;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 3;
					array3[num5++] = num2 + i * 4 + 2;
					array3[num5++] = num2 + i * 4;
					array3[num5++] = num2 + i * 4 + 1;
					array3[num5++] = num2 + i * 4 + 3;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x00086474 File Offset: 0x00084674
		public static Mesh CylinderSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 4 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 12];
				Vector3 vector = new Vector3(0f, -0.5f, 0f);
				Vector3 vector2 = new Vector3(0f, 0.5f, 0f);
				int num = 0;
				int num2 = numSegments * 2;
				int num3 = numSegments * 4;
				int num4 = numSegments * 4 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, -1f, 0f);
				array2[num4] = new Vector3(0f, 1f, 0f);
				int num5 = 0;
				float num6 = 6.2831855f / (float)numSegments;
				float num7 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					Vector3 vector3 = Mathf.Cos(num7) * Vector3.right + Mathf.Sin(num7) * Vector3.forward;
					array[num + i] = vector + vector3;
					array[numSegments + i] = vector2 + vector3;
					array2[num + i] = new Vector3(0f, -1f, 0f);
					array2[numSegments + i] = new Vector3(0f, 1f, 0f);
					array3[num5++] = num3;
					array3[num5++] = num + i;
					array3[num5++] = num + (i + 1) % numSegments;
					array3[num5++] = num4;
					array3[num5++] = numSegments + (i + 1) % numSegments;
					array3[num5++] = numSegments + i;
					num7 += num6;
					array[num2 + i * 2] = vector + vector3;
					array[num2 + i * 2 + 1] = vector2 + vector3;
					array2[num2 + i * 2] = vector3;
					array2[num2 + i * 2 + 1] = vector3;
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 2) % (numSegments * 2);
					array3[num5++] = num2 + i * 2;
					array3[num5++] = num2 + (i * 2 + 1) % (numSegments * 2);
					array3[num5++] = num2 + (i * 2 + 3) % (numSegments * 2);
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_cylinderSmoothShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x00086780 File Offset: 0x00084980
		public static Mesh SphereWireframe(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereWireframeMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments * 2 - 1) * 2];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num3 = 3.1415927f / (float)latSegments;
				float num4 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num5 = 6.2831855f / (float)longSegments;
				float num6 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num9 = array5[k];
					float num10 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num11 = array3[l];
						float y = array4[l];
						array[num7] = new Vector3(num10 * num11, y, num9 * num11);
						if (l == 0)
						{
							array2[num8++] = num;
							array2[num8++] = num7;
						}
						else
						{
							array2[num8++] = num7 - 1;
							array2[num8++] = num7;
						}
						array2[num8++] = num7;
						array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
						if (l == latSegments - 2)
						{
							array2[num8++] = num7;
							array2[num8++] = num2;
						}
						num7++;
					}
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_sphereWireframeMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereWireframeMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereWireframeMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x000869F8 File Offset: 0x00084BF8
		public static Mesh SphereSolidColor(int latSegments, int longSegments)
		{
			if (latSegments <= 0 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSolidColorMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegments * (latSegments - 1) + 2];
				int[] array2 = new int[longSegments * (latSegments - 1) * 2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegments];
				float[] array4 = new float[latSegments];
				float num3 = 3.1415927f / (float)latSegments;
				float num4 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegments];
				float[] array6 = new float[longSegments];
				float num5 = 6.2831855f / (float)longSegments;
				float num6 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num9 = array5[k];
					float num10 = array6[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num11 = array3[l];
						float y = array4[l];
						array[num7] = new Vector3(num10 * num11, y, num9 * num11);
						if (l == 0)
						{
							array2[num8++] = num;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = num7;
						}
						if (l < latSegments - 2)
						{
							array2[num8++] = num7 + 1;
							array2[num8++] = num7;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = num7 + 1;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = (num7 + latSegments) % (longSegments * (latSegments - 1));
						}
						if (l == latSegments - 2)
						{
							array2[num8++] = num7;
							array2[num8++] = (num7 + latSegments - 1) % (longSegments * (latSegments - 1));
							array2[num8++] = num2;
						}
						num7++;
					}
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereSolidColorMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereSolidColorMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x00086CC4 File Offset: 0x00084EC4
		public static Mesh SphereFlatShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = (latSegments - 2) * 4 + 6;
				int num2 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num3 = 3.1415927f / (float)latSegments;
				float num4 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num4 += num3;
					array4[i] = Mathf.Sin(num4);
					array5[i] = Mathf.Cos(num4);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num5 = 6.2831855f / (float)longSegments;
				float num6 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num6 += num5;
					array6[j] = Mathf.Sin(num6);
					array7[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num10 = array6[k];
					float num11 = array7[k];
					float num12 = array6[(k + 1) % longSegments];
					float num13 = array7[(k + 1) % longSegments];
					int num14 = num7;
					array[num7++] = vector;
					array[num7++] = new Vector3(num11 * array4[0], array5[0], num10 * array4[0]);
					array[num7++] = new Vector3(num13 * array4[0], array5[0], num12 * array4[0]);
					int num15 = num7;
					array[num7++] = vector2;
					array[num7++] = new Vector3(num11 * array4[latSegments - 2], array5[latSegments - 2], num10 * array4[latSegments - 2]);
					array[num7++] = new Vector3(num13 * array4[latSegments - 2], array5[latSegments - 2], num12 * array4[latSegments - 2]);
					Vector3 normalized = Vector3.Cross(array[num14 + 2] - array[num14], array[num14 + 1] - array[num14]).normalized;
					array2[num8++] = normalized;
					array2[num8++] = normalized;
					array2[num8++] = normalized;
					Vector3 normalized2 = Vector3.Cross(array[num15 + 1] - array[num15], array[num15 + 2] - array[num15]).normalized;
					array2[num8++] = normalized2;
					array2[num8++] = normalized2;
					array2[num8++] = normalized2;
					array3[num9++] = num14;
					array3[num9++] = num14 + 2;
					array3[num9++] = num14 + 1;
					array3[num9++] = num15;
					array3[num9++] = num15 + 1;
					array3[num9++] = num15 + 2;
					for (int l = 0; l < latSegments - 2; l++)
					{
						float num16 = array4[l];
						float y = array5[l];
						float num17 = array4[l + 1];
						float y2 = array5[l + 1];
						int num18 = num7;
						array[num7++] = new Vector3(num11 * num16, y, num10 * num16);
						array[num7++] = new Vector3(num13 * num16, y, num12 * num16);
						array[num7++] = new Vector3(num13 * num17, y2, num12 * num17);
						array[num7++] = new Vector3(num11 * num17, y2, num10 * num17);
						Vector3 normalized3 = Vector3.Cross(array[num18 + 1] - array[num18], array[num18 + 2] - array[num18]).normalized;
						array2[num8++] = normalized3;
						array2[num8++] = normalized3;
						array2[num8++] = normalized3;
						array2[num8++] = normalized3;
						array3[num9++] = num18;
						array3[num9++] = num18 + 1;
						array3[num9++] = num18 + 2;
						array3[num9++] = num18;
						array3[num9++] = num18 + 2;
						array3[num9++] = num18 + 3;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereFlatShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x0008721C File Offset: 0x0008541C
		public static Mesh SphereSmoothShaded(int latSegments, int longSegments)
		{
			if (latSegments <= 1 || longSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegments << 16 ^ longSegments;
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = latSegments - 1;
				int num2 = (latSegments - 2) * 2 + 2;
				Vector3[] array = new Vector3[longSegments * num + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegments * num2 * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 vector2 = new Vector3(0f, -1f, 0f);
				int num3 = longSegments * num;
				int num4 = num3 + 1;
				array[num3] = vector;
				array[num4] = vector2;
				array2[num3] = new Vector3(0f, 1f, 0f);
				array2[num4] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegments];
				float[] array5 = new float[latSegments];
				float num5 = 3.1415927f / (float)latSegments;
				float num6 = 0f;
				for (int i = 0; i < latSegments; i++)
				{
					num6 += num5;
					array4[i] = Mathf.Sin(num6);
					array5[i] = Mathf.Cos(num6);
				}
				float[] array6 = new float[longSegments];
				float[] array7 = new float[longSegments];
				float num7 = 6.2831855f / (float)longSegments;
				float num8 = 0f;
				for (int j = 0; j < longSegments; j++)
				{
					num8 += num7;
					array6[j] = Mathf.Sin(num8);
					array7[j] = Mathf.Cos(num8);
				}
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				for (int k = 0; k < longSegments; k++)
				{
					float num12 = array6[k];
					float num13 = array7[k];
					for (int l = 0; l < latSegments - 1; l++)
					{
						float num14 = array4[l];
						float y = array5[l];
						Vector3 vector3 = new Vector3(num13 * num14, y, num12 * num14);
						array[num9++] = vector3;
						array2[num10++] = vector3;
						int num15 = num9 - 1;
						int num16 = num15 + 1;
						int num17 = (num15 + num) % (longSegments * num);
						int num18 = (num15 + num + 1) % (longSegments * num);
						if (latSegments == 2)
						{
							array3[num11++] = num3;
							array3[num11++] = num17;
							array3[num11++] = num15;
							array3[num11++] = num4;
							array3[num11++] = num16;
							array3[num11++] = num18;
						}
						else if (l < latSegments - 2)
						{
							if (l == 0)
							{
								array3[num11++] = num3;
								array3[num11++] = num17;
								array3[num11++] = num15;
							}
							else if (l == latSegments - 3)
							{
								array3[num11++] = num4;
								array3[num11++] = num16;
								array3[num11++] = num18;
							}
							array3[num11++] = num15;
							array3[num11++] = num18;
							array3[num11++] = num16;
							array3[num11++] = num15;
							array3[num11++] = num17;
							array3[num11++] = num18;
						}
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_sphereSmoothShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x000875A4 File Offset: 0x000857A4
		public static Mesh CapsuleWireframe(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleWireframeMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4 + 1) * 2];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num8 = array5[k];
					float num9 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num10 = array3[l];
						float num11 = array4[l];
						array[num7] = new Vector3(num9 * num10, num11 + 0.5f, num8 * num10);
						array[num7 + 1] = new Vector3(num9 * num10, -num11 - 0.5f, num8 * num10);
						if (caps)
						{
							if (l == 0)
							{
								array2[newSize++] = num;
								array2[newSize++] = num7;
								if (!topCapOnly)
								{
									array2[newSize++] = num2;
									array2[newSize++] = num7 + 1;
								}
							}
							else
							{
								array2[newSize++] = num7 - 2;
								array2[newSize++] = num7;
								if (!topCapOnly)
								{
									array2[newSize++] = num7 - 1;
									array2[newSize++] = num7 + 1;
								}
							}
						}
						if (caps || l == latSegmentsPerCap - 1)
						{
							array2[newSize++] = num7;
							array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							if (!topCapOnly)
							{
								array2[newSize++] = num7 + 1;
								array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						if (sides && l == latSegmentsPerCap - 1)
						{
							array2[newSize++] = num7;
							array2[newSize++] = num7 + 1;
						}
						num7 += 2;
					}
				}
				Array.Resize<int>(ref array2, newSize);
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_capsuleWireframeMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleWireframeMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x000878EC File Offset: 0x00085AEC
		public static Mesh CapsuleSolidColor(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				int[] array2 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				float[] array3 = new float[latSegmentsPerCap];
				float[] array4 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array3[i] = Mathf.Sin(num4);
					array4[i] = Mathf.Cos(num4);
				}
				float[] array5 = new float[longSegmentsPerCap];
				float[] array6 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array5[j] = Mathf.Sin(num6);
					array6[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num8 = array5[k];
					float num9 = array6[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num10 = array3[l];
						float num11 = array4[l];
						array[num7] = new Vector3(num9 * num10, num11 + 0.5f, num8 * num10);
						array[num7 + 1] = new Vector3(num9 * num10, -num11 - 0.5f, num8 * num10);
						if (l == 0)
						{
							if (caps)
							{
								array2[newSize++] = num;
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = num7;
								if (!topCapOnly)
								{
									array2[newSize++] = num2;
									array2[newSize++] = num7 + 1;
									array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
						}
						else
						{
							if (caps)
							{
								array2[newSize++] = num7 - 2;
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = num7;
								array2[newSize++] = num7 - 2;
								array2[newSize++] = (num7 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array2[newSize++] = num7 - 1;
									array2[newSize++] = num7 + 1;
									array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[newSize++] = num7 - 1;
									array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array2[newSize++] = (num7 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array2[newSize++] = num7;
								array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = num7 + 1;
								array2[newSize++] = num7;
								array2[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array2[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num7 += 2;
					}
				}
				Array.Resize<int>(ref array2, newSize);
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleSolidColorMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x00087D04 File Offset: 0x00085F04
		public static Mesh CapsuleFlatShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * (latSegmentsPerCap - 1) * 8 + longSegmentsPerCap * 10];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				array2[num] = new Vector3(0f, 1f, 0f);
				array2[num2] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array4[i] = Mathf.Sin(num4);
					array5[i] = Mathf.Cos(num4);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array6[j] = Mathf.Sin(num6);
					array7[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num9 = array6[k];
					float num10 = array7[k];
					float num11 = array6[(k + 1) % longSegmentsPerCap];
					float num12 = array7[(k + 1) % longSegmentsPerCap];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num13 = array4[l];
						float num14 = array5[l];
						if (caps && l < latSegmentsPerCap - 1)
						{
							if (l == 0)
							{
								int num15 = num7;
								array[num7++] = vector;
								array[num7++] = new Vector3(num10 * num13, num14 + 0.5f, num9 * num13);
								array[num7++] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
								Vector3 vector3 = Vector3.Cross(array[num15 + 2] - array[num15], array[num15 + 1] - array[num15]);
								array2[num8++] = vector3;
								array2[num8++] = vector3;
								array2[num8++] = vector3;
								array3[newSize++] = num15;
								array3[newSize++] = num15 + 2;
								array3[newSize++] = num15 + 1;
								if (!topCapOnly)
								{
									int num16 = num7;
									array[num7++] = vector2;
									array[num7++] = new Vector3(num10 * num13, -num14 - 0.5f, num9 * num13);
									array[num7++] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
									Vector3 normalized = Vector3.Cross(array[num16 + 1] - array[num16], array[num16 + 2] - array[num16]).normalized;
									array2[num8++] = normalized;
									array2[num8++] = normalized;
									array2[num8++] = normalized;
									array3[newSize++] = num16;
									array3[newSize++] = num16 + 1;
									array3[newSize++] = num16 + 2;
								}
							}
							float num17 = array4[l + 1];
							float num18 = array5[l + 1];
							if (caps)
							{
								int num19 = num7;
								array[num7++] = new Vector3(num10 * num13, num14 + 0.5f, num9 * num13);
								array[num7++] = new Vector3(num10 * num17, num18 + 0.5f, num9 * num17);
								array[num7++] = new Vector3(num12 * num17, num18 + 0.5f, num11 * num17);
								array[num7++] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
								Vector3 vector4 = Vector3.Cross(array[num19 + 3] - array[num19], array[num19 + 1] - array[num19]);
								array2[num8++] = vector4;
								array2[num8++] = vector4;
								array2[num8++] = vector4;
								array2[num8++] = vector4;
								array3[newSize++] = num19;
								array3[newSize++] = num19 + 2;
								array3[newSize++] = num19 + 1;
								array3[newSize++] = num19;
								array3[newSize++] = num19 + 3;
								array3[newSize++] = num19 + 2;
								if (!topCapOnly)
								{
									int num20 = num7;
									array[num7++] = new Vector3(num10 * num13, -num14 - 0.5f, num9 * num13);
									array[num7++] = new Vector3(num10 * num17, -num18 - 0.5f, num9 * num17);
									array[num7++] = new Vector3(num12 * num17, -num18 - 0.5f, num11 * num17);
									array[num7++] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
									Vector3 vector5 = Vector3.Cross(array[num20 + 1] - array[num20], array[num20 + 3] - array[num20]);
									array2[num8++] = vector5;
									array2[num8++] = vector5;
									array2[num8++] = vector5;
									array2[num8++] = vector5;
									array3[newSize++] = num20;
									array3[newSize++] = num20 + 1;
									array3[newSize++] = num20 + 2;
									array3[newSize++] = num20;
									array3[newSize++] = num20 + 2;
									array3[newSize++] = num20 + 3;
								}
							}
						}
						else if (sides && l == latSegmentsPerCap - 1)
						{
							int num21 = num7;
							array[num7++] = new Vector3(num10 * num13, num14 + 0.5f, num9 * num13);
							array[num7++] = new Vector3(num10 * num13, -num14 - 0.5f, num9 * num13);
							array[num7++] = new Vector3(num12 * num13, -num14 - 0.5f, num11 * num13);
							array[num7++] = new Vector3(num12 * num13, num14 + 0.5f, num11 * num13);
							Vector3 normalized2 = Vector3.Cross(array[num21 + 3] - array[num21], array[num21 + 1] - array[num21]).normalized;
							array2[num8++] = normalized2;
							array2[num8++] = normalized2;
							array2[num8++] = normalized2;
							array2[num8++] = normalized2;
							array3[newSize++] = num21;
							array3[newSize++] = num21 + 2;
							array3[newSize++] = num21 + 1;
							array3[newSize++] = num21;
							array3[newSize++] = num21 + 3;
							array3[newSize++] = num21 + 2;
						}
					}
				}
				Array.Resize<int>(ref array3, newSize);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleFlatShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x000885B4 File Offset: 0x000867B4
		public static Mesh CapsuleSmoothShaded(int latSegmentsPerCap, int longSegmentsPerCap, bool caps = true, bool topCapOnly = false, bool sides = true)
		{
			if (latSegmentsPerCap <= 0 || longSegmentsPerCap <= 1)
			{
				return null;
			}
			if (!caps && !sides)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool = new Dictionary<int, Mesh>();
			}
			int key = latSegmentsPerCap << 12 ^ longSegmentsPerCap ^ (caps ? 268435456 : 0) ^ (topCapOnly ? 536870912 : 0) ^ (sides ? 1073741824 : 0);
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.TryGetValue(key, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[longSegmentsPerCap * latSegmentsPerCap * 2 + 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[longSegmentsPerCap * (latSegmentsPerCap * 4) * 3];
				Vector3 vector = new Vector3(0f, 1.5f, 0f);
				Vector3 vector2 = new Vector3(0f, -1.5f, 0f);
				int num = array.Length - 2;
				int num2 = array.Length - 1;
				array[num] = vector;
				array[num2] = vector2;
				array2[num] = new Vector3(0f, 1f, 0f);
				array2[num2] = new Vector3(0f, -1f, 0f);
				float[] array4 = new float[latSegmentsPerCap];
				float[] array5 = new float[latSegmentsPerCap];
				float num3 = 1.5707964f / (float)latSegmentsPerCap;
				float num4 = 0f;
				for (int i = 0; i < latSegmentsPerCap; i++)
				{
					num4 += num3;
					array4[i] = Mathf.Sin(num4);
					array5[i] = Mathf.Cos(num4);
				}
				float[] array6 = new float[longSegmentsPerCap];
				float[] array7 = new float[longSegmentsPerCap];
				float num5 = 6.2831855f / (float)longSegmentsPerCap;
				float num6 = 0f;
				for (int j = 0; j < longSegmentsPerCap; j++)
				{
					num6 += num5;
					array6[j] = Mathf.Sin(num6);
					array7[j] = Mathf.Cos(num6);
				}
				int num7 = 0;
				int num8 = 0;
				int newSize = 0;
				for (int k = 0; k < longSegmentsPerCap; k++)
				{
					float num9 = array6[k];
					float num10 = array7[k];
					for (int l = 0; l < latSegmentsPerCap; l++)
					{
						float num11 = array4[l];
						float num12 = array5[l];
						array[num7] = new Vector3(num10 * num11, num12 + 0.5f, num9 * num11);
						array[num7 + 1] = new Vector3(num10 * num11, -num12 - 0.5f, num9 * num11);
						array2[num8] = new Vector3(num10 * num11, num12, num9 * num11);
						array2[num8 + 1] = new Vector3(num10 * num11, -num12, num9 * num11);
						if (caps && l == 0)
						{
							array3[newSize++] = num;
							array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							array3[newSize++] = num7;
							if (!topCapOnly)
							{
								array3[newSize++] = num2;
								array3[newSize++] = num7 + 1;
								array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						else
						{
							if (caps)
							{
								array3[newSize++] = num7 - 2;
								array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = num7;
								array3[newSize++] = num7 - 2;
								array3[newSize++] = (num7 - 2 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								if (!topCapOnly)
								{
									array3[newSize++] = num7 - 1;
									array3[newSize++] = num7 + 1;
									array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[newSize++] = num7 - 1;
									array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
									array3[newSize++] = (num7 - 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								}
							}
							if (sides && l == latSegmentsPerCap - 1)
							{
								array3[newSize++] = num7;
								array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = num7 + 1;
								array3[newSize++] = num7;
								array3[newSize++] = (num7 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
								array3[newSize++] = (num7 + 1 + latSegmentsPerCap * 2) % (longSegmentsPerCap * latSegmentsPerCap * 2);
							}
						}
						num7 += 2;
						num8 += 2;
					}
				}
				Array.Resize<int>(ref array3, newSize);
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.ContainsKey(key))
				{
					PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Remove(key);
				}
				PrimitiveMeshFactory.s_capsuleSmoothShadedMeshPool.Add(key, mesh);
			}
			return mesh;
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x00088A68 File Offset: 0x00086C68
		public static Mesh Capsule2DWireframe(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 4];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 0; k < array.Length - 1; k++)
				{
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.LineStrip, 0);
				if (PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dWireframeMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x00088C18 File Offset: 0x00086E18
		public static Mesh Capsule2DSolidColor(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[(capSegments + 1) * 2];
				int[] array2 = new int[(capSegments + 1) * 12];
				int num = 0;
				int num2 = 0;
				float num3 = 3.1415927f / (float)capSegments;
				float num4 = 0f;
				for (int i = 0; i < capSegments; i++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) + 0.5f, 0f);
				for (int j = 0; j < capSegments; j++)
				{
					array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
					num4 += num3;
				}
				array[num++] = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4) - 0.5f, 0f);
				for (int k = 1; k < array.Length; k++)
				{
					array2[num2++] = 0;
					array2[num2++] = (k + 1) % array.Length;
					array2[num2++] = k;
					array2[num2++] = 0;
					array2[num2++] = k;
					array2[num2++] = (k + 1) % array.Length;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dSolidColorMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x00088DF0 File Offset: 0x00086FF0
		public static Mesh Capsule2DFlatShaded(int capSegments)
		{
			if (capSegments <= 0)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.TryGetValue(capSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				int num = (capSegments + 1) * 2;
				Vector3[] array = new Vector3[num * 2];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[num * 6];
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				float num5 = 3.1415927f / (float)capSegments;
				float num6 = 0f;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < capSegments; j++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) + 0.5f, 0f);
					for (int k = 0; k < capSegments; k++)
					{
						array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
						num6 += num5;
					}
					array[num2++] = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6) - 0.5f, 0f);
					Vector3 vector = new Vector3(0f, 0f, (i == 0) ? -1f : 1f);
					for (int l = 0; l < num; l++)
					{
						array2[num3++] = vector;
					}
				}
				for (int m = 1; m < num; m++)
				{
					array3[num4++] = 0;
					array3[num4++] = (m + 1) % num;
					array3[num4++] = m;
					array3[num4++] = num;
					array3[num4++] = num + m;
					array3[num4++] = num + (m + 1) % num;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.ContainsKey(capSegments))
				{
					PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Remove(capSegments);
				}
				PrimitiveMeshFactory.s_capsule2dFlatShadedMeshPool.Add(capSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x00089044 File Offset: 0x00087244
		public static Mesh ConeWireframe(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneWireframeMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneWireframeMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneWireframeMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 4];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = i;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					array2[num++] = numSegments;
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.normals = array;
				mesh.SetIndices(array2, MeshTopology.Lines, 0);
				if (PrimitiveMeshFactory.s_coneWireframeMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneWireframeMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneWireframeMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x00089180 File Offset: 0x00087380
		public static Mesh ConeSolidColor(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSolidColorMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSolidColorMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSolidColorMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments + 1];
				int[] array2 = new int[numSegments * 3 + (numSegments - 2) * 3];
				array[numSegments] = new Vector3(0f, 1f, 0f);
				int num = 0;
				float num2 = 6.2831855f / (float)numSegments;
				float num3 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array[i] = Mathf.Cos(num3) * Vector3.right + Mathf.Sin(num3) * Vector3.forward;
					array2[num++] = numSegments;
					array2[num++] = (i + 1) % numSegments;
					array2[num++] = i;
					if (i >= 2)
					{
						array2[num++] = 0;
						array2[num++] = i - 1;
						array2[num++] = i;
					}
					num3 += num2;
				}
				mesh.vertices = array;
				mesh.SetIndices(array2, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneSolidColorMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSolidColorMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSolidColorMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x000892DC File Offset: 0x000874DC
		public static Mesh ConeFlatShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneFlatShadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 3 + numSegments];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3[] array4 = new Vector3[numSegments];
				float num = 6.2831855f / (float)numSegments;
				float num2 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					array4[i] = Mathf.Cos(num2) * Vector3.right + Mathf.Sin(num2) * Vector3.forward;
					num2 += num;
				}
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (int j = 0; j < numSegments; j++)
				{
					int num6 = num3;
					array[num3++] = vector;
					array[num3++] = array4[j];
					array[num3++] = array4[(j + 1) % numSegments];
					Vector3 normalized = Vector3.Cross(array[num6 + 2] - array[num6], array[num6 + 1] - array[num6]).normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array2[num5++] = normalized;
					array3[num4++] = num6;
					array3[num4++] = num6 + 2;
					array3[num4++] = num6 + 1;
				}
				int num7 = num3;
				for (int k = 0; k < numSegments; k++)
				{
					array[num3++] = array4[k];
					array2[num5++] = new Vector3(0f, -1f, 0f);
					if (k >= 2)
					{
						array3[num4++] = num7;
						array3[num4++] = num7 + k - 1;
						array3[num4++] = num7 + k;
					}
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneFlatShadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneFlatShadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x00089564 File Offset: 0x00087764
		public static Mesh ConeSmoothShaded(int numSegments)
		{
			if (numSegments <= 1)
			{
				return null;
			}
			if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool == null)
			{
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool = new Dictionary<int, Mesh>();
			}
			Mesh mesh;
			if (!PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.TryGetValue(numSegments, out mesh) || mesh == null)
			{
				mesh = new Mesh();
				Vector3[] array = new Vector3[numSegments * 2 + 1];
				Vector3[] array2 = new Vector3[array.Length];
				int[] array3 = new int[numSegments * 3 + (numSegments - 2) * 3];
				int num = array.Length - 1;
				array[num] = new Vector3(0f, 1f, 0f);
				array2[num] = new Vector3(0f, 0f, 0f);
				float num2 = Mathf.Sqrt(0.5f);
				int num3 = 0;
				float num4 = 6.2831855f / (float)numSegments;
				float num5 = 0f;
				for (int i = 0; i < numSegments; i++)
				{
					float num6 = Mathf.Cos(num5);
					float num7 = Mathf.Sin(num5);
					Vector3 vector = num6 * Vector3.right + num7 * Vector3.forward;
					array[i] = vector;
					array[numSegments + i] = vector;
					array2[i] = new Vector3(num6 * num2, num2, num7 * num2);
					array2[numSegments + i] = new Vector3(0f, -1f, 0f);
					array3[num3++] = num;
					array3[num3++] = (i + 1) % numSegments;
					array3[num3++] = i;
					if (i >= 2)
					{
						array3[num3++] = numSegments;
						array3[num3++] = numSegments + i - 1;
						array3[num3++] = numSegments + i;
					}
					num5 += num4;
				}
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.SetIndices(array3, MeshTopology.Triangles, 0);
				if (PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.ContainsKey(numSegments))
				{
					PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Remove(numSegments);
				}
				PrimitiveMeshFactory.s_coneSmoothhadedMeshPool.Add(numSegments, mesh);
			}
			return mesh;
		}

		// Token: 0x0400197B RID: 6523
		private static int s_lastDrawLineFrame = -1;

		// Token: 0x0400197C RID: 6524
		private static int s_iPooledMesh = 0;

		// Token: 0x0400197D RID: 6525
		private static List<Mesh> s_lineMeshPool;

		// Token: 0x0400197E RID: 6526
		private static Mesh s_boxWireframeMesh;

		// Token: 0x0400197F RID: 6527
		private static Mesh s_boxSolidColorMesh;

		// Token: 0x04001980 RID: 6528
		private static Mesh s_boxFlatShadedMesh;

		// Token: 0x04001981 RID: 6529
		private static Mesh s_rectWireframeMesh;

		// Token: 0x04001982 RID: 6530
		private static Mesh s_rectSolidColorMesh;

		// Token: 0x04001983 RID: 6531
		private static Mesh s_rectFlatShadedMesh;

		// Token: 0x04001984 RID: 6532
		private static Dictionary<int, Mesh> s_circleWireframeMeshPool;

		// Token: 0x04001985 RID: 6533
		private static Dictionary<int, Mesh> s_circleSolidColorMeshPool;

		// Token: 0x04001986 RID: 6534
		private static Dictionary<int, Mesh> s_circleFlatShadedMeshPool;

		// Token: 0x04001987 RID: 6535
		private static Dictionary<int, Mesh> s_cylinderWireframeMeshPool;

		// Token: 0x04001988 RID: 6536
		private static Dictionary<int, Mesh> s_cylinderSolidColorMeshPool;

		// Token: 0x04001989 RID: 6537
		private static Dictionary<int, Mesh> s_cylinderFlatShadedMeshPool;

		// Token: 0x0400198A RID: 6538
		private static Dictionary<int, Mesh> s_cylinderSmoothShadedMeshPool;

		// Token: 0x0400198B RID: 6539
		private static Dictionary<int, Mesh> s_sphereWireframeMeshPool;

		// Token: 0x0400198C RID: 6540
		private static Dictionary<int, Mesh> s_sphereSolidColorMeshPool;

		// Token: 0x0400198D RID: 6541
		private static Dictionary<int, Mesh> s_sphereFlatShadedMeshPool;

		// Token: 0x0400198E RID: 6542
		private static Dictionary<int, Mesh> s_sphereSmoothShadedMeshPool;

		// Token: 0x0400198F RID: 6543
		private static Dictionary<int, Mesh> s_capsuleWireframeMeshPool;

		// Token: 0x04001990 RID: 6544
		private static Dictionary<int, Mesh> s_capsuleSolidColorMeshPool;

		// Token: 0x04001991 RID: 6545
		private static Dictionary<int, Mesh> s_capsuleFlatShadedMeshPool;

		// Token: 0x04001992 RID: 6546
		private static Dictionary<int, Mesh> s_capsuleSmoothShadedMeshPool;

		// Token: 0x04001993 RID: 6547
		private static Dictionary<int, Mesh> s_capsule2dWireframeMeshPool;

		// Token: 0x04001994 RID: 6548
		private static Dictionary<int, Mesh> s_capsule2dSolidColorMeshPool;

		// Token: 0x04001995 RID: 6549
		private static Dictionary<int, Mesh> s_capsule2dFlatShadedMeshPool;

		// Token: 0x04001996 RID: 6550
		private static Dictionary<int, Mesh> s_coneWireframeMeshPool;

		// Token: 0x04001997 RID: 6551
		private static Dictionary<int, Mesh> s_coneSolidColorMeshPool;

		// Token: 0x04001998 RID: 6552
		private static Dictionary<int, Mesh> s_coneFlatShadedMeshPool;

		// Token: 0x04001999 RID: 6553
		private static Dictionary<int, Mesh> s_coneSmoothhadedMeshPool;
	}
}
