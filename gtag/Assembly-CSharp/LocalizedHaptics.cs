using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class LocalizedHaptics : MonoBehaviour
{
	// Token: 0x06000480 RID: 1152 RVA: 0x0001CE11 File Offset: 0x0001B011
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0001CE28 File Offset: 0x0001B028
	private void Update()
	{
		float amplitude = (OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Thumb, 0f, amplitude, this.m_controller);
		float amplitude2 = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Index, 0f, amplitude2, this.m_controller);
		float amplitude3 = (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Hand, 0f, amplitude3, this.m_controller);
	}

	// Token: 0x04000524 RID: 1316
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x04000525 RID: 1317
	private OVRInput.Controller m_controller;
}
