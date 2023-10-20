using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000262 RID: 610
	internal class ArcadeLeaderboard
	{
		// Token: 0x06000ED7 RID: 3799 RVA: 0x00053DE7 File Offset: 0x00051FE7
		static ArcadeLeaderboard()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06000ED8 RID: 3800
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_IsReady")]
		internal static extern void IsReady(StatusCallback IsReadyCallback);

		// Token: 0x06000ED9 RID: 3801
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_IsReady")]
		internal static extern void IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x06000EDA RID: 3802
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_DownloadLeaderboardScores")]
		internal static extern void DownloadLeaderboardScores(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nCount);

		// Token: 0x06000EDB RID: 3803
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_DownloadLeaderboardScores")]
		internal static extern void DownloadLeaderboardScores_64(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nCount);

		// Token: 0x06000EDC RID: 3804
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_UploadLeaderboardScore")]
		internal static extern void UploadLeaderboardScore(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, string pchUserName, int nScore);

		// Token: 0x06000EDD RID: 3805
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_UploadLeaderboardScore")]
		internal static extern void UploadLeaderboardScore_64(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, string pchUserName, int nScore);

		// Token: 0x06000EDE RID: 3806
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScore")]
		internal static extern void GetLeaderboardScore(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x06000EDF RID: 3807
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScore")]
		internal static extern void GetLeaderboardScore_64(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x06000EE0 RID: 3808
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount();

		// Token: 0x06000EE1 RID: 3809
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount_64();

		// Token: 0x06000EE2 RID: 3810
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserRank")]
		internal static extern int GetLeaderboardUserRank();

		// Token: 0x06000EE3 RID: 3811
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserRank")]
		internal static extern int GetLeaderboardUserRank_64();

		// Token: 0x06000EE4 RID: 3812
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserScore")]
		internal static extern int GetLeaderboardUserScore();

		// Token: 0x06000EE5 RID: 3813
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserScore")]
		internal static extern int GetLeaderboardUserScore_64();
	}
}
