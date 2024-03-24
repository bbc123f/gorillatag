using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
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

	private void OnDestroy()
	{
	}

	public string GetBeginningString()
	{
		return "ROOM ID: " + (NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME MODE: " : (NetworkSystem.Instance.RoomName + "    GAME MODE: ")) + this.RoomType() + "\n   PLAYER      COLOR   MUTE   REPORT";
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
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		for (int i = 0; i < this.lines.Count; i++)
		{
			try
			{
				if (this.lines[i].gameObject.activeInHierarchy)
				{
					this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
					if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
					{
						this.stringBuilder.Append("\n ");
						this.stringBuilder.Append(this.NormalizeName(true, PlayFabAuthenticator.instance.GetSafety() ? this.lines[i].linePlayer.DefaultName : this.lines[i].linePlayer.NickName));
						if (this.lines[i].linePlayer != NetworkSystem.Instance.LocalPlayer)
						{
							if (this.lines[i].reportButton.isActiveAndEnabled)
							{
								this.buttonStringBuilder.Append(" MUTE                                REPORT\n");
							}
							else
							{
								this.buttonStringBuilder.Append(" MUTE                HATE SPEECH    TOXICITY      CHEATING      CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("\n");
						}
					}
				}
			}
			catch
			{
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
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

	public GorillaScoreBoard()
	{
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

	private StringBuilder stringBuilder = new StringBuilder(220);

	private StringBuilder buttonStringBuilder = new StringBuilder(720);

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal bool <NormalizeName>b__25_0(char c)
		{
			return char.IsLetterOrDigit(c);
		}

		public static readonly GorillaScoreBoard.<>c <>9 = new GorillaScoreBoard.<>c();

		public static Predicate<char> <>9__25_0;
	}
}
