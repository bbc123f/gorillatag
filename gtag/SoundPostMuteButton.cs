using System;

public class SoundPostMuteButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		SynchedMusicController[] array = this.musicControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MuteAudio(this);
		}
	}

	public SynchedMusicController[] musicControllers;
}
