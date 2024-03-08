using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Fusion
{
	public class NetworkSceneManagerDefault : NetworkSceneManagerBase
	{
		protected virtual YieldInstruction LoadSceneAsync(SceneRef sceneRef, LoadSceneParameters parameters, Action<Scene> loaded)
		{
			string scenePath;
			if (!this.TryGetScenePath(sceneRef, out scenePath))
			{
				throw new InvalidOperationException(string.Format("Not going to load {0}: unable to find the scene name", sceneRef));
			}
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenePath, parameters);
			bool alreadyHandled = false;
			UnityAction<Scene, LoadSceneMode> sceneLoadedHandler = delegate(Scene scene, LoadSceneMode _)
			{
				if (NetworkSceneManagerBase.IsScenePathOrNameEqual(scene, scenePath))
				{
					alreadyHandled = true;
					loaded(scene);
				}
			};
			SceneManager.sceneLoaded += sceneLoadedHandler;
			asyncOperation.completed += delegate(AsyncOperation _)
			{
				SceneManager.sceneLoaded -= sceneLoadedHandler;
			};
			return asyncOperation;
		}

		protected virtual YieldInstruction UnloadSceneAsync(Scene scene)
		{
			return SceneManager.UnloadSceneAsync(scene);
		}

		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, NetworkSceneManagerBase.FinishedLoadingDelegate finished)
		{
			if (base.Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Single)
			{
				return this.SwitchSceneSinglePeer(prevScene, newScene, finished);
			}
			return this.SwitchSceneMultiplePeer(prevScene, newScene, finished);
		}

		protected virtual IEnumerator SwitchSceneMultiplePeer(SceneRef prevScene, SceneRef newScene, NetworkSceneManagerBase.FinishedLoadingDelegate finished)
		{
			Scene activeScene = SceneManager.GetActiveScene();
			bool flag = prevScene == default(SceneRef) && base.IsScenePathOrNameEqual(activeScene, newScene);
			LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, NetworkProjectConfig.ConvertPhysicsMode(base.Runner.Config.PhysicsEngine));
			Scene sceneToUnload = base.Runner.MultiplePeerUnityScene;
			GameObject[] tempSceneSpawnedPrefabs = (base.Runner.IsMultiplePeerSceneTemp ? sceneToUnload.GetRootGameObjects() : Array.Empty<GameObject>());
			if (flag && NetworkRunner.GetRunnerForScene(activeScene) == null && SceneManager.sceneCount > 1)
			{
				yield return this.UnloadSceneAsync(activeScene);
			}
			if (SceneManager.sceneCount == 1 && tempSceneSpawnedPrefabs.Length == 0)
			{
				loadSceneParameters.loadSceneMode = LoadSceneMode.Single;
			}
			else if (sceneToUnload.IsValid() && base.Runner.TryMultiplePeerAssignTempScene())
			{
				yield return this.UnloadSceneAsync(sceneToUnload);
			}
			Scene loadedScene = default(Scene);
			yield return this.LoadSceneAsync(newScene, loadSceneParameters, delegate(Scene scene)
			{
				loadedScene = scene;
			});
			if (!loadedScene.IsValid())
			{
				throw new InvalidOperationException(string.Format("Failed to load scene {0}: async op failed", newScene));
			}
			List<NetworkObject> sceneObjects = base.FindNetworkObjects(loadedScene, true, true);
			Scene multiplePeerUnityScene = base.Runner.MultiplePeerUnityScene;
			base.Runner.MultiplePeerUnityScene = loadedScene;
			if (multiplePeerUnityScene.IsValid())
			{
				if (tempSceneSpawnedPrefabs.Length != 0)
				{
					GameObject[] array = tempSceneSpawnedPrefabs;
					for (int i = 0; i < array.Length; i++)
					{
						SceneManager.MoveGameObjectToScene(array[i], loadedScene);
					}
				}
				yield return this.UnloadSceneAsync(multiplePeerUnityScene);
			}
			finished(sceneObjects);
			yield break;
		}

		protected virtual IEnumerator SwitchSceneSinglePeer(SceneRef prevScene, SceneRef newScene, NetworkSceneManagerBase.FinishedLoadingDelegate finished)
		{
			Scene activeScene = SceneManager.GetActiveScene();
			Scene loadedScene;
			if (prevScene == default(SceneRef) && base.IsScenePathOrNameEqual(activeScene, newScene))
			{
				loadedScene = activeScene;
			}
			else
			{
				LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
				loadedScene = default(Scene);
				yield return this.LoadSceneAsync(newScene, loadSceneParameters, delegate(Scene scene)
				{
					loadedScene = scene;
				});
				if (!loadedScene.IsValid())
				{
					throw new InvalidOperationException(string.Format("Failed to load scene {0}: async op failed", newScene));
				}
			}
			int num;
			for (int i = this.PostLoadDelayFrames; i > 0; i = num)
			{
				yield return null;
				num = i - 1;
			}
			List<NetworkObject> list = base.FindNetworkObjects(loadedScene, true, false);
			finished(list);
			yield break;
		}

		[Header("Single Peer Options")]
		public int PostLoadDelayFrames = 1;
	}
}
