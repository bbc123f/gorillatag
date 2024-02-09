using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GorillaPressableButton : MonoBehaviour
{
	public virtual void Start()
	{
	}

	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.ButtonActivation();
				this.ButtonActivationWithHand(this.testHandLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touchTime = Time.time;
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.onPressButton.Invoke();
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
		if (component == null)
		{
			return;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[] { 67, component.isLeftHand, 0.05f });
		}
	}

	public virtual void UpdateColor()
	{
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	public virtual void ButtonActivation()
	{
	}

	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	public Material pressedMaterial;

	public Material unpressedMaterial;

	public MeshRenderer buttonRenderer;

	public bool isOn;

	public float debounceTime = 0.25f;

	public float touchTime;

	public bool testPress;

	public bool testHandLeft;

	[TextArea]
	public string offText;

	[TextArea]
	public string onText;

	public Text myText;

	[Space]
	public UnityEvent onPressButton;
}
