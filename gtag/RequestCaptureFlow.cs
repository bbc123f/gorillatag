using System;
using UnityEngine;

public class RequestCaptureFlow : MonoBehaviour
{
	private void Start()
	{
		this._sceneManager = Object.FindObjectOfType<OVRSceneManager>();
	}

	private void Update()
	{
		if (OVRInput.GetUp(this.RequestCaptureBtn, OVRInput.Controller.Active))
		{
			this._sceneManager.RequestSceneCapture();
		}
	}

	public RequestCaptureFlow()
	{
	}

	public OVRInput.Button RequestCaptureBtn = OVRInput.Button.Two;

	private OVRSceneManager _sceneManager;
}
