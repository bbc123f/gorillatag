using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class EarlyAccessButton : GorillaPressableButton
{
	// Token: 0x06000646 RID: 1606 RVA: 0x000278FC File Offset: 0x00025AFC
	private void Awake()
	{
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00027900 File Offset: 0x00025B00
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

	// Token: 0x06000648 RID: 1608 RVA: 0x0002795E File Offset: 0x00025B5E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0002797F File Offset: 0x00025B7F
	public void AlreadyOwn()
	{
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x000279B5 File Offset: 0x00025BB5
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}
}
