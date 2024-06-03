using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class SpawnerPrototype<T> : SimulationBehaviour, IPlayerJoined, IPlayerLeft, ISpawned, ISceneLoadDone where T : Component, ISpawnPointPrototype
{
	protected bool _AllowClientObjects
	{
		get
		{
			return ((this.Runner && this.Runner.IsRunning) ? this.Runner.Config : NetworkProjectConfig.Global).Simulation.Topology == SimulationConfig.Topologies.Shared;
		}
	}

	protected virtual void Awake()
	{
		this.spawnManager = base.GetComponent<ISpawnPointManagerPrototype<T>>();
	}

	public void Spawned()
	{
		if (this.SpawnMethod != SpawnerPrototype<T>.SpawnMethods.AutoOnNetworkStart)
		{
			return;
		}
		if (this.Object && this._AllowClientObjects && this.StateAuthority != SpawnerPrototype<T>.AuthorityOptions.Server)
		{
			NetworkObject playerObject = this.TrySpawn(this.Runner, this.Runner.LocalPlayer);
			this.RegisterPlayerAndObject(this.Runner.LocalPlayer, playerObject);
		}
	}

	public void SceneLoadDone()
	{
		if (this.SpawnMethod != SpawnerPrototype<T>.SpawnMethods.AutoOnNetworkStart)
		{
			return;
		}
		if (this.Object)
		{
			return;
		}
		if (!this._AllowClientObjects || this.StateAuthority == SpawnerPrototype<T>.AuthorityOptions.Server)
		{
			return;
		}
		NetworkObject playerObject = this.TrySpawn(this.Runner, this.Runner.LocalPlayer);
		this.RegisterPlayerAndObject(this.Runner.LocalPlayer, playerObject);
	}

	public void PlayerJoined(PlayerRef player)
	{
		this.PlayerJoined(this.Runner, player);
	}

	public void PlayerLeft(PlayerRef player)
	{
		this.PlayerLeft(this.Runner, player);
	}

	private void PlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if (this.SpawnMethod != SpawnerPrototype<T>.SpawnMethods.AutoOnNetworkStart)
		{
			return;
		}
		if (this._AllowClientObjects && this.StateAuthority != SpawnerPrototype<T>.AuthorityOptions.Server)
		{
			return;
		}
		NetworkObject playerObject = this.TrySpawn(runner, player);
		this.RegisterPlayerAndObject(player, playerObject);
	}

	private void PlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.DespawnPlayersObjects(runner, player);
		this.UnregisterPlayer(player);
	}

	public NetworkObject TrySpawn(NetworkRunner runner, PlayerRef player)
	{
		if (!this.PlayerPrefab || !player.IsValid)
		{
			return null;
		}
		Transform transform = (this.spawnManager != null) ? this.spawnManager.GetNextSpawnPoint(runner, player, true) : null;
		if (transform == null)
		{
			transform = base.transform;
		}
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		return runner.Spawn(this.PlayerPrefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(player), null, null, true);
	}

	[BehaviourButtonAction("Spawn For All Players On Server", true, false, null)]
	public void TrySpawnAll()
	{
		List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
		while (instancesEnumerator.MoveNext())
		{
			NetworkRunner networkRunner = instancesEnumerator.Current;
			if (networkRunner.IsRunning && networkRunner.IsServer)
			{
				foreach (PlayerRef player in networkRunner.ActivePlayers)
				{
					NetworkObject playerObject = this.TrySpawn(networkRunner, player);
					this.RegisterPlayerAndObject(player, playerObject);
				}
			}
		}
	}

	protected virtual void RegisterPlayerAndObject(PlayerRef player, NetworkObject playerObject)
	{
		List<NetworkObject> list;
		if (!this._spawnedLookup.TryGetValue(player, out list))
		{
			list = new List<NetworkObject>();
			this._spawnedLookup.Add(player, list);
		}
		if (playerObject)
		{
			list.Add(playerObject);
		}
		this.Runner.SetPlayerAlwaysInterested(player, playerObject, true);
	}

	protected void DespawnPlayersObjects(NetworkRunner runner, PlayerRef player)
	{
		if (this._spawnedLookup.ContainsKey(player))
		{
			List<NetworkObject> list = this._spawnedLookup[player];
			if (list.Count > 0)
			{
				foreach (NetworkObject networkObject in list)
				{
					runner.Despawn(networkObject, false);
				}
			}
			this.UnregisterPlayer(player);
		}
	}

	protected void UnregisterPlayer(PlayerRef player)
	{
		if (this._spawnedLookup.ContainsKey(player))
		{
			this._spawnedLookup.Remove(player);
		}
	}

	public SpawnerPrototype()
	{
	}

	protected Dictionary<PlayerRef, List<NetworkObject>> _spawnedLookup = new Dictionary<PlayerRef, List<NetworkObject>>();

	[InlineHelp]
	public NetworkObject PlayerPrefab;

	[InlineHelp]
	public SpawnerPrototype<T>.SpawnMethods SpawnMethod;

	[InlineHelp]
	[DrawIf("_AllowClientObjects", Hide = true)]
	[MultiPropertyDrawersFix]
	public SpawnerPrototype<T>.AuthorityOptions StateAuthority;

	protected ISpawnPointManagerPrototype<T> spawnManager;

	public enum SpawnMethods
	{
		AutoOnNetworkStart,
		ByScriptOnly
	}

	public enum AuthorityOptions
	{
		Auto,
		Server,
		Player
	}
}
