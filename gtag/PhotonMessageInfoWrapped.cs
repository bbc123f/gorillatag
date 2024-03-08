using System;
using Fusion;
using Photon.Pun;

public struct PhotonMessageInfoWrapped
{
	public double SentServerTime
	{
		get
		{
			return this.sentTick / 1000.0;
		}
	}

	public PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		if (info.Sender != null)
		{
			this.senderID = info.Sender.ActorNumber;
			this.sentTick = info.SentServerTimestamp;
			return;
		}
		this.senderID = -1;
		this.sentTick = int.MinValue;
	}

	public PhotonMessageInfoWrapped(RpcInfo info)
	{
		this.senderID = info.Source.PlayerId;
		this.sentTick = info.Tick.Raw;
	}

	public PhotonMessageInfoWrapped(int playerID, int tick)
	{
		this.senderID = playerID;
		this.sentTick = tick;
	}

	public int senderID;

	public int sentTick;
}
