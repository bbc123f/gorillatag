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

namespace GorillaNetworking;

public class PlayFabAuthenticator : MonoBehaviour
{
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

	public GorillaComputer gorillaComputer => GorillaComputer.instance;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		PlayFabSettings.CompressApiData = false;
		_ = new byte[1];
		if (screenDebugMode)
		{
			debugText.text = "";
		}
		Debug.Log("doing steam thing");
		m_GetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
		platform = "Steam";
		Debug.Log("Environment is *************** PRODUCTION *******************");
		PlayFabSettings.TitleId = "63FDD";
		AuthenticateWithPlayFab();
		PlayFabSettings.DisableFocusTimeCollection = true;
	}

	public void AuthenticateWithPlayFab()
	{
		if (!loginFailed)
		{
			Debug.Log("authenticating with playFab!");
			if (SteamManager.Initialized)
			{
				Debug.Log("trying to auth with steam");
				m_HAuthTicket = SteamUser.GetAuthSessionTicket(ticketBlob, ticketBlob.Length, out ticketSize);
			}
			else if (gorillaComputer != null)
			{
				gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
		}
	}

	private void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
	{
		Debug.Log("Got steam auth session ticket!");
		oculusID = SteamUser.GetSteamID().ToString();
		PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
		{
			CreateAccount = true,
			SteamTicket = GetSteamAuthTicket()
		}, OnLoginWithSteamResponse, OnPlayFabError);
	}

	private void OnLoginWithSteamResponse(LoginResult obj)
	{
		_playFabId = obj.PlayFabId;
		_sessionTicket = obj.SessionTicket;
		StartCoroutine(CachePlayFabId(new CachePlayFabIdRequest
		{
			Platform = platform,
			SessionTicket = _sessionTicket,
			PlayFabId = _playFabId
		}, OnCachePlayFabIdRequest));
	}

	private void OnCachePlayFabIdRequest(bool success)
	{
		if (success)
		{
			Debug.Log("Successfully cached PlayFab Id.  Continuing!");
			AdvanceLogin();
		}
		else
		{
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}
	}

	private void MaybeGetNonce(LoginResult obj)
	{
		_playFabId = obj.PlayFabId;
		_sessionTicket = obj.SessionTicket;
		AdvanceLogin();
	}

	private void AdvanceLogin()
	{
		RequestPhotonToken(_playFabId, _sessionTicket);
	}

	private void RequestPhotonToken(string playFabId, string sessionTicket)
	{
		LogMessage("Received Title Data. Requesting photon token for app ID: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime + " and title ID: " + PlayFabSettings.TitleId);
		_playFabPlayerIdCache = playFabId;
		_sessionTicket = sessionTicket;
		LogMessage("Using photon app id: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime + " and PlayFab ID: " + PlayFabSettings.TitleId);
		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
		{
			PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
		}, AuthenticateWithPhoton, OnPlayFabError);
	}

	private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult photonTokenResult)
	{
		AuthenticationValues obj = new AuthenticationValues(PlayFabSettings.DeviceUniqueIdentifier)
		{
			AuthType = CustomAuthenticationType.Custom
		};
		_ = _playFabPlayerIdCache;
		string photonCustomAuthenticationToken = photonTokenResult.PhotonCustomAuthenticationToken;
		obj.AddAuthParameter("username", _playFabPlayerIdCache);
		obj.AddAuthParameter("token", photonTokenResult.PhotonCustomAuthenticationToken);
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
			{ "Ticket", _sessionTicket },
			{ "Token", photonCustomAuthenticationToken },
			{ "Nonce", _nonce }
		};
		obj.SetAuthPostData(authPostData);
		Debug.Log("Set Photon auth data. Appversion is: " + PhotonNetwork.AppVersion);
		PhotonNetwork.AuthValues = obj;
		GetPlayerDisplayName(_playFabPlayerIdCache);
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
		{
			FunctionName = "AddOrRemoveDLCOwnership",
			FunctionParameter = new { }
		}, delegate
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
		});
		if (CosmeticsController.instance != null)
		{
			Debug.Log("initializing cosmetics");
			CosmeticsController.instance.Initialize();
		}
		if (gorillaComputer != null)
		{
			gorillaComputer.OnConnectedToMasterStuff();
		}
		if (PhotonNetworkController.Instance != null)
		{
			PhotonNetworkController.Instance.InitiateConnection();
		}
	}

	private void OnPlayFabError(PlayFabError obj)
	{
		LogMessage(obj.ErrorMessage);
		Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
		loginFailed = true;
		if (obj.ErrorMessage == "The account making this request is currently banned")
		{
			using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<string, List<string>> current = enumerator.Current;
					if (current.Value[0] != "Indefinite")
					{
						gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + current.Key + "\nHOURS LEFT: " + (int)((DateTime.Parse(current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0));
					}
					else
					{
						gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + current.Key);
					}
				}
				return;
			}
		}
		if (obj.ErrorMessage == "The IP making this request is currently banned")
		{
			using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<string, List<string>> current2 = enumerator.Current;
					if (current2.Value[0] != "Indefinite")
					{
						gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + current2.Key + "\nHOURS LEFT: " + (int)((DateTime.Parse(current2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0));
					}
					else
					{
						gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + current2.Key);
					}
				}
				return;
			}
		}
		if (gorillaComputer != null)
		{
			gorillaComputer.GeneralFailureMessage(gorillaComputer.unableToConnect);
		}
	}

	private static void AddGenericId(string serviceName, string userId)
	{
		PlayFabClientAPI.AddGenericID(new AddGenericIDRequest
		{
			GenericId = new GenericServiceId
			{
				ServiceName = serviceName,
				UserId = userId
			}
		}, delegate
		{
		}, delegate
		{
			Debug.LogError("Error setting generic id");
		});
	}

	public void LogMessage(string message)
	{
	}

	private void GetPlayerDisplayName(string playFabId)
	{
		PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
		{
			PlayFabId = playFabId,
			ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			}
		}, delegate(GetPlayerProfileResult result)
		{
			_displayName = result.PlayerProfile.DisplayName;
		}, delegate(PlayFabError error)
		{
			Debug.LogError(error.GenerateErrorReport());
		});
	}

	public void SetDisplayName(string playerName)
	{
		if (_displayName == null || (_displayName.Length > 4 && _displayName.Substring(0, _displayName.Length - 4) != playerName))
		{
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = playerName
			}, delegate
			{
				_displayName = playerName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			});
		}
	}

	public void ScreenDebug(string debugString)
	{
		Debug.Log(debugString);
		if (screenDebugMode)
		{
			Text text = debugText;
			text.text = text.text + debugString + "\n";
		}
	}

	public void ScreenDebugClear()
	{
		debugText.text = "";
	}

	public string GetSteamAuthTicket()
	{
		Array.Resize(ref ticketBlob, (int)ticketSize);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = ticketBlob;
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat("{0:x2}", b);
		}
		return stringBuilder.ToString();
	}

	public IEnumerator PlayfabAuthenticate(PlayfabAuthRequestData data, Action<PlayfabAuthResponseData> callback)
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
			PlayfabAuthResponseData obj = JsonUtility.FromJson<PlayfabAuthResponseData>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			if (request.responseCode == 403)
			{
				Debug.LogError($"HTTP {request.responseCode}: {request.error}, with body: {request.downloadHandler.text}");
				BanInfo banInfo = JsonUtility.FromJson<BanInfo>(request.downloadHandler.text);
				ShowBanMessage(banInfo);
				callback(null);
			}
			if (request.isHttpError && request.responseCode != 400)
			{
				retry = true;
				Debug.LogError($"HTTP {request.responseCode} error: {request.error}");
			}
			else if (request.isNetworkError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (playFabAuthRetryCount < playFabMaxRetries)
			{
				int num = (int)Mathf.Pow(2f, playFabAuthRetryCount + 1);
				Debug.LogWarning($"Retrying PlayFab auth... Retry attempt #{playFabAuthRetryCount + 1}, waiting for {num} seconds");
				playFabAuthRetryCount++;
				yield return new WaitForSeconds(num);
			}
			else
			{
				Debug.LogError("Maximum retries attempted. Please check your network connection.");
				callback(null);
			}
		}
	}

	private void ShowBanMessage(BanInfo banInfo)
	{
		try
		{
			if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
			{
				if (banInfo.BanExpirationTime != "Indefinite")
				{
					gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + (int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0));
				}
				else
				{
					gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public IEnumerator CachePlayFabId(CachePlayFabIdRequest data, Action<bool> callback)
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
			if (request.responseCode == 200)
			{
				callback(obj: true);
			}
		}
		else if (request.isHttpError && request.responseCode != 400)
		{
			retry = true;
			Debug.LogError($"HTTP {request.responseCode} error: {request.error}");
		}
		else if (request.isNetworkError)
		{
			retry = true;
		}
		if (retry)
		{
			if (playFabCacheRetryCount < playFabCacheMaxRetries)
			{
				int num = (int)Mathf.Pow(2f, playFabCacheRetryCount + 1);
				Debug.LogWarning($"Retrying PlayFab auth... Retry attempt #{playFabCacheRetryCount + 1}, waiting for {num} seconds");
				playFabCacheRetryCount++;
				yield return new WaitForSeconds(num);
				StartCoroutine(CachePlayFabId(new CachePlayFabIdRequest
				{
					Platform = platform,
					SessionTicket = _sessionTicket,
					PlayFabId = _playFabId
				}, OnCachePlayFabIdRequest));
			}
			else
			{
				Debug.LogError("Maximum retries attempted. Please check your network connection.");
				callback(obj: false);
			}
		}
	}
}
