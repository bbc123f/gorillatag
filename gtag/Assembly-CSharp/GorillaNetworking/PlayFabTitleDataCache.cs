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
	// Token: 0x020002BA RID: 698
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060012E8 RID: 4840 RVA: 0x0006E30C File Offset: 0x0006C50C
		// (set) Token: 0x060012E9 RID: 4841 RVA: 0x0006E313 File Offset: 0x0006C513
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060012EA RID: 4842 RVA: 0x0006E31B File Offset: 0x0006C51B
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0006E32C File Offset: 0x0006C52C
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

		// Token: 0x060012EC RID: 4844 RVA: 0x0006E389 File Offset: 0x0006C589
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x0006E3A5 File Offset: 0x0006C5A5
		private void Start()
		{
			this.UpdateData();
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0006E3B0 File Offset: 0x0006C5B0
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

		// Token: 0x060012EF RID: 4847 RVA: 0x0006E420 File Offset: 0x0006C620
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

		// Token: 0x060012F0 RID: 4848 RVA: 0x0006E468 File Offset: 0x0006C668
		public void UpdateData()
		{
			base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0006E477 File Offset: 0x0006C677
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

		// Token: 0x060012F2 RID: 4850 RVA: 0x0006E488 File Offset: 0x0006C688
		private void LoadKey()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("title_data_key");
			this.titleDataKey = textAsset.text;
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0006E4AC File Offset: 0x0006C6AC
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

		// Token: 0x060012F4 RID: 4852 RVA: 0x0006E504 File Offset: 0x0006C704
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

		// Token: 0x040015EF RID: 5615
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x040015F0 RID: 5616
		private const string FileName = "TitleDataCache.json";

		// Token: 0x040015F1 RID: 5617
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x040015F2 RID: 5618
		private Dictionary<string, string> titleData = new Dictionary<string, string>();

		// Token: 0x040015F3 RID: 5619
		private string titleDataKey;

		// Token: 0x040015F4 RID: 5620
		private const string titleDataUrl = "https://title-data.gtag-cf.com";

		// Token: 0x040015F5 RID: 5621
		private bool isDataUpToDate;

		// Token: 0x020004DD RID: 1245
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x020004DE RID: 1246
		private class DataRequest
		{
			// Token: 0x170002A0 RID: 672
			// (get) Token: 0x06001EE3 RID: 7907 RVA: 0x000A09D1 File Offset: 0x0009EBD1
			// (set) Token: 0x06001EE4 RID: 7908 RVA: 0x000A09D9 File Offset: 0x0009EBD9
			public string Name { get; set; }

			// Token: 0x170002A1 RID: 673
			// (get) Token: 0x06001EE5 RID: 7909 RVA: 0x000A09E2 File Offset: 0x0009EBE2
			// (set) Token: 0x06001EE6 RID: 7910 RVA: 0x000A09EA File Offset: 0x0009EBEA
			public Action<string> Callback { get; set; }

			// Token: 0x170002A2 RID: 674
			// (get) Token: 0x06001EE7 RID: 7911 RVA: 0x000A09F3 File Offset: 0x0009EBF3
			// (set) Token: 0x06001EE8 RID: 7912 RVA: 0x000A09FB File Offset: 0x0009EBFB
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
