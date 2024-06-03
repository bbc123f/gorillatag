using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class InputBehaviourPrototype : Fusion.Behaviour, INetworkRunnerCallbacks
{
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkInputPrototype value = default(NetworkInputPrototype);
		if (Input.GetKey(KeyCode.W))
		{
			value.Buttons.Set(3, true);
		}
		if (Input.GetKey(KeyCode.S))
		{
			value.Buttons.Set(4, true);
		}
		if (Input.GetKey(KeyCode.A))
		{
			value.Buttons.Set(5, true);
		}
		if (Input.GetKey(KeyCode.D))
		{
			value.Buttons.Set(6, true);
		}
		if (Input.GetKey(KeyCode.Space))
		{
			value.Buttons.Set(7, true);
		}
		if (Input.GetKey(KeyCode.C))
		{
			value.Buttons.Set(8, true);
		}
		if (Input.GetKey(KeyCode.E))
		{
			value.Buttons.Set(10, true);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			value.Buttons.Set(11, true);
		}
		if (Input.GetKey(KeyCode.F))
		{
			value.Buttons.Set(12, true);
		}
		if (Input.GetKey(KeyCode.G))
		{
			value.Buttons.Set(14, true);
		}
		if (Input.GetKey(KeyCode.R))
		{
			value.Buttons.Set(15, true);
		}
		if (Input.GetMouseButton(0))
		{
			value.Buttons.Set(1, true);
		}
		input.Set<NetworkInputPrototype>(value);
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
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

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	public InputBehaviourPrototype()
	{
	}
}
