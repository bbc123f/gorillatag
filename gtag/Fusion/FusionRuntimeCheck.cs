using System;
using UnityEngine;

namespace Fusion
{
	internal static class FusionRuntimeCheck
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeCheck()
		{
			RuntimeUnityFlagsSetup.Check_ENABLE_MONO();
			RuntimeUnityFlagsSetup.Check_NET_STANDARD_2_0();
			RuntimeUnityFlagsSetup.Check_UNITY_2019_4_OR_NEWER();
		}
	}
}
