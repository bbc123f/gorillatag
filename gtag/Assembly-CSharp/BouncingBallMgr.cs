using System;
using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
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

	public BouncingBallMgr()
	{
	}

	[SerializeField]
	private Transform trackingspace;

	[SerializeField]
	private GameObject rightControllerPivot;

	[SerializeField]
	private OVRInput.RawButton actionBtn;

	[SerializeField]
	private GameObject ball;

	private GameObject currentBall;

	private bool ballGrabbed;
}
