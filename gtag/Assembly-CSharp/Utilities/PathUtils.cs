using System;

namespace Utilities
{
	public static class PathUtils
	{
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] value = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", value)).AbsolutePath);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static PathUtils()
		{
		}

		private static readonly char[] kPathSeps = new char[]
		{
			'\\',
			'/'
		};

		private const string kFwdSlash = "/";
	}
}
