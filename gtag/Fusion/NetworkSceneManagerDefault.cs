using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

		public NetworkSceneManagerDefault()
		{
		}

		[Header("Single Peer Options")]
		public int PostLoadDelayFrames = 1;

		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			public <>c__DisplayClass1_0()
			{
			}

			internal void <LoadSceneAsync>b__0(Scene scene, LoadSceneMode _)
			{
				if (NetworkSceneManagerBase.IsScenePathOrNameEqual(scene, this.scenePath))
				{
					this.alreadyHandled = true;
					this.loaded(scene);
				}
			}

			internal void <LoadSceneAsync>b__1(AsyncOperation _)
			{
				SceneManager.sceneLoaded -= this.sceneLoadedHandler;
			}

			public string scenePath;

			public Action<Scene> loaded;

			public bool alreadyHandled;

			public UnityAction<Scene, LoadSceneMode> sceneLoadedHandler;
		}

		[CompilerGenerated]
		private sealed class <>c__DisplayClass4_0
		{
			public <>c__DisplayClass4_0()
			{
			}

			internal void <SwitchSceneMultiplePeer>b__0(Scene scene)
			{
				this.loadedScene = scene;
			}

			public Scene loadedScene;
		}

		[CompilerGenerated]
		private sealed class <>c__DisplayClass5_0
		{
			public <>c__DisplayClass5_0()
			{
			}

			internal void <SwitchSceneSinglePeer>b__0(Scene scene)
			{
				this.loadedScene = scene;
			}

			public Scene loadedScene;
		}

		[CompilerGenerated]
		private sealed class <SwitchSceneMultiplePeer>d__4 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <SwitchSceneMultiplePeer>d__4(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				NetworkSceneManagerDefault networkSceneManagerDefault = this;
				switch (num)
				{
				case 0:
				{
					this.<>1__state = -1;
					CS$<>8__locals1 = new NetworkSceneManagerDefault.<>c__DisplayClass4_0();
					Scene activeScene = SceneManager.GetActiveScene();
					bool flag = prevScene == default(SceneRef) && networkSceneManagerDefault.IsScenePathOrNameEqual(activeScene, newScene);
					loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, NetworkProjectConfig.ConvertPhysicsMode(networkSceneManagerDefault.Runner.Config.PhysicsEngine));
					sceneToUnload = networkSceneManagerDefault.Runner.MultiplePeerUnityScene;
					tempSceneSpawnedPrefabs = (networkSceneManagerDefault.Runner.IsMultiplePeerSceneTemp ? sceneToUnload.GetRootGameObjects() : Array.Empty<GameObject>());
					if (flag && NetworkRunner.GetRunnerForScene(activeScene) == null && SceneManager.sceneCount > 1)
					{
						this.<>2__current = networkSceneManagerDefault.UnloadSceneAsync(activeScene);
						this.<>1__state = 1;
						return true;
					}
					break;
				}
				case 1:
					this.<>1__state = -1;
					break;
				case 2:
					this.<>1__state = -1;
					goto IL_150;
				case 3:
				{
					this.<>1__state = -1;
					if (!CS$<>8__locals1.loadedScene.IsValid())
					{
						throw new InvalidOperationException(string.Format("Failed to load scene {0}: async op failed", newScene));
					}
					sceneObjects = networkSceneManagerDefault.FindNetworkObjects(CS$<>8__locals1.loadedScene, true, true);
					Scene multiplePeerUnityScene = networkSceneManagerDefault.Runner.MultiplePeerUnityScene;
					networkSceneManagerDefault.Runner.MultiplePeerUnityScene = CS$<>8__locals1.loadedScene;
					if (multiplePeerUnityScene.IsValid())
					{
						if (tempSceneSpawnedPrefabs.Length != 0)
						{
							GameObject[] array = tempSceneSpawnedPrefabs;
							for (int i = 0; i < array.Length; i++)
							{
								SceneManager.MoveGameObjectToScene(array[i], CS$<>8__locals1.loadedScene);
							}
						}
						this.<>2__current = networkSceneManagerDefault.UnloadSceneAsync(multiplePeerUnityScene);
						this.<>1__state = 4;
						return true;
					}
					goto IL_261;
				}
				case 4:
					this.<>1__state = -1;
					goto IL_261;
				default:
					return false;
				}
				if (SceneManager.sceneCount == 1 && tempSceneSpawnedPrefabs.Length == 0)
				{
					loadSceneParameters.loadSceneMode = LoadSceneMode.Single;
				}
				else if (sceneToUnload.IsValid() && networkSceneManagerDefault.Runner.TryMultiplePeerAssignTempScene())
				{
					this.<>2__current = networkSceneManagerDefault.UnloadSceneAsync(sceneToUnload);
					this.<>1__state = 2;
					return true;
				}
				IL_150:
				CS$<>8__locals1.loadedScene = default(Scene);
				this.<>2__current = networkSceneManagerDefault.LoadSceneAsync(newScene, loadSceneParameters, delegate(Scene scene)
				{
					CS$<>8__locals1.loadedScene = scene;
				});
				this.<>1__state = 3;
				return true;
				IL_261:
				finished(sceneObjects);
				return false;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public SceneRef prevScene;

			public NetworkSceneManagerDefault <>4__this;

			public SceneRef newScene;

			private NetworkSceneManagerDefault.<>c__DisplayClass4_0 <>8__1;

			public NetworkSceneManagerBase.FinishedLoadingDelegate finished;

			private LoadSceneParameters <loadSceneParameters>5__2;

			private Scene <sceneToUnload>5__3;

			private GameObject[] <tempSceneSpawnedPrefabs>5__4;

			private List<NetworkObject> <sceneObjects>5__5;
		}

		[CompilerGenerated]
		private sealed class <SwitchSceneSinglePeer>d__5 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <SwitchSceneSinglePeer>d__5(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				NetworkSceneManagerDefault networkSceneManagerDefault = this;
				switch (num)
				{
				case 0:
				{
					this.<>1__state = -1;
					CS$<>8__locals1 = new NetworkSceneManagerDefault.<>c__DisplayClass5_0();
					Scene activeScene = SceneManager.GetActiveScene();
					if (!(prevScene == default(SceneRef)) || !networkSceneManagerDefault.IsScenePathOrNameEqual(activeScene, newScene))
					{
						LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
						CS$<>8__locals1.loadedScene = default(Scene);
						this.<>2__current = networkSceneManagerDefault.LoadSceneAsync(newScene, loadSceneParameters, delegate(Scene scene)
						{
							CS$<>8__locals1.loadedScene = scene;
						});
						this.<>1__state = 1;
						return true;
					}
					CS$<>8__locals1.loadedScene = activeScene;
					break;
				}
				case 1:
					this.<>1__state = -1;
					if (!CS$<>8__locals1.loadedScene.IsValid())
					{
						throw new InvalidOperationException(string.Format("Failed to load scene {0}: async op failed", newScene));
					}
					break;
				case 2:
				{
					this.<>1__state = -1;
					int num2 = i - 1;
					i = num2;
					goto IL_123;
				}
				default:
					return false;
				}
				i = networkSceneManagerDefault.PostLoadDelayFrames;
				IL_123:
				if (i <= 0)
				{
					List<NetworkObject> list = networkSceneManagerDefault.FindNetworkObjects(CS$<>8__locals1.loadedScene, true, false);
					finished(list);
					return false;
				}
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public SceneRef prevScene;

			public NetworkSceneManagerDefault <>4__this;

			public SceneRef newScene;

			private NetworkSceneManagerDefault.<>c__DisplayClass5_0 <>8__1;

			public NetworkSceneManagerBase.FinishedLoadingDelegate finished;

			private int <i>5__2;
		}
	}
}
