using System;
using Photon.Pun;
using Photon.Realtime;

public class PunNetPlayer : NetPlayer
{
	public Player playerRef { get; private set; }

	public PunNetPlayer(Player playerRef)
	{
		this.playerRef = playerRef;
	}

	public override bool IsValid
	{
		get
		{
			return !this.playerRef.IsInactive;
		}
	}

	public override int ID
	{
		get
		{
			return this.playerRef.ActorNumber;
		}
	}

	public override string UserId
	{
		get
		{
			return this.playerRef.UserId;
		}
	}

	public override bool IsMaster
	{
		get
		{
			return this.playerRef.IsMasterClient;
		}
	}

	public override bool IsLocal
	{
		get
		{
			return this.playerRef == PhotonNetwork.LocalPlayer;
		}
	}

	public override bool IsNull
	{
		get
		{
			return this.playerRef == null;
		}
	}

	public override string NickName
	{
		get
		{
			return this.playerRef.NickName;
		}
	}

	public override bool InRoom
	{
		get
		{
			Player[] playerList = PhotonNetwork.PlayerList;
			for (int i = 0; i < playerList.Length; i++)
			{
				if (playerList[i] == this.playerRef)
				{
					return true;
				}
			}
			return false;
		}
	}

	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).playerRef.Equals(((PunNetPlayer)other).playerRef);
	}
}
