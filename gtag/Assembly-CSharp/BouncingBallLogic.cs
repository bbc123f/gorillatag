using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class BouncingBallLogic : MonoBehaviour
{
	// Token: 0x06000420 RID: 1056 RVA: 0x0001B1BA File Offset: 0x000193BA
	private void OnCollisionEnter()
	{
		this.audioSource.PlayOneShot(this.bounce);
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x0001B1CD File Offset: 0x000193CD
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.PlayOneShot(this.loadball);
		this.centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x0001B204 File Offset: 0x00019404
	private void Update()
	{
		if (!this.isReleased)
		{
			return;
		}
		this.UpdateVisibility();
		this.timer += Time.deltaTime;
		if (!this.isReadyForDestroy && this.timer >= this.TTL)
		{
			this.isReadyForDestroy = true;
			float length = this.pop.length;
			this.audioSource.PlayOneShot(this.pop);
			base.StartCoroutine(this.PlayPopCallback(length));
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x0001B27C File Offset: 0x0001947C
	private void UpdateVisibility()
	{
		Vector3 direction = this.centerEyeCamera.position - base.transform.position;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position, direction), out raycastHit, direction.magnitude))
		{
			if (raycastHit.collider.gameObject != base.gameObject)
			{
				this.SetVisible(false);
				return;
			}
		}
		else
		{
			this.SetVisible(true);
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001B2F0 File Offset: 0x000194F0
	private void SetVisible(bool setVisible)
	{
		if (this.isVisible && !setVisible)
		{
			base.GetComponent<MeshRenderer>().material = this.hiddenMat;
			this.isVisible = false;
		}
		if (!this.isVisible && setVisible)
		{
			base.GetComponent<MeshRenderer>().material = this.visibleMat;
			this.isVisible = true;
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x0001B345 File Offset: 0x00019545
	public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
	{
		this.isReleased = true;
		base.transform.position = pos;
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().velocity = vel;
		base.GetComponent<Rigidbody>().angularVelocity = angVel;
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0001B37E File Offset: 0x0001957E
	private IEnumerator PlayPopCallback(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040004CC RID: 1228
	[SerializeField]
	private float TTL = 5f;

	// Token: 0x040004CD RID: 1229
	[SerializeField]
	private AudioClip pop;

	// Token: 0x040004CE RID: 1230
	[SerializeField]
	private AudioClip bounce;

	// Token: 0x040004CF RID: 1231
	[SerializeField]
	private AudioClip loadball;

	// Token: 0x040004D0 RID: 1232
	[SerializeField]
	private Material visibleMat;

	// Token: 0x040004D1 RID: 1233
	[SerializeField]
	private Material hiddenMat;

	// Token: 0x040004D2 RID: 1234
	private AudioSource audioSource;

	// Token: 0x040004D3 RID: 1235
	private Transform centerEyeCamera;

	// Token: 0x040004D4 RID: 1236
	private bool isVisible = true;

	// Token: 0x040004D5 RID: 1237
	private float timer;

	// Token: 0x040004D6 RID: 1238
	private bool isReleased;

	// Token: 0x040004D7 RID: 1239
	private bool isReadyForDestroy;
}
