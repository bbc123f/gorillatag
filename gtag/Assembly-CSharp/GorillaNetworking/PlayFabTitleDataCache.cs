using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
		public static PlayFabTitleDataCache Instance
		{
			[CompilerGenerated]
			get
			{
				return PlayFabTitleDataCache.<Instance>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				PlayFabTitleDataCache.<Instance>k__BackingField = value;
			}
		}

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
			if (this.isDataUpToDate && this.updateDataCoroutine == null)
			{
				this.UpdateData();
			}
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
			this.updateDataCoroutine = base.StartCoroutine(this.UpdateDataCo());
		}

		private IEnumerator UpdateDataCo()
		{
			this.LoadDataFromFile();
			this.LoadKey();
			Dictionary<string, string> dictionary = this.titleData;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>((dictionary != null) ? dictionary.Count : 0);
			if (this.titleData != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.titleData)
				{
					string text;
					string text2;
					keyValuePair.Deconstruct(out text, out text2);
					string text3 = text;
					string text4 = text2;
					if (text3 != null)
					{
						dictionary2[text3] = ((text4 != null) ? PlayFabTitleDataCache.MD5(text4) : null);
					}
				}
			}
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
					dictionary2
				}
			});
			Stopwatch sw = Stopwatch.StartNew();
			Dictionary<string, JsonData> dictionary3;
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
				dictionary3 = JsonMapper.ToObject<Dictionary<string, JsonData>>(www.downloadHandler.text);
			}
			UnityWebRequest www = null;
			Debug.Log(string.Format("TitleData fetched: {0:N5}", sw.Elapsed.TotalSeconds));
			foreach (KeyValuePair<string, JsonData> keyValuePair2 in dictionary3)
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
			if (dictionary3.Keys.Count > 0)
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
			this.updateDataCoroutine = null;
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

		public PlayFabTitleDataCache()
		{
		}

		[CompilerGenerated]
		private bool <UpdateDataCo>b__22_0(PlayFabTitleDataCache.DataRequest request)
		{
			string data;
			if (this.titleData.TryGetValue(request.Name, out data))
			{
				request.Callback.SafeInvoke(data);
				return true;
			}
			return false;
		}

		[CompilerGenerated]
		private static PlayFabTitleDataCache <Instance>k__BackingField;

		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		private const string FileName = "TitleDataCache.json";

		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		private Dictionary<string, string> titleData = new Dictionary<string, string>();

		private string titleDataKey;

		private const string titleDataUrl = "https://title-data.gtag-cf.com";

		private bool isDataUpToDate;

		private Coroutine updateDataCoroutine;

		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
			public DataUpdate()
			{
			}
		}

		private class DataRequest
		{
			public string Name
			{
				[CompilerGenerated]
				get
				{
					return this.<Name>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					this.<Name>k__BackingField = value;
				}
			}

			public Action<string> Callback
			{
				[CompilerGenerated]
				get
				{
					return this.<Callback>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					this.<Callback>k__BackingField = value;
				}
			}

			public Action<PlayFabError> ErrorCallback
			{
				[CompilerGenerated]
				get
				{
					return this.<ErrorCallback>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					this.<ErrorCallback>k__BackingField = value;
				}
			}

			public DataRequest()
			{
			}

			[CompilerGenerated]
			private string <Name>k__BackingField;

			[CompilerGenerated]
			private Action<string> <Callback>k__BackingField;

			[CompilerGenerated]
			private Action<PlayFabError> <ErrorCallback>k__BackingField;
		}

		[CompilerGenerated]
		private sealed class <UpdateDataCo>d__22 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <UpdateDataCo>d__22(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num == -3 || num == 1)
				{
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			bool IEnumerator.MoveNext()
			{
				bool result;
				try
				{
					int num = this.<>1__state;
					PlayFabTitleDataCache playFabTitleDataCache = this;
					if (num != 0)
					{
						if (num != 1)
						{
							result = false;
						}
						else
						{
							this.<>1__state = -3;
							if (www.isNetworkError || www.isHttpError)
							{
								Debug.LogError("Failed to get TitleData from the server.\n" + www.error);
								playFabTitleDataCache.ClearRequestWithError(null);
								result = false;
								this.<>m__Finally1();
							}
							else
							{
								Dictionary<string, JsonData> dictionary = JsonMapper.ToObject<Dictionary<string, JsonData>>(www.downloadHandler.text);
								this.<>m__Finally1();
								www = null;
								Debug.Log(string.Format("TitleData fetched: {0:N5}", sw.Elapsed.TotalSeconds));
								foreach (KeyValuePair<string, JsonData> keyValuePair in dictionary)
								{
									PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = playFabTitleDataCache.OnTitleDataUpdate;
									if (onTitleDataUpdate != null)
									{
										onTitleDataUpdate.Invoke(keyValuePair.Key);
									}
									if (keyValuePair.Value == null)
									{
										playFabTitleDataCache.titleData.Remove(keyValuePair.Key);
									}
									else
									{
										playFabTitleDataCache.titleData.AddOrUpdate(keyValuePair.Key, JsonMapper.ToJson(keyValuePair.Value));
									}
								}
								if (dictionary.Keys.Count > 0)
								{
									playFabTitleDataCache.SaveDataToFile(PlayFabTitleDataCache.FilePath);
								}
								playFabTitleDataCache.requests.RemoveAll(delegate(PlayFabTitleDataCache.DataRequest request)
								{
									string data;
									if (playFabTitleDataCache.titleData.TryGetValue(request.Name, out data))
									{
										request.Callback.SafeInvoke(data);
										return true;
									}
									return false;
								});
								playFabTitleDataCache.ClearRequestWithError(null);
								playFabTitleDataCache.isDataUpToDate = true;
								playFabTitleDataCache.updateDataCoroutine = null;
								result = false;
							}
						}
					}
					else
					{
						this.<>1__state = -1;
						playFabTitleDataCache.LoadDataFromFile();
						playFabTitleDataCache.LoadKey();
						Dictionary<string, string> titleData = playFabTitleDataCache.titleData;
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>((titleData != null) ? titleData.Count : 0);
						if (playFabTitleDataCache.titleData != null)
						{
							foreach (KeyValuePair<string, string> keyValuePair2 in playFabTitleDataCache.titleData)
							{
								string text;
								string text2;
								keyValuePair2.Deconstruct(out text, out text2);
								string text3 = text;
								string text4 = text2;
								if (text3 != null)
								{
									dictionary2[text3] = ((text4 != null) ? PlayFabTitleDataCache.MD5(text4) : null);
								}
							}
						}
						string s = JsonMapper.ToJson(new Dictionary<string, object>
						{
							{
								"version",
								Application.version
							},
							{
								"key",
								playFabTitleDataCache.titleDataKey
							},
							{
								"data",
								dictionary2
							}
						});
						sw = Stopwatch.StartNew();
						www = new UnityWebRequest("https://title-data.gtag-cf.com", "POST");
						this.<>1__state = -3;
						byte[] bytes = new UTF8Encoding(true).GetBytes(s);
						www.uploadHandler = new UploadHandlerRaw(bytes);
						www.downloadHandler = new DownloadHandlerBuffer();
						www.SetRequestHeader("Content-Type", "application/json");
						this.<>2__current = www.SendWebRequest();
						this.<>1__state = 1;
						result = true;
					}
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return result;
			}

			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				if (www != null)
				{
					((IDisposable)www).Dispose();
				}
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

			public PlayFabTitleDataCache <>4__this;

			private Stopwatch <sw>5__2;

			private UnityWebRequest <www>5__3;
		}
	}
}
