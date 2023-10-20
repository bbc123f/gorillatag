using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class HandsActiveChecker : MonoBehaviour
{
	// Token: 0x0600039A RID: 922 RVA: 0x00015D52 File Offset: 0x00013F52
	private void Awake()
	{
		this._notification = Object.Instantiate<GameObject>(this._notificationPrefab);
		base.StartCoroutine(this.GetCenterEye());
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00015D74 File Offset: 0x00013F74
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

	// Token: 0x0600039C RID: 924 RVA: 0x00015DFE File Offset: 0x00013FFE
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

	// Token: 0x0400043B RID: 1083
	[SerializeField]
	private GameObject _notificationPrefab;

	// Token: 0x0400043C RID: 1084
	private GameObject _notification;

	// Token: 0x0400043D RID: 1085
	private OVRCameraRig _cameraRig;

	// Token: 0x0400043E RID: 1086
	private Transform _centerEye;
}
