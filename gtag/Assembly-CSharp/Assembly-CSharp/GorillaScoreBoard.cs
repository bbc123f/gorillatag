using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GorillaScoreBoard : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
	public void Awake()
	{
		PhotonNetwork.AddCallbackTarget(this);
		if (PhotonNetwork.InRoom && GorillaGameManager.instance != null)
		{
			this.boardText.text = this.GetBeginningString();
		}
		base.StartCoroutine(this.InfrequentUpdateCoroutine());
	}

	public string GetBeginningString()
	{
		if (GorillaGameManager.instance != null)
		{
			return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE- GAME MODE: " : (PhotonNetwork.CurrentRoom.Name + "    GAME MODE: ")) + GorillaGameManager.instance.GameMode() + "\n   PLAYER      COLOR   MUTE   REPORT";
		}
		return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE-" : PhotonNetwork.CurrentRoom.Name) + "\n   PLAYER      COLOR   MUTE   REPORT";
	}

	private IEnumerator InfrequentUpdateCoroutine()
	{
		yield return new WaitForSeconds(Random.Range(0f, 1f));
		for (;;)
		{
			this.InfrequentUpdate(false);
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private void InfrequentUpdate(bool forcedRefresh)
	{
		try
		{
			bool flag = false;
			this.i = this.lines.Count - 1;
			while (this.i > -1)
			{
				if (this.lines[this.i] == null)
				{
					this.lines.RemoveAt(this.i);
					flag = true;
				}
				else if (this.lines[this.i].linePlayer == null || !PhotonNetwork.CurrentRoom.Players.TryGetValue(this.lines[this.i].linePlayer.ActorNumber, out this.outPlayer) || (PhotonNetwork.CurrentRoom.Players.TryGetValue(this.lines[this.i].linePlayer.ActorNumber, out this.outPlayer) && this.outPlayer == null))
				{
					this.lines[this.i].enabled = false;
					Object.Destroy(this.lines[this.i].gameObject);
					this.lines.RemoveAt(this.i);
					flag = true;
				}
				this.i--;
			}
			Player[] array = (GorillaGameManager.instance != null) ? GorillaGameManager.instance.currentPlayerArray : PhotonNetwork.PlayerList;
			if (PhotonNetwork.CurrentRoom != null && this.lines.Count != array.Length)
			{
				foreach (Player player in array)
				{
					if (player != null)
					{
						bool flag2 = false;
						using (List<GorillaPlayerScoreboardLine>.Enumerator enumerator = this.lines.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.playerActorNumber == player.ActorNumber)
								{
									flag2 = true;
								}
							}
						}
						if (!flag2 && player.InRoom())
						{
							GorillaPlayerScoreboardLine component = Object.Instantiate<GameObject>(this.scoreBoardLinePrefab, base.transform).GetComponent<GorillaPlayerScoreboardLine>();
							this.lines.Add(component);
							component.playerActorNumber = player.ActorNumber;
							component.linePlayer = player;
							component.playerNameValue = player.NickName;
							RigContainer rigContainer;
							if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
							{
								component.rigContainer = rigContainer;
								component.playerVRRig = rigContainer.Rig;
							}
							flag = true;
						}
					}
				}
			}
			if (flag || forcedRefresh)
			{
				this.RedrawPlayerLines();
			}
		}
		catch
		{
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.InfrequentUpdate(true);
	}

	public void RedrawPlayerLines()
	{
		this.lines.Sort((GorillaPlayerScoreboardLine line1, GorillaPlayerScoreboardLine line2) => line1.playerActorNumber.CompareTo(line2.playerActorNumber));
		this.boardText.text = this.GetBeginningString();
		this.buttonText.text = "";
		for (int i = 0; i < this.lines.Count; i++)
		{
			try
			{
				this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
				if (this.lines[i].linePlayer != null)
				{
					Text text = this.boardText;
					text.text = text.text + "\n " + this.NormalizeName(true, this.lines[i].linePlayer.NickName);
					if (this.lines[i].linePlayer != PhotonNetwork.LocalPlayer)
					{
						if (this.lines[i].reportButton.isActiveAndEnabled)
						{
							Text text2 = this.buttonText;
							text2.text += "MUTE                                REPORT\n";
						}
						else
						{
							Text text3 = this.buttonText;
							text3.text += "MUTE                HATE SPEECH    TOXICITY      CHEATING      CANCEL\n";
						}
					}
					else
					{
						Text text4 = this.buttonText;
						text4.text += "\n";
					}
				}
			}
			catch
			{
			}
		}
	}

	void IOnEventCallback.OnEvent(EventData photonEvent)
	{
	}

	public IEnumerator RefreshData(int actorNumber1, int actorNumber2)
	{
		yield return new WaitForSeconds(1f);
		using (List<GorillaPlayerScoreboardLine>.Enumerator enumerator = this.lines.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine = enumerator.Current;
				if (gorillaPlayerScoreboardLine.playerActorNumber != actorNumber1)
				{
					int playerActorNumber = gorillaPlayerScoreboardLine.playerActorNumber;
				}
			}
			yield break;
		}
		yield break;
	}

	private int GetActorIDFromUserID(string userID)
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (player.UserId == userID)
			{
				return player.ActorNumber;
			}
		}
		return -1;
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text.Substring(0, 10);
			}
			text = text.ToUpper();
		}
		return text;
	}

	public GameObject scoreBoardLinePrefab;

	public int startingYValue;

	public int lineHeight;

	public GorillaGameManager gameManager;

	public string gameType;

	public bool includeMMR;

	public bool isActive;

	public List<GorillaPlayerScoreboardLine> lines;

	public Text boardText;

	public Text buttonText;

	private Player playerForVRRig;

	private int i;

	private VRRig currentRig;

	private Player outPlayer;
}
