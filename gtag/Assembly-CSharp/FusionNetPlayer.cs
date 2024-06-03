using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion;
using UnityEngine;

public class FusionNetPlayer : NetPlayer
{
	public PlayerRef playerRef
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

	public FusionNetPlayer(PlayerRef playerRef)
	{
		this.playerRef = playerRef;
	}

	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	public override bool IsValid
	{
		get
		{
			return this.playerRef.IsValid;
		}
	}

	public override int ID
	{
		get
		{
			return this.playerRef.PlayerId;
		}
	}

	public override string UserId
	{
		get
		{
			return NetworkSystem.Instance.GetUserID(this.playerRef.PlayerId);
		}
	}

	public override bool IsMaster
	{
		get
		{
			return this.runner.IsSharedModeMasterClient;
		}
	}

	public override bool IsLocal
	{
		get
		{
			return this.playerRef == this.runner.LocalPlayer;
		}
	}

	public override bool IsNull
	{
		get
		{
			PlayerRef playerRef = this.playerRef;
			return false;
		}
	}

	public override string NickName
	{
		get
		{
			return NetworkSystem.Instance.GetNickName(this.ID);
		}
	}

	public override string DefaultName
	{
		get
		{
			return "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
		}
	}

	public override bool InRoom
	{
		get
		{
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.playerRef)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((FusionNetPlayer)myPlayer).playerRef.Equals(((FusionNetPlayer)other).playerRef);
	}

	[CompilerGenerated]
	private PlayerRef <playerRef>k__BackingField;
}
