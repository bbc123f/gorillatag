using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x0200026E RID: 622
	public class Logger
	{
		// Token: 0x06000F78 RID: 3960 RVA: 0x00053FCE File Offset: 0x000521CE
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x00053FEB File Offset: 0x000521EB
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x00053FFC File Offset: 0x000521FC
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

		// Token: 0x06000F7B RID: 3963 RVA: 0x00054088 File Offset: 0x00052288
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

		// Token: 0x040011CC RID: 4556
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x040011CD RID: 4557
		private static bool _hasDetected;

		// Token: 0x040011CE RID: 4558
		private static bool _usingUnityLog = true;

		// Token: 0x040011CF RID: 4559
		private static Type _unityLogType;
	}
}
