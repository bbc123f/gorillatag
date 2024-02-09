using System;
using UnityEngine;

public class SPPquad : MonoBehaviour
{
	private void Start()
	{
		this.passthroughLayer = base.GetComponent<OVRPassthroughLayer>();
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	public void Grab(OVRInput.Controller grabHand)
	{
		this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
		this.controllerHand = grabHand;
	}

	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
	}

	private OVRPassthroughLayer passthroughLayer;

	public MeshFilter projectionObject;

	private OVRInput.Controller controllerHand;
}
