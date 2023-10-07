using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class GorillaHatButtonParent : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	// Token: 0x0600096A RID: 2410 RVA: 0x00038CFC File Offset: 0x00036EFC
	public void Start()
	{
		this.hat = PlayerPrefs.GetString("hatCosmetic", "none");
		this.face = PlayerPrefs.GetString("faceCosmetic", "none");
		this.badge = PlayerPrefs.GetString("badgeCosmetic", "none");
		this.leftHandHold = PlayerPrefs.GetString("leftHandHoldCosmetic", "none");
		this.rightHandHold = PlayerPrefs.GetString("rightHandHoldCosmetic", "none");
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00038D74 File Offset: 0x00036F74
	public void LateUpdate()
	{
		if (!this.initialized && GorillaTagger.Instance.offlineVRRig.initializedCosmetics)
		{
			this.initialized = true;
			if (GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("AdministratorBadge"))
			{
				foreach (GameObject gameObject in this.adminObjects)
				{
					Debug.Log("doing this?");
					gameObject.SetActive(true);
				}
			}
			if (GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("earlyaccess"))
			{
				this.UpdateButtonState();
				this.screen.UpdateText("WELCOME TO THE HAT ROOM!\nTHANK YOU FOR PURCHASING THE EARLY ACCESS SUPPORTER PACK! PLEASE ENJOY THESE VARIOUS HATS AND NOT-HATS!", true);
			}
		}
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x00038E1C File Offset: 0x0003701C
	public void PressButton(bool isOn, GorillaHatButton.HatButtonType buttonType, string buttonValue)
	{
		if (this.initialized && GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("earlyaccess"))
		{
			switch (buttonType)
			{
			case GorillaHatButton.HatButtonType.Hat:
				if (this.hat != buttonValue)
				{
					this.hat = buttonValue;
					PlayerPrefs.SetString("hatCosmetic", buttonValue);
				}
				else
				{
					this.hat = "none";
					PlayerPrefs.SetString("hatCosmetic", "none");
				}
				break;
			case GorillaHatButton.HatButtonType.Face:
				if (this.face != buttonValue)
				{
					this.face = buttonValue;
					PlayerPrefs.SetString("faceCosmetic", buttonValue);
				}
				else
				{
					this.face = "none";
					PlayerPrefs.SetString("faceCosmetic", "none");
				}
				break;
			case GorillaHatButton.HatButtonType.Badge:
				if (this.badge != buttonValue)
				{
					this.badge = buttonValue;
					PlayerPrefs.SetString("badgeCosmetic", buttonValue);
				}
				else
				{
					this.badge = "none";
					PlayerPrefs.SetString("badgeCosmetic", "none");
				}
				break;
			}
			PlayerPrefs.Save();
			this.UpdateButtonState();
		}
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x00038F30 File Offset: 0x00037130
	private void UpdateButtonState()
	{
		foreach (GorillaHatButton gorillaHatButton in this.hatButtons)
		{
			switch (gorillaHatButton.buttonType)
			{
			case GorillaHatButton.HatButtonType.Hat:
				gorillaHatButton.isOn = (gorillaHatButton.cosmeticName == this.hat);
				break;
			case GorillaHatButton.HatButtonType.Face:
				gorillaHatButton.isOn = (gorillaHatButton.cosmeticName == this.face);
				break;
			case GorillaHatButton.HatButtonType.Badge:
				gorillaHatButton.isOn = (gorillaHatButton.cosmeticName == this.badge);
				break;
			}
			gorillaHatButton.UpdateColor();
		}
	}

	// Token: 0x04000B86 RID: 2950
	public GorillaHatButton[] hatButtons;

	// Token: 0x04000B87 RID: 2951
	public GameObject[] adminObjects;

	// Token: 0x04000B88 RID: 2952
	public string hat;

	// Token: 0x04000B89 RID: 2953
	public string face;

	// Token: 0x04000B8A RID: 2954
	public string badge;

	// Token: 0x04000B8B RID: 2955
	public string leftHandHold;

	// Token: 0x04000B8C RID: 2956
	public string rightHandHold;

	// Token: 0x04000B8D RID: 2957
	public bool initialized;

	// Token: 0x04000B8E RID: 2958
	public GorillaLevelScreen screen;
}
