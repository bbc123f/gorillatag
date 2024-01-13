using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GorillaScoreboardSpawner : MonoBehaviourPunCallbacks
{
	public string gameType;

	public bool includeMMR;

	public GameObject scoreboardPrefab;

	public GameObject notInRoomText;

	public GameObject controllingParentGameObject;

	public bool isActive = true;

	public GorillaScoreBoard currentScoreboard;

	public bool lastVisible;

	public void Awake()
	{
		StartCoroutine(UpdateBoard());
	}

	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	public override void OnJoinedRoom()
	{
		if (IsCurrentScoreboard())
		{
			notInRoomText.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(scoreboardPrefab, base.transform);
			currentScoreboard = gameObject.GetComponent<GorillaScoreBoard>();
			gameObject.transform.rotation = base.transform.rotation;
			if (includeMMR)
			{
				gameObject.GetComponent<GorillaScoreBoard>().includeMMR = true;
				gameObject.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		OnLeftRoom();
	}

	private IEnumerator UpdateBoard()
	{
		while (true)
		{
			try
			{
				if (currentScoreboard != null)
				{
					bool activeSelf = controllingParentGameObject.activeSelf;
					foreach (GorillaPlayerScoreboardLine line in currentScoreboard.lines)
					{
						if (activeSelf != line.lastVisible)
						{
							line.HideShowLine(activeSelf);
						}
						line.lastVisible = activeSelf;
					}
					if (currentScoreboard.boardText.enabled != activeSelf)
					{
						currentScoreboard.boardText.enabled = activeSelf;
					}
					if (currentScoreboard.buttonText.enabled != activeSelf)
					{
						currentScoreboard.buttonText.enabled = activeSelf;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public override void OnLeftRoom()
	{
		if (currentScoreboard != null)
		{
			Object.Destroy(currentScoreboard.gameObject);
			currentScoreboard = null;
		}
		if ((bool)notInRoomText)
		{
			notInRoomText.SetActive(value: true);
		}
	}
}
