using System;
using Fusion;

[SimulationBehaviour(Stages = SimulationStages.Forward, Modes = SimulationModes.Server | SimulationModes.Host)]
public class PlayerSpawnerPrototype : SpawnerPrototype<PlayerSpawnPointPrototype>, IPlayerJoined, IPlayerLeft, ISceneLoadDone
{
	protected override void RegisterPlayerAndObject(PlayerRef player, NetworkObject playerObject)
	{
		base.RegisterPlayerAndObject(player, playerObject);
		this.Runner.SetPlayerObject(player, playerObject);
	}

	public PlayerSpawnerPrototype()
	{
	}
}
