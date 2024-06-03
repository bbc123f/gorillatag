using System;
using UnityEngine;

public class LocalizedHaptics : MonoBehaviour
{
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	private void Update()
	{
		float amplitude = (OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Thumb, 0f, amplitude, this.m_controller);
		float amplitude2 = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Index, 0f, amplitude2, this.m_controller);
		float amplitude3 = (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller) > 0.5f) ? 1f : 0f;
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Hand, 0f, amplitude3, this.m_controller);
	}

	public LocalizedHaptics()
	{
	}

	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	private OVRInput.Controller m_controller;
}
