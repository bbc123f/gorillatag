using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class GrabObject : MonoBehaviour
{
	// Token: 0x060003DB RID: 987 RVA: 0x00017CFE File Offset: 0x00015EFE
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

	// Token: 0x060003DC RID: 988 RVA: 0x00017D22 File Offset: 0x00015F22
	public void Release()
	{
		GrabObject.ReleasedObject releasedObjectDelegate = this.ReleasedObjectDelegate;
		if (releasedObjectDelegate == null)
		{
			return;
		}
		releasedObjectDelegate();
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00017D34 File Offset: 0x00015F34
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

	// Token: 0x020003D1 RID: 977
	public enum ManipulationType
	{
		// Token: 0x04001C1A RID: 7194
		Default,
		// Token: 0x04001C1B RID: 7195
		ForcedHand,
		// Token: 0x04001C1C RID: 7196
		DollyHand,
		// Token: 0x04001C1D RID: 7197
		DollyAttached,
		// Token: 0x04001C1E RID: 7198
		HorizontalScaled,
		// Token: 0x04001C1F RID: 7199
		VerticalScaled,
		// Token: 0x04001C20 RID: 7200
		Menu
	}

	// Token: 0x020003D2 RID: 978
	// (Invoke) Token: 0x06001B7D RID: 7037
	public delegate void GrabbedObject(OVRInput.Controller grabHand);

	// Token: 0x020003D3 RID: 979
	// (Invoke) Token: 0x06001B81 RID: 7041
	public delegate void ReleasedObject();

	// Token: 0x020003D4 RID: 980
	// (Invoke) Token: 0x06001B85 RID: 7045
	public delegate void SetCursorPosition(Vector3 cursorPosition);
}
