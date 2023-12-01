using System;
using System.Collections;
using System.Collections.Generic;
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
			this.AuthenticateWithPlayFab();
			PlayFabSettings.DisableFocusTimeCollection = true;
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
				if (this.gorillaComputer != null)
				{
					this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
					this.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
					Debug.Log("Couldn't authenticate steam account");
					return;
				}
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
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
			this._sessionTicket = obj.SessionTicket;
			this.AdvanceLogin();
		}

		private void AdvanceLogin()
		{
			this.RequestPhotonToken(this._playFabId, this._sessionTicket);
		}

		private void RequestPhotonToken(string playFabId, string sessionTicket)
		{
			this.LogMessage("Received Title Data. Requesting photon token for app ID: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime + " and title ID: " + PlayFabSettings.TitleId);
			this._playFabPlayerIdCache = playFabId;
			this._sessionTicket = sessionTicket;
			this.LogMessage("Using photon app id: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime + " and PlayFab ID: " + PlayFabSettings.TitleId);
			PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
			{
				PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
			}, new Action<GetPhotonAuthenticationTokenResult>(this.AuthenticateWithPhoton), new Action<PlayFabError>(this.OnPlayFabError), null, null);
		}

		private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult photonTokenResult)
		{
			AuthenticationValues authenticationValues = new AuthenticationValues(PlayFabSettings.DeviceUniqueIdentifier);
			authenticationValues.AuthType = CustomAuthenticationType.Custom;
			string playFabPlayerIdCache = this._playFabPlayerIdCache;
			string photonCustomAuthenticationToken = photonTokenResult.PhotonCustomAuthenticationToken;
			authenticationValues.AddAuthParameter("username", this._playFabPlayerIdCache);
			authenticationValues.AddAuthParameter("token", photonTokenResult.PhotonCustomAuthenticationToken);
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
					"Token",
					photonCustomAuthenticationToken
				},
				{
					"Nonce",
					this._nonce
				}
			};
			authenticationValues.SetAuthPostData(authPostData);
			Debug.Log("Set Photon auth data. Appversion is: " + PhotonNetwork.AppVersion);
			PhotonNetwork.AuthValues = authenticationValues;
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
			if (PhotonNetworkController.Instance != null)
			{
				PhotonNetworkController.Instance.InitiateConnection();
			}
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

		public static volatile PlayFabAuthenticator instance;

		public const string Playfab_TitleId_Prod = "63FDD";

		public const string Playfab_TitleId_Dev = "195C0";

		public const string Playfab_Auth_API = "https://auth-prod.gtag-cf.com";

		public string _playFabPlayerIdCache;

		private string _sessionTicket;

		private string _playFabId;

		private string _displayName;

		private string _nonce;

		private string _orgScopedId;

		public string userID;

		private string userToken;

		private string platform;

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

		[Serializable]
		public class CachePlayFabIdRequest
		{
			public string Platform;

			public string SessionTicket;

			public string PlayFabId;
		}

		[Serializable]
		public class PlayfabAuthRequestData
		{
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
			public string SessionTicket;

			public string EntityToken;

			public string PlayFabId;

			public string EntityId;

			public string EntityType;
		}

		public class BanInfo
		{
			public string BanMessage;

			public string BanExpirationTime;
		}
	}
}
