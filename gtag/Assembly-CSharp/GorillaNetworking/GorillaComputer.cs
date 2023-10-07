﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Backtrace.Unity;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	// Token: 0x020002B2 RID: 690
	public class GorillaComputer : MonoBehaviourPunCallbacks
	{
		// Token: 0x0600123D RID: 4669 RVA: 0x0006898E File Offset: 0x00066B8E
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x000689A6 File Offset: 0x00066BA6
		public GorillaComputer.ComputerState currentState
		{
			get
			{
				return this.stateStack.Peek();
			}
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x000689B3 File Offset: 0x00066BB3
		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x000689C0 File Offset: 0x00066BC0
		private void Awake()
		{
			this.offlineScoreboard.Initialize(this.scoreboardRenderer, this.wrongVersionMaterial);
			this.screenText.Initialize(this.computerScreenRenderer, this.wrongVersionMaterial);
			this.functionSelectText.Initialize(this.computerScreenRenderer, this.wrongVersionMaterial);
			this.wallScreenText.Initialize(this.wallScreenRenderer, this.wrongVersionMaterial);
			this.tutorialWallScreenText.Initialize(this.tutorialWallScreenRenderer, this.wrongVersionMaterial);
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.currentTextField = "";
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitializeColorState();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x00068B22 File Offset: 0x00066D22
		private IEnumerator Start()
		{
			yield return null;
			if (BacktraceClient.Instance && this.includeUpdatedServerSynchTest == 1)
			{
				Object.Destroy(BacktraceClient.Instance.gameObject);
			}
			yield break;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x00068B31 File Offset: 0x00066D31
		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x00068B50 File Offset: 0x00066D50
		private void Update()
		{
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				this.UpdateFailureText("NO WIFI OR LAN CONNECTION DETECTED.");
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
			{
				this.lastUpdateTime = Time.time;
				this.UpdateScreen();
			}
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00068BC4 File Offset: 0x00066DC4
		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
				{
					FunctionName = "ReturnCurrentVersionNew",
					FunctionParameter = new
					{
						CurrentVersion = PhotonNetworkController.Instance.GameVersionString,
						UpdatedSynchTest = this.includeUpdatedServerSynchTest
					}
				}, new Action<PlayFab.ClientModels.ExecuteCloudScriptResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared), null, null);
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				RuntimePlatform platform = Application.platform;
				this.SaveModAccountData();
			}
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x00068C54 File Offset: 0x00066E54
		public void PressButton(GorillaKeyboardButton buttonPressed)
		{
			switch (this.currentState)
			{
			case GorillaComputer.ComputerState.Startup:
				this.ProcessStartupState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Color:
				this.ProcessColorState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Name:
				this.ProcessNameState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Turn:
				this.ProcessTurnState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Mic:
				this.ProcessMicState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Room:
				this.ProcessRoomState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Queue:
				this.ProcessQueueState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Group:
				this.ProcessGroupState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Voice:
				this.ProcessVoiceState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Credits:
				this.ProcessCreditsState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Visuals:
				this.ProcessVisualsState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.NameWarning:
				this.ProcessNameWarningState(buttonPressed);
				break;
			case GorillaComputer.ComputerState.Support:
				this.ProcessSupportState(buttonPressed);
				break;
			}
			buttonPressed.GetComponent<MeshRenderer>().material = this.pressedMaterial;
			buttonPressed.pressTime = Time.time;
			base.StartCoroutine(this.ButtonColorUpdate(buttonPressed));
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x00068D49 File Offset: 0x00066F49
		private IEnumerator ButtonColorUpdate(GorillaKeyboardButton button)
		{
			yield return new WaitForSeconds(this.buttonFadeTime);
			if (button.pressTime != 0f && Time.time > this.buttonFadeTime + button.pressTime)
			{
				button.GetComponent<MeshRenderer>().material = this.unpressedMaterial;
				button.pressTime = 0f;
			}
			yield break;
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x00068D5F File Offset: 0x00066F5F
		private void InitializeStartupState()
		{
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00068D61 File Offset: 0x00066F61
		private void InitializeRoomState()
		{
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x00068D64 File Offset: 0x00066F64
		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00068DD4 File Offset: 0x00066FD4
		private void InitializeNameState()
		{
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			PhotonNetwork.LocalPlayer.NickName = this.savedName;
			this.currentName = this.savedName;
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x00068F14 File Offset: 0x00067114
		private void InitializeTurnState()
		{
			this.gorillaTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			string defaultValue = (Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP";
			this.turnType = PlayerPrefs.GetString("stickTurning", defaultValue);
			this.turnValue = PlayerPrefs.GetInt("turnFactor", 4);
			this.gorillaTurn.ChangeTurnMode(this.turnType, this.turnValue);
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x00068F80 File Offset: 0x00067180
		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x00068F98 File Offset: 0x00067198
		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = (PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1);
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x00069007 File Offset: 0x00067207
		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00069040 File Offset: 0x00067240
		private void InitializeVoiceState()
		{
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", "TRUE");
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00069058 File Offset: 0x00067258
		private void InitializeGameMode()
		{
			this.currentGameMode = PlayerPrefs.GetString("currentGameMode", "INFECTION");
			if (this.currentGameMode != "CASUAL" && this.currentGameMode != "INFECTION" && this.currentGameMode != "HUNT" && this.currentGameMode != "BATTLE")
			{
				PlayerPrefs.SetString("currentGameMode", "INFECTION");
				PlayerPrefs.Save();
				this.currentGameMode = "INFECTION";
			}
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(this.currentGameMode, this.leftHanded);
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x00069107 File Offset: 0x00067307
		private void InitializeCreditsState()
		{
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00069109 File Offset: 0x00067309
		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00069118 File Offset: 0x00067318
		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x00069124 File Offset: 0x00067324
		private void InitializeVisualsState()
		{
			this.disableParticles = (PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE");
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x00069178 File Offset: 0x00067378
		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
			this.UpdateScreen();
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x0006919A File Offset: 0x0006739A
		private void PopState()
		{
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x000691C8 File Offset: 0x000673C8
		private void SwitchToColorState()
		{
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.SwitchState(GorillaComputer.ComputerState.Color, true);
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x0006923A File Offset: 0x0006743A
		private void SwitchToRoomState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Room, true);
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x00069244 File Offset: 0x00067444
		private void SwitchToNameState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Name, true);
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x0006924E File Offset: 0x0006744E
		private void SwitchToTurnState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Turn, true);
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00069258 File Offset: 0x00067458
		private void SwitchToMicState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Mic, true);
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00069262 File Offset: 0x00067462
		private void SwitchToQueueState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Queue, true);
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x0006926C File Offset: 0x0006746C
		private void SwitchToGroupState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Group, true);
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00069276 File Offset: 0x00067476
		private void SwitchToVoiceState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Voice, true);
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x00069280 File Offset: 0x00067480
		private void SwitchToCreditsState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Credits, true);
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x0006928B File Offset: 0x0006748B
		private void SwitchToSupportState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Support, true);
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x00069296 File Offset: 0x00067496
		private void SwitchToVisualsState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Visuals, true);
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x000692A1 File Offset: 0x000674A1
		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x000692B7 File Offset: 0x000674B7
		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x000692C2 File Offset: 0x000674C2
		private void ProcessStartupState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			this.SwitchToRoomState();
			this.UpdateScreen();
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x000692D8 File Offset: 0x000674D8
		private void ProcessColorState(GorillaKeyboardButton buttonPressed)
		{
			int num;
			if (int.TryParse(buttonPressed.characterString, out num))
			{
				switch (this.colorCursorLine)
				{
				case 0:
					this.redText = num.ToString();
					break;
				case 1:
					this.greenText = num.ToString();
					break;
				case 2:
					this.blueText = num.ToString();
					break;
				}
				int num2;
				if (int.TryParse(this.redText, out num2))
				{
					this.redValue = (float)num2 / 9f;
				}
				if (int.TryParse(this.greenText, out num2))
				{
					this.greenValue = (float)num2 / 9f;
				}
				if (int.TryParse(this.blueText, out num2))
				{
					this.blueValue = (float)num2 / 9f;
				}
				PlayerPrefs.SetFloat("redValue", this.redValue);
				PlayerPrefs.SetFloat("greenValue", this.greenValue);
				PlayerPrefs.SetFloat("blueValue", this.blueValue);
				GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
				PlayerPrefs.Save();
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[]
					{
						this.redValue,
						this.greenValue,
						this.blueValue,
						this.leftHanded
					});
				}
			}
			else
			{
				string characterString = buttonPressed.characterString;
				if (!(characterString == "up"))
				{
					if (!(characterString == "down"))
					{
						if (!(characterString == "option1"))
						{
							if (!(characterString == "option2"))
							{
								if (!(characterString == "option3"))
								{
									if (!(characterString == "enter"))
									{
									}
								}
								else
								{
									this.colorCursorLine = 2;
								}
							}
							else
							{
								this.colorCursorLine = 1;
							}
						}
						else
						{
							this.colorCursorLine = 0;
						}
					}
					else
					{
						this.SwitchToTurnState();
					}
				}
				else
				{
					this.SwitchToNameState();
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x000694D0 File Offset: 0x000676D0
		public void ProcessNameState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			if (!(characterString == "up"))
			{
				if (!(characterString == "down"))
				{
					if (!(characterString == "enter"))
					{
						if (!(characterString == "delete"))
						{
							if (this.currentName.Length < 12 && buttonPressed.characterString.Length == 1)
							{
								this.currentName += buttonPressed.characterString;
							}
						}
						else if (this.currentName.Length > 0)
						{
							this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						}
					}
					else if (this.currentName != this.savedName && this.currentName != "")
					{
						this.CheckAutoBanList(this.currentName, false);
					}
				}
				else
				{
					this.SwitchToColorState();
				}
			}
			else
			{
				this.SwitchToRoomState();
			}
			this.UpdateScreen();
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x000695D0 File Offset: 0x000677D0
		private void ProcessRoomState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(characterString);
			if (num <= 1740784714U)
			{
				if (num != 1035581717U)
				{
					if (num != 1128467232U)
					{
						if (num == 1740784714U)
						{
							if (characterString == "delete")
							{
								if (this.roomToJoin.Length > 0)
								{
									this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
									goto IL_183;
								}
								goto IL_183;
							}
						}
					}
					else if (characterString == "up")
					{
						this.SwitchToSupportState();
						goto IL_183;
					}
				}
				else if (characterString == "down")
				{
					this.SwitchToNameState();
					goto IL_183;
				}
			}
			else if (num <= 2921858642U)
			{
				if (num != 2905081023U)
				{
					if (num == 2921858642U)
					{
						if (characterString == "option2")
						{
							goto IL_183;
						}
					}
				}
				else if (characterString == "option1")
				{
					PhotonNetworkController.Instance.AttemptDisconnect();
					goto IL_183;
				}
			}
			else if (num != 2938636261U)
			{
				if (num == 3724402957U)
				{
					if (characterString == "enter")
					{
						if (this.roomToJoin != "")
						{
							this.CheckAutoBanList(this.roomToJoin, true);
							goto IL_183;
						}
						goto IL_183;
					}
				}
			}
			else if (characterString == "option3")
			{
				goto IL_183;
			}
			if (this.roomToJoin.Length < 10)
			{
				this.roomToJoin += buttonPressed.characterString;
			}
			IL_183:
			this.UpdateScreen();
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x00069768 File Offset: 0x00067968
		private void ProcessTurnState(GorillaKeyboardButton buttonPressed)
		{
			int num;
			if (int.TryParse(buttonPressed.characterString, out num))
			{
				this.turnValue = num;
				PlayerPrefs.SetInt("turnFactor", this.turnValue);
				PlayerPrefs.Save();
				this.gorillaTurn.ChangeTurnMode(this.turnType, this.turnValue);
			}
			else
			{
				string characterString = buttonPressed.characterString;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(characterString);
				if (num2 <= 1740784714U)
				{
					if (num2 != 1035581717U)
					{
						if (num2 != 1128467232U)
						{
							if (num2 == 1740784714U)
							{
								if (!(characterString == "delete"))
								{
								}
							}
						}
						else if (characterString == "up")
						{
							this.SwitchToColorState();
						}
					}
					else if (characterString == "down")
					{
						this.SwitchToMicState();
					}
				}
				else if (num2 <= 2921858642U)
				{
					if (num2 != 2905081023U)
					{
						if (num2 == 2921858642U)
						{
							if (characterString == "option2")
							{
								this.turnType = "SMOOTH";
								PlayerPrefs.SetString("stickTurning", this.turnType);
								PlayerPrefs.Save();
								this.gorillaTurn.ChangeTurnMode(this.turnType, this.turnValue);
							}
						}
					}
					else if (characterString == "option1")
					{
						this.turnType = "SNAP";
						PlayerPrefs.SetString("stickTurning", this.turnType);
						PlayerPrefs.Save();
						this.gorillaTurn.ChangeTurnMode(this.turnType, this.turnValue);
					}
				}
				else if (num2 != 2938636261U)
				{
					if (num2 == 3724402957U)
					{
						if (!(characterString == "enter"))
						{
						}
					}
				}
				else if (characterString == "option3")
				{
					this.turnType = "NONE";
					PlayerPrefs.SetString("stickTurning", this.turnType);
					PlayerPrefs.Save();
					this.gorillaTurn.ChangeTurnMode(this.turnType, this.turnValue);
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x00069974 File Offset: 0x00067B74
		private void ProcessMicState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			if (!(characterString == "up"))
			{
				if (!(characterString == "down"))
				{
					if (!(characterString == "option1"))
					{
						if (!(characterString == "option2"))
						{
							if (characterString == "option3")
							{
								this.pttType = "PUSH TO MUTE";
								PlayerPrefs.SetString("pttType", this.pttType);
								PlayerPrefs.Save();
							}
						}
						else
						{
							this.pttType = "PUSH TO TALK";
							PlayerPrefs.SetString("pttType", this.pttType);
							PlayerPrefs.Save();
						}
					}
					else
					{
						this.pttType = "ALL CHAT";
						PlayerPrefs.SetString("pttType", this.pttType);
						PlayerPrefs.Save();
					}
				}
				else
				{
					this.SwitchToQueueState();
				}
			}
			else
			{
				this.SwitchToTurnState();
			}
			this.UpdateScreen();
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x00069A48 File Offset: 0x00067C48
		private void ProcessQueueState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			if (!(characterString == "up"))
			{
				if (!(characterString == "down"))
				{
					if (!(characterString == "option1"))
					{
						if (!(characterString == "option2"))
						{
							if (characterString == "option3")
							{
								if (this.allowedInCompetitive)
								{
									this.currentQueue = "COMPETITIVE";
									PlayerPrefs.SetString("currentQueue", this.currentQueue);
									PlayerPrefs.Save();
								}
							}
						}
						else
						{
							this.currentQueue = "MINIGAMES";
							PlayerPrefs.SetString("currentQueue", this.currentQueue);
							PlayerPrefs.Save();
						}
					}
					else
					{
						this.currentQueue = "DEFAULT";
						PlayerPrefs.SetString("currentQueue", this.currentQueue);
						PlayerPrefs.Save();
					}
				}
				else
				{
					this.SwitchToGroupState();
				}
			}
			else
			{
				this.SwitchToMicState();
			}
			this.UpdateScreen();
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x00069B24 File Offset: 0x00067D24
		private void ProcessGroupState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(characterString);
			if (num <= 906799682U)
			{
				if (num != 822911587U)
				{
					if (num != 873244444U)
					{
						if (num == 906799682U)
						{
							if (characterString == "3")
							{
								this.groupMapJoin = "CANYON";
								this.groupMapJoinIndex = 2;
								PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
								PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
								PlayerPrefs.Save();
							}
						}
					}
					else if (characterString == "1")
					{
						this.groupMapJoin = "FOREST";
						this.groupMapJoinIndex = 0;
						PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
						PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
						PlayerPrefs.Save();
					}
				}
				else if (characterString == "4")
				{
					this.groupMapJoin = "CITY";
					this.groupMapJoinIndex = 3;
					PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
					PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
					PlayerPrefs.Save();
				}
			}
			else if (num <= 1035581717U)
			{
				if (num != 923577301U)
				{
					if (num == 1035581717U)
					{
						if (characterString == "down")
						{
							this.SwitchToVoiceState();
						}
					}
				}
				else if (characterString == "2")
				{
					this.groupMapJoin = "CAVE";
					this.groupMapJoinIndex = 1;
					PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
					PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
					PlayerPrefs.Save();
				}
			}
			else if (num != 1128467232U)
			{
				if (num == 3724402957U)
				{
					if (characterString == "enter")
					{
						this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
					}
				}
			}
			else if (characterString == "up")
			{
				this.SwitchToQueueState();
			}
			this.roomFull = false;
			this.UpdateScreen();
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x00069D54 File Offset: 0x00067F54
		private void ProcessVoiceState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			if (!(characterString == "up"))
			{
				if (!(characterString == "down"))
				{
					if (!(characterString == "option1"))
					{
						if (characterString == "option2")
						{
							this.voiceChatOn = "FALSE";
							PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
							PlayerPrefs.Save();
							RigContainer.RefreshAllRigVoices();
						}
					}
					else
					{
						this.voiceChatOn = "TRUE";
						PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
						PlayerPrefs.Save();
						RigContainer.RefreshAllRigVoices();
					}
				}
				else
				{
					this.SwitchToVisualsState();
				}
			}
			else
			{
				this.SwitchToGroupState();
			}
			this.UpdateScreen();
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x00069E00 File Offset: 0x00068000
		private void ProcessVisualsState(GorillaKeyboardButton buttonPressed)
		{
			int num;
			if (int.TryParse(buttonPressed.characterString, out num))
			{
				this.instrumentVolume = (float)num / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
			}
			else
			{
				string characterString = buttonPressed.characterString;
				if (!(characterString == "up"))
				{
					if (!(characterString == "down"))
					{
						if (!(characterString == "option1"))
						{
							if (characterString == "option2")
							{
								this.disableParticles = true;
								PlayerPrefs.SetString("disableParticles", "TRUE");
								PlayerPrefs.Save();
								GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
							}
						}
						else
						{
							this.disableParticles = false;
							PlayerPrefs.SetString("disableParticles", "FALSE");
							PlayerPrefs.Save();
							GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
						}
					}
					else
					{
						this.SwitchToCreditsState();
					}
				}
				else
				{
					this.SwitchToVoiceState();
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x00069EF8 File Offset: 0x000680F8
		private void ProcessCreditsState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			if (!(characterString == "up"))
			{
				if (!(characterString == "down"))
				{
					if (characterString == "enter")
					{
						this.creditsView.ProcessButtonPress(buttonPressed);
					}
				}
				else
				{
					this.SwitchToSupportState();
				}
			}
			else
			{
				this.SwitchToVisualsState();
			}
			this.UpdateScreen();
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x00069F58 File Offset: 0x00068158
		private void ProcessSupportState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			if (!(characterString == "up"))
			{
				if (!(characterString == "down"))
				{
					if (characterString == "enter")
					{
						this.displaySupport = true;
					}
				}
				else
				{
					this.displaySupport = false;
					this.SwitchToRoomState();
				}
			}
			else
			{
				this.displaySupport = false;
				this.SwitchToCreditsState();
			}
			this.UpdateScreen();
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x00069FC0 File Offset: 0x000681C0
		private void ProcessNameWarningState(GorillaKeyboardButton buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
			}
			else
			{
				string characterString = buttonPressed.characterString;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(characterString);
				if (num <= 1740784714U)
				{
					if (num != 1035581717U)
					{
						if (num != 1128467232U)
						{
							if (num == 1740784714U)
							{
								if (characterString == "delete")
								{
									if (this.warningConfirmationInputString.Length > 0)
									{
										this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
										goto IL_154;
									}
									goto IL_154;
								}
							}
						}
						else if (characterString == "up")
						{
							goto IL_154;
						}
					}
					else if (characterString == "down")
					{
						goto IL_154;
					}
				}
				else if (num <= 2921858642U)
				{
					if (num != 2905081023U)
					{
						if (num == 2921858642U)
						{
							if (characterString == "option2")
							{
								goto IL_154;
							}
						}
					}
					else if (characterString == "option1")
					{
						goto IL_154;
					}
				}
				else if (num != 2938636261U)
				{
					if (num == 3724402957U)
					{
						if (characterString == "enter")
						{
							goto IL_154;
						}
					}
				}
				else if (characterString == "option3")
				{
					goto IL_154;
				}
				if (this.warningConfirmationInputString.Length < 3)
				{
					this.warningConfirmationInputString += buttonPressed.characterString;
				}
			}
			IL_154:
			this.UpdateScreen();
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x0006A128 File Offset: 0x00068328
		public void UpdateScreen()
		{
			if (PhotonNetworkController.Instance != null && !PhotonNetworkController.Instance.wrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.screenText.Text = string.Concat(new string[]
					{
						"GORILLA OS\n\n",
						PhotonNetworkController.Instance.TotalUsers().ToString(),
						" PLAYERS ONLINE\n\n",
						this.usersBanned.ToString(),
						" USERS BANNED YESTERDAY\n\nPRESS ANY KEY TO BEGIN"
					});
					break;
				case GorillaComputer.ComputerState.Color:
				{
					this.screenText.Text = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
					GorillaText gorillaText = this.screenText;
					gorillaText.Text = gorillaText.Text + "\n\n  RED: " + Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : "");
					GorillaText gorillaText2 = this.screenText;
					gorillaText2.Text = gorillaText2.Text + "\n\nGREEN: " + Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : "");
					GorillaText gorillaText3 = this.screenText;
					gorillaText3.Text = gorillaText3.Text + "\n\n BLUE: " + Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : "");
					break;
				}
				case GorillaComputer.ComputerState.Name:
				{
					this.screenText.Text = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\nCURRENT NAME: " + this.savedName;
					GorillaText gorillaText4 = this.screenText;
					gorillaText4.Text = gorillaText4.Text + "\n\n    NEW NAME: " + this.currentName;
					break;
				}
				case GorillaComputer.ComputerState.Turn:
					this.screenText.Text = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING. PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.\n CURRENT TURN TYPE: " + this.turnType + "\nCURRENT TURN SPEED: " + this.turnValue.ToString();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.screenText.Text = "CHOOSE ALL CHAT, PUSH TO TALK, OR PUSH TO MUTE. THE BUTTONS FOR PUSH TO TALK AND PUSH TO MUTE ARE ANY OF THE FACE BUTTONS.\nPRESS OPTION 1 TO CHOOSE ALL CHAT.\nPRESS OPTION 2 TO CHOOSE PUSH TO TALK.\nPRESS OPTION 3 TO CHOOSE PUSH TO MUTE.\n\nCURRENT MIC SETTING: " + this.pttType;
					break;
				case GorillaComputer.ComputerState.Room:
				{
					this.screenText.Text = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE. PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.\n\nCURRENT ROOM: ";
					if (PhotonNetwork.InRoom)
					{
						GorillaText gorillaText5 = this.screenText;
						gorillaText5.Text += PhotonNetwork.CurrentRoom.Name;
						GorillaText gorillaText6 = this.screenText;
						gorillaText6.Text = gorillaText6.Text + "\n\nPLAYERS IN ROOM: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
					}
					else
					{
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text += "-NOT IN ROOM-";
						GorillaText gorillaText8 = this.screenText;
						gorillaText8.Text = gorillaText8.Text + "\n\nPLAYERS ONLINE: " + PhotonNetworkController.Instance.TotalUsers().ToString();
					}
					GorillaText gorillaText9 = this.screenText;
					gorillaText9.Text = gorillaText9.Text + "\n\nROOM TO JOIN: " + this.roomToJoin;
					if (this.roomFull)
					{
						GorillaText gorillaText10 = this.screenText;
						gorillaText10.Text += "\n\nROOM FULL. JOIN ROOM FAILED.";
					}
					else if (this.roomNotAllowed)
					{
						GorillaText gorillaText11 = this.screenText;
						gorillaText11.Text += "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					}
					break;
				}
				case GorillaComputer.ComputerState.Queue:
					if (this.allowedInCompetitive)
					{
						this.screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN. PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.\n\nCURRENT QUEUE: " + this.currentQueue;
					}
					else
					{
						this.screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY. PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES\n\nCURRENT QUEUE: " + this.currentQueue;
					}
					break;
				case GorillaComputer.ComputerState.Group:
					if (this.allowedMapsToJoin.Length == 1)
					{
						this.screenText.Text = "USE THIS TO JOIN A PUBLIC ROOM WITH A GROUP OF FRIENDS. GET EVERYONE IN A PRIVATE ROOM. PRESS THE NUMBER KEYS TO SELECT THE MAP. 1 FOR " + this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)].ToUpper() + ". WHILE EVERYONE IS SITTING NEXT TO THE COMPUTER, PRESS ENTER. YOU WILL ALL JOIN A PUBLIC ROOM TOGETHER AS LONG AS EVERYONE IS NEXT TO THE COMPUTER.\nCURRENT MAP SELECTION : " + this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)].ToUpper();
					}
					else
					{
						this.screenText.Text = "USE THIS TO JOIN A PUBLIC ROOM WITH A GROUP OF FRIENDS. GET EVERYONE IN A PRIVATE ROOM. PRESS THE NUMBER KEYS TO SELECT THE MAP. 1 FOR FOREST, 2 FOR CAVE, AND 3 FOR CANYON, AND 4 FOR CITY. WHILE EVERYONE IS SITTING NEXT TO THE COMPUTER, PRESS ENTER. YOU WILL ALL JOIN A PUBLIC ROOM TOGETHER AS LONG AS EVERYONE IS NEXT TO THE COMPUTER.\nCURRENT MAP SELECTION : " + this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)].ToUpper();
					}
					break;
				case GorillaComputer.ComputerState.Voice:
					this.screenText.Text = "USE THIS TO ENABLE OR DISABLE VOICE CHAT.\nPRESS OPTION 1 TO ENABLE VOICE CHAT.\nPRESS OPTION 2 TO DISABLE VOICE CHAT.\n\nVOICE CHAT ON: " + this.voiceChatOn;
					break;
				case GorillaComputer.ComputerState.Credits:
					this.screenText.Text = this.creditsView.GetScreenText();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.screenText.Text = "UPDATE ITEMS SETTINGS. PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.\n\nITEM PARTICLES ON: " + (this.disableParticles ? "FALSE" : "TRUE") + "\nINSTRUMENT VOLUME: " + Mathf.CeilToInt(this.instrumentVolume * 50f).ToString();
					break;
				case GorillaComputer.ComputerState.Time:
					this.screenText.Text = string.Concat(new string[]
					{
						"UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: ",
						BetterDayNightManager.instance.currentSetting.ToString().ToUpper(),
						". \nTIME OF DAY: ",
						BetterDayNightManager.instance.currentTimeOfDay.ToUpper(),
						". \n"
					});
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.screenText.Text = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
					if (this.warningConfirmationInputString.ToLower() == "yes")
					{
						GorillaText gorillaText12 = this.screenText;
						gorillaText12.Text += "\n\nPRESS ANY KEY TO CONTINUE";
					}
					else
					{
						GorillaText gorillaText13 = this.screenText;
						gorillaText13.Text = gorillaText13.Text + "\n\nTYPE 'YES' TO CONFIRM: " + this.warningConfirmationInputString;
					}
					break;
				case GorillaComputer.ComputerState.Loading:
					this.screenText.Text = "LOADING...";
					break;
				case GorillaComputer.ComputerState.Support:
					if (this.displaySupport)
					{
						string text = "STEAM";
						this.screenText.Text = string.Concat(new string[]
						{
							"SUPPORT\n\nPLAYERID   ",
							PlayFabAuthenticator.instance._playFabPlayerIdCache,
							"\nVERSION    ",
							this.version.ToUpper(),
							"\nPLATFORM   ",
							text,
							"\nBUILD DATE ",
							this.buildDate,
							"\n"
						});
					}
					else
					{
						this.screenText.Text = "SUPPORT\n\n";
						GorillaText gorillaText14 = this.screenText;
						gorillaText14.Text += "PRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION\n\n\n\n";
						GorillaText gorillaText15 = this.screenText;
						gorillaText15.Text += "<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER ";
						GorillaText gorillaText16 = this.screenText;
						gorillaText16.Text += "THAN ANOTHER AXIOM SUPPORT</color>";
					}
					break;
				}
			}
			if (!PhotonNetwork.InRoom)
			{
				this.currentGameModeText.text = "CURRENT MODE\n-NOT IN ROOM-";
				return;
			}
			if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<GorillaTagManager>() != null)
			{
				if (!GorillaGameManager.instance.GetComponent<GorillaTagManager>().IsGameModeTag())
				{
					this.currentGameModeText.text = "CURRENT MODE\nCASUAL";
					return;
				}
				this.currentGameModeText.text = "CURRENT MODE\nINFECTION";
				return;
			}
			else
			{
				if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<GorillaHuntManager>() != null)
				{
					this.currentGameModeText.text = "CURRENT MODE\nHUNT";
					return;
				}
				if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<GorillaBattleManager>() != null)
				{
					this.currentGameModeText.text = "CURRENT MODE\nPAINTBRAWL";
					return;
				}
				this.currentGameModeText.text = "CURRENT MODE\nERROR";
				return;
			}
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x0006A8CC File Offset: 0x00068ACC
		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Text = string.Concat(new string[]
			{
				"ROOM   ",
				(this.currentState == GorillaComputer.ComputerState.Room) ? "<-" : "",
				"\nNAME   ",
				(this.currentState == GorillaComputer.ComputerState.Name) ? "<-" : "",
				"\nCOLOR  ",
				(this.currentState == GorillaComputer.ComputerState.Color) ? "<-" : "",
				"\nTURN   ",
				(this.currentState == GorillaComputer.ComputerState.Turn) ? "<-" : "",
				"\nMIC    ",
				(this.currentState == GorillaComputer.ComputerState.Mic) ? "<-" : "",
				"\nQUEUE  ",
				(this.currentState == GorillaComputer.ComputerState.Queue) ? "<-" : "",
				"\nGROUP  ",
				(this.currentState == GorillaComputer.ComputerState.Group) ? "<-" : "",
				"\nVOICE  ",
				(this.currentState == GorillaComputer.ComputerState.Voice) ? "<-" : "",
				"\nITEMS  ",
				(this.currentState == GorillaComputer.ComputerState.Visuals) ? "<-" : "",
				"\nCREDITS",
				(this.currentState == GorillaComputer.ComputerState.Credits) ? "<-" : "",
				"\nSUPPORT",
				(this.currentState == GorillaComputer.ComputerState.Support) ? "<-" : ""
			});
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0006AA60 File Offset: 0x00068C60
		private void OnReturnCurrentVersion(PlayFab.ClientModels.ExecuteCloudScriptResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", out obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", out obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("BannedUsers", out obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.versionText = "WELCOME TO GORILLA TAG! HEAD OUTSIDE TO AUTOMATICALLY JOIN A PUBLIC GAME, OR USE THE TERMINAL TO JOIN A SPECIFIC ROOM OR ADJUST YOUR SETTINGS.";
			this.UpdateScreen();
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0006AB2C File Offset: 0x00068D2C
		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this.roomToJoin);
					return;
				case 1:
					this.roomToJoin = "";
					this.SwitchToWarningState();
					return;
				case 2:
				{
					this.roomToJoin = "";
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
					break;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0006ABF0 File Offset: 0x00068DF0
		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					PhotonNetwork.LocalPlayer.NickName = this.currentName;
					break;
				case 1:
					PhotonNetwork.LocalPlayer.NickName = "gorilla";
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
				{
					PhotonNetwork.LocalPlayer.NickName = "gorilla";
					this.currentName = "gorilla";
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
					break;
				}
				}
			}
			this.offlineVRRigNametagText.text = this.currentName;
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue,
					this.leftHanded
				});
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0006AD62 File Offset: 0x00068F62
		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0006AD7C File Offset: 0x00068F7C
		private void CheckAutoBanList(string nameToCheck, bool forRoom)
		{
			if (forRoom)
			{
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = new PlayFab.CloudScriptModels.EntityKey
					{
						Id = PlayFabSettings.staticPlayer.EntityId,
						Type = PlayFabSettings.staticPlayer.EntityType
					},
					FunctionName = "CheckForBadName",
					FunctionParameter = new Dictionary<string, string>
					{
						{
							"name",
							nameToCheck
						},
						{
							"forRoom",
							forRoom.ToString()
						}
					},
					GeneratePlayStreamEvent = new bool?(false)
				}, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck), null, null);
			}
			else
			{
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = new PlayFab.CloudScriptModels.EntityKey
					{
						Id = PlayFabSettings.staticPlayer.EntityId,
						Type = PlayFabSettings.staticPlayer.EntityType
					},
					FunctionName = "CheckForBadName",
					FunctionParameter = new Dictionary<string, string>
					{
						{
							"name",
							nameToCheck
						},
						{
							"forRoom",
							forRoom.ToString()
						}
					},
					GeneratePlayStreamEvent = new bool?(false)
				}, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck), null, null);
			}
			this.SwitchToLoadingState();
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0006AEB8 File Offset: 0x000690B8
		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string value in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(value) >= 0)
				{
					return false;
				}
			}
			foreach (string value2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(value2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0006AF78 File Offset: 0x00069178
		public void UpdateFailureText(string failMessage)
		{
			GorillaLevelScreen[] array = this.levelScreens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateText(failMessage, false);
			}
			this.offlineScoreboard.EnableFailedState(failMessage);
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
			this.wallScreenText.EnableFailedState(failMessage);
			this.tutorialWallScreenText.EnableFailedState(failMessage);
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0006AFE0 File Offset: 0x000691E0
		private void RestoreFromFailureState()
		{
			foreach (GorillaLevelScreen gorillaLevelScreen in this.levelScreens)
			{
				gorillaLevelScreen.UpdateText(gorillaLevelScreen.startingText, true);
			}
			this.offlineScoreboard.DisableFailedState();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
			this.wallScreenText.DisableFailedState();
			this.tutorialWallScreenText.DisableFailedState();
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x0006B048 File Offset: 0x00069248
		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			PhotonNetworkController.Instance.WrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x0006B06C File Offset: 0x0006926C
		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance._playFabPlayerIdCache,
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance._playFabPlayerIdCache + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x0006B2FC File Offset: 0x000694FC
		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x0006B32C File Offset: 0x0006952C
		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			this.OnServerTimeUpdated();
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x0006B38C File Offset: 0x0006958C
		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			this.OnServerTimeUpdated();
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x0006B450 File Offset: 0x00069650
		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.currentGameMode = gameMode;
			PlayerPrefs.SetString("currentGameMode", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			foreach (ModeSelectButton modeSelectButton in this.modeSelectButtons)
			{
				modeSelectButton.buttonRenderer.material = ((this.currentGameMode == modeSelectButton.gameMode) ? modeSelectButton.pressedMaterial : modeSelectButton.unpressedMaterial);
			}
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x0006B4DC File Offset: 0x000696DC
		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			if (mapJoinIndex >= this.allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.SwitchToRoomState();
				return;
			}
			if (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible)
			{
				PhotonNetworkController.Instance.friendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
				foreach (string text in PhotonNetworkController.Instance.friendIDList)
				{
				}
				PhotonNetworkController.Instance.shuffler = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
				PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
				foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
				{
					if (chosenFriendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
					{
						GorillaGameManager.instance.photonView.RPC("JoinPubWithFriends", player, new object[]
						{
							PhotonNetworkController.Instance.shuffler,
							PhotonNetworkController.Instance.keyStr
						});
					}
				}
				PhotonNetwork.SendAllOutgoingCommands();
				GorillaNetworkJoinTrigger triggeredTrigger = null;
				if (this.allowedMapsToJoin[mapJoinIndex] == "forest")
				{
					triggeredTrigger = this.forestMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "cave")
				{
					triggeredTrigger = this.caveMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "canyon")
				{
					triggeredTrigger = this.canyonMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "city")
				{
					triggeredTrigger = this.cityMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "mountain")
				{
					triggeredTrigger = this.mountainMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "clouds")
				{
					triggeredTrigger = this.skyjungleMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "basement")
				{
					triggeredTrigger = this.basementMapTrigger;
				}
				else if (this.allowedMapsToJoin[mapJoinIndex] == "beach")
				{
					triggeredTrigger = this.beachMapTrigger;
				}
				PhotonNetworkController.Instance.AttemptJoinPublicWithFriends(triggeredTrigger);
				this.SwitchToRoomState();
			}
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0006B740 File Offset: 0x00069940
		public void SaveModAccountData()
		{
			string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
			if (File.Exists(path))
			{
				return;
			}
			ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
			executeCloudScriptRequest.FunctionName = "ReturnMyOculusHash";
			PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				object obj;
				if (((JsonObject)result.FunctionResult).TryGetValue("oculusHash", out obj))
				{
					StreamWriter streamWriter = new StreamWriter(path);
					streamWriter.Write(PlayFabAuthenticator.instance._playFabPlayerIdCache + "." + (string)obj);
					streamWriter.Close();
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x0006B7B8 File Offset: 0x000699B8
		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
		}

		// Token: 0x04001522 RID: 5410
		public static volatile GorillaComputer instance;

		// Token: 0x04001523 RID: 5411
		public static bool hasInstance;

		// Token: 0x04001524 RID: 5412
		public bool tryGetTimeAgain;

		// Token: 0x04001525 RID: 5413
		public Material unpressedMaterial;

		// Token: 0x04001526 RID: 5414
		public Material pressedMaterial;

		// Token: 0x04001527 RID: 5415
		public string currentTextField;

		// Token: 0x04001528 RID: 5416
		public float buttonFadeTime;

		// Token: 0x04001529 RID: 5417
		public GorillaText offlineScoreboard;

		// Token: 0x0400152A RID: 5418
		public GorillaText screenText;

		// Token: 0x0400152B RID: 5419
		public GorillaText functionSelectText;

		// Token: 0x0400152C RID: 5420
		public GorillaText wallScreenText;

		// Token: 0x0400152D RID: 5421
		public GorillaText tutorialWallScreenText;

		// Token: 0x0400152E RID: 5422
		public Text offlineVRRigNametagText;

		// Token: 0x0400152F RID: 5423
		public string versionText = "";

		// Token: 0x04001530 RID: 5424
		public string versionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		// Token: 0x04001531 RID: 5425
		public string unableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		// Token: 0x04001532 RID: 5426
		public Material wrongVersionMaterial;

		// Token: 0x04001533 RID: 5427
		public MeshRenderer wallScreenRenderer;

		// Token: 0x04001534 RID: 5428
		public MeshRenderer tutorialWallScreenRenderer;

		// Token: 0x04001535 RID: 5429
		public MeshRenderer computerScreenRenderer;

		// Token: 0x04001536 RID: 5430
		public MeshRenderer scoreboardRenderer;

		// Token: 0x04001537 RID: 5431
		public GorillaLevelScreen[] levelScreens;

		// Token: 0x04001538 RID: 5432
		public long startupMillis;

		// Token: 0x04001539 RID: 5433
		public DateTime startupTime;

		// Token: 0x0400153A RID: 5434
		public Text currentGameModeText;

		// Token: 0x0400153B RID: 5435
		public int includeUpdatedServerSynchTest;

		// Token: 0x0400153C RID: 5436
		public float updateCooldown = 1f;

		// Token: 0x0400153D RID: 5437
		public float lastUpdateTime;

		// Token: 0x0400153E RID: 5438
		public bool isConnectedToMaster;

		// Token: 0x0400153F RID: 5439
		public bool internetFailure;

		// Token: 0x04001540 RID: 5440
		public string[] allowedMapsToJoin;

		// Token: 0x04001541 RID: 5441
		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		// Token: 0x04001542 RID: 5442
		public bool stateUpdated;

		// Token: 0x04001543 RID: 5443
		public bool screenChanged;

		// Token: 0x04001544 RID: 5444
		private int usersBanned;

		// Token: 0x04001545 RID: 5445
		private float redValue;

		// Token: 0x04001546 RID: 5446
		private string redText;

		// Token: 0x04001547 RID: 5447
		private float blueValue;

		// Token: 0x04001548 RID: 5448
		private string blueText;

		// Token: 0x04001549 RID: 5449
		private float greenValue;

		// Token: 0x0400154A RID: 5450
		private string greenText;

		// Token: 0x0400154B RID: 5451
		private int colorCursorLine;

		// Token: 0x0400154C RID: 5452
		public string savedName;

		// Token: 0x0400154D RID: 5453
		public string currentName;

		// Token: 0x0400154E RID: 5454
		private string[] exactOneWeek;

		// Token: 0x0400154F RID: 5455
		private string[] anywhereOneWeek;

		// Token: 0x04001550 RID: 5456
		private string[] anywhereTwoWeek;

		// Token: 0x04001551 RID: 5457
		[SerializeField]
		public TextAsset exactOneWeekFile;

		// Token: 0x04001552 RID: 5458
		public TextAsset anywhereOneWeekFile;

		// Token: 0x04001553 RID: 5459
		public TextAsset anywhereTwoWeekFile;

		// Token: 0x04001554 RID: 5460
		private string warningConfirmationInputString = string.Empty;

		// Token: 0x04001555 RID: 5461
		public string roomToJoin;

		// Token: 0x04001556 RID: 5462
		public bool roomFull;

		// Token: 0x04001557 RID: 5463
		public bool roomNotAllowed;

		// Token: 0x04001558 RID: 5464
		private int turnValue;

		// Token: 0x04001559 RID: 5465
		private string turnType;

		// Token: 0x0400155A RID: 5466
		private GorillaSnapTurn gorillaTurn;

		// Token: 0x0400155B RID: 5467
		public string pttType;

		// Token: 0x0400155C RID: 5468
		public string currentQueue;

		// Token: 0x0400155D RID: 5469
		public bool allowedInCompetitive;

		// Token: 0x0400155E RID: 5470
		public string groupMapJoin;

		// Token: 0x0400155F RID: 5471
		public int groupMapJoinIndex;

		// Token: 0x04001560 RID: 5472
		public GorillaFriendCollider friendJoinCollider;

		// Token: 0x04001561 RID: 5473
		public GorillaNetworkJoinTrigger caveMapTrigger;

		// Token: 0x04001562 RID: 5474
		public GorillaNetworkJoinTrigger forestMapTrigger;

		// Token: 0x04001563 RID: 5475
		public GorillaNetworkJoinTrigger canyonMapTrigger;

		// Token: 0x04001564 RID: 5476
		public GorillaNetworkJoinTrigger cityMapTrigger;

		// Token: 0x04001565 RID: 5477
		public GorillaNetworkJoinTrigger mountainMapTrigger;

		// Token: 0x04001566 RID: 5478
		public GorillaNetworkJoinTrigger skyjungleMapTrigger;

		// Token: 0x04001567 RID: 5479
		public GorillaNetworkJoinTrigger basementMapTrigger;

		// Token: 0x04001568 RID: 5480
		public GorillaNetworkJoinTrigger beachMapTrigger;

		// Token: 0x04001569 RID: 5481
		public string voiceChatOn;

		// Token: 0x0400156A RID: 5482
		public ModeSelectButton[] modeSelectButtons;

		// Token: 0x0400156B RID: 5483
		public string currentGameMode;

		// Token: 0x0400156C RID: 5484
		public string version;

		// Token: 0x0400156D RID: 5485
		public string buildDate;

		// Token: 0x0400156E RID: 5486
		public string buildCode;

		// Token: 0x0400156F RID: 5487
		public bool disableParticles;

		// Token: 0x04001570 RID: 5488
		public float instrumentVolume;

		// Token: 0x04001571 RID: 5489
		public CreditsView creditsView;

		// Token: 0x04001572 RID: 5490
		private bool displaySupport;

		// Token: 0x04001573 RID: 5491
		public bool leftHanded;

		// Token: 0x04001574 RID: 5492
		public Action OnServerTimeUpdated;

		// Token: 0x020004C7 RID: 1223
		public enum ComputerState
		{
			// Token: 0x04001FD3 RID: 8147
			Startup,
			// Token: 0x04001FD4 RID: 8148
			Color,
			// Token: 0x04001FD5 RID: 8149
			Name,
			// Token: 0x04001FD6 RID: 8150
			Turn,
			// Token: 0x04001FD7 RID: 8151
			Mic,
			// Token: 0x04001FD8 RID: 8152
			Room,
			// Token: 0x04001FD9 RID: 8153
			Queue,
			// Token: 0x04001FDA RID: 8154
			Group,
			// Token: 0x04001FDB RID: 8155
			Voice,
			// Token: 0x04001FDC RID: 8156
			Credits,
			// Token: 0x04001FDD RID: 8157
			Visuals,
			// Token: 0x04001FDE RID: 8158
			Time,
			// Token: 0x04001FDF RID: 8159
			NameWarning,
			// Token: 0x04001FE0 RID: 8160
			Loading,
			// Token: 0x04001FE1 RID: 8161
			Support
		}

		// Token: 0x020004C8 RID: 1224
		private enum NameCheckResult
		{
			// Token: 0x04001FE3 RID: 8163
			Success,
			// Token: 0x04001FE4 RID: 8164
			Warning,
			// Token: 0x04001FE5 RID: 8165
			Ban
		}
	}
}
