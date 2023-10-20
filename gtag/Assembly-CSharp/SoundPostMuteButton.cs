using System;

// Token: 0x020001C2 RID: 450
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x06000B73 RID: 2931 RVA: 0x00046934 File Offset: 0x00044B34
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		SynchedMusicController[] array = this.musicControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MuteAudio(this);
		}
	}

	// Token: 0x04000EEF RID: 3823
	public SynchedMusicController[] musicControllers;
}
