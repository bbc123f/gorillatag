using System;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GorillaHuntComputer : MonoBehaviour
{
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

	public Text text;

	public Image material;

	public Image hat;

	public Image face;

	public Image badge;

	public Image leftHand;

	public Image rightHand;

	public Photon.Realtime.Player myTarget;

	public Photon.Realtime.Player tempTarget;

	[DebugReadout]
	public VRRig myRig;

	public Sprite tempSprite;

	public CosmeticsController.CosmeticItem tempItem;
}
