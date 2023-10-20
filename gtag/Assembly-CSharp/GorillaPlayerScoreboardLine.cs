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

// Token: 0x020001B3 RID: 435
public class GorillaPlayerScoreboardLine : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	// Token: 0x06000B15 RID: 2837 RVA: 0x00044699 File Offset: 0x00042899
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
		base.StartCoroutine(this.UpdateLine(Random.Range(0f, 1f)));
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x000446D3 File Offset: 0x000428D3
	public void UpdateLevel()
	{
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x000446D8 File Offset: 0x000428D8
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

	// Token: 0x06000B18 RID: 2840 RVA: 0x000447DB File Offset: 0x000429DB
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

	// Token: 0x06000B19 RID: 2841 RVA: 0x000447F4 File Offset: 0x000429F4
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

	// Token: 0x06000B1A RID: 2842 RVA: 0x000448FC File Offset: 0x00042AFC
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

	// Token: 0x06000B1B RID: 2843 RVA: 0x000449C0 File Offset: 0x00042BC0
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

	// Token: 0x06000B1C RID: 2844 RVA: 0x00044B18 File Offset: 0x00042D18
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

	// Token: 0x06000B1D RID: 2845 RVA: 0x00044B6C File Offset: 0x00042D6C
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

	// Token: 0x06000B1E RID: 2846 RVA: 0x00044BF8 File Offset: 0x00042DF8
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

	// Token: 0x06000B1F RID: 2847 RVA: 0x00044C84 File Offset: 0x00042E84
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

	// Token: 0x04000E54 RID: 3668
	public Text playerName;

	// Token: 0x04000E55 RID: 3669
	public Text playerLevel;

	// Token: 0x04000E56 RID: 3670
	public Text playerMMR;

	// Token: 0x04000E57 RID: 3671
	public Image playerSwatch;

	// Token: 0x04000E58 RID: 3672
	public Texture infectedTexture;

	// Token: 0x04000E59 RID: 3673
	public Player linePlayer;

	// Token: 0x04000E5A RID: 3674
	public VRRig playerVRRig;

	// Token: 0x04000E5B RID: 3675
	public int currentMatIndex;

	// Token: 0x04000E5C RID: 3676
	public string playerLevelValue;

	// Token: 0x04000E5D RID: 3677
	public string playerMMRValue;

	// Token: 0x04000E5E RID: 3678
	public string playerNameValue;

	// Token: 0x04000E5F RID: 3679
	public int playerActorNumber;

	// Token: 0x04000E60 RID: 3680
	public bool initialized;

	// Token: 0x04000E61 RID: 3681
	public GorillaPlayerLineButton muteButton;

	// Token: 0x04000E62 RID: 3682
	public GorillaPlayerLineButton reportButton;

	// Token: 0x04000E63 RID: 3683
	public GameObject speakerIcon;

	// Token: 0x04000E64 RID: 3684
	public bool canPressNextReportButton = true;

	// Token: 0x04000E65 RID: 3685
	public Text[] texts;

	// Token: 0x04000E66 RID: 3686
	public SpriteRenderer[] sprites;

	// Token: 0x04000E67 RID: 3687
	public MeshRenderer[] meshes;

	// Token: 0x04000E68 RID: 3688
	public Image[] images;

	// Token: 0x04000E69 RID: 3689
	private PhotonVoiceView myVoiceView;

	// Token: 0x04000E6A RID: 3690
	private Recorder myRecorder;

	// Token: 0x04000E6B RID: 3691
	private bool linePlayerInitialized;

	// Token: 0x04000E6C RID: 3692
	private int mute;

	// Token: 0x04000E6D RID: 3693
	private int emptyRigCount;

	// Token: 0x04000E6E RID: 3694
	public GameObject myRig;

	// Token: 0x04000E6F RID: 3695
	private bool reportedCheating;

	// Token: 0x04000E70 RID: 3696
	private bool reportedToxicity;

	// Token: 0x04000E71 RID: 3697
	private bool reportedHateSpeech;

	// Token: 0x04000E72 RID: 3698
	private string currentNickname;

	// Token: 0x04000E73 RID: 3699
	public bool doneReporting;

	// Token: 0x04000E74 RID: 3700
	public bool lastVisible = true;

	// Token: 0x04000E75 RID: 3701
	internal RigContainer rigContainer;
}
