using System;
using System.Collections;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
	private void Start()
	{
		this.localRotation = this.flashlightRoot.localRotation;
		this.localPosition = this.flashlightRoot.localPosition;
		this.skeletons = new OVRSkeleton[2];
		this.hands = new OVRHand[2];
		this.externalController = base.GetComponent<GrabObject>();
		if (this.externalController)
		{
			GrabObject grabObject = this.externalController;
			grabObject.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(grabObject.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject grabObject2 = this.externalController;
			grabObject2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(grabObject2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(false);
		}
	}

	private void LateUpdate()
	{
		if (!this.externalController)
		{
			this.FindHands();
			if (OVRInput.GetActiveController() != OVRInput.Controller.RTouch && OVRInput.GetActiveController() != OVRInput.Controller.LTouch && OVRInput.GetActiveController() != OVRInput.Controller.Touch)
			{
				if (this.handIndex >= 0)
				{
					this.AlignWithHand(this.hands[this.handIndex], this.skeletons[this.handIndex]);
				}
				if (this.infoText)
				{
					this.infoText.text = "Pinch to toggle flashlight";
					return;
				}
			}
			else
			{
				this.AlignWithController(OVRInput.Controller.RTouch);
				if (OVRInput.GetUp(OVRInput.RawButton.A, OVRInput.Controller.Active) && base.GetComponent<Flashlight>())
				{
					base.GetComponent<Flashlight>().ToggleFlashlight();
				}
				if (this.infoText)
				{
					this.infoText.text = "Press A to toggle flashlight";
				}
			}
		}
	}

	private void FindHands()
	{
		if (this.skeletons[0] == null || this.skeletons[1] == null)
		{
			OVRSkeleton[] array = Object.FindObjectsOfType<OVRSkeleton>();
			if (array[0])
			{
				this.skeletons[0] = array[0];
				this.hands[0] = this.skeletons[0].GetComponent<OVRHand>();
				this.handIndex = 0;
			}
			if (array[1])
			{
				this.skeletons[1] = array[1];
				this.hands[1] = this.skeletons[1].GetComponent<OVRHand>();
				this.handIndex = 1;
				return;
			}
		}
		else if (this.handIndex == 0)
		{
			if (this.hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index))
			{
				this.handIndex = 1;
				return;
			}
		}
		else if (this.hands[0].GetFingerIsPinching(OVRHand.HandFinger.Index))
		{
			this.handIndex = 0;
		}
	}

	private void AlignWithHand(OVRHand hand, OVRSkeleton skeleton)
	{
		if (this.pinching)
		{
			if (hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < 0.8f)
			{
				this.pinching = false;
			}
		}
		else if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
		{
			if (base.GetComponent<Flashlight>())
			{
				base.GetComponent<Flashlight>().ToggleFlashlight();
			}
			this.pinching = true;
		}
		this.flashlightRoot.position = skeleton.Bones[6].Transform.position;
		this.flashlightRoot.rotation = Quaternion.LookRotation(skeleton.Bones[6].Transform.position - skeleton.Bones[0].Transform.position);
	}

	private void AlignWithController(OVRInput.Controller controller)
	{
		base.transform.position = OVRInput.GetLocalControllerPosition(controller);
		base.transform.rotation = OVRInput.GetLocalControllerRotation(controller);
		this.flashlightRoot.localRotation = this.localRotation;
		this.flashlightRoot.localPosition = this.localPosition;
	}

	public void Grab(OVRInput.Controller grabHand)
	{
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(true);
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeLighting(new Color(0f, 0f, 0f, 0.95f), 0f, 0.25f));
	}

	public void Release()
	{
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(false);
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeLighting(Color.clear, 1f, 0.25f));
	}

	private IEnumerator FadeLighting(Color newColor, float sceneLightIntensity, float fadeTime)
	{
		float timer = 0f;
		Color currentColor = Camera.main.backgroundColor;
		float currentLight = (this.sceneLight ? this.sceneLight.intensity : 0f);
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, num);
			if (this.sceneLight)
			{
				this.sceneLight.intensity = Mathf.Lerp(currentLight, sceneLightIntensity, num);
			}
			yield return null;
		}
		yield break;
	}

	public Light sceneLight;

	public Transform flashlightRoot;

	private Vector3 localPosition = Vector3.zero;

	private Quaternion localRotation = Quaternion.identity;

	public TextMesh infoText;

	private GrabObject externalController;

	private OVRSkeleton[] skeletons;

	private OVRHand[] hands;

	private int handIndex = -1;

	private bool pinching;
}
