using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class GrabObject : MonoBehaviour
{
	// Token: 0x060003DB RID: 987 RVA: 0x00017F22 File Offset: 0x00016122
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

	// Token: 0x060003DC RID: 988 RVA: 0x00017F46 File Offset: 0x00016146
	public void Release()
	{
		GrabObject.ReleasedObject releasedObjectDelegate = this.ReleasedObjectDelegate;
		if (releasedObjectDelegate == null)
		{
			return;
		}
		releasedObjectDelegate();
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00017F58 File Offset: 0x00016158
	public void CursorPos(Vector3 cursorPos)
	{
		GrabObject.SetCursorPosition cursorPositionDelegate = this.CursorPositionDelegate;
		if (cursorPositionDelegate == null)
		{
			return;
		}
		cursorPositionDelegate(cursorPos);
	}

	// Token: 0x04000473 RID: 1139
	[TextArea]
	public string ObjectName;

	// Token: 0x04000474 RID: 1140
	[TextArea]
	public string ObjectInstructions;

	// Token: 0x04000475 RID: 1141
	public GrabObject.ManipulationType objectManipulationType;

	// Token: 0x04000476 RID: 1142
	public bool showLaserWhileGrabbed;

	// Token: 0x04000477 RID: 1143
	[HideInInspector]
	public Quaternion grabbedRotation = Quaternion.identity;

	// Token: 0x04000478 RID: 1144
	public GrabObject.GrabbedObject GrabbedObjectDelegate;

	// Token: 0x04000479 RID: 1145
	public GrabObject.ReleasedObject ReleasedObjectDelegate;

	// Token: 0x0400047A RID: 1146
	public GrabObject.SetCursorPosition CursorPositionDelegate;

	// Token: 0x020003CF RID: 975
	public enum ManipulationType
	{
		// Token: 0x04001C0D RID: 7181
		Default,
		// Token: 0x04001C0E RID: 7182
		ForcedHand,
		// Token: 0x04001C0F RID: 7183
		DollyHand,
		// Token: 0x04001C10 RID: 7184
		DollyAttached,
		// Token: 0x04001C11 RID: 7185
		HorizontalScaled,
		// Token: 0x04001C12 RID: 7186
		VerticalScaled,
		// Token: 0x04001C13 RID: 7187
		Menu
	}

	// Token: 0x020003D0 RID: 976
	// (Invoke) Token: 0x06001B74 RID: 7028
	public delegate void GrabbedObject(OVRInput.Controller grabHand);

	// Token: 0x020003D1 RID: 977
	// (Invoke) Token: 0x06001B78 RID: 7032
	public delegate void ReleasedObject();

	// Token: 0x020003D2 RID: 978
	// (Invoke) Token: 0x06001B7C RID: 7036
	public delegate void SetCursorPosition(Vector3 cursorPosition);
}
