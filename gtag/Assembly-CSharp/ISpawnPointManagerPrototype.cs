using System;
using Fusion;
using UnityEngine;

public interface ISpawnPointManagerPrototype<T> where T : Component, ISpawnPointPrototype
{
	void CollectSpawnPoints(NetworkRunner runner);

	Transform GetNextSpawnPoint(NetworkRunner runner, PlayerRef player, bool skipIfBlocked = true);
}
