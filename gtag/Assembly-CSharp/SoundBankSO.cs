using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class SoundBankSO : ScriptableObject
{
	// Token: 0x040010A3 RID: 4259
	public AudioClip[] sounds;

	// Token: 0x040010A4 RID: 4260
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x040010A5 RID: 4261
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
