using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x0200024C RID: 588
	public class UserStats
	{
		// Token: 0x06000E68 RID: 3688 RVA: 0x00052CD0 File Offset: 0x00050ED0
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x00052CE0 File Offset: 0x00050EE0
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.IsReady_64(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			}
			return UserStats.IsReady(new StatusCallback(UserStats.IsReadyIl2cppCallback));
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x00052D4D File Offset: 0x00050F4D
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00052D5C File Offset: 0x00050F5C
		public static int DownloadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadStats_64(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			}
			return UserStats.DownloadStats(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x00052DCC File Offset: 0x00050FCC
		public static int GetStat(string name, int defaultValue)
		{
			int result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x00052DF8 File Offset: 0x00050FF8
		public static float GetStat(string name, float defaultValue)
		{
			float result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00052E24 File Offset: 0x00051024
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x00052E3F File Offset: 0x0005103F
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x00052E5A File Offset: 0x0005105A
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x00052E68 File Offset: 0x00051068
		public static int UploadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadStats_64(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			}
			return UserStats.UploadStats(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x00052ED8 File Offset: 0x000510D8
		public static bool GetAchievement(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievement_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievement(pchName, ref num);
			}
			return num == 1;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x00052F08 File Offset: 0x00051108
		public static int GetAchievementUnlockTime(string pchName)
		{
			int result = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievementUnlockTime_64(pchName, ref result);
			}
			else
			{
				UserStats.GetAchievementUnlockTime(pchName, ref result);
			}
			return result;
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x00052F34 File Offset: 0x00051134
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x00052F3B File Offset: 0x0005113B
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x00052F42 File Offset: 0x00051142
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x00052F49 File Offset: 0x00051149
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x00052F60 File Offset: 0x00051160
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x00052F77 File Offset: 0x00051177
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x00052F84 File Offset: 0x00051184
		public static int DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, UserStats.LeaderBoardRequestType eLeaderboardDataRequest, UserStats.LeaderBoardTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadLeaderboardScores_64(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
			}
			return UserStats.DownloadLeaderboardScores(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x00052FFF File Offset: 0x000511FF
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x0005300C File Offset: 0x0005120C
		public static int UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadLeaderboardScore_64(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
			}
			return UserStats.UploadLeaderboardScore(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x00053080 File Offset: 0x00051280
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				UserStats.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				UserStats.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x000530EE File Offset: 0x000512EE
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x00053103 File Offset: 0x00051303
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x00053118 File Offset: 0x00051318
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x0400117B RID: 4475
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400117C RID: 4476
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x0400117D RID: 4477
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x0400117E RID: 4478
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x0400117F RID: 4479
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x0200048E RID: 1166
		public enum LeaderBoardRequestType
		{
			// Token: 0x04001EF7 RID: 7927
			GlobalData,
			// Token: 0x04001EF8 RID: 7928
			GlobalDataAroundUser,
			// Token: 0x04001EF9 RID: 7929
			LocalData,
			// Token: 0x04001EFA RID: 7930
			LocalDataAroundUser
		}

		// Token: 0x0200048F RID: 1167
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04001EFC RID: 7932
			AllTime,
			// Token: 0x04001EFD RID: 7933
			Daily,
			// Token: 0x04001EFE RID: 7934
			Weekly,
			// Token: 0x04001EFF RID: 7935
			Monthly
		}

		// Token: 0x02000490 RID: 1168
		public enum LeaderBoardSortMethod
		{
			// Token: 0x04001F01 RID: 7937
			None,
			// Token: 0x04001F02 RID: 7938
			Ascending,
			// Token: 0x04001F03 RID: 7939
			Descending
		}

		// Token: 0x02000491 RID: 1169
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x04001F05 RID: 7941
			None,
			// Token: 0x04001F06 RID: 7942
			Numeric,
			// Token: 0x04001F07 RID: 7943
			TimeSeconds,
			// Token: 0x04001F08 RID: 7944
			TimeMilliSeconds
		}

		// Token: 0x02000492 RID: 1170
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04001F0A RID: 7946
			None,
			// Token: 0x04001F0B RID: 7947
			KeepBest,
			// Token: 0x04001F0C RID: 7948
			ForceUpdate
		}

		// Token: 0x02000493 RID: 1171
		public enum AchievementDisplayAttribute
		{
			// Token: 0x04001F0E RID: 7950
			Name,
			// Token: 0x04001F0F RID: 7951
			Desc,
			// Token: 0x04001F10 RID: 7952
			Hidden
		}
	}
}
