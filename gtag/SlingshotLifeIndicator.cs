using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SlingshotLifeIndicator : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(false);
		}
	}

	private void LateUpdate()
	{
		if (!PhotonNetwork.InRoom || (this.checkedBattle && !this.inBattle))
		{
			if (this.indicator1.activeSelf)
			{
				this.indicator1.SetActive(false);
			}
			if (this.indicator2.activeSelf)
			{
				this.indicator2.SetActive(false);
			}
			if (this.indicator3.activeSelf)
			{
				this.indicator3.SetActive(false);
			}
			return;
		}
		if (this.bMgr == null)
		{
			this.checkedBattle = true;
			this.inBattle = true;
			if (GorillaGameManager.instance == null)
			{
				return;
			}
			this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
			if (this.bMgr == null)
			{
				this.inBattle = false;
				return;
			}
		}
		VRRig vrrig = this.myRig;
		if (((vrrig != null) ? vrrig.creator : null) == null)
		{
			return;
		}
		int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
		this.SetActive(this.indicator1, playerLives >= 1);
		this.SetActive(this.indicator2, playerLives >= 2);
		this.SetActive(this.indicator3, playerLives >= 3);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		this.Reset();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.Reset();
	}

	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	public VRRig myRig;

	public GorillaBattleManager bMgr;

	public Player myPlayer;

	public bool checkedBattle;

	public bool inBattle;

	public GameObject indicator1;

	public GameObject indicator2;

	public GameObject indicator3;
}
