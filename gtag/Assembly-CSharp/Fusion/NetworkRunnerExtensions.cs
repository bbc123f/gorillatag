using System;

namespace Fusion
{
	public static class NetworkRunnerExtensions
	{
		public static bool SetActiveScene(this NetworkRunner runner, string sceneNameOrPath)
		{
			NetworkSceneManagerBase networkSceneManagerBase = runner.SceneManager as NetworkSceneManagerBase;
			if (networkSceneManagerBase != null)
			{
				SceneRef activeScene;
				if (networkSceneManagerBase.TryGetSceneRef(sceneNameOrPath, out activeScene))
				{
					runner.SetActiveScene(activeScene);
					return true;
				}
				return false;
			}
			else
			{
				int sceneBuildIndex = FusionUnitySceneManagerUtils.GetSceneBuildIndex(sceneNameOrPath);
				if (sceneBuildIndex >= 0)
				{
					runner.SetActiveScene(sceneBuildIndex);
					return true;
				}
				return false;
			}
		}
	}
}
