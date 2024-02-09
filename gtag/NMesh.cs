using System;
using System.Collections.Generic;
using UnityEngine;

internal class NMesh
{
	public static void Add(GameObject go)
	{
		NMesh.init();
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		if (go.TryGetComponent<MeshRenderer>(out meshRenderer) && go.TryGetComponent<MeshFilter>(out meshFilter))
		{
			go.SetActive(false);
			Material sharedMaterial = meshRenderer.sharedMaterial;
			MeshFilter meshFilter2;
			if (!NMesh.__Meshes.ContainsKey(sharedMaterial.name))
			{
				NMesh.__Meshes.Add(sharedMaterial.name, new GameObject(sharedMaterial.name));
				NMesh.__Meshes[sharedMaterial.name].transform.parent = NMesh.__root.transform;
				NMesh.__Meshes[sharedMaterial.name].transform.position = Vector3.zero;
				NMesh.__Meshes[sharedMaterial.name].transform.rotation = Quaternion.identity;
				NMesh.__Meshes[sharedMaterial.name].AddComponent<MeshRenderer>().material = sharedMaterial;
				meshFilter2 = NMesh.__Meshes[sharedMaterial.name].AddComponent<MeshFilter>();
				meshFilter2.mesh = new Mesh();
			}
			else
			{
				meshFilter2 = NMesh.__Meshes[sharedMaterial.name].GetComponent<MeshFilter>();
			}
			CombineInstance[] array = new CombineInstance[2];
			array[0].mesh = meshFilter2.sharedMesh;
			array[0].transform = meshFilter2.transform.localToWorldMatrix;
			array[1].mesh = meshFilter.sharedMesh;
			array[1].transform = meshFilter.transform.localToWorldMatrix;
			meshFilter2.mesh = new Mesh();
			meshFilter2.sharedMesh.CombineMeshes(array, true, true);
			return;
		}
		throw new Exception("Attempted to NMesh GameObject \"" + go.name + "\" which has no MeshRenderer or MeshFilter component");
	}

	private static void init()
	{
		if (NMesh.__initialized)
		{
			return;
		}
		NMesh.__Meshes = new Dictionary<string, GameObject>();
		NMesh.__root = new GameObject("NMesh");
		NMesh.__root.transform.parent = null;
		NMesh.__root.transform.position = Vector3.zero;
		NMesh.__root.transform.rotation = Quaternion.identity;
		NMesh.__initialized = true;
	}

	private static Dictionary<string, GameObject> __Meshes;

	private static bool __initialized;

	private static GameObject __root;
}
