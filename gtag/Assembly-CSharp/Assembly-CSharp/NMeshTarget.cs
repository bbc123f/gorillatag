using System;
using UnityEngine;

public class NMeshTarget : MonoBehaviour
{
	private void Start()
	{
		NMesh.Add(base.gameObject);
	}
}
