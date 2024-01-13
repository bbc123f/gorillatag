using System;
using System.Diagnostics;
using UnityEngine;
using Utilities;

public static class GTDev
{
	[Serializable]
	public struct LogEntry
	{
		public int level;

		public DateTime time;

		public string message;

		public int checksum;
	}

	private const string kSlash = "/";

	private static int gDevID;

	private static bool gHasDevID;

	public static int DevID
	{
		get
		{
			if (gHasDevID)
			{
				return gDevID;
			}
			int staticHash = SystemInfo.deviceUniqueIdentifier.GetStaticHash();
			int staticHash2 = Environment.UserDomainName.GetStaticHash();
			int staticHash3 = Environment.UserName.GetStaticHash();
			int staticHash4 = Application.unityVersion.GetStaticHash();
			gDevID = (i1: staticHash, i2: staticHash2, i3: staticHash3, i4: staticHash4).GetStaticHash();
			gHasDevID = true;
			return gDevID;
		}
	}

	public static event Action<LogEntry> OnLogEntry;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
	}

	public static void Log(string msg, string lvl = null)
	{
		_Log(UnityEngine.Debug.Log, msg, "Log" + "/" + (lvl ?? string.Empty));
	}

	public static void LogError(string msg, string lvl = null)
	{
		_Log(UnityEngine.Debug.LogError, msg, "LogError" + "/" + (lvl ?? string.Empty));
	}

	public static void LogWarning(string msg, string lvl = null)
	{
		_Log(UnityEngine.Debug.LogWarning, msg, "LogWarning" + "/" + (lvl ?? string.Empty));
	}

	public static void LogDebug(string msg, string lvl = null)
	{
		_Log(null, msg, "LogDebug" + "/" + (lvl ?? string.Empty));
	}

	public static void LogSilent(string msg, string lvl = null)
	{
		_Log(null, msg, "LogSilent" + "/" + (lvl ?? string.Empty));
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly(string msg, string lvl = null)
	{
		_Log(UnityEngine.Debug.Log, msg, "LogEditorOnly" + "/" + (lvl ?? string.Empty));
	}

	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
		call?.Invoke();
	}

	private static void _Log(Action<object> call, string msg, string level = null)
	{
		call?.Invoke("[GTDev] " + msg);
		GTDev.OnLogEntry?.Invoke(new LogEntry
		{
			time = DateTime.UtcNow,
			message = msg,
			level = StaticHash.Calculate(level),
			checksum = StaticHash.Calculate(msg)
		});
	}
}
