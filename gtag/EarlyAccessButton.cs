using System.Collections;
using GorillaNetworking;
using UnityEngine;

public class EarlyAccessButton : GorillaPressableButton
{
	private void Awake()
	{
	}

	public void Update()
	{
		if (PhotonNetworkController.Instance != null && PhotonNetworkController.Instance.wrongVersion)
		{
			base.enabled = false;
			GetComponent<BoxCollider>().enabled = false;
			buttonRenderer.material = pressedMaterial;
			myText.text = "UNAVAILABLE";
		}
	}

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		StartCoroutine(ButtonColorUpdate());
	}

	public void AlreadyOwn()
	{
		base.enabled = false;
		GetComponent<BoxCollider>().enabled = false;
		buttonRenderer.material = pressedMaterial;
		myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		buttonRenderer.material = pressedMaterial;
		yield return new WaitForSeconds(debounceTime);
		buttonRenderer.material = (isOn ? pressedMaterial : unpressedMaterial);
	}
}
