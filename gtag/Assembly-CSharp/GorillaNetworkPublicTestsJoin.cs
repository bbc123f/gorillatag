using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox
{
	// Token: 0x06000C5F RID: 3167 RVA: 0x0004B34F File Offset: 0x0004954F
	public void Awake()
	{
		this.count = 0;
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0004B358 File Offset: 0x00049558
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

	// Token: 0x06000C61 RID: 3169 RVA: 0x0004B59C File Offset: 0x0004979C
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

	// Token: 0x04000FDC RID: 4060
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04000FDD RID: 4061
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04000FDE RID: 4062
	public string gameModeName;

	// Token: 0x04000FDF RID: 4063
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04000FE0 RID: 4064
	public string componentTypeToAdd;

	// Token: 0x04000FE1 RID: 4065
	public GameObject componentTarget;

	// Token: 0x04000FE2 RID: 4066
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04000FE3 RID: 4067
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04000FE4 RID: 4068
	private Transform tosPition;

	// Token: 0x04000FE5 RID: 4069
	private Transform othsTosPosition;

	// Token: 0x04000FE6 RID: 4070
	private PhotonView fotVew;

	// Token: 0x04000FE7 RID: 4071
	private int count;

	// Token: 0x04000FE8 RID: 4072
	private bool waiting;

	// Token: 0x04000FE9 RID: 4073
	private Vector3 lastPosition;

	// Token: 0x04000FEA RID: 4074
	private VRRig tempRig;
}
