using System;
using System.Collections.Generic;
using Oculus.Spatializer.Propagation;
using UnityEngine;

public class ONSPPropagationGeometry : MonoBehaviour
{
	private struct MeshMaterial
	{
		public MeshFilter meshFilter;

		public ONSPPropagationMaterial[] materials;
	}

	private struct TerrainMaterial
	{
		public Terrain terrain;

		public ONSPPropagationMaterial[] materials;

		public Mesh[] treePrototypeMeshes;
	}

	public static string GeometryAssetDirectory = "AudioGeometry";

	public string filePathRelative = "";

	public bool fileEnabled;

	public bool includeChildMeshes = true;

	private IntPtr geometryHandle = IntPtr.Zero;

	public static int OSPSuccess = 0;

	public const string GEOMETRY_FILE_EXTENSION = "ovramesh";

	private static int terrainDecimation = 4;

	public static string GeometryAssetPath => Application.streamingAssetsPath + "/" + GeometryAssetDirectory;

	public string filePath => GeometryAssetPath + "/" + filePathRelative;

	private static string GetPath(Transform current)
	{
		if (current.parent == null)
		{
			return current.gameObject.scene.name + "/" + current.name;
		}
		return GetPath(current.parent) + "-" + current.name;
	}

	private void Awake()
	{
		CreatePropagationGeometry();
	}

	private void CreatePropagationGeometry()
	{
		if (ONSPPropagation.Interface.CreateAudioGeometry(out geometryHandle) != OSPSuccess)
		{
			throw new Exception("Unable to create geometry handle");
		}
		if (filePath != null && filePath.Length != 0 && fileEnabled && Application.isPlaying)
		{
			if (!ReadFile())
			{
				Debug.LogError("Failed to read file, attempting to regenerate audio geometry");
				UploadGeometry();
			}
		}
		else
		{
			UploadGeometry();
		}
	}

	private void Update()
	{
		if (!(geometryHandle == IntPtr.Zero))
		{
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			float[] matrix4x = new float[16]
			{
				localToWorldMatrix[0, 0],
				localToWorldMatrix[1, 0],
				0f - localToWorldMatrix[2, 0],
				localToWorldMatrix[3, 0],
				localToWorldMatrix[0, 1],
				localToWorldMatrix[1, 1],
				0f - localToWorldMatrix[2, 1],
				localToWorldMatrix[3, 1],
				localToWorldMatrix[0, 2],
				localToWorldMatrix[1, 2],
				0f - localToWorldMatrix[2, 2],
				localToWorldMatrix[3, 2],
				localToWorldMatrix[0, 3],
				localToWorldMatrix[1, 3],
				0f - localToWorldMatrix[2, 3],
				localToWorldMatrix[3, 3]
			};
			ONSPPropagation.Interface.AudioGeometrySetTransform(geometryHandle, matrix4x);
		}
	}

	private void OnDestroy()
	{
		if (geometryHandle != IntPtr.Zero && ONSPPropagation.Interface.DestroyAudioGeometry(geometryHandle) != OSPSuccess)
		{
			throw new Exception("Unable to destroy geometry");
		}
		geometryHandle = IntPtr.Zero;
	}

	private static void traverseMeshHierarchy(GameObject obj, ONSPPropagationMaterial[] currentMaterials, bool includeChildren, List<MeshMaterial> meshMaterials, List<TerrainMaterial> terrainMaterials, bool ignoreStatic, ref int ignoredMeshCount)
	{
		if (!obj.activeInHierarchy)
		{
			return;
		}
		MeshFilter[] components = obj.GetComponents<MeshFilter>();
		Terrain[] components2 = obj.GetComponents<Terrain>();
		ONSPPropagationMaterial[] components3 = obj.GetComponents<ONSPPropagationMaterial>();
		if (components3 != null && components3.Length != 0)
		{
			int num = components3.Length;
			if (currentMaterials != null)
			{
				num = Math.Max(num, currentMaterials.Length);
			}
			ONSPPropagationMaterial[] array = new ONSPPropagationMaterial[num];
			if (currentMaterials != null)
			{
				for (int i = components3.Length; i < num; i++)
				{
					array[i] = currentMaterials[i];
				}
			}
			currentMaterials = array;
			for (int j = 0; j < components3.Length; j++)
			{
				currentMaterials[j] = components3[j];
			}
		}
		MeshFilter[] array2 = components;
		foreach (MeshFilter meshFilter in array2)
		{
			Mesh sharedMesh = meshFilter.sharedMesh;
			if (!(sharedMesh == null))
			{
				if (ignoreStatic && !sharedMesh.isReadable)
				{
					Debug.LogWarning("Mesh: " + meshFilter.gameObject.name + " not readable, cannot be static.", meshFilter.gameObject);
					ignoredMeshCount++;
					continue;
				}
				MeshMaterial item = default(MeshMaterial);
				item.meshFilter = meshFilter;
				item.materials = currentMaterials;
				meshMaterials.Add(item);
			}
		}
		Terrain[] array3 = components2;
		foreach (Terrain terrain in array3)
		{
			TerrainMaterial item2 = default(TerrainMaterial);
			item2.terrain = terrain;
			item2.materials = currentMaterials;
			terrainMaterials.Add(item2);
		}
		if (!includeChildren)
		{
			return;
		}
		foreach (Transform item3 in obj.transform)
		{
			if (item3.GetComponent<ONSPPropagationGeometry>() == null)
			{
				traverseMeshHierarchy(item3.gameObject, currentMaterials, includeChildren, meshMaterials, terrainMaterials, ignoreStatic, ref ignoredMeshCount);
			}
		}
	}

