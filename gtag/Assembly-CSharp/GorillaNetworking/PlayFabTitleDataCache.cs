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
	// Token: 0x020002BC RID: 700
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060012EF RID: 4847 RVA: 0x0006E7D8 File Offset: 0x0006C9D8
		// (set) Token: 0x060012F0 RID: 4848 RVA: 0x0006E7DF File Offset: 0x0006C9DF
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060012F1 RID: 4849 RVA: 0x0006E7E7 File Offset: 0x0006C9E7
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0006E7F8 File Offset: 0x0006C9F8
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

		// Token: 0x060012F3 RID: 4851 RVA: 0x0006E855 File Offset: 0x0006CA55
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0006E871 File Offset: 0x0006CA71
		private void Start()
		{
			this.UpdateData();
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0006E87C File Offset: 0x0006CA7C
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

		// Token: 0x060012F6 RID: 4854 RVA: 0x0006E8EC File Offset: 0x0006CAEC
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

		// Token: 0x060012F7 RID: 4855 RVA: 0x0006E934 File Offset: 0x0006CB34
		public void UpdateData()
		{
			base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x0006E943 File Offset: 0x0006CB43
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

		// Token: 0x060012F9 RID: 4857 RVA: 0x0006E954 File Offset: 0x0006CB54
		private void LoadKey()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("title_data_key");
			this.titleDataKey = textAsset.text;
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0006E978 File Offset: 0x0006CB78
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

		// Token: 0x060012FB RID: 4859 RVA: 0x0006E9D0 File Offset: 0x0006CBD0
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

		// Token: 0x040015FC RID: 5628
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x040015FD RID: 5629
		private const string FileName = "TitleDataCache.json";

		// Token: 0x040015FE RID: 5630
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x040015FF RID: 5631
		private Dictionary<string, string> titleData = new Dictionary<string, string>();

		// Token: 0x04001600 RID: 5632
		private string titleDataKey;

		// Token: 0x04001601 RID: 5633
		private const string titleDataUrl = "https://title-data.gtag-cf.com";

		// Token: 0x04001602 RID: 5634
		private bool isDataUpToDate;

		// Token: 0x020004DF RID: 1247
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x020004E0 RID: 1248
		private class DataRequest
		{
			// Token: 0x170002A2 RID: 674
			// (get) Token: 0x06001EEC RID: 7916 RVA: 0x000A0CDD File Offset: 0x0009EEDD
			// (set) Token: 0x06001EED RID: 7917 RVA: 0x000A0CE5 File Offset: 0x0009EEE5
			public string Name { get; set; }

			// Token: 0x170002A3 RID: 675
			// (get) Token: 0x06001EEE RID: 7918 RVA: 0x000A0CEE File Offset: 0x0009EEEE
			// (set) Token: 0x06001EEF RID: 7919 RVA: 0x000A0CF6 File Offset: 0x0009EEF6
			public Action<string> Callback { get; set; }

			// Token: 0x170002A4 RID: 676
			// (get) Token: 0x06001EF0 RID: 7920 RVA: 0x000A0CFF File Offset: 0x0009EEFF
			// (set) Token: 0x06001EF1 RID: 7921 RVA: 0x000A0D07 File Offset: 0x0009EF07
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
