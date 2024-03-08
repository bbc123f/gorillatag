using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public abstract class SpawnPointManagerPrototype<T> : Fusion.Behaviour, ISpawnPointManagerPrototype<T> where T : Component, ISpawnPointPrototype
{
	private void Awake()
	{
		this.rng = new NetworkRNG(0);
	}

	public void CollectSpawnPoints(NetworkRunner runner)
	{
		this._spawnPoints.Clear();
		this._spawnPoints.AddRange(runner.SimulationUnityScene.FindObjectsOfTypeInOrder(false));
	}

	public virtual Transform GetNextSpawnPoint(NetworkRunner runner, PlayerRef player, bool skipIfBlocked = true)
	{
		this.CollectSpawnPoints(runner);
		int count = this._spawnPoints.Count;
		if (this._spawnPoints == null || count == 0)
		{
			return null;
		}
		int num;
		Component component;
		if (this.Sequence == SpawnPointManagerPrototype<T>.SpawnSequence.PlayerId)
		{
			num = player % count;
			component = this._spawnPoints[num];
		}
		else if (this.Sequence == SpawnPointManagerPrototype<T>.SpawnSequence.RoundRobin)
		{
			num = (this.LastSpawnIndex + 1) % count;
			component = this._spawnPoints[num];
		}
		else
		{
			num = this.rng.RangeInclusive(0, count);
			component = this._spawnPoints[num];
		}
		if (!skipIfBlocked || this.BlockingLayers.value == 0 || !this.IsBlocked(component))
		{
			this.LastSpawnIndex = num;
			return component.transform;
		}
		ValueTuple<int, Component> nextUnblocked = this.GetNextUnblocked(num);
		if (nextUnblocked.Item1 > -1)
		{
			this.LastSpawnIndex = nextUnblocked.Item1;
			return nextUnblocked.Item2.transform;
		}
		component = nextUnblocked.Item2;
		return this.AllSpawnPointsBlockedFallback();
	}

	public virtual Transform AllSpawnPointsBlockedFallback()
	{
		return base.transform;
	}

	public virtual ValueTuple<int, Component> GetNextUnblocked(int failedIndex)
	{
		int i = 1;
		int count = this._spawnPoints.Count;
		while (i < count)
		{
			Component component = this._spawnPoints[i % count];
			if (!this.IsBlocked(component))
			{
				return new ValueTuple<int, Component>(i, component);
			}
			i++;
		}
		return new ValueTuple<int, Component>(-1, null);
	}

	public virtual bool IsBlocked(Component spawnPoint)
	{
		PhysicsScene physicsScene = spawnPoint.gameObject.scene.GetPhysicsScene();
		if (SpawnPointManagerPrototype<T>.blocked3D == null)
		{
			SpawnPointManagerPrototype<T>.blocked3D = new Collider[1];
		}
		int num = physicsScene.OverlapSphere(spawnPoint.transform.position, this.BlockedCheckRadius, SpawnPointManagerPrototype<T>.blocked3D, this.BlockingLayers.value, QueryTriggerInteraction.UseGlobal);
		if (num > 0)
		{
			Debug.LogWarning(SpawnPointManagerPrototype<T>.blocked3D[0].name + " is blocking " + spawnPoint.name);
		}
		return num > 0;
	}

	[InlineHelp]
	public SpawnPointManagerPrototype<T>.SpawnSequence Sequence;

	[InlineHelp]
	public LayerMask BlockingLayers;

	[InlineHelp]
	public float BlockedCheckRadius = 2f;

	[NonSerialized]
	internal List<Component> _spawnPoints = new List<Component>();

	[NonSerialized]
	public int LastSpawnIndex = -1;

	private NetworkRNG rng;

	protected static Collider[] blocked3D;

	public enum SpawnSequence
	{
		PlayerId,
		RoundRobin,
		Random
	}
}
