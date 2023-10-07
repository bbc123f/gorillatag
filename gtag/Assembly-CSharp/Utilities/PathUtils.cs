using System;

namespace Utilities
{
	// Token: 0x02000285 RID: 645
	public static class PathUtils
	{
		// Token: 0x060010AC RID: 4268 RVA: 0x00058DB0 File Offset: 0x00056FB0
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] value = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", value)).AbsolutePath);
		}

		// Token: 0x0400124E RID: 4686
		private static readonly char[] kPathSeps = new char[]
		{
			'\\',
			'/'
		};

		// Token: 0x0400124F RID: 4687
		private const string kFwdSlash = "/";
	}
}
