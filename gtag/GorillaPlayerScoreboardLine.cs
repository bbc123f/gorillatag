using System;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class GorillaPlayerScoreboardLine : MonoBehaviour
{
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
	}

	public void InitializeLine()
	{
		this.myVoiceView = null;
		this.UpdatePlayerText();
		if (this.linePlayer == PhotonNetwork.LocalPlayer)
		{
			this.muteButton.gameObject.SetActive(false);
			this.reportButton.gameObject.SetActive(false);
			this.hateSpeechButton.SetActive(false);
			this.toxicityButton.SetActive(false);
			this.cheatingButton.SetActive(false);
			this.cancelButton.SetActive(false);
			return;
		}
		this.muteButton.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null && GorillaScoreboardTotalUpdater.instance.reportDict.ContainsKey(this.playerActorNumber))
		{
			GorillaScoreboardTotalUpdater.PlayerReports playerReports = GorillaScoreboardTotalUpdater.instance.reportDict[this.playerActorNumber];
			this.reportedCheating = playerReports.cheating;
			this.reportedHateSpeech = playerReports.hateSpeech;
			this.reportedToxicity = playerReports.toxicity;
			this.reportInProgress = playerReports.pressedReport;
		}
		else
		{
			this.reportedCheating = false;
			this.reportedHateSpeech = false;
			this.reportedToxicity = false;
			this.reportInProgress = false;
		}
		this.reportButton.isOn = this.reportedCheating || this.reportedHateSpeech || this.reportedToxicity;
		this.reportButton.UpdateColor();
		this.SwapToReportState(this.reportInProgress);
		this.muteButton.gameObject.SetActive(true);
		this.mute = PlayerPrefs.GetInt(this.linePlayer.UserId, 0);
		this.muteButton.isOn = this.mute != 0;
		this.muteButton.UpdateColor();
		if (this.rigContainer != null)
		{
			this.rigContainer.Muted = this.mute != 0;
		}
	}

	public void SetLineData(Player player)
	{
		if (player == this.linePlayer)
		{
			return;
		}
		if (this.playerActorNumber != player.ActorNumber)
		{
			this.initTime = Time.time;
		}
		this.playerActorNumber = player.ActorNumber;
		this.linePlayer = player;
		this.playerNameValue = player.NickName;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.rigContainer = rigContainer;
			this.playerVRRig = rigContainer.Rig;
		}
		this.InitializeLine();
	}

	public void UpdateLine()
	{
		try
		{
			if (this.linePlayer != null)
			{
				if (this.playerName.text != this.playerVRRig.playerText.text)
				{
					this.UpdatePlayerText();
				}
				if (this.rigContainer != null)
				{
					if (Time.time > this.initTime + this.emptyRigCooldown)
					{
						if (this.playerVRRig.photonView != null)
						{
							this.emptyRigCount = 0;
						}
						else
						{
							this.emptyRigCount++;
							if (this.emptyRigCount > 5)
							{
								GorillaNot.instance.SendReport("empty rig", this.linePlayer.UserId, this.linePlayer.NickName);
							}
						}
					}
					if (this.playerVRRig.setMatIndex != this.currentMatIndex && this.playerVRRig.setMatIndex != 0 && this.playerVRRig.setMatIndex > -1 && this.playerVRRig.setMatIndex < this.playerVRRig.materialsToChangeTo.Length)
					{
						this.playerSwatch.material = this.playerVRRig.materialsToChangeTo[this.playerVRRig.setMatIndex];
						this.currentMatIndex = this.playerVRRig.setMatIndex;
					}
					if (this.playerVRRig.setMatIndex == 0 && this.playerSwatch.material != null)
					{
						this.playerSwatch.material = null;
						this.currentMatIndex = 0;
					}
					if (this.playerSwatch.color != this.playerVRRig.materialsToChangeTo[0].color)
					{
						this.playerSwatch.color = this.playerVRRig.materialsToChangeTo[0].color;
					}
					if (this.myVoiceView == null)
					{
						this.myVoiceView = this.rigContainer.Voice;
					}
					if (this.myRecorder == null)
					{
						this.myRecorder = PhotonNetworkController.Instance.GetComponent<Recorder>();
					}
					if (this.playerVRRig != null)
					{
						if (this.playerVRRig.remoteUseReplacementVoice || this.playerVRRig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE")
						{
							if (this.playerVRRig.SpeakingLoudness > this.playerVRRig.replacementVoiceLoudnessThreshold && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
							{
								this.speakerIcon.enabled = true;
							}
							else
							{
								this.speakerIcon.enabled = false;
							}
						}
						else if ((this.myVoiceView != null && this.myVoiceView.IsSpeaking) || (this.playerVRRig.photonView.IsMine && this.myRecorder != null && this.myRecorder.IsCurrentlyTransmitting))
						{
							this.speakerIcon.enabled = true;
						}
						else
						{
							this.speakerIcon.enabled = false;
						}
					}
					else
					{
						this.speakerIcon.enabled = false;
					}
				}
			}
		}
		catch
		{
		}
	}

	private void UpdatePlayerText()
	{
		if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
		{
			this.playerName.text = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
			this.currentNickname = this.linePlayer.NickName;
			return;
		}
		if (this.rigContainer.Rig.Initialized)
		{
			this.playerName.text = this.playerVRRig.playerText.text;
		}
		else if (this.currentNickname == null || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
		{
			this.playerName.text = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
		}
		this.currentNickname = this.linePlayer.NickName;
	}

	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		if (buttonType != GorillaPlayerLineButton.ButtonType.Mute)
		{
			if (buttonType == GorillaPlayerLineButton.ButtonType.Report)
			{
				this.SetReportState(true, buttonType);
				return;
			}
			this.SetReportState(false, buttonType);
		}
		else if (this.linePlayer != null && this.playerVRRig != null)
		{
			this.mute = (isOn ? 1 : 0);
			PlayerPrefs.SetInt(this.linePlayer.UserId, this.mute);
			if (this.rigContainer != null)
			{
				this.rigContainer.Muted = this.mute != 0;
			}
			PlayerPrefs.Save();
			this.muteButton.UpdateColor();
			GorillaPlayerScoreboardLine.MutePlayer(this.linePlayer.UserId, this.playerName.text, this.mute);
			return;
		}
	}

	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		this.canPressNextReportButton = buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report;
		this.reportInProgress = reportState;
		if (reportState)
		{
			this.SwapToReportState(true);
		}
		else
		{
			this.SwapToReportState(false);
			if (this.linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				if ((!this.reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!this.reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!this.reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
				{
					GorillaPlayerScoreboardLine.ReportPlayer(this.linePlayer.UserId, buttonType, this.playerName.text);
					this.doneReporting = true;
				}
				this.reportedCheating = this.reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating;
				this.reportedToxicity = this.reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity;
				this.reportedHateSpeech = this.reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech;
				this.reportButton.isOn = true;
				this.reportButton.UpdateColor();
			}
		}
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateLineState(this);
		}
		this.parentScoreboard.RedrawPlayerLines();
	}

	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		WebFlags webFlags = new WebFlags(1);
		raiseEventOptions.Flags = webFlags;
		raiseEventOptions.TargetActors = GorillaPlayerScoreboardLine.targetActors;
		byte b = 50;
		object[] array = new object[]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			PhotonNetwork.LocalPlayer.NickName,
			PhotonNetwork.CurrentRoom.IsVisible,
			PhotonNetwork.CurrentRoom.ToStringStripped()
		};
		PhotonNetwork.RaiseEvent(b, array, raiseEventOptions, SendOptions.SendReliable);
	}

	public static void MutePlayer(string PlayerID, string OtherPlayerNickName, int muting)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		WebFlags webFlags = new WebFlags(1);
		raiseEventOptions.Flags = webFlags;
		raiseEventOptions.TargetActors = GorillaPlayerScoreboardLine.targetActors;
		byte b = 51;
		object[] array = new object[]
		{
			PlayerID,
			muting,
			OtherPlayerNickName,
			PhotonNetwork.LocalPlayer.NickName,
			PhotonNetwork.CurrentRoom.IsVisible,
			PhotonNetwork.CurrentRoom.ToStringStripped()
		};
		PhotonNetwork.RaiseEvent(b, array, raiseEventOptions, SendOptions.SendReliable);
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
				GorillaNot.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}
		return text;
	}

	public void ResetData()
	{
		this.playerActorNumber = -1;
		this.linePlayer = null;
		this.playerNameValue = string.Empty;
	}

	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterSL(this);
	}

	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterSL(this);
	}

	private void SwapToReportState(bool reportInProgress)
	{
		this.reportButton.gameObject.SetActive(!reportInProgress);
		this.hateSpeechButton.SetActive(reportInProgress);
		this.toxicityButton.SetActive(reportInProgress);
		this.cheatingButton.SetActive(reportInProgress);
		this.cancelButton.SetActive(reportInProgress);
	}

	private static int[] targetActors = new int[] { -1 };

	public Text playerName;

	public Text playerLevel;

	public Text playerMMR;

	public Image playerSwatch;

	public Texture infectedTexture;

	public Player linePlayer;

	public VRRig playerVRRig;

	public int currentMatIndex;

	public string playerLevelValue;

	public string playerMMRValue;

	public string playerNameValue;

	public int playerActorNumber;

	public GorillaPlayerLineButton muteButton;

	public GorillaPlayerLineButton reportButton;

	public GameObject hateSpeechButton;

	public GameObject toxicityButton;

	public GameObject cheatingButton;

	public GameObject cancelButton;

	public SpriteRenderer speakerIcon;

	public bool canPressNextReportButton = true;

	public Text[] texts;

	public SpriteRenderer[] sprites;

	public MeshRenderer[] meshes;

	public Image[] images;

	private PhotonVoiceView myVoiceView;

	private Recorder myRecorder;

	private int mute;

	private int emptyRigCount;

	public GameObject myRig;

	public bool reportedCheating;

	public bool reportedToxicity;

	public bool reportedHateSpeech;

	public bool reportInProgress;

	private string currentNickname;

	public bool doneReporting;

	public bool lastVisible = true;

	public GorillaScoreBoard parentScoreboard;

	public float initTime;

	public float emptyRigCooldown = 10f;

	internal RigContainer rigContainer;
}
