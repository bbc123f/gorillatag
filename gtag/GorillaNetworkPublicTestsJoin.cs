using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox
{
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

	public void Awake()
	{
		count = 0;
	}

	public void LateUpdate()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (((!Player.Instance.GetComponent<Rigidbody>().useGravity && !Player.Instance.isClimbing) || Player.Instance.GetComponent<Rigidbody>().isKinematic) && !waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					StartCoroutine(GracePeriod());
				}
				if ((Player.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || Player.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					StartCoroutine(GracePeriod());
				}
				_ = (Player.Instance.transform.position - lastPosition).magnitude;
				_ = 4f;
			}
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
			{
				tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
				if (tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
				{
					count++;
					if (count >= 3 && !waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
					{
						StartCoroutine(GracePeriod());
					}
				}
			}
			else
			{
				count = 0;
			}
			lastPosition = Player.Instance.transform.position;
		}
		catch
		{
		}
	}

	private IEnumerator GracePeriod()
	{
		waiting = true;
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
					GorillaNot.instance.SendReport("jimp 2mcuh." + Player.Instance.jumpMultiplier + "." + Player.Instance.maxJumpSpeed + ".", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
			{
				tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
				if (tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
				{
					count++;
					if (count >= 3)
					{
						GorillaNot.instance.SendReport("tee hee", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
					}
				}
			}
			else
			{
				count = 0;
			}
			waiting = false;
		}
		catch
		{
		}
	}
}