	private int uploadMesh(IntPtr geometryHandle, GameObject meshObject, Matrix4x4 worldToLocal)
	{
		int ignoredMeshCount = 0;
		return uploadMesh(geometryHandle, meshObject, worldToLocal, ignoreStatic: false, ref ignoredMeshCount);
	}

	private int uploadMesh(IntPtr geometryHandle, GameObject meshObject, Matrix4x4 worldToLocal, bool ignoreStatic, ref int ignoredMeshCount)
	{
		List<MeshMaterial> list = new List<MeshMaterial>();
		List<TerrainMaterial> list2 = new List<TerrainMaterial>();
		traverseMeshHierarchy(meshObject, null, includeChildMeshes, list, list2, ignoreStatic, ref ignoredMeshCount);
		ONSPPropagationMaterial[] array = new ONSPPropagationMaterial[1] { base.gameObject.AddComponent<ONSPPropagationMaterial>() };
		array[0].SetPreset(ONSPPropagationMaterial.Preset.Foliage);
		int totalVertexCount = 0;
		uint totalIndexCount = 0u;
		int totalFaceCount = 0;
		int totalMaterialCount = 0;
		foreach (MeshMaterial item in list)
		{
			updateCountsForMesh(ref totalVertexCount, ref totalIndexCount, ref totalFaceCount, ref totalMaterialCount, item.meshFilter.sharedMesh);
		}
		for (int i = 0; i < list2.Count; i++)
		{
			TerrainMaterial value = list2[i];
			TerrainData terrainData = value.terrain.terrainData;
			int heightmapResolution = terrainData.heightmapResolution;
			int heightmapResolution2 = terrainData.heightmapResolution;
			int num = (heightmapResolution - 1) / terrainDecimation + 1;
			int num2 = (heightmapResolution2 - 1) / terrainDecimation + 1;
			int num3 = num * num2;
			int num4 = (num - 1) * (num2 - 1) * 6;
			totalMaterialCount++;
			totalVertexCount += num3;
			totalIndexCount += (uint)num4;
			totalFaceCount += num4 / 3;
			TreePrototype[] treePrototypes = terrainData.treePrototypes;
			value.treePrototypeMeshes = new Mesh[treePrototypes.Length];
			for (int j = 0; j < treePrototypes.Length; j++)
			{
				MeshFilter[] componentsInChildren = treePrototypes[j].prefab.GetComponentsInChildren<MeshFilter>();
				int num5 = int.MaxValue;
				int num6 = -1;
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					int vertexCount = componentsInChildren[k].sharedMesh.vertexCount;
					if (vertexCount < num5)
					{
						num5 = vertexCount;
						num6 = k;
					}
				}
				value.treePrototypeMeshes[j] = componentsInChildren[num6].sharedMesh;
			}
			TreeInstance[] treeInstances = terrainData.treeInstances;
			for (int l = 0; l < treeInstances.Length; l++)
			{
				TreeInstance treeInstance = treeInstances[l];
				updateCountsForMesh(ref totalVertexCount, ref totalIndexCount, ref totalFaceCount, ref totalMaterialCount, value.treePrototypeMeshes[treeInstance.prototypeIndex]);
			}
			list2[i] = value;
		}
		List<Vector3> tempVertices = new List<Vector3>();
		List<int> tempIndices = new List<int>();
		MeshGroup[] array2 = new MeshGroup[totalMaterialCount];
		float[] array3 = new float[totalVertexCount * 3];
		int[] array4 = new int[totalIndexCount];
		int vertexOffset = 0;
		int indexOffset = 0;
		int groupOffset = 0;
		foreach (MeshMaterial item2 in list)
		{
			MeshFilter meshFilter = item2.meshFilter;
			Matrix4x4 matrix = worldToLocal * meshFilter.gameObject.transform.localToWorldMatrix;
			uploadMeshFilter(tempVertices, tempIndices, array2, array3, array4, ref vertexOffset, ref indexOffset, ref groupOffset, meshFilter.sharedMesh, item2.materials, matrix);
		}
		foreach (TerrainMaterial item3 in list2)
		{
			TerrainData terrainData2 = item3.terrain.terrainData;
			Matrix4x4 matrix4x = worldToLocal * item3.terrain.gameObject.transform.localToWorldMatrix;
			int heightmapResolution3 = terrainData2.heightmapResolution;
			int heightmapResolution4 = terrainData2.heightmapResolution;
			float[,] heights = terrainData2.GetHeights(0, 0, heightmapResolution3, heightmapResolution4);
			Vector3 size = terrainData2.size;
			size = new Vector3(size.x / (float)(heightmapResolution3 - 1) * (float)terrainDecimation, size.y, size.z / (float)(heightmapResolution4 - 1) * (float)terrainDecimation);
			int num7 = (heightmapResolution3 - 1) / terrainDecimation + 1;
			int num8 = (heightmapResolution4 - 1) / terrainDecimation + 1;
			int num9 = num7 * num8;
			int num10 = (num7 - 1) * (num8 - 1) * 2;
			array2[groupOffset].faceType = FaceType.TRIANGLES;
			array2[groupOffset].faceCount = (UIntPtr)(ulong)num10;
			array2[groupOffset].indexOffset = (UIntPtr)(ulong)indexOffset;
			if (item3.materials != null && item3.materials.Length != 0)
			{
				item3.materials[0].StartInternal();
				array2[groupOffset].material = item3.materials[0].materialHandle;
			}
			else
			{
				array2[groupOffset].material = IntPtr.Zero;
			}
			for (int m = 0; m < num8; m++)
			{
				for (int n = 0; n < num7; n++)
				{
					int num11 = (vertexOffset + m * num7 + n) * 3;
					Vector3 vector = matrix4x.MultiplyPoint3x4(Vector3.Scale(size, new Vector3(m, heights[n * terrainDecimation, m * terrainDecimation], n)));
					array3[num11] = vector.x;
					array3[num11 + 1] = vector.y;
					array3[num11 + 2] = vector.z;
				}
			}
			for (int num12 = 0; num12 < num8 - 1; num12++)
			{
				for (int num13 = 0; num13 < num7 - 1; num13++)
				{
					array4[indexOffset] = vertexOffset + num12 * num7 + num13;
					array4[indexOffset + 1] = vertexOffset + num12 * num7 + num13 + 1;
					array4[indexOffset + 2] = vertexOffset + (num12 + 1) * num7 + num13;
					array4[indexOffset + 3] = vertexOffset + (num12 + 1) * num7 + num13;
					array4[indexOffset + 4] = vertexOffset + num12 * num7 + num13 + 1;
					array4[indexOffset + 5] = vertexOffset + (num12 + 1) * num7 + num13 + 1;
					indexOffset += 6;
				}
			}
			vertexOffset += num9;
			groupOffset++;
			TreeInstance[] treeInstances = terrainData2.treeInstances;
			for (int l = 0; l < treeInstances.Length; l++)
			{
				TreeInstance treeInstance2 = treeInstances[l];
				Vector3 vector2 = Vector3.Scale(treeInstance2.position, terrainData2.size);
				Matrix4x4 localToWorldMatrix = item3.terrain.gameObject.transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, localToWorldMatrix.GetColumn(3) + new Vector4(vector2.x, vector2.y, vector2.z, 0f));
				Matrix4x4 matrix2 = worldToLocal * localToWorldMatrix;
				uploadMeshFilter(tempVertices, tempIndices, array2, array3, array4, ref vertexOffset, ref indexOffset, ref groupOffset, item3.treePrototypeMeshes[treeInstance2.prototypeIndex], array, matrix2);
			}
		}
		return ONSPPropagation.Interface.AudioGeometryUploadMeshArrays(geometryHandle, array3, totalVertexCount, array4, array4.Length, array2, array2.Length);
	}

	private static void uploadMeshFilter(List<Vector3> tempVertices, List<int> tempIndices, MeshGroup[] groups, float[] vertices, int[] indices, ref int vertexOffset, ref int indexOffset, ref int groupOffset, Mesh mesh, ONSPPropagationMaterial[] materials, Matrix4x4 matrix)
	{
		tempVertices.Clear();
		mesh.GetVertices(tempVertices);
		int count = tempVertices.Count;
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = matrix.MultiplyPoint3x4(tempVertices[i]);
			int num = (vertexOffset + i) * 3;
			vertices[num] = vector.x;
			vertices[num + 1] = vector.y;
			vertices[num + 2] = vector.z;
		}
		for (int j = 0; j < mesh.subMeshCount; j++)
		{
			MeshTopology topology = mesh.GetTopology(j);
			if (topology != 0 && topology != MeshTopology.Quads)
			{
				continue;
			}
			tempIndices.Clear();
			mesh.GetIndices(tempIndices, j);
			int count2 = tempIndices.Count;
			for (int k = 0; k < count2; k++)
			{
				indices[indexOffset + k] = tempIndices[k] + vertexOffset;
			}
			switch (topology)
			{
			case MeshTopology.Triangles:
				groups[groupOffset + j].faceType = FaceType.TRIANGLES;
				groups[groupOffset + j].faceCount = (UIntPtr)(ulong)(count2 / 3);
				break;
			case MeshTopology.Quads:
				groups[groupOffset + j].faceType = FaceType.QUADS;
				groups[groupOffset + j].faceCount = (UIntPtr)(ulong)(count2 / 4);
				break;
			}
			groups[groupOffset + j].indexOffset = (UIntPtr)(ulong)indexOffset;
			if (materials != null && materials.Length != 0)
			{
				int num2 = j;
				if (num2 >= materials.Length)
				{
					num2 = materials.Length - 1;
				}
				materials[num2].StartInternal();
				groups[groupOffset + j].material = materials[num2].materialHandle;
			}
			else
			{
				groups[groupOffset + j].material = IntPtr.Zero;
			}
			indexOffset += count2;
		}
		vertexOffset += count;
		groupOffset += mesh.subMeshCount;
	}

	private static void updateCountsForMesh(ref int totalVertexCount, ref uint totalIndexCount, ref int totalFaceCount, ref int totalMaterialCount, Mesh mesh)
	{
		totalMaterialCount += mesh.subMeshCount;
		totalVertexCount += mesh.vertexCount;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			MeshTopology topology = mesh.GetTopology(i);
			if (topology == MeshTopology.Triangles || topology == MeshTopology.Quads)
			{
				uint indexCount = mesh.GetIndexCount(i);
				totalIndexCount += indexCount;
				switch (topology)
				{
				case MeshTopology.Triangles:
					totalFaceCount += (int)indexCount / 3;
					break;
				case MeshTopology.Quads:
					totalFaceCount += (int)indexCount / 4;
					break;
				}
			}
		}
	}

	public void UploadGeometry()
	{
		int ignoredMeshCount = 0;
		if (uploadMesh(geometryHandle, base.gameObject, base.gameObject.transform.worldToLocalMatrix, ignoreStatic: true, ref ignoredMeshCount) != OSPSuccess)
		{
			throw new Exception("Unable to upload audio mesh geometry");
		}
		if (ignoredMeshCount != 0)
		{
			Debug.LogError("Failed to upload meshes, " + ignoredMeshCount + " static meshes ignored. Turn on \"File Enabled\" to process static meshes offline", base.gameObject);
		}
	}

	public bool ReadFile()
	{
		if (filePath == null || filePath.Length == 0)
		{
			Debug.LogError("Invalid mesh file path");
			return false;
		}
		if (ONSPPropagation.Interface.AudioGeometryReadMeshFile(geometryHandle, filePath) != OSPSuccess)
		{
			Debug.LogError("Error reading mesh file " + filePath);
			return false;
		}
		return true;
	}

	public bool WriteToObj()
	{
		IntPtr geometry = IntPtr.Zero;
		if (ONSPPropagation.Interface.CreateAudioGeometry(out geometry) != OSPSuccess)
		{
			throw new Exception("Failed to create temp geometry handle");
		}
		if (uploadMesh(geometry, base.gameObject, base.gameObject.transform.worldToLocalMatrix) != OSPSuccess)
		{
			Debug.LogError("Error uploading mesh " + base.gameObject.name);
			return false;
		}
		if (ONSPPropagation.Interface.AudioGeometryWriteMeshFileObj(geometry, filePath + ".obj") != OSPSuccess)
		{
			Debug.LogError("Error writing .obj file " + filePath + ".obj");
			return false;
		}
		if (ONSPPropagation.Interface.DestroyAudioGeometry(geometry) != OSPSuccess)
		{
			throw new Exception("Failed to destroy temp geometry handle");
		}
		return true;
	}
}
