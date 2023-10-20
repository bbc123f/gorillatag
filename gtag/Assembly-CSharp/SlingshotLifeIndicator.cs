using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class SlingshotLifeIndicator : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	// Token: 0x06000551 RID: 1361 RVA: 0x00021EC8 File Offset: 0x000200C8
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

	// Token: 0x06000552 RID: 1362 RVA: 0x00021EF0 File Offset: 0x000200F0
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

	// Token: 0x06000553 RID: 1363 RVA: 0x00022024 File Offset: 0x00020224
	public override void OnDisable()
	{
		base.OnDisable();
		this.Reset();
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x00022032 File Offset: 0x00020232
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.Reset();
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00022040 File Offset: 0x00020240
	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	// Token: 0x04000646 RID: 1606
	public VRRig myRig;

	// Token: 0x04000647 RID: 1607
	public GorillaBattleManager bMgr;

	// Token: 0x04000648 RID: 1608
	public Player myPlayer;

	// Token: 0x04000649 RID: 1609
	public bool checkedBattle;

	// Token: 0x0400064A RID: 1610
	public bool inBattle;

	// Token: 0x0400064B RID: 1611
	public GameObject indicator1;

	// Token: 0x0400064C RID: 1612
	public GameObject indicator2;

	// Token: 0x0400064D RID: 1613
	public GameObject indicator3;
}
