using System;

public abstract class NetPlayer
{
	public abstract bool IsValid { get; }

	public abstract int ID { get; }

	public abstract string UserId { get; }

	public abstract bool IsMaster { get; }

	public abstract bool IsLocal { get; }

	public abstract bool IsNull { get; }

	public abstract string NickName { get; }

	public abstract bool InRoom { get; }

	public abstract bool Equals(NetPlayer myPlayer, NetPlayer other);
}
