using System;
using UnityEngine;

public class PassthroughSurface : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(this.projectionObject.GetComponent<MeshRenderer>());
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
	}

	public OVRPassthroughLayer passthroughLayer;

	public MeshFilter projectionObject;
}
