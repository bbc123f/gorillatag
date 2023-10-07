using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class BouncingBallMgr : MonoBehaviour
{
	// Token: 0x06000428 RID: 1064 RVA: 0x0001B5D4 File Offset: 0x000197D4
	private void Update()
	{
		if (!this.ballGrabbed && OVRInput.GetDown(this.actionBtn, OVRInput.Controller.Active))
		{
			this.currentBall = Object.Instantiate<GameObject>(this.ball, this.rightControllerPivot.transform.position, Quaternion.identity);
			this.currentBall.transform.parent = this.rightControllerPivot.transform;
			this.ballGrabbed = true;
		}
		if (this.ballGrabbed && OVRInput.GetUp(this.actionBtn, OVRInput.Controller.Active))
		{
			this.currentBall.transform.parent = null;
			Vector3 position = this.currentBall.transform.position;
			Vector3 vel = this.trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
			Vector3 localControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
			this.currentBall.GetComponent<BouncingBallLogic>().Release(position, vel, localControllerAngularVelocity);
			this.ballGrabbed = false;
		}
	}

	// Token: 0x040004D8 RID: 1240
	[SerializeField]
	private Transform trackingspace;

	// Token: 0x040004D9 RID: 1241
	[SerializeField]
	private GameObject rightControllerPivot;

	// Token: 0x040004DA RID: 1242
	[SerializeField]
	private OVRInput.RawButton actionBtn;

	// Token: 0x040004DB RID: 1243
	[SerializeField]
	private GameObject ball;

	// Token: 0x040004DC RID: 1244
	private GameObject currentBall;

	// Token: 0x040004DD RID: 1245
	private bool ballGrabbed;
}
