using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class EarlyAccessButton : GorillaPressableButton
{
	// Token: 0x06000647 RID: 1607 RVA: 0x0002773C File Offset: 0x0002593C
	private void Awake()
	{
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00027740 File Offset: 0x00025940
	public void Update()
	{
		if (PhotonNetworkController.Instance != null && PhotonNetworkController.Instance.wrongVersion)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0002779E File Offset: 0x0002599E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x000277BF File Offset: 0x000259BF
	public void AlreadyOwn()
	{
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x000277F5 File Offset: 0x000259F5
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}
}
