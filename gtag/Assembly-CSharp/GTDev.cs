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

// Token: 0x0200005B RID: 91
public static class GTDev
{
	// Token: 0x060001AA RID: 426 RVA: 0x0000C43F File Offset: 0x0000A63F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void InitializeOnLoad()
	{
		GTDev.FetchDevID();
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000C447 File Offset: 0x0000A647
	[GTDev.HideInCallStackAttribute]
	public static void Log(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, null, channel, "Log");
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000C462 File Offset: 0x0000A662
	[GTDev.HideInCallStackAttribute]
	public static void Log(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, context, channel, "Log");
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000C47D File Offset: 0x0000A67D
	[GTDev.HideInCallStackAttribute]
	public static void LogError(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogError), msg, null, channel, "LogError");
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000C498 File Offset: 0x0000A698
	[GTDev.HideInCallStackAttribute]
	public static void LogError(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogError), msg, context, channel, "LogError");
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000C4B3 File Offset: 0x0000A6B3
	[GTDev.HideInCallStackAttribute]
	public static void LogWarning(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogWarning), msg, null, channel, "LogWarning");
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000C4CE File Offset: 0x0000A6CE
	[GTDev.HideInCallStackAttribute]
	public static void LogWarning(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.LogWarning), msg, context, channel, "LogWarning");
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000C4E9 File Offset: 0x0000A6E9
	[GTDev.HideInCallStackAttribute]
	public static void LogSilent(string msg, string channel = null)
	{
		GTDev._Log(null, msg, null, channel, "LogSilent");
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000C4F9 File Offset: 0x0000A6F9
	[GTDev.HideInCallStackAttribute]
	public static void LogSilent(string msg, Object context, string channel = null)
	{
		GTDev._Log(null, msg, context, channel, "LogSilent");
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000C509 File Offset: 0x0000A709
	[GTDev.HideInCallStackAttribute]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly(string msg, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, null, channel, "LogEditorOnly");
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000C524 File Offset: 0x0000A724
	[GTDev.HideInCallStackAttribute]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly(string msg, Object context, string channel = null)
	{
		GTDev._Log(new Action<object>(Debug.Log), msg, context, channel, "LogEditorOnly");
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000C53F File Offset: 0x0000A73F
	[GTDev.HideInCallStackAttribute]
	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
		if (call != null)
		{
			call();
		}
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060001B6 RID: 438 RVA: 0x0000C54C File Offset: 0x0000A74C
	// (remove) Token: 0x060001B7 RID: 439 RVA: 0x0000C580 File Offset: 0x0000A780
	public static event Action<GTDev.LogEntry> OnLogEntry;

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000C5B3 File Offset: 0x0000A7B3
	public static int DevID
	{
		get
		{
			return GTDev.FetchDevID();
		}
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000C5BC File Offset: 0x0000A7BC
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

	// Token: 0x060001BA RID: 442 RVA: 0x0000C61C File Offset: 0x0000A81C
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

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060001BB RID: 443 RVA: 0x0000C6A9 File Offset: 0x0000A8A9
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

	// Token: 0x060001BC RID: 444 RVA: 0x0000C6BC File Offset: 0x0000A8BC
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

	// Token: 0x060001BD RID: 445 RVA: 0x0000C730 File Offset: 0x0000A930
	private static bool IsIgnoredMethod(MethodBase method)
	{
		if (GTDev.gIgnoreMethods != null)
		{
			return GTDev.gIgnoreMethods.Contains(method);
		}
		GTDev.gIgnoreMethods = new HashSet<MethodBase>(Reflection.GetMethodsWithAttribute<GTDev.HideInCallStackAttribute>());
		return GTDev.gIgnoreMethods.Contains(method);
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000C760 File Offset: 0x0000A960
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

	// Token: 0x04000281 RID: 641
	private static int gDevID;

	// Token: 0x04000282 RID: 642
	private static bool gHasDevID;

	// Token: 0x04000283 RID: 643
	private static string gProjectFolder;

	// Token: 0x04000284 RID: 644
	private static FieldInfo gFetchProjectFolder;

	// Token: 0x04000285 RID: 645
	private static bool gProjectFolderSet;

	// Token: 0x04000286 RID: 646
	private static HashSet<MethodBase> gIgnoreMethods;

	// Token: 0x04000287 RID: 647
	public static readonly SessionState SessionState = SessionState.Shared;

	// Token: 0x0200039D RID: 925
	public class HideInCallStackAttribute : Attribute
	{
	}

	// Token: 0x0200039E RID: 926
	[Serializable]
	public struct LogEntry
	{
		// Token: 0x04001B56 RID: 6998
		public string channel;

		// Token: 0x04001B57 RID: 6999
		public DateTime time;

		// Token: 0x04001B58 RID: 7000
		public string message;
	}
}
