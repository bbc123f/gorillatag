using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionCallbackHandler : MonoBehaviour, INetworkRunnerCallbacks
{
	public void Setup(NetworkSystemFusion parentController)
	{
		this.parent = parentController;
		this.parent.runner.AddCallbacks(new INetworkRunnerCallbacks[] { this });
	}

	private void OnDestroy()
	{
		this.RemoveCallbacks();
	}

	private async void RemoveCallbacks()
	{
		await Task.Delay(500);
		this.parent.runner.RemoveCallbacks(new INetworkRunnerCallbacks[] { this });
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		this.parent.OnJoinedSession();
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		this.parent.OnJoinFailed(reason);
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		Debug.Log("Received custom auth response:");
		foreach (KeyValuePair<string, object> keyValuePair in data)
		{
			Debug.Log(keyValuePair.Key + ":" + (keyValuePair.Value as string));
		}
	}

	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		this.parent.OnDisconnectedFromSession();
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.parent.MigrateHost(hostMigrationToken);
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkedInput input2 = NetInput.GetInput();
		input.Set<NetworkedInput>(input2);
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerJoined(player);
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerLeft(player);
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		this.parent.OnRunnerShutDown();
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	public FusionCallbackHandler()
	{
	}

	private NetworkSystemFusion parent;

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <RemoveCallbacks>d__3 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			FusionCallbackHandler fusionCallbackHandler = this;
			try
			{
				TaskAwaiter taskAwaiter;
				if (num != 0)
				{
					taskAwaiter = Task.Delay(500).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FusionCallbackHandler.<RemoveCallbacks>d__3>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter);
					num2 = -1;
				}
				taskAwaiter.GetResult();
				fusionCallbackHandler.parent.runner.RemoveCallbacks(new INetworkRunnerCallbacks[] { fusionCallbackHandler });
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public FusionCallbackHandler <>4__this;

		private TaskAwaiter <>u__1;
	}
}
