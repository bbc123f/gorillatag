using System;
using UnityEngine;

public class AugmentedObject : MonoBehaviour
{
	private void Start()
	{
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	private void Update()
	{
		if (this.controllerHand != OVRInput.Controller.None && OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
		{
			this.ToggleShadowType();
		}
		if (this.shadow)
		{
			if (this.groundShadow)
			{
				this.shadow.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				return;
			}
			this.shadow.transform.localPosition = Vector3.zero;
		}
	}

	public void Grab(OVRInput.Controller grabHand)
	{
		this.controllerHand = grabHand;
	}

	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
	}

	private void ToggleShadowType()
	{
		this.groundShadow = !this.groundShadow;
	}

	public AugmentedObject()
	{
	}

	public OVRInput.Controller controllerHand;

	public Transform shadow;

	private bool groundShadow;
}
