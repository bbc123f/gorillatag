using System;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class GorillaHuntComputer : MonoBehaviour
{
	// Token: 0x06000973 RID: 2419 RVA: 0x00038F80 File Offset: 0x00037180
	private void Update()
	{
		if (PhotonNetwork.InRoom && GorillaGameManager.instance != null && GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>() != null)
		{
			if (!GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().huntStarted)
			{
				if (GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().waitingToStartNextHuntGame && GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().currentTarget.Contains(PhotonNetwork.LocalPlayer) && !GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().currentHunted.Contains(PhotonNetwork.LocalPlayer) && GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().countDownTime == 0)
				{
					this.material.gameObject.SetActive(false);
					this.hat.gameObject.SetActive(false);
					this.face.gameObject.SetActive(false);
					this.badge.gameObject.SetActive(false);
					this.leftHand.gameObject.SetActive(false);
					this.rightHand.gameObject.SetActive(false);
					this.text.text = "YOU WON! CONGRATS, HUNTER!";
					return;
				}
				if (GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().countDownTime != 0)
				{
					this.material.gameObject.SetActive(false);
					this.hat.gameObject.SetActive(false);
					this.face.gameObject.SetActive(false);
					this.badge.gameObject.SetActive(false);
					this.leftHand.gameObject.SetActive(false);
					this.rightHand.gameObject.SetActive(false);
					this.text.text = "GAME STARTING IN:\n" + GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>().countDownTime.ToString() + "...";
					return;
				}
				this.material.gameObject.SetActive(false);
				this.hat.gameObject.SetActive(false);
				this.face.gameObject.SetActive(false);
				this.badge.gameObject.SetActive(false);
				this.leftHand.gameObject.SetActive(false);
				this.rightHand.gameObject.SetActive(false);
				this.text.text = "WAITING TO START";
				return;
			}
			else
			{
				this.myTarget = GorillaGameManager.instance.GetComponent<GorillaHuntManager>().GetTargetOf(PhotonNetwork.LocalPlayer);
				if (this.myTarget == null)
				{
					this.material.gameObject.SetActive(false);
					this.hat.gameObject.SetActive(false);
					this.face.gameObject.SetActive(false);
					this.badge.gameObject.SetActive(false);
					this.leftHand.gameObject.SetActive(false);
					this.rightHand.gameObject.SetActive(false);
					this.text.text = "YOU ARE DEAD\nTAG OTHERS\nTO SLOW THEM";
					return;
				}
				if (GorillaGameManager.instance.FindVRRigForPlayer(this.myTarget) != null)
				{
					this.myRig = GorillaGameManager.instance.FindPlayerVRRig(this.myTarget);
					if (this.myRig != null)
					{
						this.material.material = this.myRig.materialsToChangeTo[this.myRig.setMatIndex];
						Text text = this.text;
						string[] array = new string[5];
						array[0] = "TARGET:\n";
						int num = 1;
						bool doIt = true;
						Photon.Realtime.Player creator = this.myRig.creator;
						array[num] = this.NormalizeName(doIt, (creator != null) ? creator.NickName : null);
						array[2] = "\nDISTANCE: ";
						array[3] = Mathf.CeilToInt((GorillaLocomotion.Player.Instance.headCollider.transform.position - this.myRig.transform.position).magnitude).ToString();
						array[4] = "M";
						text.text = string.Concat(array);
						this.SetImage(this.myRig.cosmeticSet.items[0].displayName, ref this.hat);
						this.SetImage(this.myRig.cosmeticSet.items[2].displayName, ref this.face);
						this.SetImage(this.myRig.cosmeticSet.items[1].displayName, ref this.badge);
						this.SetImage(this.GetPrioritizedItemForHand(this.myRig, true).displayName, ref this.leftHand);
						this.SetImage(this.GetPrioritizedItemForHand(this.myRig, false).displayName, ref this.rightHand);
						this.material.gameObject.SetActive(true);
						return;
					}
				}
			}
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
		}
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00039470 File Offset: 0x00037670
	private void SetImage(string itemDisplayName, ref Image image)
	{
		this.tempItem = CosmeticsController.instance.GetItemFromDict(CosmeticsController.instance.GetItemNameFromDisplayName(itemDisplayName));
		if (this.tempItem.displayName != "NOTHING" && this.myRig != null && this.myRig.IsItemAllowed(this.tempItem.itemName))
		{
			image.gameObject.SetActive(true);
			image.sprite = this.tempItem.itemPicture;
			return;
		}
		image.gameObject.SetActive(false);
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x00039508 File Offset: 0x00037708
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x00039580 File Offset: 0x00037780
	public CosmeticsController.CosmeticItem GetPrioritizedItemForHand(VRRig targetRig, bool forLeftHand)
	{
		if (forLeftHand)
		{
			CosmeticsController.CosmeticItem cosmeticItem = targetRig.cosmeticSet.items[7];
			if (cosmeticItem.displayName != "null")
			{
				return cosmeticItem;
			}
			cosmeticItem = targetRig.cosmeticSet.items[4];
			if (cosmeticItem.displayName != "null")
			{
				return cosmeticItem;
			}
			return targetRig.cosmeticSet.items[5];
		}
		else
		{
			CosmeticsController.CosmeticItem cosmeticItem = targetRig.cosmeticSet.items[8];
			if (cosmeticItem.displayName != "null")
			{
				return cosmeticItem;
			}
			cosmeticItem = targetRig.cosmeticSet.items[3];
			if (cosmeticItem.displayName != "null")
			{
				return cosmeticItem;
			}
			return targetRig.cosmeticSet.items[6];
		}
	}

	// Token: 0x04000B93 RID: 2963
	public Text text;

	// Token: 0x04000B94 RID: 2964
	public Image material;

	// Token: 0x04000B95 RID: 2965
	public Image hat;

	// Token: 0x04000B96 RID: 2966
	public Image face;

	// Token: 0x04000B97 RID: 2967
	public Image badge;

	// Token: 0x04000B98 RID: 2968
	public Image leftHand;

	// Token: 0x04000B99 RID: 2969
	public Image rightHand;

	// Token: 0x04000B9A RID: 2970
	public Photon.Realtime.Player myTarget;

	// Token: 0x04000B9B RID: 2971
	public Photon.Realtime.Player tempTarget;

	// Token: 0x04000B9C RID: 2972
	[DebugReadout]
	public VRRig myRig;

	// Token: 0x04000B9D RID: 2973
	public Sprite tempSprite;

	// Token: 0x04000B9E RID: 2974
	public CosmeticsController.CosmeticItem tempItem;
}
