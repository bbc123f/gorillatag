using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200004C RID: 76
public class DevErrorSoundAnnoyer : MonoBehaviour
{
	// Token: 0x0400025D RID: 605
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x0400025E RID: 606
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400025F RID: 607
	[SerializeField]
	private Text errorUIText;

	// Token: 0x04000260 RID: 608
	[SerializeField]
	private Font errorFont;

	// Token: 0x04000261 RID: 609
	public string displayedText;
}
