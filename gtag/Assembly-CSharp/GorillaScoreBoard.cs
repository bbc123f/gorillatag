using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000183 RID: 387
public class GorillaScoreBoard : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
	// Token: 0x060009CC RID: 2508 RVA: 0x0003BC2B File Offset: 0x00039E2B
	public void Awake()
	{
		PhotonNetwork.AddCallbackTarget(this);
		if (PhotonNetwork.InRoom && GorillaGameManager.instance != null)
		{
			this.boardText.text = this.GetBeginningString();
		}
		base.StartCoroutine(this.InfrequentUpdateCoroutine());
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0003BC68 File Offset: 0x00039E68
	public string GetBeginningString()
	{
		if (GorillaGameManager.instance != null)
		{
			return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE- GAME MODE: " : (PhotonNetwork.CurrentRoom.Name + "    GAME MODE: ")) + GorillaGameManager.instance.GameMode() + "\n   PLAYER      COLOR   MUTE   REPORT";
		}
		return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE-" : PhotonNetwork.CurrentRoom.Name) + "\n   PLAYER      COLOR   MUTE   REPORT";
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0003BCF3 File Offset: 0x00039EF3
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

	// Token: 0x060009CF RID: 2511 RVA: 0x0003BD04 File Offset: 0x00039F04
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

	// Token: 0x060009D0 RID: 2512 RVA: 0x0003BFA8 File Offset: 0x0003A1A8
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.InfrequentUpdate(true);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0003BFB4 File Offset: 0x0003A1B4
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

	// Token: 0x060009D2 RID: 2514 RVA: 0x0003C158 File Offset: 0x0003A358
	void IOnEventCallback.OnEvent(EventData photonEvent)
	{
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0003C15A File Offset: 0x0003A35A
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

	// Token: 0x060009D4 RID: 2516 RVA: 0x0003C178 File Offset: 0x0003A378
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

	// Token: 0x060009D5 RID: 2517 RVA: 0x0003C1B4 File Offset: 0x0003A3B4
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

	// Token: 0x04000C0B RID: 3083
	public GameObject scoreBoardLinePrefab;

	// Token: 0x04000C0C RID: 3084
	public int startingYValue;

	// Token: 0x04000C0D RID: 3085
	public int lineHeight;

	// Token: 0x04000C0E RID: 3086
	public GorillaGameManager gameManager;

	// Token: 0x04000C0F RID: 3087
	public string gameType;

	// Token: 0x04000C10 RID: 3088
	public bool includeMMR;

	// Token: 0x04000C11 RID: 3089
	public bool isActive;

	// Token: 0x04000C12 RID: 3090
	public List<GorillaPlayerScoreboardLine> lines;

	// Token: 0x04000C13 RID: 3091
	public Text boardText;

	// Token: 0x04000C14 RID: 3092
	public Text buttonText;

	// Token: 0x04000C15 RID: 3093
	private Player playerForVRRig;

	// Token: 0x04000C16 RID: 3094
	private int i;

	// Token: 0x04000C17 RID: 3095
	private VRRig currentRig;

	// Token: 0x04000C18 RID: 3096
	private Player outPlayer;
}
