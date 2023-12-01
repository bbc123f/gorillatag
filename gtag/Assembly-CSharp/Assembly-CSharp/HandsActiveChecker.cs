using System;
using System.Collections;
using UnityEngine;

public class HandsActiveChecker : MonoBehaviour
{
	private void Awake()
	{
		this._notification = Object.Instantiate<GameObject>(this._notificationPrefab);
		base.StartCoroutine(this.GetCenterEye());
	}

	private void Update()
	{
		if (OVRPlugin.GetHandTrackingEnabled())
		{
			this._notification.SetActive(false);
			return;
		}
		this._notification.SetActive(true);
		if (this._centerEye)
		{
			this._notification.transform.position = this._centerEye.position + this._centerEye.forward * 0.5f;
			this._notification.transform.rotation = this._centerEye.rotation;
		}
	}

	private IEnumerator GetCenterEye()
	{
		if ((this._cameraRig = Object.FindObjectOfType<OVRCameraRig>()) != null)
		{
			while (!this._centerEye)
			{
				this._centerEye = this._cameraRig.centerEyeAnchor;
				yield return null;
			}
		}
		yield break;
	}

	[SerializeField]
	private GameObject _notificationPrefab;

	private GameObject _notification;

	private OVRCameraRig _cameraRig;

	private Transform _centerEye;
}
