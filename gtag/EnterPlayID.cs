﻿using System;

public struct EnterPlayID
{
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static EnterPlayID()
	{
	}

	private static int currentID = 1;

	private int id;
}
