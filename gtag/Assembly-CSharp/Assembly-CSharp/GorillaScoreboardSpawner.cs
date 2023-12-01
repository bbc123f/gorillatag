using System;
using System.Collections;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GorillaScoreboardSpawner : MonoBehaviourPunCallbacks
{
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

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

	public override void OnDisconnected(DisconnectCause cause)
	{
		this.OnLeftRoom();
	}

	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GorillaLocomotion.Player.Instance.inOverlay;
	}

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

	public string gameType;

	public bool includeMMR;

	public GameObject scoreboardPrefab;

	public GameObject notInRoomText;

	public GameObject controllingParentGameObject;

	public bool isActive = true;

	public GorillaScoreBoard currentScoreboard;

	public bool lastVisible;

	public bool forOverlay;
}
