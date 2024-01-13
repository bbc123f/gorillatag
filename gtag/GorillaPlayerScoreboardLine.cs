using System;
using System.Collections;
using ExitGames.Client.Photon;
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

	public void Start()
	{
		emptyRigCount = 0;
		reportedCheating = false;
		reportedHateSpeech = false;
		reportedToxicity = false;
		StartCoroutine(UpdateLine(UnityEngine.Random.Range(0f, 1f)));
	}

	public void UpdateLevel()
	{
	}

	public void HideShowLine(bool active)
	{
		if (!(playerVRRig != null))
		{
			return;
		}
		Text[] array = texts;
		foreach (Text text in array)
		{
			if (text.enabled != active)
			{
				text.enabled = active;
			}
		}
		SpriteRenderer[] array2 = sprites;
		foreach (SpriteRenderer spriteRenderer in array2)
		{
			if (spriteRenderer.enabled != active)
			{
				spriteRenderer.enabled = active;
			}
		}
		MeshRenderer[] array3 = meshes;
		foreach (MeshRenderer meshRenderer in array3)
		{
			if (meshRenderer.enabled != active)
			{
				meshRenderer.enabled = active;
			}
		}
		Image[] array4 = images;
		foreach (Image image in array4)
		{
			if (image.enabled != active)
			{
				image.enabled = active;
			}
		}
		if (playerSwatch != null && playerSwatch.gameObject.activeSelf != active)
		{
			playerSwatch.gameObject.SetActive(active);
		}
	}

	private IEnumerator UpdateLine(float jiggleTime)
	{
		yield return null;
		WaitForSeconds wait = new WaitForSeconds(1f);
		bool ranOnce = false;
		while (true)
		{
			try
			{
				if (!initialized && linePlayer != null)
				{
					initialized = true;
					playerName.text = NormalizeName(linePlayer.NickName != currentNickname, linePlayer.NickName);
					currentNickname = linePlayer.NickName;
					if (linePlayer != PhotonNetwork.LocalPlayer)
					{
						mute = PlayerPrefs.GetInt(linePlayer.UserId, 0);
						PlayerPrefs.SetInt(linePlayer.UserId, mute);
						muteButton.isOn = ((mute != 0) ? true : false);
						muteButton.UpdateColor();
						if (rigContainer != null)
						{
							rigContainer.Muted = ((mute != 0) ? true : false);
						}
					}
					else
					{
						muteButton.gameObject.SetActive(value: false);
						reportButton.gameObject.SetActive(value: false);
					}
				}
				if (linePlayer != null)
				{
					if (playerName.text != linePlayer.NickName && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(linePlayer.UserId))
					{
						playerName.text = NormalizeName(linePlayer.NickName != currentNickname, linePlayer.NickName);
						currentNickname = linePlayer.NickName;
					}
					if (rigContainer != null)
					{
						if (playerVRRig.photonView != null)
						{
							emptyRigCount = 0;
						}
						else
						{
							emptyRigCount++;
							if (emptyRigCount > 5)
							{
								GorillaNot.instance.SendReport("empty rig", linePlayer.UserId, linePlayer.NickName);
							}
						}
						if (!linePlayerInitialized && linePlayer != PhotonNetwork.LocalPlayer.Get(playerActorNumber))
						{
							linePlayer = PhotonNetwork.LocalPlayer.Get(playerActorNumber);
							playerSwatch.color = playerVRRig.materialsToChangeTo[0].color;
							playerSwatch.material = playerVRRig.materialsToChangeTo[playerVRRig.setMatIndex];
							linePlayerInitialized = true;
						}
						if (playerVRRig.setMatIndex != currentMatIndex && playerVRRig.setMatIndex != 0 && playerVRRig.setMatIndex > -1 && playerVRRig.setMatIndex < playerVRRig.materialsToChangeTo.Length)
						{
							playerSwatch.material = playerVRRig.materialsToChangeTo[playerVRRig.setMatIndex];
							currentMatIndex = playerVRRig.setMatIndex;
						}
						if (playerVRRig.setMatIndex == 0 && playerSwatch.material != null)
						{
							playerSwatch.material = null;
							currentMatIndex = 0;
						}
						if (playerSwatch.color != playerVRRig.materialsToChangeTo[0].color)
						{
							playerSwatch.color = playerVRRig.materialsToChangeTo[0].color;
						}
						if (myVoiceView == null)
						{
							myVoiceView = rigContainer.Voice;
						}
						if (myRecorder == null)
						{
							myRecorder = PhotonNetworkController.Instance.GetComponent<Recorder>();
						}
						if ((playerVRRig != null && myVoiceView != null && myVoiceView.IsSpeaking) || (playerVRRig.photonView.IsMine && myRecorder != null && myRecorder.IsCurrentlyTransmitting))
						{
							speakerIcon.SetActive(value: true);
						}
						else
						{
							speakerIcon.SetActive(value: false);
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
	}

	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		switch (buttonType)
		{
		case GorillaPlayerLineButton.ButtonType.Mute:
			if (linePlayer != null && playerVRRig != null)
			{
				mute = (isOn ? 1 : 0);
				PlayerPrefs.SetInt(linePlayer.UserId, mute);
				if (rigContainer != null)
				{
					rigContainer.Muted = ((mute != 0) ? true : false);
				}
				PlayerPrefs.Save();
				muteButton.UpdateColor();
				MutePlayer(linePlayer.UserId, linePlayer.NickName, mute);
			}
			break;
		case GorillaPlayerLineButton.ButtonType.Report:
			SetReportState(reportState: true, buttonType);
			break;
		default:
			SetReportState(reportState: false, buttonType);
			break;
		}
	}

	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		canPressNextReportButton = buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report;
		if (reportState)
		{
			GorillaPlayerLineButton[] componentsInChildren = GetComponentsInChildren<GorillaPlayerLineButton>(includeInactive: true);
			foreach (GorillaPlayerLineButton gorillaPlayerLineButton in componentsInChildren)
			{
				gorillaPlayerLineButton.gameObject.SetActive(gorillaPlayerLineButton.buttonType != GorillaPlayerLineButton.ButtonType.Report);
			}
		}
		else
		{
			GorillaPlayerLineButton[] componentsInChildren = GetComponentsInChildren<GorillaPlayerLineButton>(includeInactive: true);
			foreach (GorillaPlayerLineButton gorillaPlayerLineButton2 in componentsInChildren)
			{
				gorillaPlayerLineButton2.gameObject.SetActive(gorillaPlayerLineButton2.buttonType == GorillaPlayerLineButton.ButtonType.Report || gorillaPlayerLineButton2.buttonType == GorillaPlayerLineButton.ButtonType.Mute);
			}
			if (linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				if ((!reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
				{
					ReportPlayer(linePlayer.UserId, buttonType, linePlayer.NickName);
					doneReporting = true;
				}
				reportedCheating = reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating;
				reportedToxicity = reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity;
				reportedHateSpeech = reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech;
				reportButton.isOn = true;
				reportButton.UpdateColor();
			}
		}
		base.transform.parent.GetComponent<GorillaScoreBoard>().RedrawPlayerLines();
	}

	public void GetUserLevel(string myPlayFabeId)
	{
		PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest
		{
			PlayFabId = myPlayFabeId,
			Keys = null
		}, delegate(GetUserDataResult result)
		{
			if (result.Data == null || !result.Data.ContainsKey("PlayerLevel"))
			{
				playerLevelValue = "1";
			}
			else
			{
				playerLevelValue = result.Data["PlayerLevel"].Value;
			}
			if (result.Data == null || !result.Data.ContainsKey("Player1v1MMR"))
			{
				playerMMRValue = "-1";
			}
			else
			{
				playerMMRValue = result.Data["Player1v1MMR"].Value;
			}
			playerLevel.text = playerLevelValue;
			playerMMR.text = playerMMRValue;
		}, delegate
		{
		});
	}

	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		WebFlags flags = new WebFlags(1);
		raiseEventOptions.Flags = flags;
		byte eventCode = 50;
		object[] eventContent = new object[6]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			PhotonNetwork.LocalPlayer.NickName,
			PhotonNetwork.CurrentRoom.IsVisible,
			PhotonNetwork.CurrentRoom.ToStringFull()
		};
		PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
	}

	public static void MutePlayer(string PlayerID, string OtherPlayerNickName, int muting)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		WebFlags flags = new WebFlags(1);
		raiseEventOptions.Flags = flags;
		byte eventCode = 51;
		object[] eventContent = new object[6]
		{
			PlayerID,
			muting,
			OtherPlayerNickName,
			PhotonNetwork.LocalPlayer.NickName,
			PhotonNetwork.CurrentRoom.IsVisible,
			PhotonNetwork.CurrentRoom.ToStringFull()
		};
		PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				text = new string(Array.FindAll(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
				GorillaNot.instance.SendReport("evading the name ban", linePlayer.UserId, linePlayer.NickName);
			}
		}
		return text;
	}
}
