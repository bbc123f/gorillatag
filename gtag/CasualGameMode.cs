using System;
using GorillaGameModes;
using Photon.Pun;

public class CasualGameMode : GorillaGameManager
{
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	public override GameModeType GameType()
	{
		return GameModeType.Casual;
	}

	public override string GameModeName()
	{
		return "CASUAL";
	}

	public CasualGameMode()
	{
	}
}
