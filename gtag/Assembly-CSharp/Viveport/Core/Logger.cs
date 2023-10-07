using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x0200026D RID: 621
	public class Logger
	{
		// Token: 0x06000F71 RID: 3953 RVA: 0x00053BF2 File Offset: 0x00051DF2
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x00053C0F File Offset: 0x00051E0F
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x00053C20 File Offset: 0x00051E20
		private static void UnityLog(string message)
		{
			try
			{
				if (Logger._unityLogType == null)
				{
					Logger._unityLogType = Logger.GetType("UnityEngine.Debug");
				}
				Logger._unityLogType.GetMethod("Log", new Type[]
				{
					typeof(string)
				}).Invoke(null, new object[]
				{
					message
				});
				Logger._usingUnityLog = true;
			}
			catch (Exception)
			{
				Logger.ConsoleLog(message);
				Logger._usingUnityLog = false;
			}
			Logger._hasDetected = true;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x00053CAC File Offset: 0x00051EAC
		private static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x040011C6 RID: 4550
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x040011C7 RID: 4551
		private static bool _hasDetected;

		// Token: 0x040011C8 RID: 4552
		private static bool _usingUnityLog = true;

		// Token: 0x040011C9 RID: 4553
		private static Type _unityLogType;
	}
}
