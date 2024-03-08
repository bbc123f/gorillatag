using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion
{
	public abstract class NetworkSceneManagerBase : Behaviour, INetworkSceneManager
	{
		public NetworkRunner Runner { get; private set; }

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void LateUpdate()
		{
			if (!this.Runner)
			{
				return;
			}
			if (this.Runner.CurrentScene != this._currentScene)
			{
				this._currentSceneOutdated = true;
			}
			if (!this._currentSceneOutdated || this._runningCoroutine != null)
			{
				return;
			}
			NetworkSceneManagerBase networkSceneManagerBase;
			if (NetworkSceneManagerBase.s_currentlyLoading.TryGetTarget(out networkSceneManagerBase))
			{
				if (networkSceneManagerBase)
				{
					return;
				}
				NetworkSceneManagerBase.s_currentlyLoading.SetTarget(null);
			}
			SceneRef currentScene = this._currentScene;
			this._currentScene = this.Runner.CurrentScene;
			this._currentSceneOutdated = false;
			this._runningCoroutine = this.SwitchSceneWrapper(currentScene, this._currentScene);
			base.StartCoroutine(this._runningCoroutine);
		}

		public static bool IsScenePathOrNameEqual(Scene scene, string nameOrPath)
		{
			return scene.path == nameOrPath || scene.name == nameOrPath;
		}

		public static bool TryGetScenePathFromBuildSettings(SceneRef sceneRef, out string path)
		{
			if (sceneRef.IsValid)
			{
				path = SceneUtility.GetScenePathByBuildIndex(sceneRef);
				if (!string.IsNullOrEmpty(path))
				{
					return true;
				}
			}
			path = string.Empty;
			return false;
		}

		public virtual bool TryGetScenePath(SceneRef sceneRef, out string path)
		{
			return NetworkSceneManagerBase.TryGetScenePathFromBuildSettings(sceneRef, out path);
		}

		public virtual bool TryGetSceneRef(string nameOrPath, out SceneRef sceneRef)
		{
			int sceneBuildIndex = FusionUnitySceneManagerUtils.GetSceneBuildIndex(nameOrPath);
			if (sceneBuildIndex >= 0)
			{
				sceneRef = sceneBuildIndex;
				return true;
			}
			sceneRef = default(SceneRef);
			return false;
		}

		public bool IsScenePathOrNameEqual(Scene scene, SceneRef sceneRef)
		{
			string text;
			return this.TryGetScenePath(sceneRef, out text) && NetworkSceneManagerBase.IsScenePathOrNameEqual(scene, text);
		}

		public List<NetworkObject> FindNetworkObjects(Scene scene, bool disable = true, bool addVisibilityNodes = false)
		{
			List<NetworkObject> list = new List<NetworkObject>();
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			List<NetworkObject> list2 = new List<NetworkObject>();
			foreach (GameObject gameObject in rootGameObjects)
			{
				list.Clear();
				gameObject.GetComponentsInChildren<NetworkObject>(true, list);
				foreach (NetworkObject networkObject in list)
				{
					if (networkObject.Flags.IsSceneObject() && (networkObject.gameObject.activeInHierarchy || networkObject.Flags.IsActivatedByUser()))
					{
						list2.Add(networkObject);
					}
				}
				if (addVisibilityNodes)
				{
					RunnerVisibilityNode.AddVisibilityNodes(gameObject, this.Runner);
				}
			}
			if (disable)
			{
				foreach (NetworkObject networkObject2 in list2)
				{
					networkObject2.gameObject.SetActive(false);
				}
			}
			return list2;
		}

		void INetworkSceneManager.Initialize(NetworkRunner runner)
		{
			this.Initialize(runner);
		}

		void INetworkSceneManager.Shutdown(NetworkRunner runner)
		{
			this.Shutdown(runner);
		}

		bool INetworkSceneManager.IsReady(NetworkRunner runner)
		{
			return this._runningCoroutine == null && !this._currentSceneOutdated && !(runner.CurrentScene != this._currentScene);
		}

		protected virtual void Initialize(NetworkRunner runner)
		{
			this.Runner = runner;
		}

		protected virtual void Shutdown(NetworkRunner runner)
		{
			try
			{
				if (this._runningCoroutine != null)
				{
					this.LogWarn(string.Format("There is an ongoing scene load ({0}), stopping and disposing coroutine.", this._currentScene));
					base.StopCoroutine(this._runningCoroutine);
					IDisposable disposable = this._runningCoroutine as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			finally
			{
				this.Runner = null;
				this._runningCoroutine = null;
				this._currentScene = SceneRef.None;
				this._currentSceneOutdated = false;
			}
		}

		protected abstract IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, NetworkSceneManagerBase.FinishedLoadingDelegate finished);

		[Conditional("FUSION_NETWORK_SCENE_MANAGER_TRACE")]
		protected void LogTrace(string msg)
		{
		}

		protected void LogError(string msg)
		{
			Log.Error("[NetworkSceneManager] " + ((this != null) ? base.name : "<destroyed>") + ": " + msg);
		}

		protected void LogWarn(string msg)
		{
			Log.Warn("[NetworkSceneManager] " + ((this != null) ? base.name : "<destroyed>") + ": " + msg);
		}

		private IEnumerator SwitchSceneWrapper(SceneRef prevScene, SceneRef newScene)
		{
			bool finishCalled = false;
			Dictionary<Guid, NetworkObject> sceneObjects = new Dictionary<Guid, NetworkObject>();
			Exception error = null;
			NetworkSceneManagerBase.FinishedLoadingDelegate finishedLoadingDelegate = delegate(IEnumerable<NetworkObject> objects)
			{
				finishCalled = true;
				foreach (NetworkObject networkObject in objects)
				{
					sceneObjects.Add(networkObject.NetworkGuid, networkObject);
				}
			};
			try
			{
				NetworkSceneManagerBase.s_currentlyLoading.SetTarget(this);
				this.Runner.InvokeSceneLoadStart();
				IEnumerator coro = this.SwitchScene(prevScene, newScene, finishedLoadingDelegate);
				bool next = true;
				while (next)
				{
					try
					{
						next = coro.MoveNext();
					}
					catch (Exception ex)
					{
						error = ex;
						break;
					}
					if (next)
					{
						yield return coro.Current;
					}
				}
				coro = null;
			}
			finally
			{
				NetworkSceneManagerBase.s_currentlyLoading.SetTarget(null);
				this._runningCoroutine = null;
			}
			if (error != null)
			{
				this.LogError(string.Format("Failed to switch scenes: {0}", error));
			}
			else if (!finishCalled)
			{
				this.LogError("Failed to switch scenes: SwitchScene implementation did not invoke finished delegate");
			}
			else
			{
				this.Runner.RegisterSceneObjects(sceneObjects.Values);
				this.Runner.InvokeSceneLoadDone();
			}
			yield break;
			yield break;
		}

		private static WeakReference<NetworkSceneManagerBase> s_currentlyLoading = new WeakReference<NetworkSceneManagerBase>(null);

		[InlineHelp]
		[ToggleLeft]
		[MultiPropertyDrawersFix]
		public bool ShowHierarchyWindowOverlay = true;

		private IEnumerator _runningCoroutine;

		private bool _currentSceneOutdated;

		private SceneRef _currentScene;

		protected delegate void FinishedLoadingDelegate(IEnumerable<NetworkObject> sceneObjects);
	}
}
