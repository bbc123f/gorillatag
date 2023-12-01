using System;
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
	public class GorillaComputer : MonoBehaviourPunCallbacks
	{
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		public GorillaComputer.ComputerState currentState
		{
			get
			{
				return this.stateStack.Peek();
			}
		}

		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

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

		private IEnumerator Start()
		{
			yield return null;
			if (BacktraceClient.Instance && this.includeUpdatedServerSynchTest == 1)
			{
				Object.Destroy(BacktraceClient.Instance.gameObject);
			}
			yield break;
		}

		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
		}

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

		private void InitializeStartupState()
		{
		}

		private void InitializeRoomState()
		{
		}

		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

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

		private void InitializeTurnState()
		{
			this.gorillaTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			string defaultValue = (Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP";
			this.turnType = PlayerPrefs.GetString("stickTurning", defaultValue);
			this.turnValue = PlayerPrefs.GetInt("turnFactor", 4);
			this.gorillaTurn.ChangeTurnMode(this.turnType, this.turnValue);
		}

		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");
		}

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

		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		private void InitializeVoiceState()
		{
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", "TRUE");
		}

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

		private void InitializeCreditsState()
		{
		}

		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		private void InitializeVisualsState()
		{
			this.disableParticles = (PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE");
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
			this.UpdateScreen();
		}

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

		private void SwitchToColorState()
		{
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.SwitchState(GorillaComputer.ComputerState.Color, true);
		}

		private void SwitchToRoomState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Room, true);
		}

		private void SwitchToNameState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Name, true);
		}

		private void SwitchToTurnState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Turn, true);
		}

		private void SwitchToMicState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Mic, true);
		}

		private void SwitchToQueueState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Queue, true);
		}

		private void SwitchToGroupState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Group, true);
		}

		private void SwitchToVoiceState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Voice, true);
		}

		private void SwitchToCreditsState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Credits, true);
		}

		private void SwitchToSupportState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Support, true);
		}

		private void SwitchToVisualsState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Visuals, true);
		}

		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		private void ProcessStartupState(GorillaKeyboardButton buttonPressed)
		{
			string characterString = buttonPressed.characterString;
			this.SwitchToRoomState();
			this.UpdateScreen();
		}

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

		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

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

		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			PhotonNetworkController.Instance.WrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

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

		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated();
		}

		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated();
			}
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

		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
		}

		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		[OnEnterPlay_Set(false)]
		public static bool hasInstance;

		public bool tryGetTimeAgain;

		public Material unpressedMaterial;

		public Material pressedMaterial;

		public string currentTextField;

		public float buttonFadeTime;

		public GorillaText offlineScoreboard;

		public GorillaText screenText;

		public GorillaText functionSelectText;

		public GorillaText wallScreenText;

		public GorillaText tutorialWallScreenText;

		public Text offlineVRRigNametagText;

		public string versionText = "";

		public string versionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		public string unableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		public Material wrongVersionMaterial;

		public MeshRenderer wallScreenRenderer;

		public MeshRenderer tutorialWallScreenRenderer;

		public MeshRenderer computerScreenRenderer;

		public MeshRenderer scoreboardRenderer;

		public GorillaLevelScreen[] levelScreens;

		public long startupMillis;

		public DateTime startupTime;

		public Text currentGameModeText;

		public int includeUpdatedServerSynchTest;

		public float updateCooldown = 1f;

		public float lastUpdateTime;

		public bool isConnectedToMaster;

		public bool internetFailure;

		public string[] allowedMapsToJoin;

		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		public bool stateUpdated;

		public bool screenChanged;

		private int usersBanned;

		private float redValue;

		private string redText;

		private float blueValue;

		private string blueText;

		private float greenValue;

		private string greenText;

		private int colorCursorLine;

		public string savedName;

		public string currentName;

		private string[] exactOneWeek;

		private string[] anywhereOneWeek;

		private string[] anywhereTwoWeek;

		[SerializeField]
		public TextAsset exactOneWeekFile;

		public TextAsset anywhereOneWeekFile;

		public TextAsset anywhereTwoWeekFile;

		private string warningConfirmationInputString = string.Empty;

		public string roomToJoin;

		public bool roomFull;

		public bool roomNotAllowed;

		private int turnValue;

		private string turnType;

		private GorillaSnapTurn gorillaTurn;

		public string pttType;

		public string currentQueue;

		public bool allowedInCompetitive;

		public string groupMapJoin;

		public int groupMapJoinIndex;

		public GorillaFriendCollider friendJoinCollider;

		public GorillaNetworkJoinTrigger caveMapTrigger;

		public GorillaNetworkJoinTrigger forestMapTrigger;

		public GorillaNetworkJoinTrigger canyonMapTrigger;

		public GorillaNetworkJoinTrigger cityMapTrigger;

		public GorillaNetworkJoinTrigger mountainMapTrigger;

		public GorillaNetworkJoinTrigger skyjungleMapTrigger;

		public GorillaNetworkJoinTrigger basementMapTrigger;

		public GorillaNetworkJoinTrigger beachMapTrigger;

		public string voiceChatOn;

		public ModeSelectButton[] modeSelectButtons;

		public string currentGameMode;

		public string version;

		public string buildDate;

		public string buildCode;

		public bool disableParticles;

		public float instrumentVolume;

		public CreditsView creditsView;

		private bool displaySupport;

		public bool leftHanded;

		public Action OnServerTimeUpdated;

		public enum ComputerState
		{
			Startup,
			Color,
			Name,
			Turn,
			Mic,
			Room,
			Queue,
			Group,
			Voice,
			Credits,
			Visuals,
			Time,
			NameWarning,
			Loading,
			Support
		}

		private enum NameCheckResult
		{
			Success,
			Warning,
			Ban
		}
	}
}
