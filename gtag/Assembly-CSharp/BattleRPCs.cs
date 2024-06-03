using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class BattleRPCs : RPCNetworkBase
{
	public override void SetClassTarget(IGorillaSerializeable target, GorillaSerializer netHandler)
	{
		this.battleTarget = (GorillaBattleManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	[PunRPC]
	public void ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportSlingshotHit");
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		this.battleTarget.ReportSlingshotHit(taggedPlayer, hitLocation, projectileCount, info);
	}

	public BattleRPCs()
	{
	}

	private GameModeSerializer serializer;

	private GorillaBattleManager battleTarget;
}
