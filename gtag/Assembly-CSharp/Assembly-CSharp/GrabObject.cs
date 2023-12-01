using System;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
	public void Grab(OVRInput.Controller grabHand)
	{
		this.grabbedRotation = base.transform.rotation;
		GrabObject.GrabbedObject grabbedObjectDelegate = this.GrabbedObjectDelegate;
		if (grabbedObjectDelegate == null)
		{
			return;
		}
		grabbedObjectDelegate(grabHand);
	}

	public void Release()
	{
		GrabObject.ReleasedObject releasedObjectDelegate = this.ReleasedObjectDelegate;
		if (releasedObjectDelegate == null)
		{
			return;
		}
		releasedObjectDelegate();
	}

	public void CursorPos(Vector3 cursorPos)
	{
		GrabObject.SetCursorPosition cursorPositionDelegate = this.CursorPositionDelegate;
		if (cursorPositionDelegate == null)
		{
			return;
		}
		cursorPositionDelegate(cursorPos);
	}

	[TextArea]
	public string ObjectName;

	[TextArea]
	public string ObjectInstructions;

	public GrabObject.ManipulationType objectManipulationType;

	public bool showLaserWhileGrabbed;

	[HideInInspector]
	public Quaternion grabbedRotation = Quaternion.identity;

	public GrabObject.GrabbedObject GrabbedObjectDelegate;

	public GrabObject.ReleasedObject ReleasedObjectDelegate;

	public GrabObject.SetCursorPosition CursorPositionDelegate;

	public enum ManipulationType
	{
		Default,
		ForcedHand,
		DollyHand,
		DollyAttached,
		HorizontalScaled,
		VerticalScaled,
		Menu
	}

	public delegate void GrabbedObject(OVRInput.Controller grabHand);

	public delegate void ReleasedObject();

	public delegate void SetCursorPosition(Vector3 cursorPosition);
}
