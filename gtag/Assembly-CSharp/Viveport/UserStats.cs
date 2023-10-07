using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x0200024B RID: 587
	public class UserStats
	{
		// Token: 0x06000E61 RID: 3681 RVA: 0x000528F4 File Offset: 0x00050AF4
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x00052904 File Offset: 0x00050B04
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

		// Token: 0x06000E63 RID: 3683 RVA: 0x00052971 File Offset: 0x00050B71
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x00052980 File Offset: 0x00050B80
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

		// Token: 0x06000E65 RID: 3685 RVA: 0x000529F0 File Offset: 0x00050BF0
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

		// Token: 0x06000E66 RID: 3686 RVA: 0x00052A1C File Offset: 0x00050C1C
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

		// Token: 0x06000E67 RID: 3687 RVA: 0x00052A48 File Offset: 0x00050C48
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x00052A63 File Offset: 0x00050C63
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x00052A7E File Offset: 0x00050C7E
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x00052A8C File Offset: 0x00050C8C
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

		// Token: 0x06000E6B RID: 3691 RVA: 0x00052AFC File Offset: 0x00050CFC
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

		// Token: 0x06000E6C RID: 3692 RVA: 0x00052B2C File Offset: 0x00050D2C
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

		// Token: 0x06000E6D RID: 3693 RVA: 0x00052B58 File Offset: 0x00050D58
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00052B5F File Offset: 0x00050D5F
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x00052B66 File Offset: 0x00050D66
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x00052B6D File Offset: 0x00050D6D
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x00052B84 File Offset: 0x00050D84
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x00052B9B File Offset: 0x00050D9B
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x00052BA8 File Offset: 0x00050DA8
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

		// Token: 0x06000E74 RID: 3700 RVA: 0x00052C23 File Offset: 0x00050E23
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x00052C30 File Offset: 0x00050E30
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

		// Token: 0x06000E76 RID: 3702 RVA: 0x00052CA4 File Offset: 0x00050EA4
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

		// Token: 0x06000E77 RID: 3703 RVA: 0x00052D12 File Offset: 0x00050F12
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x00052D27 File Offset: 0x00050F27
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x00052D3C File Offset: 0x00050F3C
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x04001175 RID: 4469
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04001176 RID: 4470
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x04001177 RID: 4471
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x04001178 RID: 4472
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x04001179 RID: 4473
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x0200048C RID: 1164
		public enum LeaderBoardRequestType
		{
			// Token: 0x04001EEA RID: 7914
			GlobalData,
			// Token: 0x04001EEB RID: 7915
			GlobalDataAroundUser,
			// Token: 0x04001EEC RID: 7916
			LocalData,
			// Token: 0x04001EED RID: 7917
			LocalDataAroundUser
		}

		// Token: 0x0200048D RID: 1165
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04001EEF RID: 7919
			AllTime,
			// Token: 0x04001EF0 RID: 7920
			Daily,
			// Token: 0x04001EF1 RID: 7921
			Weekly,
			// Token: 0x04001EF2 RID: 7922
			Monthly
		}

		// Token: 0x0200048E RID: 1166
		public enum LeaderBoardSortMethod
		{
			// Token: 0x04001EF4 RID: 7924
			None,
			// Token: 0x04001EF5 RID: 7925
			Ascending,
			// Token: 0x04001EF6 RID: 7926
			Descending
		}

		// Token: 0x0200048F RID: 1167
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x04001EF8 RID: 7928
			None,
			// Token: 0x04001EF9 RID: 7929
			Numeric,
			// Token: 0x04001EFA RID: 7930
			TimeSeconds,
			// Token: 0x04001EFB RID: 7931
			TimeMilliSeconds
		}

		// Token: 0x02000490 RID: 1168
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04001EFD RID: 7933
			None,
			// Token: 0x04001EFE RID: 7934
			KeepBest,
			// Token: 0x04001EFF RID: 7935
			ForceUpdate
		}

		// Token: 0x02000491 RID: 1169
		public enum AchievementDisplayAttribute
		{
			// Token: 0x04001F01 RID: 7937
			Name,
			// Token: 0x04001F02 RID: 7938
			Desc,
			// Token: 0x04001F03 RID: 7939
			Hidden
		}
	}
}
