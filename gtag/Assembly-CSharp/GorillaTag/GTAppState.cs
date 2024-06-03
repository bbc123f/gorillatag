using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	public static class GTAppState
	{
		[OnEnterPlay_Set(false)]
		public static bool isQuitting
		{
			[CompilerGenerated]
			get
			{
				return GTAppState.<isQuitting>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				GTAppState.<isQuitting>k__BackingField = value;
			}
		}

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

		[CompilerGenerated]
		private static bool <isQuitting>k__BackingField;

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			public <>c()
			{
			}

			internal void <HandleOnSubsystemRegistration>b__4_0()
			{
				GTAppState.isQuitting = true;
			}

			public static readonly GTAppState.<>c <>9 = new GTAppState.<>c();

			public static Action <>9__4_0;
		}
	}
}
