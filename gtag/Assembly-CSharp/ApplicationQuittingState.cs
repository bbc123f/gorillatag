using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ApplicationQuittingState
{
	public static bool IsQuitting
	{
		[CompilerGenerated]
		get
		{
			return ApplicationQuittingState.<IsQuitting>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			ApplicationQuittingState.<IsQuitting>k__BackingField = value;
		}
	}

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += ApplicationQuittingState.HandleApplicationQuitting;
	}

	private static void HandleApplicationQuitting()
	{
		ApplicationQuittingState.IsQuitting = true;
	}

	[CompilerGenerated]
	[OnEnterPlay_Set(false)]
	private static bool <IsQuitting>k__BackingField;
}
