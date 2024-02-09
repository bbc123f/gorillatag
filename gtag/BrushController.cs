using System;
using System.Collections;
using UnityEngine;

public class BrushController : MonoBehaviour
{
	private void Start()
	{
		this.brush.controllerHand = OVRInput.Controller.None;
		if (!this.brush.lineContainer)
		{
			this.brush.lineContainer = new GameObject("LineContainer");
		}
		this.backgroundSphere.material.renderQueue = 3998;
		this.backgroundSphere.transform.parent = null;
		this.backgroundSphere.enabled = false;
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	private void Update()
	{
		this.backgroundSphere.transform.position = Camera.main.transform.position;
	}

	public void Grab(OVRInput.Controller grabHand)
	{
		this.brush.controllerHand = grabHand;
		this.brush.lineContainer.SetActive(true);
		this.backgroundSphere.enabled = true;
		if (this.grabRoutine != null)
		{
			base.StopCoroutine(this.grabRoutine);
		}
		if (this.releaseRoutine != null)
		{
			base.StopCoroutine(this.releaseRoutine);
		}
		this.grabRoutine = this.FadeSphere(Color.grey, 0.25f, false);
		base.StartCoroutine(this.grabRoutine);
	}

	public void Release()
	{
		this.brush.controllerHand = OVRInput.Controller.None;
		this.brush.lineContainer.SetActive(false);
		if (this.grabRoutine != null)
		{
			base.StopCoroutine(this.grabRoutine);
		}
		if (this.releaseRoutine != null)
		{
			base.StopCoroutine(this.releaseRoutine);
		}
		this.releaseRoutine = this.FadeSphere(new Color(0.5f, 0.5f, 0.5f, 0f), 0.25f, true);
		base.StartCoroutine(this.releaseRoutine);
	}

	private IEnumerator FadeCameraClearColor(Color newColor, float fadeTime)
	{
		float timer = 0f;
		Color currentColor = Camera.main.backgroundColor;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, num);
			yield return null;
		}
		yield break;
	}

	private IEnumerator FadeSphere(Color newColor, float fadeTime, bool disableOnFinish = false)
	{
		float timer = 0f;
		Color currentColor = this.backgroundSphere.material.GetColor("_Color");
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			this.backgroundSphere.material.SetColor("_Color", Color.Lerp(currentColor, newColor, num));
			if (disableOnFinish && timer >= fadeTime)
			{
				this.backgroundSphere.enabled = false;
			}
			yield return null;
		}
		yield break;
	}

	public PassthroughBrush brush;

	public MeshRenderer backgroundSphere;

	private IEnumerator grabRoutine;

	private IEnumerator releaseRoutine;
}
