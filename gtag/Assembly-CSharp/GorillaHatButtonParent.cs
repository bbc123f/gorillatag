using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class GorillaHatButtonParent : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	// Token: 0x0600096E RID: 2414 RVA: 0x00038CB4 File Offset: 0x00036EB4
	public void Start()
	{
		this.hat = PlayerPrefs.GetString("hatCosmetic", "none");
		this.face = PlayerPrefs.GetString("faceCosmetic", "none");
		this.badge = PlayerPrefs.GetString("badgeCosmetic", "none");
		this.leftHandHold = PlayerPrefs.GetString("leftHandHoldCosmetic", "none");
		this.rightHandHold = PlayerPrefs.GetString("rightHandHoldCosmetic", "none");
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x00038D2C File Offset: 0x00036F2C
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

	// Token: 0x06000970 RID: 2416 RVA: 0x00038DD4 File Offset: 0x00036FD4
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

	// Token: 0x06000971 RID: 2417 RVA: 0x00038EE8 File Offset: 0x000370E8
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

	// Token: 0x04000B8A RID: 2954
	public GorillaHatButton[] hatButtons;

	// Token: 0x04000B8B RID: 2955
	public GameObject[] adminObjects;

	// Token: 0x04000B8C RID: 2956
	public string hat;

	// Token: 0x04000B8D RID: 2957
	public string face;

	// Token: 0x04000B8E RID: 2958
	public string badge;

	// Token: 0x04000B8F RID: 2959
	public string leftHandHold;

	// Token: 0x04000B90 RID: 2960
	public string rightHandHold;

	// Token: 0x04000B91 RID: 2961
	public bool initialized;

	// Token: 0x04000B92 RID: 2962
	public GorillaLevelScreen screen;
}
