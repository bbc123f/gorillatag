using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	public void Awake()
	{
		this.count = 0;
	}

	public void LateUpdate()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (((!Player.Instance.GetComponent<Rigidbody>().useGravity && !Player.Instance.isClimbing) || Player.Instance.GetComponent<Rigidbody>().isKinematic) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((Player.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || Player.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (Player.Instance.transform.position - this.lastPosition).magnitude;
			}
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
			{
				this.tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
				if (this.tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (this.tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
				{
					this.count++;
					if (this.count >= 3 && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
					{
						base.StartCoroutine(this.GracePeriod());
					}
				}
			}
			else
			{
				this.count = 0;
			}
			this.lastPosition = Player.Instance.transform.position;
		}
		catch
		{
		}
	}

	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if ((!Player.Instance.GetComponent<Rigidbody>().useGravity && !Player.Instance.isClimbing) || Player.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					GorillaNot.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (Player.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || Player.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					GorillaNot.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						Player.Instance.jumpMultiplier.ToString(),
						".",
						Player.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
			{
				this.tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
				if (this.tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (this.tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
				{
					this.count++;
					if (this.count >= 3)
					{
						GorillaNot.instance.SendReport("tee hee", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
					}
				}
			}
			else
			{
				this.count = 0;
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	public GameObject[] makeSureThisIsDisabled;

	public GameObject[] makeSureThisIsEnabled;

	public string gameModeName;

	public PhotonNetworkController photonNetworkController;

	public string componentTypeToAdd;

	public GameObject componentTarget;

	public GorillaLevelScreen[] joinScreens;

	public GorillaLevelScreen[] leaveScreens;

	private Transform tosPition;

	private Transform othsTosPosition;

	private PhotonView fotVew;

	private int count;

	private bool waiting;

	private Vector3 lastPosition;

	private VRRig tempRig;
}
