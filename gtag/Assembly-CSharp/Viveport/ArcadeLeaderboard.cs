using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x0200024C RID: 588
	public class ArcadeLeaderboard
	{
		// Token: 0x06000E7B RID: 3707 RVA: 0x00052D59 File Offset: 0x00050F59
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			ArcadeLeaderboard.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x00052D68 File Offset: 0x00050F68
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			ArcadeLeaderboard.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(ArcadeLeaderboard.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.IsReady_64(new StatusCallback(ArcadeLeaderboard.IsReadyIl2cppCallback));
				return;
			}
			ArcadeLeaderboard.IsReady(new StatusCallback(ArcadeLeaderboard.IsReadyIl2cppCallback));
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x00052DD5 File Offset: 0x00050FD5
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			ArcadeLeaderboard.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x00052DE4 File Offset: 0x00050FE4
		public static void DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, ArcadeLeaderboard.LeaderboardTimeRange eLeaderboardDataTimeRange, int nCount)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			ArcadeLeaderboard.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(ArcadeLeaderboard.DownloadLeaderboardScoresIl2cppCallback));
			eLeaderboardDataTimeRange = ArcadeLeaderboard.LeaderboardTimeRange.AllTime;
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.DownloadLeaderboardScores_64(new StatusCallback(ArcadeLeaderboard.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nCount);
				return;
			}
			ArcadeLeaderboard.DownloadLeaderboardScores(new StatusCallback(ArcadeLeaderboard.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nCount);
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x00052E5A File Offset: 0x0005105A
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			ArcadeLeaderboard.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x00052E68 File Offset: 0x00051068
		public static void UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, string pchUserName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			ArcadeLeaderboard.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(ArcadeLeaderboard.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.UploadLeaderboardScore_64(new StatusCallback(ArcadeLeaderboard.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, pchUserName, nScore);
				return;
			}
			ArcadeLeaderboard.UploadLeaderboardScore(new StatusCallback(ArcadeLeaderboard.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, pchUserName, nScore);
		}

		// Token: 0x06000E81 RID: 3713 RVA: 0x00052EDC File Offset: 0x000510DC
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				ArcadeLeaderboard.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x00052F48 File Offset: 0x00051148
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return ArcadeLeaderboard.GetLeaderboardScoreCount_64();
			}
			return ArcadeLeaderboard.GetLeaderboardScoreCount();
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x00052F5D File Offset: 0x0005115D
		public static int GetLeaderboardUserRank()
		{
			if (IntPtr.Size == 8)
			{
				return ArcadeLeaderboard.GetLeaderboardUserRank_64();
			}
			return ArcadeLeaderboard.GetLeaderboardUserRank();
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x00052F72 File Offset: 0x00051172
		public static int GetLeaderboardUserScore()
		{
			if (IntPtr.Size == 8)
			{
				return ArcadeLeaderboard.GetLeaderboardUserScore_64();
			}
			return ArcadeLeaderboard.GetLeaderboardUserScore();
		}

		// Token: 0x0400117A RID: 4474
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400117B RID: 4475
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x0400117C RID: 4476
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000492 RID: 1170
		public enum LeaderboardTimeRange
		{
			// Token: 0x04001F05 RID: 7941
			AllTime
		}
	}
}
