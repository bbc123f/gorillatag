using System;
using System.Collections;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B5 RID: 437
public class GorillaScoreboardSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B26 RID: 2854 RVA: 0x00044D69 File Offset: 0x00042F69
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x00044D78 File Offset: 0x00042F78
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00044D88 File Offset: 0x00042F88
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

	// Token: 0x06000B29 RID: 2857 RVA: 0x00044E01 File Offset: 0x00043001
	public override void OnDisconnected(DisconnectCause cause)
	{
		this.OnLeftRoom();
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x00044E09 File Offset: 0x00043009
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GorillaLocomotion.Player.Instance.inOverlay;
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00044E29 File Offset: 0x00043029
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

	// Token: 0x06000B2C RID: 2860 RVA: 0x00044E38 File Offset: 0x00043038
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

	// Token: 0x04000E7E RID: 3710
	public string gameType;

	// Token: 0x04000E7F RID: 3711
	public bool includeMMR;

	// Token: 0x04000E80 RID: 3712
	public GameObject scoreboardPrefab;

	// Token: 0x04000E81 RID: 3713
	public GameObject notInRoomText;

	// Token: 0x04000E82 RID: 3714
	public GameObject controllingParentGameObject;

	// Token: 0x04000E83 RID: 3715
	public bool isActive = true;

	// Token: 0x04000E84 RID: 3716
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x04000E85 RID: 3717
	public bool lastVisible;

	// Token: 0x04000E86 RID: 3718
	public bool forOverlay;
}
