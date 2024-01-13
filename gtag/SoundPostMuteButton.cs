public class SoundPostMuteButton : GorillaPressableButton
{
	public SynchedMusicController[] musicControllers;

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		SynchedMusicController[] array = musicControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MuteAudio(this);
		}
	}
}
