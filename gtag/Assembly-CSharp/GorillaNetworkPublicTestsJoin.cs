using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox
{
	// Token: 0x06000C65 RID: 3173 RVA: 0x0004B5B7 File Offset: 0x000497B7
	public void Awake()
	{
		this.count = 0;
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0004B5C0 File Offset: 0x000497C0
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

	// Token: 0x06000C67 RID: 3175 RVA: 0x0004B804 File Offset: 0x00049A04
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

	// Token: 0x04000FE0 RID: 4064
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04000FE1 RID: 4065
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04000FE2 RID: 4066
	public string gameModeName;

	// Token: 0x04000FE3 RID: 4067
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04000FE4 RID: 4068
	public string componentTypeToAdd;

	// Token: 0x04000FE5 RID: 4069
	public GameObject componentTarget;

	// Token: 0x04000FE6 RID: 4070
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04000FE7 RID: 4071
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04000FE8 RID: 4072
	private Transform tosPition;

	// Token: 0x04000FE9 RID: 4073
	private Transform othsTosPosition;

	// Token: 0x04000FEA RID: 4074
	private PhotonView fotVew;

	// Token: 0x04000FEB RID: 4075
	private int count;

	// Token: 0x04000FEC RID: 4076
	private bool waiting;

	// Token: 0x04000FED RID: 4077
	private Vector3 lastPosition;

	// Token: 0x04000FEE RID: 4078
	private VRRig tempRig;
}
