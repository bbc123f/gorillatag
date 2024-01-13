using System;
using System.Diagnostics;

namespace Utilities;

public static class BuildSafeEditorCallbacks
{
	[Conditional("UNITY_EDITOR")]
	public class OnScriptsReload : Attribute
	{
		public bool activeOnly;

		public OnScriptsReload(bool activeOnly = false)
		{
			this.activeOnly = activeOnly;
		}
	}
}
