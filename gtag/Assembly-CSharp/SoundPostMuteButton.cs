using System;

// Token: 0x020001C1 RID: 449
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x06000B6D RID: 2925 RVA: 0x000466CC File Offset: 0x000448CC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		SynchedMusicController[] array = this.musicControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MuteAudio(this);
		}
	}

	// Token: 0x04000EEB RID: 3819
	public SynchedMusicController[] musicControllers;
}
