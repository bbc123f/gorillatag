using System;
using UnityEngine;

[Serializable]
internal class SoundIdRemapping
{
	public int SoundIn
	{
		get
		{
			return this.soundIn;
		}
	}

	public int SoundOut
	{
		get
		{
			return this.soundOut;
		}
	}

	public SoundIdRemapping()
	{
	}

	[GorillaSoundLookup]
	[SerializeField]
	private int soundIn = 1;

	[GorillaSoundLookup]
	[SerializeField]
	private int soundOut = 2;
}
