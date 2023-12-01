using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		public static PlayFabTitleDataCache Instance { get; private set; }

		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback)
		{
			if (this.isDataUpToDate && this.titleData.ContainsKey(name))
			{
				callback.SafeInvoke(this.titleData[name]);
				return;
			}
			PlayFabTitleDataCache.DataRequest item = new PlayFabTitleDataCache.DataRequest
			{
				Name = name,
				Callback = callback,
				ErrorCallback = errorCallback
			};
			this.requests.Add(item);
		}

		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
		}

		private void Start()
		{
			this.UpdateData();
		}

		public void LoadDataFromFile()
		{
			try
			{
				if (!File.Exists(PlayFabTitleDataCache.FilePath))
				{
					Debug.LogWarning("Title data file " + PlayFabTitleDataCache.FilePath + " does not exist!");
				}
				else
				{
					string json = File.ReadAllText(PlayFabTitleDataCache.FilePath);
					this.titleData = JsonMapper.ToObject<Dictionary<string, string>>(json);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error reading PlayFab title data from file: {0}", arg));
			}
		}

		private void SaveDataToFile(string filepath)
		{
			try
			{
				string contents = JsonMapper.ToJson(this.titleData);
				File.WriteAllText(filepath, contents);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error writing PlayFab title data to file: {0}", arg));
			}
		}

		public void UpdateData()
		{
			base.StartCoroutine(this.UpdateDataCo());
		}

		private IEnumerator UpdateDataCo()
		{
			this.LoadDataFromFile();
			this.LoadKey();
			Dictionary<string, string> value = this.titleData.ToDictionary((KeyValuePair<string, string> keyValuePair) => keyValuePair.Key, (KeyValuePair<string, string> keyValuePair) => PlayFabTitleDataCache.MD5(keyValuePair.Value));
			string s = JsonMapper.ToJson(new Dictionary<string, object>
			{
				{
					"version",
					Application.version
				},
				{
					"key",
					this.titleDataKey
				},
				{
					"data",
					value
				}
			});
			Stopwatch sw = Stopwatch.StartNew();
			Dictionary<string, JsonData> dictionary;
			using (UnityWebRequest www = new UnityWebRequest("https://title-data.gtag-cf.com", "POST"))
			{
				byte[] bytes = new UTF8Encoding(true).GetBytes(s);
				www.uploadHandler = new UploadHandlerRaw(bytes);
				www.downloadHandler = new DownloadHandlerBuffer();
				www.SetRequestHeader("Content-Type", "application/json");
				yield return www.SendWebRequest();
				if (www.isNetworkError || www.isHttpError)
				{
					Debug.LogError("Failed to get TitleData from the server.\n" + www.error);
					this.ClearRequestWithError(null);
					yield break;
				}
				dictionary = JsonMapper.ToObject<Dictionary<string, JsonData>>(www.downloadHandler.text);
			}
			UnityWebRequest www = null;
			Debug.Log(string.Format("TitleData fetched: {0:N5}", sw.Elapsed.TotalSeconds));
			foreach (KeyValuePair<string, JsonData> keyValuePair2 in dictionary)
			{
				PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = this.OnTitleDataUpdate;
				if (onTitleDataUpdate != null)
				{
					onTitleDataUpdate.Invoke(keyValuePair2.Key);
				}
				if (keyValuePair2.Value == null)
				{
					this.titleData.Remove(keyValuePair2.Key);
				}
				else
				{
					this.titleData.AddOrUpdate(keyValuePair2.Key, JsonMapper.ToJson(keyValuePair2.Value));
				}
			}
			if (dictionary.Keys.Count > 0)
			{
				this.SaveDataToFile(PlayFabTitleDataCache.FilePath);
			}
			this.requests.RemoveAll(delegate(PlayFabTitleDataCache.DataRequest request)
			{
				string data;
				if (this.titleData.TryGetValue(request.Name, out data))
				{
					request.Callback.SafeInvoke(data);
					return true;
				}
				return false;
			});
			this.ClearRequestWithError(null);
			this.isDataUpToDate = true;
			yield break;
			yield break;
		}

		private void LoadKey()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("title_data_key");
			this.titleDataKey = textAsset.text;
		}

		private static string MD5(string value)
		{
			HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(value);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private void ClearRequestWithError(PlayFabError e = null)
		{
			if (e == null)
			{
				e = new PlayFabError();
			}
			foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
			{
				dataRequest.ErrorCallback.SafeInvoke(e);
			}
			this.requests.Clear();
		}

		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		private const string FileName = "TitleDataCache.json";

		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		private Dictionary<string, string> titleData = new Dictionary<string, string>();

		private string titleDataKey;

		private const string titleDataUrl = "https://title-data.gtag-cf.com";

		private bool isDataUpToDate;

		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		private class DataRequest
		{
			public string Name { get; set; }

			public Action<string> Callback { get; set; }

			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
