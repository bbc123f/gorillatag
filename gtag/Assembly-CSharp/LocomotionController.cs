using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class LocomotionController : MonoBehaviour
{
	// Token: 0x0600029C RID: 668 RVA: 0x00011510 File Offset: 0x0000F710
	private void Start()
	{
		if (this.CameraRig == null)
		{
			this.CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		}
	}

	// Token: 0x04000372 RID: 882
	public OVRCameraRig CameraRig;

	// Token: 0x04000373 RID: 883
	public CapsuleCollider CharacterController;

	// Token: 0x04000374 RID: 884
	public SimpleCapsuleWithStickMovement PlayerController;
}
