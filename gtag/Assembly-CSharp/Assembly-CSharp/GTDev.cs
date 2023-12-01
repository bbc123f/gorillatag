using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using BuildSafe;
using UnityEngine;

public static class GTDev
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void InitializeOnLoad()
	{
		GTDev.FetchDevID();
	}

	[GTDev.HideInCallStackAttribute]
	public static void Log(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, null, channel, "Log");
	}

	[GTDev.HideInCallStackAttribute]
	public static void Log(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, context, channel, "Log");
	}

	[GTDev.HideInCallStackAttribute]
	public static void LogError(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogError), msg, null, channel, "LogError");
	}

	[GTDev.HideInCallStackAttribute]
	public static void LogError(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogError), msg, context, channel, "LogError");
	}

	[GTDev.HideInCallStackAttribute]
	public static void LogWarning(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogWarning), msg, null, channel, "LogWarning");
	}

	[GTDev.HideInCallStackAttribute]
	public static void LogWarning(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogWarning), msg, context, channel, "LogWarning");
	}

	[GTDev.HideInCallStackAttribute]
	public static void LogSilent(string msg, string channel = null)
	{
		GTDev._Log(null, msg, null, channel, "LogSilent");
	}

	[GTDev.HideInCallStackAttribute]
	public static void LogSilent(string msg, Object context, string channel = null)
	{
		GTDev._Log(null, msg, context, channel, "LogSilent");
	}

	[GTDev.HideInCallStackAttribute]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, null, channel, "LogEditorOnly");
	}

	[GTDev.HideInCallStackAttribute]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, context, channel, "LogEditorOnly");
	}

	[GTDev.HideInCallStackAttribute]
	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
		if (call != null)
		{
			call();
		}
	}

	public static event Action<GTDev.LogEntry> OnLogEntry;

	public static int DevID
	{
		get
		{
			return GTDev.FetchDevID();
		}
	}

	private static int FetchDevID()
	{
		if (GTDev.gHasDevID)
		{
			return GTDev.gDevID;
		}
		int i = StaticHash.Calculate(SystemInfo.deviceUniqueIdentifier);
		int i2 = StaticHash.Calculate(Environment.UserDomainName);
		int i3 = StaticHash.Calculate(Environment.UserName);
		int i4 = StaticHash.Calculate(Application.unityVersion);
		GTDev.gDevID = StaticHash.Combine(i, i2, i3, i4);
		GTDev.gHasDevID = true;
		return GTDev.gDevID;
	}

	private static void _Log(Action<object> call, string msg, Object ctx, string chl, [CallerMemberName] string clr = null)
	{
		string channel = string.IsNullOrWhiteSpace(chl) ? clr : (chl + "//" + clr);
		StackTraceLogType stackTraceLogType = Application.stackTraceLogType;
		Application.stackTraceLogType = StackTraceLogType.None;
		string str = GTDev.ExtractFormattedStackTrace(new StackTrace(1, true));
		if (call != null)
		{
			call(msg + "\n" + str);
		}
		Application.stackTraceLogType = stackTraceLogType;
		Action<GTDev.LogEntry> onLogEntry = GTDev.OnLogEntry;
		if (onLogEntry == null)
		{
			return;
		}
		onLogEntry(new GTDev.LogEntry
		{
			time = DateTime.UtcNow,
			message = msg,
			channel = channel
		});
	}

	private static string projectFolder
	{
		get
		{
			if (!GTDev.gProjectFolderSet)
			{
				GTDev.CacheProjectFolder();
			}
			return GTDev.gProjectFolder;
		}
	}

	private static void CacheProjectFolder()
	{
		if (GTDev.gFetchProjectFolder == null)
		{
			GTDev.gFetchProjectFolder = typeof(StackTraceUtility).GetRuntimeFields().FirstOrDefault((FieldInfo f) => f.Name == "projectFolder");
		}
		FieldInfo fieldInfo = GTDev.gFetchProjectFolder;
		GTDev.gProjectFolder = (string)((fieldInfo != null) ? fieldInfo.GetValue(null) : null);
		GTDev.gProjectFolderSet = true;
	}

	private static bool IsIgnoredMethod(MethodBase method)
	{
		if (GTDev.gIgnoreMethods != null)
		{
			return GTDev.gIgnoreMethods.Contains(method);
		}
		GTDev.gIgnoreMethods = new HashSet<MethodBase>(Reflection.GetMethodsWithAttribute<GTDev.HideInCallStackAttribute>());
		return GTDev.gIgnoreMethods.Contains(method);
	}

	[SecuritySafeCritical]
	private static string ExtractFormattedStackTrace(StackTrace stackTrace)
	{
		StringBuilder stringBuilder = new StringBuilder(255);
		for (int i = 0; i < stackTrace.FrameCount; i++)
		{
			StackFrame frame = stackTrace.GetFrame(i);
			MethodBase method = frame.GetMethod();
			if (!(method == null) && !GTDev.IsIgnoredMethod(method))
			{
				Type declaringType = method.DeclaringType;
				if (!(declaringType == null))
				{
					string @namespace = declaringType.Namespace;
					if (!string.IsNullOrEmpty(@namespace))
					{
						stringBuilder.Append(@namespace);
						stringBuilder.Append(".");
					}
					stringBuilder.Append(declaringType.Name);
					stringBuilder.Append(":");
					stringBuilder.Append(method.Name);
					stringBuilder.Append("(");
					ParameterInfo[] parameters = method.GetParameters();
					bool flag = true;
					for (int j = 0; j < parameters.Length; j++)
					{
						if (!flag)
						{
							stringBuilder.Append(", ");
						}
						else
						{
							flag = false;
						}
						stringBuilder.Append(parameters[j].ParameterType.Name);
					}
					stringBuilder.Append(")");
					string text = frame.GetFileName();
					if (text != null && (declaringType.Name != "Debug" || declaringType.Namespace != "UnityEngine") && (declaringType.Name != "Logger" || declaringType.Namespace != "UnityEngine") && (declaringType.Name != "DebugLogHandler" || declaringType.Namespace != "UnityEngine") && (declaringType.Name != "Assert" || declaringType.Namespace != "UnityEngine.Assertions") && (method.Name != "print" || declaringType.Name != "MonoBehaviour" || declaringType.Namespace != "UnityEngine"))
					{
						stringBuilder.Append(" (at ");
						if (!string.IsNullOrEmpty(GTDev.projectFolder) && text.Replace("\\", "/").StartsWith(GTDev.projectFolder))
						{
							text = text.Substring(GTDev.projectFolder.Length, text.Length - GTDev.projectFolder.Length);
						}
						stringBuilder.Append(text);
						stringBuilder.Append(":");
						stringBuilder.Append(frame.GetFileLineNumber().ToString());
						stringBuilder.Append(")");
					}
					stringBuilder.Append("\n");
				}
			}
		}
		return stringBuilder.ToString();
	}

	[OnEnterPlay_Set(0)]
	private static int gDevID;

	[OnEnterPlay_Set(false)]
	private static bool gHasDevID;

	private static string gProjectFolder;

	private static FieldInfo gFetchProjectFolder;

	private static bool gProjectFolderSet;

	private static HashSet<MethodBase> gIgnoreMethods;

	public static readonly SessionState SessionState = SessionState.Shared;

	public class HideInCallStackAttribute : Attribute
	{
	}

	[Serializable]
	public struct LogEntry
	{
		public string channel;

		public DateTime time;

		public string message;
	}
}
