using System;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;

public class PunNetPlayer : NetPlayer
{
	public Player playerRef
	{
		[CompilerGenerated]
		get
		{
			return this.<playerRef>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<playerRef>k__BackingField = value;
		}
	}

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

	public override string DefaultName
	{
		get
		{
			return this.playerRef.DefaultName;
		}
	}

	public override bool InRoom
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.ID) != null;
		}
	}

	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).playerRef.Equals(((PunNetPlayer)other).playerRef);
	}

	[CompilerGenerated]
	private Player <playerRef>k__BackingField;
}
