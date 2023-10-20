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
	// Token: 0x020002BA RID: 698
	public class PlayFabAuthenticator : MonoBehaviour
	{
		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060012D6 RID: 4822 RVA: 0x0006DD4C File Offset: 0x0006BF4C
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0006DD58 File Offset: 0x0006BF58
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

		// Token: 0x060012D8 RID: 4824 RVA: 0x0006DE0C File Offset: 0x0006C00C
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

		// Token: 0x060012D9 RID: 4825 RVA: 0x0006DEA8 File Offset: 0x0006C0A8
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

		// Token: 0x060012DA RID: 4826 RVA: 0x0006DF14 File Offset: 0x0006C114
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

		// Token: 0x060012DB RID: 4827 RVA: 0x0006DF7B File Offset: 0x0006C17B
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

		// Token: 0x060012DC RID: 4828 RVA: 0x0006DF9B File Offset: 0x0006C19B
		private void MaybeGetNonce(LoginResult obj)
		{
			this._playFabId = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			this.AdvanceLogin();
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0006DFBB File Offset: 0x0006C1BB
		private void AdvanceLogin()
		{
			this.RequestPhotonToken(this._playFabId, this._sessionTicket);
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0006DFD0 File Offset: 0x0006C1D0
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

		// Token: 0x060012DF RID: 4831 RVA: 0x0006E078 File Offset: 0x0006C278
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

		// Token: 0x060012E0 RID: 4832 RVA: 0x0006E208 File Offset: 0x0006C408
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

		// Token: 0x060012E1 RID: 4833 RVA: 0x0006E440 File Offset: 0x0006C640
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

		// Token: 0x060012E2 RID: 4834 RVA: 0x0006E4B0 File Offset: 0x0006C6B0
		public void LogMessage(string message)
		{
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0006E4B4 File Offset: 0x0006C6B4
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

		// Token: 0x060012E4 RID: 4836 RVA: 0x0006E514 File Offset: 0x0006C714
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

		// Token: 0x060012E5 RID: 4837 RVA: 0x0006E5B4 File Offset: 0x0006C7B4
		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0006E5E0 File Offset: 0x0006C7E0
		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0006E5F4 File Offset: 0x0006C7F4
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

		// Token: 0x060012E8 RID: 4840 RVA: 0x0006E649 File Offset: 0x0006C849
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

		// Token: 0x060012E9 RID: 4841 RVA: 0x0006E668 File Offset: 0x0006C868
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

		// Token: 0x060012EA RID: 4842 RVA: 0x0006E71C File Offset: 0x0006C91C
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

		// Token: 0x040015DF RID: 5599
		public static volatile PlayFabAuthenticator instance;

		// Token: 0x040015E0 RID: 5600
		public const string Playfab_TitleId_Prod = "63FDD";

		// Token: 0x040015E1 RID: 5601
		public const string Playfab_TitleId_Dev = "195C0";

		// Token: 0x040015E2 RID: 5602
		public const string Playfab_Auth_API = "https://auth-prod.gtag-cf.com";

		// Token: 0x040015E3 RID: 5603
		public string _playFabPlayerIdCache;

		// Token: 0x040015E4 RID: 5604
		private string _sessionTicket;

		// Token: 0x040015E5 RID: 5605
		private string _playFabId;

		// Token: 0x040015E6 RID: 5606
		private string _displayName;

		// Token: 0x040015E7 RID: 5607
		private string _nonce;

		// Token: 0x040015E8 RID: 5608
		private string _orgScopedId;

		// Token: 0x040015E9 RID: 5609
		public string userID;

		// Token: 0x040015EA RID: 5610
		private string userToken;

		// Token: 0x040015EB RID: 5611
		private string platform;

		// Token: 0x040015EC RID: 5612
		private byte[] m_Ticket;

		// Token: 0x040015ED RID: 5613
		private uint m_pcbTicket;

		// Token: 0x040015EE RID: 5614
		public Text debugText;

		// Token: 0x040015EF RID: 5615
		public bool screenDebugMode;

		// Token: 0x040015F0 RID: 5616
		public bool loginFailed;

		// Token: 0x040015F1 RID: 5617
		[FormerlySerializedAs("loginDisplayID")]
		public string oculusID = "";

		// Token: 0x040015F2 RID: 5618
		public GameObject emptyObject;

		// Token: 0x040015F3 RID: 5619
		private int playFabAuthRetryCount;

		// Token: 0x040015F4 RID: 5620
		private int playFabMaxRetries = 5;

		// Token: 0x040015F5 RID: 5621
		private int playFabCacheRetryCount;

		// Token: 0x040015F6 RID: 5622
		private int playFabCacheMaxRetries = 5;

		// Token: 0x040015F7 RID: 5623
		private HAuthTicket m_HAuthTicket;

		// Token: 0x040015F8 RID: 5624
		private byte[] ticketBlob = new byte[1024];

		// Token: 0x040015F9 RID: 5625
		private uint ticketSize;

		// Token: 0x040015FA RID: 5626
		protected Callback<GetAuthSessionTicketResponse_t> m_GetAuthSessionTicketResponse;

		// Token: 0x020004D7 RID: 1239
		[Serializable]
		public class CachePlayFabIdRequest
		{
			// Token: 0x0400202E RID: 8238
			public string Platform;

			// Token: 0x0400202F RID: 8239
			public string SessionTicket;

			// Token: 0x04002030 RID: 8240
			public string PlayFabId;
		}

		// Token: 0x020004D8 RID: 1240
		[Serializable]
		public class PlayfabAuthRequestData
		{
			// Token: 0x04002031 RID: 8241
			public string CustomId;

			// Token: 0x04002032 RID: 8242
			public string AppId;

			// Token: 0x04002033 RID: 8243
			public string AppVersion;

			// Token: 0x04002034 RID: 8244
			public string Nonce;

			// Token: 0x04002035 RID: 8245
			public string OculusId;

			// Token: 0x04002036 RID: 8246
			public string Platform;
		}

		// Token: 0x020004D9 RID: 1241
		[Serializable]
		public class PlayfabAuthResponseData
		{
			// Token: 0x04002037 RID: 8247
			public string SessionTicket;

			// Token: 0x04002038 RID: 8248
			public string EntityToken;

			// Token: 0x04002039 RID: 8249
			public string PlayFabId;

			// Token: 0x0400203A RID: 8250
			public string EntityId;

			// Token: 0x0400203B RID: 8251
			public string EntityType;
		}

		// Token: 0x020004DA RID: 1242
		public class BanInfo
		{
			// Token: 0x0400203C RID: 8252
			public string BanMessage;

			// Token: 0x0400203D RID: 8253
			public string BanExpirationTime;
		}
	}
}
