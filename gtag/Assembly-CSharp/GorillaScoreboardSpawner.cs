using System;
using System.Collections;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B6 RID: 438
public class GorillaScoreboardSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B2C RID: 2860 RVA: 0x00044FD1 File Offset: 0x000431D1
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00044FE0 File Offset: 0x000431E0
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00044FF0 File Offset: 0x000431F0
	public override void OnJoinedRoom()
	{
		if (this.IsCurrentScoreboard())
		{
			this.notInRoomText.SetActive(false);
			GameObject gameObject = Object.Instantiate<GameObject>(this.scoreboardPrefab, base.transform);
			this.currentScoreboard = gameObject.GetComponent<GorillaScoreBoard>();
			gameObject.transform.rotation = base.transform.rotation;
			if (this.includeMMR)
			{
				gameObject.GetComponent<GorillaScoreBoard>().includeMMR = true;
				gameObject.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00045069 File Offset: 0x00043269
	public override void OnDisconnected(DisconnectCause cause)
	{
		this.OnLeftRoom();
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00045071 File Offset: 0x00043271
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GorillaLocomotion.Player.Instance.inOverlay;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x00045091 File Offset: 0x00043291
	private IEnumerator UpdateBoard()
	{
		for (;;)
		{
			try
			{
				if (this.currentScoreboard != null)
				{
					bool flag = this.IsVisible();
					foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
					{
						if (flag != gorillaPlayerScoreboardLine.lastVisible)
						{
							gorillaPlayerScoreboardLine.HideShowLine(flag);
						}
						gorillaPlayerScoreboardLine.lastVisible = flag;
					}
					if (this.currentScoreboard.boardText.enabled != flag)
					{
						this.currentScoreboard.boardText.enabled = flag;
					}
					if (this.currentScoreboard.buttonText.enabled != flag)
					{
						this.currentScoreboard.buttonText.enabled = flag;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000450A0 File Offset: 0x000432A0
	public override void OnLeftRoom()
	{
		if (this.currentScoreboard != null)
		{
			Object.Destroy(this.currentScoreboard.gameObject);
			this.currentScoreboard = null;
		}
		if (this.notInRoomText)
		{
			this.notInRoomText.SetActive(true);
		}
	}

	// Token: 0x04000E82 RID: 3714
	public string gameType;

	// Token: 0x04000E83 RID: 3715
	public bool includeMMR;

	// Token: 0x04000E84 RID: 3716
	public GameObject scoreboardPrefab;

	// Token: 0x04000E85 RID: 3717
	public GameObject notInRoomText;

	// Token: 0x04000E86 RID: 3718
	public GameObject controllingParentGameObject;

	// Token: 0x04000E87 RID: 3719
	public bool isActive = true;

	// Token: 0x04000E88 RID: 3720
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x04000E89 RID: 3721
	public bool lastVisible;

	// Token: 0x04000E8A RID: 3722
	public bool forOverlay;
}
