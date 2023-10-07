using System;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class SoundBankSO : ScriptableObject
{
	// Token: 0x0400109E RID: 4254
	public AudioClip[] sounds;

	// Token: 0x0400109F RID: 4255
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x040010A0 RID: 4256
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
