using System;
using System.Collections;
using UnityEngine;

public class BouncingBallLogic : MonoBehaviour
{
	private void OnCollisionEnter()
	{
		this.audioSource.PlayOneShot(this.bounce);
	}

	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.PlayOneShot(this.loadball);
		this.centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
	}

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

	public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
	{
		this.isReleased = true;
		base.transform.position = pos;
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().velocity = vel;
		base.GetComponent<Rigidbody>().angularVelocity = angVel;
	}

	private IEnumerator PlayPopCallback(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);
		Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private float TTL = 5f;

	[SerializeField]
	private AudioClip pop;

	[SerializeField]
	private AudioClip bounce;

	[SerializeField]
	private AudioClip loadball;

	[SerializeField]
	private Material visibleMat;

	[SerializeField]
	private Material hiddenMat;

	private AudioSource audioSource;

	private Transform centerEyeCamera;

	private bool isVisible = true;

	private float timer;

	private bool isReleased;

	private bool isReadyForDestroy;
}
