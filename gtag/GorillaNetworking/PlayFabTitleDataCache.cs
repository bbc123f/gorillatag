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

namespace GorillaNetworking;

public class PlayFabTitleDataCache : MonoBehaviour
{
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

	public DataUpdate OnTitleDataUpdate;

	private const string FileName = "TitleDataCache.json";

	private readonly List<DataRequest> requests = new List<DataRequest>();

	private Dictionary<string, string> titleData = new Dictionary<string, string>();

	private string titleDataKey;

	private const string titleDataUrl = "https://title-data.gtag-cf.com";

	private bool isDataUpToDate;

	public static PlayFabTitleDataCache Instance { get; private set; }

	private static string FilePath => Path.Combine(Application.persistentDataPath, "TitleDataCache.json");

	public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback)
	{
		if (isDataUpToDate && titleData.ContainsKey(name))
		{
			callback.SafeInvoke(titleData[name]);
			return;
		}
		DataRequest item = new DataRequest
		{
			Name = name,
			Callback = callback,
			ErrorCallback = errorCallback
		};
		requests.Add(item);
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		UpdateData();
	}

	public void LoadDataFromFile()
	{
		try
		{
			if (!File.Exists(FilePath))
			{
				UnityEngine.Debug.LogWarning("Title data file " + FilePath + " does not exist!");
				return;
			}
			string json = File.ReadAllText(FilePath);
			titleData = JsonMapper.ToObject<Dictionary<string, string>>(json);
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError($"Error reading PlayFab title data from file: {arg}");
		}
	}

	private void SaveDataToFile(string filepath)
	{
		try
		{
			string contents = JsonMapper.ToJson(titleData);
			File.WriteAllText(filepath, contents);
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError($"Error writing PlayFab title data to file: {arg}");
		}
	}

	public void UpdateData()
	{
		StartCoroutine(UpdateDataCo());
	}

	private IEnumerator UpdateDataCo()
	{
		LoadDataFromFile();
		LoadKey();
		Dictionary<string, string> value = titleData.ToDictionary((KeyValuePair<string, string> keyValuePair) => keyValuePair.Key, (KeyValuePair<string, string> keyValuePair) => MD5(keyValuePair.Value));
		string s = JsonMapper.ToJson(new Dictionary<string, object>
		{
			{
				"version",
				Application.version
			},
			{ "key", titleDataKey },
			{ "data", value }
		});
		Stopwatch sw = Stopwatch.StartNew();
		Dictionary<string, JsonData> dictionary;
		using (UnityWebRequest www = new UnityWebRequest("https://title-data.gtag-cf.com", "POST"))
		{
			byte[] bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(s);
			www.uploadHandler = new UploadHandlerRaw(bytes);
			www.downloadHandler = new DownloadHandlerBuffer();
			www.SetRequestHeader("Content-Type", "application/json");
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				UnityEngine.Debug.LogError("Failed to get TitleData from the server.\n" + www.error);
				ClearRequestWithError();
				yield break;
			}
			dictionary = JsonMapper.ToObject<Dictionary<string, JsonData>>(www.downloadHandler.text);
		}
		UnityEngine.Debug.Log($"TitleData fetched: {sw.Elapsed.TotalSeconds:N5}");
		foreach (KeyValuePair<string, JsonData> item in dictionary)
		{
			OnTitleDataUpdate?.Invoke(item.Key);
			if (item.Value == null)
			{
				titleData.Remove(item.Key);
			}
			else
			{
				titleData.AddOrUpdate(item.Key, JsonMapper.ToJson(item.Value));
			}
		}
		if (dictionary.Keys.Count > 0)
		{
			SaveDataToFile(FilePath);
		}
		requests.RemoveAll(delegate(DataRequest request)
		{
			if (titleData.TryGetValue(request.Name, out var value2))
			{
				request.Callback.SafeInvoke(value2);
				return true;
			}
			return false;
		});
		ClearRequestWithError();
		isDataUpToDate = true;
	}

	private void LoadKey()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("title_data_key");
		titleDataKey = textAsset.text;
	}

	private static string MD5(string value)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.Default.GetBytes(value);
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array2 = array;
		foreach (byte b in array2)
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
		foreach (DataRequest request in requests)
		{
			request.ErrorCallback.SafeInvoke(e);
		}
		requests.Clear();
	}
}
