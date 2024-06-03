using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	public class PlayFabAuthenticator : MonoBehaviour
	{
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		public void Awake()
		{
			if (PlayFabAuthenticator.instance == null)
			{
				PlayFabAuthenticator.instance = this;
			}
			else if (PlayFabAuthenticator.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			PlayFabSettings.CompressApiData = false;
			new byte[1];
			if (this.screenDebugMode)
			{
				this.debugText.text = "";
			}
			Debug.Log("doing steam thing");
			this.m_GetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(new Callback<GetAuthSessionTicketResponse_t>.DispatchDelegate(this.OnGetAuthSessionTicketResponse));
			this.platform = "Steam";
			Debug.Log("Environment is *************** PRODUCTION *******************");
			PlayFabSettings.TitleId = "63FDD";
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "75dc33b5-d5fc-4b4c-bf88-eb70baabe183";
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = "c5fddf06-024c-41f9-81ec-9411dc9c1b27";
			this.AuthenticateWithPlayFab();
			PlayFabSettings.DisableFocusTimeCollection = true;
		}

		private void Start()
		{
		}

		public void AuthenticateWithPlayFab()
		{
			if (!this.loginFailed)
			{
				Debug.Log("authenticating with playFab!");
				if (SteamManager.Initialized)
				{
					Debug.Log("trying to auth with steam");
					this.m_HAuthTicket = SteamUser.GetAuthSessionTicket(this.ticketBlob, this.ticketBlob.Length, out this.ticketSize);
					return;
				}
				base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
			}
		}

		private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
		{
			yield return null;
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				this.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
			yield break;
		}

		private void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
		{
			Debug.Log("Got steam auth session ticket!");
			this.oculusID = SteamUser.GetSteamID().ToString();
			PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
			{
				CreateAccount = new bool?(true),
				SteamTicket = this.GetSteamAuthTicket()
			}, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError), null, null);
		}

		private void OnLoginWithSteamResponse(LoginResult obj)
		{
			this._playFabId = obj.PlayFabId;
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
			{
				Platform = this.platform,
				SessionTicket = this._sessionTicket,
				PlayFabId = this._playFabId
			}, new Action<bool>(this.OnCachePlayFabIdRequest)));
		}

		private void OnCachePlayFabIdRequest(bool success)
		{
			if (success)
			{
				Debug.Log("Successfully cached PlayFab Id.  Continuing!");
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}

		private void MaybeGetNonce(LoginResult obj)
		{
			this._playFabId = obj.PlayFabId;
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			this.AdvanceLogin();
		}

		private void AdvanceLogin()
		{
			this.AuthenticateWithPhoton();
		}

		private void AuthenticateWithPhoton()
		{
			AuthenticationValues authenticationValues = new AuthenticationValues(PlayFabSettings.DeviceUniqueIdentifier);
			authenticationValues.AuthType = CustomAuthenticationType.Custom;
			string playFabPlayerIdCache = this._playFabPlayerIdCache;
			authenticationValues.AddAuthParameter("username", this._playFabPlayerIdCache);
			Dictionary<string, object> authPostData = new Dictionary<string, object>
			{
				{
					"AppId",
					PlayFabSettings.TitleId
				},
				{
					"AppVersion",
					PhotonNetwork.AppVersion ?? "-1"
				},
				{
					"Ticket",
					this._sessionTicket
				},
				{
					"Nonce",
					this._nonce
				}
			};
			authenticationValues.SetAuthPostData(authPostData);
			PhotonNetwork.AuthValues = authenticationValues;
			Debug.Log("Set Photon auth data. Appversion is: " + PhotonNetwork.AppVersion);
			this.GetPlayerDisplayName(this._playFabPlayerIdCache);
			ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
			executeCloudScriptRequest.FunctionName = "AddOrRemoveDLCOwnership";
			executeCloudScriptRequest.FunctionParameter = new
			{

			};
			PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate(ExecuteCloudScriptResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetUserCosmeticsAllowed();
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetUserCosmeticsAllowed();
				}
			}, null, null);
			if (CosmeticsController.instance != null)
			{
				Debug.Log("initializing cosmetics");
				CosmeticsController.instance.Initialize();
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.OnConnectedToMasterStuff();
			}
			else
			{
				base.StartCoroutine(this.ComputerOnConnectedToMaster());
			}
			if (PhotonNetworkController.Instance != null)
			{
				NetworkSystem.Instance.SetAuthenticationValues(null);
			}
		}

		private IEnumerator ComputerOnConnectedToMaster()
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			this.gorillaComputer.OnConnectedToMasterStuff();
			yield break;
		}

		private void OnPlayFabError(PlayFabError obj)
		{
			this.LogMessage(obj.ErrorMessage);
			Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
			this.loginFailed = true;
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
					return;
				}
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
			}
		}

		private static void AddGenericId(string serviceName, string userId)
		{
			AddGenericIDRequest addGenericIDRequest = new AddGenericIDRequest();
			addGenericIDRequest.GenericId = new GenericServiceId
			{
				ServiceName = serviceName,
				UserId = userId
			};
			PlayFabClientAPI.AddGenericID(addGenericIDRequest, delegate(AddGenericIDResult _)
			{
			}, delegate(PlayFabError _)
			{
				Debug.LogError("Error setting generic id");
			}, null, null);
		}

		public void LogMessage(string message)
		{
		}

		private void GetPlayerDisplayName(string playFabId)
		{
			GetPlayerProfileRequest getPlayerProfileRequest = new GetPlayerProfileRequest();
			getPlayerProfileRequest.PlayFabId = playFabId;
			getPlayerProfileRequest.ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			};
			PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, delegate(GetPlayerProfileResult result)
			{
				this._displayName = result.PlayerProfile.DisplayName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}, null, null);
		}

		public void SetDisplayName(string playerName)
		{
			if (this._displayName == null || (this._displayName.Length > 4 && this._displayName.Substring(0, this._displayName.Length - 4) != playerName))
			{
				UpdateUserTitleDisplayNameRequest updateUserTitleDisplayNameRequest = new UpdateUserTitleDisplayNameRequest();
				updateUserTitleDisplayNameRequest.DisplayName = playerName;
				PlayFabClientAPI.UpdateUserTitleDisplayName(updateUserTitleDisplayNameRequest, delegate(UpdateUserTitleDisplayNameResult result)
				{
					this._displayName = playerName;
				}, delegate(PlayFabError error)
				{
					Debug.LogError(error.GenerateErrorReport());
				}, null, null);
			}
		}

		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		public string GetSteamAuthTicket()
		{
			Array.Resize<byte>(ref this.ticketBlob, (int)this.ticketSize);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in this.ticketBlob)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		public IEnumerator PlayfabAuthenticate(PlayFabAuthenticator.PlayfabAuthRequestData data, Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
		{
			UnityWebRequest request = new UnityWebRequest("https://auth-prod.gtag-cf.com/api/PlayFabAuthentication", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (!request.isNetworkError && !request.isHttpError)
			{
				PlayFabAuthenticator.PlayfabAuthResponseData obj = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
				callback(obj);
			}
			else
			{
				if (request.responseCode == 403L)
				{
					Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
					PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
					this.ShowBanMessage(banInfo);
					callback(null);
				}
				if (request.isHttpError && request.responseCode != 400L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
				}
				else if (request.isNetworkError)
				{
					retry = true;
				}
			}
			if (retry)
			{
				if (this.playFabAuthRetryCount < this.playFabMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabAuthRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabAuthRetryCount + 1, num));
					this.playFabAuthRetryCount++;
					yield return new WaitForSeconds((float)num);
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
				}
			}
			yield break;
		}

		private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + ((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
					}
					else
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public IEnumerator CachePlayFabId(PlayFabAuthenticator.CachePlayFabIdRequest data, Action<bool> callback)
		{
			Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest("https://auth-prod.gtag-cf.com/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (!request.isNetworkError && !request.isHttpError)
			{
				if (request.responseCode == 200L)
				{
					callback(true);
				}
			}
			else if (request.isHttpError && request.responseCode != 400L)
			{
				retry = true;
				Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else if (request.isNetworkError)
			{
				retry = true;
			}
			if (retry)
			{
				if (this.playFabCacheRetryCount < this.playFabCacheMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabCacheRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabCacheRetryCount + 1, num));
					this.playFabCacheRetryCount++;
					yield return new WaitForSeconds((float)num);
					base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = this.platform,
						SessionTicket = this._sessionTicket,
						PlayFabId = this._playFabId
					}, new Action<bool>(this.OnCachePlayFabIdRequest)));
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(false);
				}
			}
			yield break;
		}

		public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
		{
			Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
			if (onSafetyUpdate != null)
			{
				onSafetyUpdate(isSafety);
			}
			this.isSafeAccount = isSafety;
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (isSafety)
			{
				if (isAutoSet)
				{
					PlayerPrefs.SetInt("autoSafety", 1);
					this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
				}
				else
				{
					PlayerPrefs.SetInt("optSafety", 1);
					this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
				}
				if (GorillaComputer.instance != null)
				{
					GorillaComputer.instance.voiceChatOn = "FALSE";
					PlayerPrefs.SetString("voiceChatOn", "FALSE");
				}
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				return;
			}
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (isAutoSet)
			{
				PlayerPrefs.SetInt("autoSafety", 0);
			}
			else
			{
				PlayerPrefs.SetInt("optSafety", 0);
			}
			PlayerPrefs.Save();
		}

		public bool GetSafety()
		{
			return this.isSafeAccount;
		}

		public PlayFabAuthenticator.SafetyType GetSafetyType()
		{
			return this.safetyType;
		}

		public PlayFabAuthenticator()
		{
		}

		[CompilerGenerated]
		private void <GetPlayerDisplayName>b__54_0(GetPlayerProfileResult result)
		{
			this._displayName = result.PlayerProfile.DisplayName;
		}

		public static volatile PlayFabAuthenticator instance;

		public const string Playfab_TitleId_Prod = "63FDD";

		public const string Playfab_TitleId_Dev = "195C0";

		public const string Photon_AppIdRealtime_Prod = "75dc33b5-d5fc-4b4c-bf88-eb70baabe183";

		public const string Photon_AppIdVoice_Prod = "c5fddf06-024c-41f9-81ec-9411dc9c1b27";

		public const string Photon_AppIdRealtime_Dev = "6a3946c5-d4ea-4705-bdb7-0a0c7e831ca7";

		public const string Photon_AppIdVoice_Dev = "456d1e18-05c7-4a54-abc9-330fa0bcd2aa";

		public const string Playfab_Auth_API = "https://auth-prod.gtag-cf.com";

		public string _playFabPlayerIdCache;

		private string _sessionTicket;

		private string _playFabId;

		private string _displayName;

		private string _nonce;

		private string _orgScopedId;

		public string userID;

		private ulong userIDLong;

		private string userToken;

		private string platform;

		private bool isSafeAccount;

		private bool doubleCheckEntitlement;

		public Action<bool> OnSafetyUpdate;

		private PlayFabAuthenticator.SafetyType safetyType;

		private byte[] m_Ticket;

		private uint m_pcbTicket;

		public Text debugText;

		public bool screenDebugMode;

		public bool loginFailed;

		[FormerlySerializedAs("loginDisplayID")]
		public string oculusID = "";

		public GameObject emptyObject;

		private int playFabAuthRetryCount;

		private int playFabMaxRetries = 5;

		private int playFabCacheRetryCount;

		private int playFabCacheMaxRetries = 5;

		private HAuthTicket m_HAuthTicket;

		private byte[] ticketBlob = new byte[1024];

		private uint ticketSize;

		protected Callback<GetAuthSessionTicketResponse_t> m_GetAuthSessionTicketResponse;

		public enum SafetyType
		{
			None,
			Auto,
			OptIn
		}

		[Serializable]
		public class CachePlayFabIdRequest
		{
			public CachePlayFabIdRequest()
			{
			}

			public string Platform;

			public string SessionTicket;

			public string PlayFabId;
		}

		[Serializable]
		public class PlayfabAuthRequestData
		{
			public PlayfabAuthRequestData()
			{
			}

			public string CustomId;

			public string AppId;

			public string AppVersion;

			public string Nonce;

			public string OculusId;

			public string Platform;
		}

		[Serializable]
		public class PlayfabAuthResponseData
		{
			public PlayfabAuthResponseData()
			{
			}

			public string SessionTicket;

			public string EntityToken;

			public string PlayFabId;

			public string EntityId;

			public string EntityType;
		}

		public class BanInfo
		{
			public BanInfo()
			{
			}

			public string BanMessage;

			public string BanExpirationTime;
		}

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

			internal void <AuthenticateWithPhoton>b__49_0(ExecuteCloudScriptResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetUserCosmeticsAllowed();
				}
			}

			internal void <AuthenticateWithPhoton>b__49_1(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetUserCosmeticsAllowed();
				}
			}

			internal void <AddGenericId>b__52_0(AddGenericIDResult _)
			{
			}

			internal void <AddGenericId>b__52_1(PlayFabError _)
			{
				Debug.LogError("Error setting generic id");
			}

			internal void <GetPlayerDisplayName>b__54_1(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}

			internal void <SetDisplayName>b__55_1(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}

			public static readonly PlayFabAuthenticator.<>c <>9 = new PlayFabAuthenticator.<>c();

			public static Action<ExecuteCloudScriptResult> <>9__49_0;

			public static Action<PlayFabError> <>9__49_1;

			public static Action<AddGenericIDResult> <>9__52_0;

			public static Action<PlayFabError> <>9__52_1;

			public static Action<PlayFabError> <>9__54_1;

			public static Action<PlayFabError> <>9__55_1;
		}

		[CompilerGenerated]
		private sealed class <>c__DisplayClass55_0
		{
			public <>c__DisplayClass55_0()
			{
			}

			internal void <SetDisplayName>b__0(UpdateUserTitleDisplayNameResult result)
			{
				this.<>4__this._displayName = this.playerName;
			}

			public PlayFabAuthenticator <>4__this;

			public string playerName;
		}

		[CompilerGenerated]
		private sealed class <CachePlayFabId>d__64 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <CachePlayFabId>d__64(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayFabAuthenticator playFabAuthenticator = this;
				switch (num)
				{
				case 0:
				{
					this.<>1__state = -1;
					Debug.Log("Trying to cache playfab Id");
					request = new UnityWebRequest("https://auth-prod.gtag-cf.com/api/CachePlayFabId", "POST");
					byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
					retry = false;
					request.uploadHandler = new UploadHandlerRaw(bytes);
					request.downloadHandler = new DownloadHandlerBuffer();
					request.SetRequestHeader("Content-Type", "application/json");
					this.<>2__current = request.SendWebRequest();
					this.<>1__state = 1;
					return true;
				}
				case 1:
					this.<>1__state = -1;
					if (!request.isNetworkError && !request.isHttpError)
					{
						if (request.responseCode == 200L)
						{
							callback(true);
						}
					}
					else if (request.isHttpError && request.responseCode != 400L)
					{
						retry = true;
						Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
					}
					else if (request.isNetworkError)
					{
						retry = true;
					}
					if (retry)
					{
						if (playFabAuthenticator.playFabCacheRetryCount < playFabAuthenticator.playFabCacheMaxRetries)
						{
							int num2 = (int)Mathf.Pow(2f, (float)(playFabAuthenticator.playFabCacheRetryCount + 1));
							Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", playFabAuthenticator.playFabCacheRetryCount + 1, num2));
							playFabAuthenticator.playFabCacheRetryCount++;
							this.<>2__current = new WaitForSeconds((float)num2);
							this.<>1__state = 2;
							return true;
						}
						Debug.LogError("Maximum retries attempted. Please check your network connection.");
						callback(false);
					}
					break;
				case 2:
					this.<>1__state = -1;
					playFabAuthenticator.StartCoroutine(playFabAuthenticator.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = playFabAuthenticator.platform,
						SessionTicket = playFabAuthenticator._sessionTicket,
						PlayFabId = playFabAuthenticator._playFabId
					}, new Action<bool>(playFabAuthenticator.OnCachePlayFabIdRequest)));
					break;
				default:
					return false;
				}
				return false;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public PlayFabAuthenticator.CachePlayFabIdRequest data;

			public Action<bool> callback;

			public PlayFabAuthenticator <>4__this;

			private UnityWebRequest <request>5__2;

			private bool <retry>5__3;
		}

		[CompilerGenerated]
		private sealed class <ComputerOnConnectedToMaster>d__50 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <ComputerOnConnectedToMaster>d__50(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayFabAuthenticator playFabAuthenticator = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
					frameYield = new WaitForEndOfFrame();
				}
				if (!(playFabAuthenticator.gorillaComputer == null))
				{
					playFabAuthenticator.gorillaComputer.OnConnectedToMasterStuff();
					return false;
				}
				this.<>2__current = frameYield;
				this.<>1__state = 1;
				return true;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public PlayFabAuthenticator <>4__this;

			private WaitForEndOfFrame <frameYield>5__2;
		}

		[CompilerGenerated]
		private sealed class <DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame>d__43 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame>d__43(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayFabAuthenticator playFabAuthenticator = this;
				if (num == 0)
				{
					this.<>1__state = -1;
					this.<>2__current = null;
					this.<>1__state = 1;
					return true;
				}
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				if (playFabAuthenticator.gorillaComputer != null)
				{
					playFabAuthenticator.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
					playFabAuthenticator.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
					Debug.Log("Couldn't authenticate steam account");
				}
				else
				{
					Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", playFabAuthenticator);
				}
				return false;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public PlayFabAuthenticator <>4__this;
		}

		[CompilerGenerated]
		private sealed class <PlayfabAuthenticate>d__62 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <PlayfabAuthenticate>d__62(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayFabAuthenticator playFabAuthenticator = this;
				switch (num)
				{
				case 0:
				{
					this.<>1__state = -1;
					request = new UnityWebRequest("https://auth-prod.gtag-cf.com/api/PlayFabAuthentication", "POST");
					byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
					retry = false;
					request.uploadHandler = new UploadHandlerRaw(bytes);
					request.downloadHandler = new DownloadHandlerBuffer();
					request.SetRequestHeader("Content-Type", "application/json");
					this.<>2__current = request.SendWebRequest();
					this.<>1__state = 1;
					return true;
				}
				case 1:
					this.<>1__state = -1;
					if (!request.isNetworkError && !request.isHttpError)
					{
						PlayFabAuthenticator.PlayfabAuthResponseData obj = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
						callback(obj);
					}
					else
					{
						if (request.responseCode == 403L)
						{
							Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
							PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
							playFabAuthenticator.ShowBanMessage(banInfo);
							callback(null);
						}
						if (request.isHttpError && request.responseCode != 400L)
						{
							retry = true;
							Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
						}
						else if (request.isNetworkError)
						{
							retry = true;
						}
					}
					if (retry)
					{
						if (playFabAuthenticator.playFabAuthRetryCount < playFabAuthenticator.playFabMaxRetries)
						{
							int num2 = (int)Mathf.Pow(2f, (float)(playFabAuthenticator.playFabAuthRetryCount + 1));
							Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", playFabAuthenticator.playFabAuthRetryCount + 1, num2));
							playFabAuthenticator.playFabAuthRetryCount++;
							this.<>2__current = new WaitForSeconds((float)num2);
							this.<>1__state = 2;
							return true;
						}
						Debug.LogError("Maximum retries attempted. Please check your network connection.");
						callback(null);
					}
					break;
				case 2:
					this.<>1__state = -1;
					break;
				default:
					return false;
				}
				return false;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public PlayFabAuthenticator.PlayfabAuthRequestData data;

			public Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback;

			public PlayFabAuthenticator <>4__this;

			private UnityWebRequest <request>5__2;

			private bool <retry>5__3;
		}
	}
}
