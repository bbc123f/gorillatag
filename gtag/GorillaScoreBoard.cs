using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GorillaScoreBoard : MonoBehaviour
{
	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		this.linesParent.SetActive(awake);
	}

	public string GetBeginningString()
	{
		return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE- GAME MODE: " : (PhotonNetwork.CurrentRoom.Name + "    GAME MODE: ")) + this.RoomType() + "\n   PLAYER      COLOR   MUTE   REPORT";
	}

	public string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		for (int i = 0; i < count; i++)
		{
			this.tempGmName = this.gmNames[i];
			if (this.initialGameMode.Contains(this.tempGmName))
			{
				this.gmName = this.tempGmName;
				break;
			}
		}
		return this.gmName;
	}

	public void RedrawPlayerLines()
	{
		this.boardText.text = this.GetBeginningString();
		this.buttonText.text = "";
		for (int i = 0; i < this.lines.Count; i++)
		{
			try
			{
				if (this.lines[i].gameObject.activeInHierarchy)
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
			}
			catch
			{
			}
		}
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

	private void Start()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
	}

	public GameObject scoreBoardLinePrefab;

	public int startingYValue;

	public int lineHeight;

	public GorillaGameManager gameManager;

	public string gameType;

	public bool includeMMR;

	public bool isActive;

	public GameObject linesParent;

	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	public Text boardText;

	public Text buttonText;

	public bool needsUpdate;

	public Text notInRoomText;

	public string initialGameMode;

	private string tempGmName;

	private string gmName;

	private const string error = "ERROR";

	private List<string> gmNames;
}
