using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;

public class FusionRegionCrawler : MonoBehaviour, INetworkRunnerCallbacks
{
	public int PlayerCountGlobal
	{
		get
		{
			return this.globalPlayerCount;
		}
	}

	public void Start()
	{
		this.regionRunner = base.gameObject.AddComponent<NetworkRunner>();
		this.regionRunner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
		base.StartCoroutine(this.OccasionalUpdate());
	}

	public IEnumerator OccasionalUpdate()
	{
		while (this.refreshPlayerCountAutomatically)
		{
			yield return this.UpdatePlayerCount();
			yield return new WaitForSeconds(this.UpdateFrequency);
		}
		yield break;
	}

	public IEnumerator UpdatePlayerCount()
	{
		int tempGlobalPlayerCount = 0;
		StartGameArgs startGameArgs = default(StartGameArgs);
		foreach (string fixedRegion in NetworkSystem.Instance.regionNames)
		{
			startGameArgs.CustomPhotonAppSettings = new AppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = fixedRegion;
			this.waitingForSessionListUpdate = true;
			this.regionRunner.JoinSessionLobby(SessionLobby.ClientServer, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false));
			while (this.waitingForSessionListUpdate)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (SessionInfo sessionInfo in this.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += this.tempSessionPlayerCount;
		}
		string[] array = null;
		this.globalPlayerCount = tempGlobalPlayerCount;
		FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = this.OnPlayerCountUpdated;
		if (onPlayerCountUpdated != null)
		{
			onPlayerCountUpdated(this.globalPlayerCount);
		}
		yield break;
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		if (this.waitingForSessionListUpdate)
		{
			this.sessionInfoCache = sessionList;
			this.waitingForSessionListUpdate = false;
		}
	}

	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
	{
	}

	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}

	public FusionRegionCrawler()
	{
	}

	public FusionRegionCrawler.PlayerCountUpdated OnPlayerCountUpdated;

	private NetworkRunner regionRunner;

	private List<SessionInfo> sessionInfoCache;

	private bool waitingForSessionListUpdate;

	private int globalPlayerCount;

	private float UpdateFrequency = 10f;

	private bool refreshPlayerCountAutomatically = true;

	private int tempSessionPlayerCount;

	public delegate void PlayerCountUpdated(int playerCount);

	[CompilerGenerated]
	private sealed class <OccasionalUpdate>d__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <OccasionalUpdate>d__12(int <>1__state)
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
			FusionRegionCrawler fusionRegionCrawler = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				break;
			case 1:
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(fusionRegionCrawler.UpdateFrequency);
				this.<>1__state = 2;
				return true;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			if (!fusionRegionCrawler.refreshPlayerCountAutomatically)
			{
				return false;
			}
			this.<>2__current = fusionRegionCrawler.UpdatePlayerCount();
			this.<>1__state = 1;
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

		public FusionRegionCrawler <>4__this;
	}

	[CompilerGenerated]
	private sealed class <UpdatePlayerCount>d__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdatePlayerCount>d__13(int <>1__state)
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
			FusionRegionCrawler fusionRegionCrawler = this;
			StartGameArgs startGameArgs;
			if (num == 0)
			{
				this.<>1__state = -1;
				tempGlobalPlayerCount = 0;
				startGameArgs = default(StartGameArgs);
				array = NetworkSystem.Instance.regionNames;
				i = 0;
				goto IL_124;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			IL_B6:
			if (fusionRegionCrawler.waitingForSessionListUpdate)
			{
				this.<>2__current = new WaitForEndOfFrame();
				this.<>1__state = 1;
				return true;
			}
			foreach (SessionInfo sessionInfo in fusionRegionCrawler.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += fusionRegionCrawler.tempSessionPlayerCount;
			i++;
			IL_124:
			if (i >= array.Length)
			{
				array = null;
				fusionRegionCrawler.globalPlayerCount = tempGlobalPlayerCount;
				FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = fusionRegionCrawler.OnPlayerCountUpdated;
				if (onPlayerCountUpdated != null)
				{
					onPlayerCountUpdated(fusionRegionCrawler.globalPlayerCount);
				}
				return false;
			}
			string fixedRegion = array[i];
			startGameArgs.CustomPhotonAppSettings = new AppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = fixedRegion;
			fusionRegionCrawler.waitingForSessionListUpdate = true;
			fusionRegionCrawler.regionRunner.JoinSessionLobby(SessionLobby.ClientServer, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false));
			goto IL_B6;
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

		public FusionRegionCrawler <>4__this;

		private int <tempGlobalPlayerCount>5__2;

		private string[] <>7__wrap2;

		private int <>7__wrap3;
	}
}
