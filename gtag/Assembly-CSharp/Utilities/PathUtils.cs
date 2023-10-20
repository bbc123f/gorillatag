using System;

namespace Utilities
{
	// Token: 0x02000287 RID: 647
	public static class PathUtils
	{
		// Token: 0x060010B3 RID: 4275 RVA: 0x0005918C File Offset: 0x0005738C
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] value = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", value)).AbsolutePath);
		}

		// Token: 0x0400125B RID: 4699
		private static readonly char[] kPathSeps = new char[]
		{
			'\\',
			'/'
		};

		// Token: 0x0400125C RID: 4700
		private const string kFwdSlash = "/";
	}
}
