using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SlingshotLifeIndicator : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	public VRRig myRig;

	public GorillaBattleManager bMgr;

	public Player myPlayer;

	public bool checkedBattle;

	public bool inBattle;

	public GameObject indicator1;

	public GameObject indicator2;

	public GameObject indicator3;

	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(value: true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(value: false);
		}
	}

	private void LateUpdate()
	{
		if (PhotonNetwork.InRoom && (!checkedBattle || inBattle))
		{
			if (bMgr == null)
			{
				checkedBattle = true;
				inBattle = true;
				if (GorillaGameManager.instance == null)
				{
					return;
				}
				bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
				if (bMgr == null)
				{
					inBattle = false;
					return;
				}
			}
			if (myRig?.creator != null)
			{
				int playerLives = bMgr.GetPlayerLives(myRig.creator);
				SetActive(indicator1, playerLives >= 1);
				SetActive(indicator2, playerLives >= 2);
				SetActive(indicator3, playerLives >= 3);
			}
		}
		else
		{
			if (indicator1.activeSelf)
			{
				indicator1.SetActive(value: false);
			}
			if (indicator2.activeSelf)
			{
				indicator2.SetActive(value: false);
			}
			if (indicator3.activeSelf)
			{
				indicator3.SetActive(value: false);
			}
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		Reset();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		Reset();
	}

	public void Reset()
	{
		bMgr = null;
		inBattle = false;
		checkedBattle = false;
	}
}
