using System;
using UnityEngine;

namespace GorillaTag
{
	public static class GTAppState
	{
		[OnEnterPlay_Set(false)]
		public static bool isQuitting { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate()
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log("GTAppState: SystemInfo.maxTextureArraySlices=" + SystemInfo.maxTextureArraySlices.ToString());
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
