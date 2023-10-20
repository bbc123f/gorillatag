using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E7 RID: 743
	public class CowController : MonoBehaviour
	{
		// Token: 0x06001415 RID: 5141 RVA: 0x00071FB6 File Offset: 0x000701B6
		private void Start()
		{
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x00071FB8 File Offset: 0x000701B8
		public void PlayMooSound()
		{
			this._mooCowAudioSource.timeSamples = 0;
			this._mooCowAudioSource.Play();
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x00071FD1 File Offset: 0x000701D1
		public void GoMooCowGo()
		{
			this._cowAnimation.Rewind();
			this._cowAnimation.Play();
		}

		// Token: 0x040016C0 RID: 5824
		[SerializeField]
		private Animation _cowAnimation;

		// Token: 0x040016C1 RID: 5825
		[SerializeField]
		private AudioSource _mooCowAudioSource;
	}
}
