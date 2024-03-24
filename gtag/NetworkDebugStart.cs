using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Prototyping/Network Debug Start")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class NetworkDebugStart : Fusion.Behaviour
{
	public NetworkDebugStart.Stage CurrentStage
	{
		get
		{
			return this._currentStage;
		}
		internal set
		{
			this._currentStage = value;
		}
	}

	public int LastCreatedClientIndex
	{
		[CompilerGenerated]
		get
		{
			return this.<LastCreatedClientIndex>k__BackingField;
		}
		[CompilerGenerated]
		internal set
		{
			this.<LastCreatedClientIndex>k__BackingField = value;
		}
	}

	public GameMode CurrentServerMode
	{
		[CompilerGenerated]
		get
		{
			return this.<CurrentServerMode>k__BackingField;
		}
		[CompilerGenerated]
		internal set
		{
			this.<CurrentServerMode>k__BackingField = value;
		}
	}

	protected bool CanAddClients
	{
		get
		{
			return this.CurrentStage == NetworkDebugStart.Stage.AllConnected && this.CurrentServerMode > (GameMode)0 && this.CurrentServerMode != GameMode.Shared && this.CurrentServerMode != GameMode.Single;
		}
	}

	protected bool CanAddSharedClients
	{
		get
		{
			return this.CurrentStage == NetworkDebugStart.Stage.AllConnected && this.CurrentServerMode > (GameMode)0 && this.CurrentServerMode == GameMode.Shared;
		}
	}

	protected bool IsShutdown
	{
		get
		{
			return this.CurrentStage == NetworkDebugStart.Stage.Disconnected;
		}
	}

	protected bool IsShutdownAndMultiPeer
	{
		get
		{
			return this.CurrentStage == NetworkDebugStart.Stage.Disconnected && this.UsingMultiPeerMode;
		}
	}

	protected bool UsingMultiPeerMode
	{
		get
		{
			return NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
		}
	}

	protected bool ShowAutoClients
	{
		get
		{
			return this.StartMode != NetworkDebugStart.StartModes.Manual && this.UsingMultiPeerMode && this.AutoStartAs != GameMode.Single;
		}
	}

	protected virtual void Start()
	{
		if (NetworkDebugStart._initialScenePath == null)
		{
			if (string.IsNullOrEmpty(this.InitialScenePath))
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (activeScene.IsValid())
				{
					NetworkDebugStart._initialScenePath = activeScene.path;
				}
				else
				{
					NetworkDebugStart._initialScenePath = SceneManager.GetSceneByBuildIndex(0).path;
				}
				this.InitialScenePath = NetworkDebugStart._initialScenePath;
			}
			else
			{
				NetworkDebugStart._initialScenePath = this.InitialScenePath;
			}
		}
		bool flag = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
		NetworkRunner networkRunner = Object.FindObjectOfType<NetworkRunner>();
		if (networkRunner && networkRunner != this.RunnerPrefab)
		{
			if (networkRunner.State != NetworkRunner.States.Shutdown)
			{
				base.enabled = false;
				NetworkDebugStartGUI component = base.GetComponent<NetworkDebugStartGUI>();
				if (component)
				{
					Object.Destroy(component);
				}
				Object.Destroy(this);
				return;
			}
			if (this.RunnerPrefab == null)
			{
				this.RunnerPrefab = networkRunner;
			}
		}
		if (this.StartMode == NetworkDebugStart.StartModes.Manual)
		{
			return;
		}
		NetworkDebugStartGUI networkDebugStartGUI;
		if (this.StartMode == NetworkDebugStart.StartModes.Automatic)
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(this.AutoStartAs, sceneRef, flag ? this.AutoClients : ((this.AutoStartAs == GameMode.Client || this.AutoStartAs == GameMode.AutoHostOrClient) ? 1 : 0)));
				return;
			}
		}
		else if (!base.TryGetComponent<NetworkDebugStartGUI>(out networkDebugStartGUI))
		{
			base.gameObject.AddComponent<NetworkDebugStartGUI>();
		}
	}

	protected bool TryGetSceneRef(out SceneRef sceneRef)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		if (activeScene.buildIndex < 0 || activeScene.buildIndex >= SceneManager.sceneCountInBuildSettings)
		{
			sceneRef = default(SceneRef);
			return false;
		}
		sceneRef = activeScene.buildIndex;
		return true;
	}

	[BehaviourButtonAction("StartSinglePlayer", true, false, "IsShutdown")]
	public virtual void StartSinglePlayer()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			base.StartCoroutine(this.StartWithClients(GameMode.Single, sceneRef, 0));
		}
	}

	[BehaviourButtonAction("StartServer", true, false, "IsShutdown")]
	public virtual void StartServer()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			base.StartCoroutine(this.StartWithClients(GameMode.Server, sceneRef, 0));
		}
	}

	[BehaviourButtonAction("StartHost", true, false, "IsShutdown")]
	public virtual void StartHost()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			base.StartCoroutine(this.StartWithClients(GameMode.Host, sceneRef, 0));
		}
	}

	[BehaviourButtonAction("Start Client", true, false, "IsShutdown")]
	public virtual void StartClient()
	{
		base.StartCoroutine(this.StartWithClients(GameMode.Client, default(SceneRef), 1));
	}

	[BehaviourButtonAction("Start Shared Client", true, false, "IsShutdown")]
	public virtual void StartSharedClient()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			base.StartCoroutine(this.StartWithClients(GameMode.Shared, sceneRef, 1));
		}
	}

	[BehaviourButtonAction("Start Auto Host Or Client", true, false, "IsShutdown")]
	public virtual void StartAutoClient()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			base.StartCoroutine(this.StartWithClients(GameMode.AutoHostOrClient, sceneRef, 1));
		}
	}

	[BehaviourButtonAction("Start Server Plus Clients", true, false, "IsShutdownAndMultiPeer")]
	public virtual void StartServerPlusClients()
	{
		this.StartServerPlusClients(this.AutoClients);
	}

	[BehaviourButtonAction("Start Host Plus Clients", true, false, "IsShutdownAndMultiPeer")]
	public void StartHostPlusClients()
	{
		this.StartHostPlusClients(this.AutoClients);
	}

	[BehaviourButtonAction("Shutdown", true, false, "CurrentStage")]
	public void Shutdown()
	{
		this.ShutdownAll();
	}

	public virtual void StartServerPlusClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Server, sceneRef, clientCount));
				return;
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartHostPlusClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Host, sceneRef, clientCount));
				return;
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartMultipleClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Client, sceneRef, clientCount));
				return;
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartMultipleSharedClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.Shared, sceneRef, clientCount));
				return;
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartMultipleAutoClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			SceneRef sceneRef;
			if (this.TryGetSceneRef(out sceneRef))
			{
				base.StartCoroutine(this.StartWithClients(GameMode.AutoHostOrClient, sceneRef, clientCount));
				return;
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void ShutdownAll()
	{
		foreach (NetworkRunner networkRunner in NetworkRunner.Instances.ToList<NetworkRunner>())
		{
			if (networkRunner != null && networkRunner.IsRunning)
			{
				networkRunner.Shutdown(true, ShutdownReason.Ok, false);
			}
		}
		SceneManager.LoadSceneAsync(NetworkDebugStart._initialScenePath);
		Object.Destroy(this.RunnerPrefab.gameObject);
		Object.Destroy(base.gameObject);
		this.CurrentStage = NetworkDebugStart.Stage.Disconnected;
		this.CurrentServerMode = (GameMode)0;
	}

	protected IEnumerator StartWithClients(GameMode serverMode, SceneRef sceneRef, int clientCount)
	{
		if (this.CurrentStage != NetworkDebugStart.Stage.Disconnected)
		{
			yield break;
		}
		bool includesServerStart = serverMode != GameMode.Shared && serverMode != GameMode.Client && serverMode != GameMode.AutoHostOrClient;
		if (!includesServerStart && clientCount == 0)
		{
			Debug.LogError(string.Format("{0} is set to {1}, and {2} is set to zero. Starting no network runners.", "GameMode", serverMode, "clientCount"));
			yield break;
		}
		this.CurrentStage = NetworkDebugStart.Stage.StartingUp;
		SceneManager.GetActiveScene();
		if (!this.RunnerPrefab)
		{
			Debug.LogError("RunnerPrefab not set, can't perform debug start.");
			yield break;
		}
		this.RunnerPrefab = Object.Instantiate<NetworkRunner>(this.RunnerPrefab);
		Object.DontDestroyOnLoad(this.RunnerPrefab);
		this.RunnerPrefab.name = "Temporary Runner Prefab";
		NetworkProjectConfig global = NetworkProjectConfig.Global;
		if (global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
		{
			int num = (includesServerStart ? 0 : 1);
			if (clientCount > num)
			{
				Debug.LogWarning(string.Format("Instance mode must be set to {0} to perform a debug start multiple peers. Restricting client count to {1}.", "Multiple", num));
				clientCount = num;
			}
		}
		if ((serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient || serverMode == GameMode.Server || serverMode == GameMode.Host) && clientCount > 1 && global.PeerMode == NetworkProjectConfig.PeerModes.Multiple && string.IsNullOrEmpty(this.DefaultRoomName))
		{
			this.DefaultRoomName = Guid.NewGuid().ToString();
			Debug.Log("Generated Session Name: " + this.DefaultRoomName);
		}
		if (base.gameObject.transform.parent)
		{
			Debug.LogWarning("NetworkDebugStart can't be a child game object, un-parenting.");
			base.gameObject.transform.parent = null;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		this.CurrentServerMode = serverMode;
		if (includesServerStart)
		{
			this._server = Object.Instantiate<NetworkRunner>(this.RunnerPrefab);
			this._server.name = serverMode.ToString();
			Task serverTask = this.InitializeNetworkRunner(this._server, serverMode, NetAddress.Any(this.ServerPort), sceneRef, delegate(NetworkRunner runner)
			{
			});
			while (!serverTask.IsCompleted)
			{
				yield return new WaitForSeconds(1f);
			}
			if (serverTask.IsFaulted)
			{
				this.ShutdownAll();
				yield break;
			}
			yield return this.StartClients(clientCount, serverMode, sceneRef);
			serverTask = null;
		}
		else
		{
			yield return this.StartClients(clientCount, serverMode, sceneRef);
		}
		if (includesServerStart && this.AlwaysShowStats && serverMode != GameMode.Shared)
		{
			FusionStats.Create(null, this._server, new FusionStats.DefaultLayouts?(FusionStats.DefaultLayouts.Left), new FusionStats.DefaultLayouts?(FusionStats.DefaultLayouts.Left), null, null);
		}
		yield break;
	}

	[BehaviourButtonAction("Add Additional Client", null, "CanAddClients")]
	public void AddClient()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			this.AddClient(GameMode.Client, sceneRef);
		}
	}

	[BehaviourButtonAction("Add Additional Shared Client", null, "CanAddSharedClients")]
	public void AddSharedClient()
	{
		SceneRef sceneRef;
		if (this.TryGetSceneRef(out sceneRef))
		{
			this.AddClient(GameMode.Shared, sceneRef);
		}
	}

	public Task AddClient(GameMode serverMode, SceneRef sceneRef)
	{
		NetworkRunner networkRunner = Object.Instantiate<NetworkRunner>(this.RunnerPrefab);
		Object.DontDestroyOnLoad(networkRunner);
		Object @object = networkRunner;
		string text = "Client {0}";
		int num = 65;
		int lastCreatedClientIndex = this.LastCreatedClientIndex;
		this.LastCreatedClientIndex = lastCreatedClientIndex + 1;
		@object.name = string.Format(text, (char)(num + lastCreatedClientIndex));
		GameMode gameMode = GameMode.Client;
		if (serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient)
		{
			gameMode = serverMode;
		}
		Task task = this.InitializeNetworkRunner(networkRunner, gameMode, NetAddress.Any(0), sceneRef, null);
		if (this.AlwaysShowStats && this.LastCreatedClientIndex == 0)
		{
			FusionStats.Create(null, networkRunner, new FusionStats.DefaultLayouts?(FusionStats.DefaultLayouts.Right), new FusionStats.DefaultLayouts?(FusionStats.DefaultLayouts.Right), null, null);
		}
		return task;
	}

	protected IEnumerator StartClients(int clientCount, GameMode serverMode, SceneRef sceneRef = default(SceneRef))
	{
		this.CurrentStage = NetworkDebugStart.Stage.ConnectingClients;
		List<Task> clientTasks = new List<Task>();
		int num;
		for (int i = 0; i < clientCount; i = num)
		{
			clientTasks.Add(this.AddClient(serverMode, sceneRef));
			yield return new WaitForSeconds(0.1f);
			num = i + 1;
		}
		Task clientsStartTask = Task.WhenAll(clientTasks);
		while (!clientsStartTask.IsCompleted)
		{
			yield return new WaitForSeconds(1f);
		}
		if (clientsStartTask.IsFaulted)
		{
			Debug.LogWarning(clientsStartTask.Exception);
		}
		this.CurrentStage = NetworkDebugStart.Stage.AllConnected;
		yield break;
	}

	protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
	{
		INetworkSceneManager networkSceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault<INetworkSceneManager>();
		if (networkSceneManager == null)
		{
			Debug.Log("NetworkRunner does not have any component implementing INetworkSceneManager interface, adding NetworkSceneManagerDefault.", runner);
			networkSceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
		}
		return runner.StartGame(new StartGameArgs
		{
			GameMode = gameMode,
			Address = new NetAddress?(address),
			Scene = new SceneRef?(scene),
			SessionName = this.DefaultRoomName,
			Initialized = initialized,
			SceneManager = networkSceneManager
		});
	}

	public NetworkDebugStart()
	{
	}

	[InlineHelp]
	[WarnIf("RunnerPrefab", false, "No RunnerPrefab supplied. Will search for a NetworkRunner in the scene at startup.")]
	[MultiPropertyDrawersFix]
	public NetworkRunner RunnerPrefab;

	[InlineHelp]
	[MultiPropertyDrawersFix]
	[WarnIf("StartMode", 2.0, "Start network by calling the methods StartHost(), StartServer(), StartClient(), StartHostPlusClients(), or StartServerPlusClients()", MsgType = 1)]
	public NetworkDebugStart.StartModes StartMode;

	[InlineHelp]
	[FormerlySerializedAs("Server")]
	[DrawIf("StartMode", 1.0, Hide = true)]
	public GameMode AutoStartAs = GameMode.Shared;

	[InlineHelp]
	[DrawIf("StartMode", 0.0, Hide = true)]
	public bool AutoHideGUI = true;

	[InlineHelp]
	[DrawIf("ShowAutoClients", Hide = true)]
	public int AutoClients = 1;

	[InlineHelp]
	public ushort ServerPort = 27015;

	[InlineHelp]
	public string DefaultRoomName = "";

	[InlineHelp]
	public bool AlwaysShowStats;

	[NonSerialized]
	private NetworkRunner _server;

	[InlineHelp]
	[ScenePath]
	[MultiPropertyDrawersFix]
	public string InitialScenePath;

	private static string _initialScenePath;

	[InlineHelp]
	[SerializeField]
	[EditorDisabled(false)]
	[MultiPropertyDrawersFix]
	protected NetworkDebugStart.Stage _currentStage;

	[CompilerGenerated]
	private int <LastCreatedClientIndex>k__BackingField;

	[CompilerGenerated]
	private GameMode <CurrentServerMode>k__BackingField;

	public enum StartModes
	{
		UserInterface,
		Automatic,
		Manual
	}

	public enum Stage
	{
		Disconnected,
		StartingUp,
		UnloadOriginalScene,
		ConnectingServer,
		ConnectingClients,
		AllConnected
	}

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

		internal void <StartWithClients>b__54_0(NetworkRunner runner)
		{
		}

		public static readonly NetworkDebugStart.<>c <>9 = new NetworkDebugStart.<>c();

		public static Action<NetworkRunner> <>9__54_0;
	}

	[CompilerGenerated]
	private sealed class <StartClients>d__58 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <StartClients>d__58(int <>1__state)
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
			NetworkDebugStart networkDebugStart = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				networkDebugStart.CurrentStage = NetworkDebugStart.Stage.ConnectingClients;
				clientTasks = new List<Task>();
				i = 0;
				break;
			case 1:
			{
				this.<>1__state = -1;
				int num2 = i + 1;
				i = num2;
				break;
			}
			case 2:
				this.<>1__state = -1;
				goto IL_D2;
			default:
				return false;
			}
			if (i < clientCount)
			{
				clientTasks.Add(networkDebugStart.AddClient(serverMode, sceneRef));
				this.<>2__current = new WaitForSeconds(0.1f);
				this.<>1__state = 1;
				return true;
			}
			clientsStartTask = Task.WhenAll(clientTasks);
			IL_D2:
			if (clientsStartTask.IsCompleted)
			{
				if (clientsStartTask.IsFaulted)
				{
					Debug.LogWarning(clientsStartTask.Exception);
				}
				networkDebugStart.CurrentStage = NetworkDebugStart.Stage.AllConnected;
				return false;
			}
			this.<>2__current = new WaitForSeconds(1f);
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

		public NetworkDebugStart <>4__this;

		public GameMode serverMode;

		public SceneRef sceneRef;

		public int clientCount;

		private List<Task> <clientTasks>5__2;

		private Task <clientsStartTask>5__3;

		private int <i>5__4;
	}

	[CompilerGenerated]
	private sealed class <StartWithClients>d__54 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <StartWithClients>d__54(int <>1__state)
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
			NetworkDebugStart networkDebugStart = this;
			switch (num)
			{
			case 0:
			{
				this.<>1__state = -1;
				if (networkDebugStart.CurrentStage != NetworkDebugStart.Stage.Disconnected)
				{
					return false;
				}
				includesServerStart = serverMode != GameMode.Shared && serverMode != GameMode.Client && serverMode != GameMode.AutoHostOrClient;
				if (!includesServerStart && clientCount == 0)
				{
					Debug.LogError(string.Format("{0} is set to {1}, and {2} is set to zero. Starting no network runners.", "GameMode", serverMode, "clientCount"));
					return false;
				}
				networkDebugStart.CurrentStage = NetworkDebugStart.Stage.StartingUp;
				SceneManager.GetActiveScene();
				if (!networkDebugStart.RunnerPrefab)
				{
					Debug.LogError("RunnerPrefab not set, can't perform debug start.");
					return false;
				}
				networkDebugStart.RunnerPrefab = Object.Instantiate<NetworkRunner>(networkDebugStart.RunnerPrefab);
				Object.DontDestroyOnLoad(networkDebugStart.RunnerPrefab);
				networkDebugStart.RunnerPrefab.name = "Temporary Runner Prefab";
				NetworkProjectConfig global = NetworkProjectConfig.Global;
				if (global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
				{
					int num2 = (includesServerStart ? 0 : 1);
					if (clientCount > num2)
					{
						Debug.LogWarning(string.Format("Instance mode must be set to {0} to perform a debug start multiple peers. Restricting client count to {1}.", "Multiple", num2));
						clientCount = num2;
					}
				}
				if ((serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient || serverMode == GameMode.Server || serverMode == GameMode.Host) && clientCount > 1 && global.PeerMode == NetworkProjectConfig.PeerModes.Multiple && string.IsNullOrEmpty(networkDebugStart.DefaultRoomName))
				{
					networkDebugStart.DefaultRoomName = Guid.NewGuid().ToString();
					Debug.Log("Generated Session Name: " + networkDebugStart.DefaultRoomName);
				}
				if (networkDebugStart.gameObject.transform.parent)
				{
					Debug.LogWarning("NetworkDebugStart can't be a child game object, un-parenting.");
					networkDebugStart.gameObject.transform.parent = null;
				}
				Object.DontDestroyOnLoad(networkDebugStart.gameObject);
				networkDebugStart.CurrentServerMode = serverMode;
				if (!includesServerStart)
				{
					this.<>2__current = networkDebugStart.StartClients(clientCount, serverMode, sceneRef);
					this.<>1__state = 3;
					return true;
				}
				networkDebugStart._server = Object.Instantiate<NetworkRunner>(networkDebugStart.RunnerPrefab);
				networkDebugStart._server.name = serverMode.ToString();
				serverTask = networkDebugStart.InitializeNetworkRunner(networkDebugStart._server, serverMode, NetAddress.Any(networkDebugStart.ServerPort), sceneRef, delegate(NetworkRunner runner)
				{
				});
				break;
			}
			case 1:
				this.<>1__state = -1;
				break;
			case 2:
				this.<>1__state = -1;
				serverTask = null;
				goto IL_310;
			case 3:
				this.<>1__state = -1;
				goto IL_310;
			default:
				return false;
			}
			if (!serverTask.IsCompleted)
			{
				this.<>2__current = new WaitForSeconds(1f);
				this.<>1__state = 1;
				return true;
			}
			if (serverTask.IsFaulted)
			{
				networkDebugStart.ShutdownAll();
				return false;
			}
			this.<>2__current = networkDebugStart.StartClients(clientCount, serverMode, sceneRef);
			this.<>1__state = 2;
			return true;
			IL_310:
			if (includesServerStart && networkDebugStart.AlwaysShowStats && serverMode != GameMode.Shared)
			{
				FusionStats.Create(null, networkDebugStart._server, new FusionStats.DefaultLayouts?(FusionStats.DefaultLayouts.Left), new FusionStats.DefaultLayouts?(FusionStats.DefaultLayouts.Left), null, null);
			}
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

		public NetworkDebugStart <>4__this;

		public GameMode serverMode;

		public int clientCount;

		public SceneRef sceneRef;

		private bool <includesServerStart>5__2;

		private Task <serverTask>5__3;
	}
}
