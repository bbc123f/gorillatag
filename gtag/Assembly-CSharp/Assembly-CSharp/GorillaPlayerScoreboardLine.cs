using System;
using System.Collections;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class GorillaPlayerScoreboardLine : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
		base.StartCoroutine(this.UpdateLine(Random.Range(0f, 1f)));
	}

	public void UpdateLevel()
	{
	}

	public void HideShowLine(bool active)
	{
		if (this.playerVRRig != null)
		{
			foreach (Text text in this.texts)
			{
				if (text.enabled != active)
				{
					text.enabled = active;
				}
			}
			foreach (SpriteRenderer spriteRenderer in this.sprites)
			{
				if (spriteRenderer.enabled != active)
				{
					spriteRenderer.enabled = active;
				}
			}
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				if (meshRenderer.enabled != active)
				{
					meshRenderer.enabled = active;
				}
			}
			foreach (Image image in this.images)
			{
				if (image.enabled != active)
				{
					image.enabled = active;
				}
			}
			if (this.playerSwatch != null && this.playerSwatch.gameObject.activeSelf != active)
			{
				this.playerSwatch.gameObject.SetActive(active);
			}
		}
	}

	private IEnumerator UpdateLine(float jiggleTime)
	{
		yield return null;
		WaitForSeconds wait = new WaitForSeconds(1f);
		bool ranOnce = false;
		for (;;)
		{
			try
			{
				if (!this.initialized && this.linePlayer != null)
				{
					this.initialized = true;
					this.UpdatePlayerText();
					if (this.linePlayer != PhotonNetwork.LocalPlayer)
					{
						this.mute = PlayerPrefs.GetInt(this.linePlayer.UserId, 0);
						PlayerPrefs.SetInt(this.linePlayer.UserId, this.mute);
						this.muteButton.isOn = (this.mute != 0);
						this.muteButton.UpdateColor();
						if (this.rigContainer != null)
						{
							this.rigContainer.Muted = (this.mute != 0);
						}
					}
					else
					{
						this.muteButton.gameObject.SetActive(false);
						this.reportButton.gameObject.SetActive(false);
					}
				}
				if (this.linePlayer != null)
				{
					if (this.playerName.text != this.playerVRRig.playerText.text)
					{
						this.UpdatePlayerText();
					}
					if (this.rigContainer != null)
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
						if (!this.linePlayerInitialized && this.linePlayer != PhotonNetwork.LocalPlayer.Get(this.playerActorNumber))
						{
							this.linePlayer = PhotonNetwork.LocalPlayer.Get(this.playerActorNumber);
							this.playerSwatch.color = this.playerVRRig.materialsToChangeTo[0].color;
							this.playerSwatch.material = this.playerVRRig.materialsToChangeTo[this.playerVRRig.setMatIndex];
							this.linePlayerInitialized = true;
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
						if ((this.playerVRRig != null && this.myVoiceView != null && this.myVoiceView.IsSpeaking) || (this.playerVRRig.photonView.IsMine && this.myRecorder != null && this.myRecorder.IsCurrentlyTransmitting))
						{
							this.speakerIcon.SetActive(true);
						}
						else
						{
							this.speakerIcon.SetActive(false);
						}
					}
				}
			}
			catch
			{
			}
			if (!ranOnce)
			{
				yield return new WaitForSeconds(jiggleTime);
				ranOnce = true;
			}
			else
			{
				yield return wait;
			}
		}
		yield break;
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
				this.rigContainer.Muted = (this.mute != 0);
			}
			PlayerPrefs.Save();
			this.muteButton.UpdateColor();
			GorillaPlayerScoreboardLine.MutePlayer(this.linePlayer.UserId, this.playerName.text, this.mute);
			return;
		}
	}

	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		this.canPressNextReportButton = (buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report);
		if (reportState)
		{
			foreach (GorillaPlayerLineButton gorillaPlayerLineButton in base.GetComponentsInChildren<GorillaPlayerLineButton>(true))
			{
				gorillaPlayerLineButton.gameObject.SetActive(gorillaPlayerLineButton.buttonType != GorillaPlayerLineButton.ButtonType.Report);
			}
		}
		else
		{
			foreach (GorillaPlayerLineButton gorillaPlayerLineButton2 in base.GetComponentsInChildren<GorillaPlayerLineButton>(true))
			{
				gorillaPlayerLineButton2.gameObject.SetActive(gorillaPlayerLineButton2.buttonType == GorillaPlayerLineButton.ButtonType.Report || gorillaPlayerLineButton2.buttonType == GorillaPlayerLineButton.ButtonType.Mute);
			}
			if (this.linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				if ((!this.reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!this.reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!this.reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
				{
					GorillaPlayerScoreboardLine.ReportPlayer(this.linePlayer.UserId, buttonType, this.playerName.text);
					this.doneReporting = true;
				}
				this.reportedCheating = (this.reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating);
				this.reportedToxicity = (this.reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity);
				this.reportedHateSpeech = (this.reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech);
				this.reportButton.isOn = true;
				this.reportButton.UpdateColor();
			}
		}
		base.transform.parent.GetComponent<GorillaScoreBoard>().RedrawPlayerLines();
	}

	public void GetUserLevel(string myPlayFabeId)
	{
		GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
		getUserDataRequest.PlayFabId = myPlayFabeId;
		getUserDataRequest.Keys = null;
		PlayFabClientAPI.GetUserReadOnlyData(getUserDataRequest, delegate(GetUserDataResult result)
		{
			if (result.Data == null || !result.Data.ContainsKey("PlayerLevel"))
			{
				this.playerLevelValue = "1";
			}
			else
			{
				this.playerLevelValue = result.Data["PlayerLevel"].Value;
			}
			if (result.Data == null || !result.Data.ContainsKey("Player1v1MMR"))
			{
				this.playerMMRValue = "-1";
			}
			else
			{
				this.playerMMRValue = result.Data["Player1v1MMR"].Value;
			}
			this.playerLevel.text = this.playerLevelValue;
			this.playerMMR.text = this.playerMMRValue;
		}, delegate(PlayFabError error)
		{
		}, null, null);
	}

	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		WebFlags flags = new WebFlags(1);
		raiseEventOptions.Flags = flags;
		byte eventCode = 50;
		object[] eventContent = new object[]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			PhotonNetwork.LocalPlayer.NickName,
			PhotonNetwork.CurrentRoom.IsVisible,
			PhotonNetwork.CurrentRoom.ToStringStripped()
		};
		PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
	}

	public static void MutePlayer(string PlayerID, string OtherPlayerNickName, int muting)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		WebFlags flags = new WebFlags(1);
		raiseEventOptions.Flags = flags;
		byte eventCode = 51;
		object[] eventContent = new object[]
		{
			PlayerID,
			muting,
			OtherPlayerNickName,
			PhotonNetwork.LocalPlayer.NickName,
			PhotonNetwork.CurrentRoom.IsVisible,
			PhotonNetwork.CurrentRoom.ToStringStripped()
		};
		PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
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

	public bool initialized;

	public GorillaPlayerLineButton muteButton;

	public GorillaPlayerLineButton reportButton;

	public GameObject speakerIcon;

	public bool canPressNextReportButton = true;

	public Text[] texts;

	public SpriteRenderer[] sprites;

	public MeshRenderer[] meshes;

	public Image[] images;

	private PhotonVoiceView myVoiceView;

	private Recorder myRecorder;

	private bool linePlayerInitialized;

	private int mute;

	private int emptyRigCount;

	public GameObject myRig;

	private bool reportedCheating;

	private bool reportedToxicity;

	private bool reportedHateSpeech;

	private string currentNickname;

	public bool doneReporting;

	public bool lastVisible = true;

	internal RigContainer rigContainer;
}
