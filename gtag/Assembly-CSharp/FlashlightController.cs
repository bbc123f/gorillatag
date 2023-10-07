using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class FlashlightController : MonoBehaviour
{
	// Token: 0x060003D2 RID: 978 RVA: 0x00017ACC File Offset: 0x00015CCC
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

	// Token: 0x060003D3 RID: 979 RVA: 0x00017B94 File Offset: 0x00015D94
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

	// Token: 0x060003D4 RID: 980 RVA: 0x00017C64 File Offset: 0x00015E64
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

	// Token: 0x060003D5 RID: 981 RVA: 0x00017D34 File Offset: 0x00015F34
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

	// Token: 0x060003D6 RID: 982 RVA: 0x00017DEC File Offset: 0x00015FEC
	private void AlignWithController(OVRInput.Controller controller)
	{
		base.transform.position = OVRInput.GetLocalControllerPosition(controller);
		base.transform.rotation = OVRInput.GetLocalControllerRotation(controller);
		this.flashlightRoot.localRotation = this.localRotation;
		this.flashlightRoot.localPosition = this.localPosition;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00017E40 File Offset: 0x00016040
	public void Grab(OVRInput.Controller grabHand)
	{
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(true);
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeLighting(new Color(0f, 0f, 0f, 0.95f), 0f, 0.25f));
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00017E9C File Offset: 0x0001609C
	public void Release()
	{
		if (base.GetComponent<Flashlight>())
		{
			base.GetComponent<Flashlight>().EnableFlashlight(false);
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeLighting(Color.clear, 1f, 0.25f));
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00017ED9 File Offset: 0x000160D9
	private IEnumerator FadeLighting(Color newColor, float sceneLightIntensity, float fadeTime)
	{
		float timer = 0f;
		Color currentColor = Camera.main.backgroundColor;
		float currentLight = this.sceneLight ? this.sceneLight.intensity : 0f;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / fadeTime);
			Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, t);
			if (this.sceneLight)
			{
				this.sceneLight.intensity = Mathf.Lerp(currentLight, sceneLightIntensity, t);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000469 RID: 1129
	public Light sceneLight;

	// Token: 0x0400046A RID: 1130
	public Transform flashlightRoot;

	// Token: 0x0400046B RID: 1131
	private Vector3 localPosition = Vector3.zero;

	// Token: 0x0400046C RID: 1132
	private Quaternion localRotation = Quaternion.identity;

	// Token: 0x0400046D RID: 1133
	public TextMesh infoText;

	// Token: 0x0400046E RID: 1134
	private GrabObject externalController;

	// Token: 0x0400046F RID: 1135
	private OVRSkeleton[] skeletons;

	// Token: 0x04000470 RID: 1136
	private OVRHand[] hands;

	// Token: 0x04000471 RID: 1137
	private int handIndex = -1;

	// Token: 0x04000472 RID: 1138
	private bool pinching;
}
